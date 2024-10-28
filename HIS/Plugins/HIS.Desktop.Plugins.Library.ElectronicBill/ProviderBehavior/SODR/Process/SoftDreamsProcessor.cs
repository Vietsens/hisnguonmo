using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR.Process.IssueCreateV2;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR.Process
{
    internal class SoftDreamsProcessor
    {
        private string Token;

        private string Api;

        public SoftDreamsProcessor(string api, string user, string pass)
        {
            this.Api = api;
            this.LoginData(api, user, pass);
        }

        private void LoginData(string api, string user, string pass)
        {
            bool flag = !string.IsNullOrEmpty(api) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass);
            if (!flag)
            {
                throw new Exception("Thông tin đăng nhập không hợp lệ vui lòng kiểm tra lại.");
            }
            var objData = new
            {
                Username = user,
                Password = pass
            };
            string fullapi = Base.RequestUriStore.CombileUrl(api, UriStore.LoginUrl);
            ResultDataV2 resultDataV = ApiConsumer.CreateRequest<ResultDataV2>(fullapi, null, objData);
            bool flag2 = resultDataV != null && !string.IsNullOrEmpty(resultDataV.AccessToken);
            if (flag2)
            {
                this.Token = resultDataV.AccessToken;
                return;
            }
            throw new Exception("Thông tin đăng nhập không hợp lệ vui lòng kiểm tra lại.");
        }

        public ResultDataV2 CreateInvoice(IssueCreateV2 data)
        {
            bool flag = data != null;
            if (!flag)
            {
                throw new Exception("Thông tin hóa đơn không hợp lệ vui lòng kiểm tra lại.");
            }
            string fullapi = Base.RequestUriStore.CombileUrl(this.Api, UriStore.EinvoiceIssueUrl);
            ResultDataV2 resultDataV = ApiConsumer.CreateRequest<ResultDataV2>(fullapi, this.Token, data);
            bool flag2 = resultDataV != null && resultDataV.Status == "1" && resultDataV.InvoiceResult != null;
            if (flag2)
            {
                return resultDataV;
            }
            throw new Exception("Tạo hóa đơn điện tử thất bại");
        }

        public bool DeleteInvoice(string pattern, string key)
        {
            bool flag = !string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(key);
            if (!flag)
            {
                throw new Exception("Thông tin hủy hóa đơn không hợp lệ vui lòng kiểm tra lại.");
            }
            var objData = new
            {
                Pattern = pattern,
                Ikey = key
            };
            string fullapi = Base.RequestUriStore.CombileUrl(this.Api, UriStore.EinvoiceCancel);
            ResultDataV2 resultDataV = ApiConsumer.CreateRequest<ResultDataV2>(fullapi, this.Token, objData);
            bool flag2 = resultDataV != null && resultDataV.Status == "1";
            if (flag2)
            {
                return true;
            }
            throw new Exception("Hủy hóa đơn điện tử thất bại");
        }

        public string GetInvoice(string pattern, string key)
        {
            bool flag = !string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(key);
            if (!flag)
            {
                throw new Exception("Thông tin tải hóa đơn không hợp lệ vui lòng kiểm tra lại.");
            }
            var objData = new
            {
                Pattern = pattern,
                Ikey = key,
                Type = "PDF"
            };
            string fullapi = Base.RequestUriStore.CombileUrl(new string[]
            {
                this.Api,
                UriStore.EinvoiceDownload
            });
            ResultDataV2 resultDataV = ApiConsumer.CreateRequest<ResultDataV2>(fullapi, this.Token, objData);
            bool flag2 = resultDataV != null && resultDataV.Status == "1" && !string.IsNullOrEmpty(resultDataV.Data);
            if (flag2)
            {
                return resultDataV.Data;
            }
            throw new Exception("Tải hóa đơn điện tử thất bại");
        }
    }
}

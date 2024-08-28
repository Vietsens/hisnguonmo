using Inventec.Common.ElectronicBillViettel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel
{
    public class ElectronicBillViettelManager
    {
        DataInitApi DataApi;

        public ElectronicBillViettelManager(DataInitApi dataApi)
        {
            this.DataApi = dataApi;
        }

        public Response Run(object data)
        {
            Response result = new Response();
            IRun iRun = null;
            try
            {
                if (this.Check(ref result))
                {
                    iRun = new ElectronicBillViettelProcessor(DataApi);
                    result = iRun != null ? iRun.Run(data) : null;
                }
            }
            catch (Exception ex)
            {
                result = new Response();
                result.errorCode = Base.Constants.ErrorCode;
                result.description = ex.Message;
            }
            return result;
        }

        private bool Check(ref Response resp)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (this.DataApi == null || String.IsNullOrWhiteSpace(this.DataApi.VIETTEL_Address))
                {
                    mess = "Không xác định được địa chỉ hệ thống hóa đơn điện tử";
                }
                else if (String.IsNullOrWhiteSpace(this.DataApi.Pass) || String.IsNullOrWhiteSpace(this.DataApi.User))
                {
                    mess = "Tài khoản đăng nhập hệ thống hóa đơn điện tử không được trống";
                }
                else if (String.IsNullOrWhiteSpace(this.DataApi.SupplierTaxCode))
                {
                    mess = "Mã số thuế không được trống";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    resp.errorCode = Base.Constants.ErrorCode;
                    resp.description = mess;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

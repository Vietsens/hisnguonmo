using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams
{
    public class EBillSoftDreamsManager
    {
        DataInitApi DataApi;

        public EBillSoftDreamsManager(DataInitApi dataApi)
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
                    iRun = new EBillSoftDreamsProcessor(DataApi);
                    result = iRun != null ? iRun.Run(data) : null;
                }
            }
            catch (Exception ex)
            {
                result = new Response();
                result.Success = false;
                result.Messages.Add("Lỗi dữ liệu. " + ex.Message);
                throw ex;
            }
            return result;
        }

        private bool Check(ref Response resp)
        {
            bool result = true;
            try
            {
                string mess = "";

                if (this.DataApi == null || String.IsNullOrWhiteSpace(this.DataApi.Address))
                {
                    mess = "Không xác định được địa chỉ hệ thống hóa đơn điện tử";
                }
                else if (String.IsNullOrWhiteSpace(this.DataApi.User))
                {
                    mess = "Không xác định được tài khoản đăng nhập";
                }
                else if (String.IsNullOrWhiteSpace(this.DataApi.Pass))
                {
                    mess = "Không xác định được mật khẩu đăng nhập";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    resp.Success = false;
                    resp.Messages.Add(mess);
                }
            }
            catch (Exception ex)
            {
                result = false;
                resp.Success = false;
                resp.Messages.Add("Lỗi kiểm tra dữ liệu. " + ex.Message);
            }
            return result;
        }
    }
}

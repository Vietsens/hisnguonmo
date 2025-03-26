/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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

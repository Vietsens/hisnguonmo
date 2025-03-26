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
using Inventec.Common.ElectronicBill.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa
{
    public class ElectronicBillMisaManager
    {
        DataInit Data;
        public ElectronicBillMisaManager(DataInit data)
        {
            this.Data = data;
            MappingError.CreateMappingError();
        }

        public Response Run(Type type)
        {
            Response result = new Response();
            try
            {
                if (this.Check(ref result))
                {
                    IRun iRun = ElectronicBillMisaProcessor.GetProcessor(type);
                    result = iRun != null ? iRun.Run(Data) : null;
                }
            }
            catch (Exception)
            {
                result = new Response();
                result.description = "Lỗi xử lý hóa đơn";
                throw;
            }
            return result;
        }

        private bool Check(ref Response resp)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (this.Data == null || String.IsNullOrWhiteSpace(this.Data.BaseUrl))
                {
                    mess = "Không xác định được địa chỉ hệ thống hóa đơn điện tử";
                }
                else if (String.IsNullOrWhiteSpace(this.Data.Pass) || String.IsNullOrWhiteSpace(this.Data.User))
                {
                    mess = "Tài khoản đăng nhập hệ thống hóa đơn điện tử không được trống";
                }
                else if (String.IsNullOrWhiteSpace(this.Data.AppID))
                {
                    mess = "Không xác định được mã bí mật của ứng dụng kết nối với MeInvoice";
                }
                else if (String.IsNullOrWhiteSpace(this.Data.TaxCode))
                {
                    mess = "Không xác định được mã số thuế đăng ký MeInvoice";
                }
                else if (String.IsNullOrWhiteSpace(this.Data.SignUrl))
                {
                    mess = "Không xác định được địa chỉ máy chủ ký số";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
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

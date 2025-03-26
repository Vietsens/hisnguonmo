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

namespace Inventec.Common.ElectronicBill.Misa.Processor
{
    class SignInvoice : IRun
    {
        DataInit Data;

        public SignInvoice()
        {

        }

        Response IRun.Run(DataInit data)
        {
            Response result = new Response();
            try
            {
                this.Data = data;

                if (data.DataSign.GetType() == typeof(SignXml))
                {
                    result = DoSignInvoice((SignXml)data.DataSign);
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

        private Response DoSignInvoice(SignXml dataSign)
        {
            Response result = new Response();
            try
            {
                if (this.CheckSignData(dataSign, ref result))
                {
                    var apiResult = new Base.ApiConsumer(this.Data.SignUrl, this.Data.AppID, this.Data.TaxCode, this.Data.User, this.Data.Pass)
                        .Sign<SignResult>(Base.RequestUriStore.SignXml, dataSign);
                    if (apiResult == null || String.IsNullOrWhiteSpace(apiResult.PayLoad))
                    {
                        throw new Exception("ký số thất bại. " + apiResult.Message);
                    }
                    else
                    {
                        result.XmlData = apiResult.PayLoad;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Response();
                result.description = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool CheckSignData(SignXml signXml, ref Response data)
        {
            bool result = true;
            try
            {
                string mess = "";
                if (signXml == null)
                {
                    mess = "Không có dữ liệu ký";
                }
                else if (String.IsNullOrWhiteSpace(signXml.PinCode))
                {
                    mess = "Không có thông tin mã pin";
                }
                else if (String.IsNullOrWhiteSpace(signXml.XmlContent))
                {
                    mess = "Không có thông tin hóa đơn cần ký";
                }

                if (!String.IsNullOrWhiteSpace(mess))
                {
                    result = false;
                    if (data == null) data = new Model.Response();

                    data.description = mess;
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

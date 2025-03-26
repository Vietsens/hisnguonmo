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
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using System.IO;
using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.ElectronicBill.Base;
using System.ServiceModel;
using Inventec.Common.ElectronicBill.WSPublicVNPT;
using Inventec.Common.ElectronicBill.PorttalServiceVNPT;

namespace Inventec.Common.ElectronicBill
{
    public class ElectronicBillManager
    {
        ElectronicBillInput electronicBillInput { get; set; }

        public ElectronicBillManager(ElectronicBillInput _electronicBillInput)
        {
            this.electronicBillInput = _electronicBillInput;
        }

        public ElectronicBillResult Run(int cmdType)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            IRun iRun = null;
            try
            {
                if (this.Check(ref result))
                {
                    iRun = new ElectronicBillProcessor(this.electronicBillInput);
                    result = iRun != null ? iRun.Run(cmdType) : null;
                }
            }
            catch (Exception)
            {
                result = null;
                throw;
            }
            return result;
        }

        private bool Check(ref ElectronicBillResult invoiceResults)
        {
            bool result = true;
            try
            {
                ElectronicBillResult invoiceResult = new ElectronicBillResult();
                if (String.IsNullOrEmpty(this.electronicBillInput.serviceUrl))
                {
                    invoiceResult.Status = -1;
                    invoiceResult.MessLog = "" + ResultCode.LOI_KHONG_THAY_WEBSERVICE;
                    return false;
                }

                //Check electronicBillInput
                if (electronicBillInput == null || String.IsNullOrEmpty(electronicBillInput.account) || String.IsNullOrEmpty(electronicBillInput.acPass))
                {
                    invoiceResult.MessLog = "ERR:1";
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
    }
}

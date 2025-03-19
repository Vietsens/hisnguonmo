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
using System.Web;

namespace Inventec.Common.ElectronicBill.Misa
{
    class ElectronicBillMisaProcessor
    {
        public static IRun GetProcessor(Type type)
        {
            IRun result = null;
            switch (type)
            {
                case Type.CreateInvoice:
                    result = new Processor.CreateInvoice();
                    break;
                case Type.PreviewInvoice:
                    result = new Processor.PreviewInvoice();
                    break;
                case Type.SignInvoice:
                    result = new Processor.SignInvoice();
                    break;
                case Type.ReleaseInvoice:
                    result = new Processor.ReleaseInvoice();
                    break;
                case Type.ViewInvoice:
                    result = new Processor.ViewInvoice();
                    break;
                case Type.DeleteInvoice:
                    result = new Processor.DeleteInvoice();
                    break;
                case Type.ConvertInvoice:
                    result = new Processor.ConvertInvoice();
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}

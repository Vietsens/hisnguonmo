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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class ElectronicBillType
    {
        public enum ENUM
        { 
            CREATE_INVOICE,
            GET_INVOICE_LINK,
            DELETE_INVOICE,
            CANCEL_INVOICE,
            CONVERT_INVOICE,
            CREATE_INVOICE_DATA,
            GET_INVOICE_INFO,
            GET_INVOICE_SHOW, 
            REPLACE_INVOICE, 
            RESEARCH_INVOICE
            //ImportAndPublishInv,
            //downloadInvPDFFkeyNoPay
        }
    }
}

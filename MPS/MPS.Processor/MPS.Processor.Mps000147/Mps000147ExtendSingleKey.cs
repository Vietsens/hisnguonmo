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
using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000147
{
    class Mps000147ExtendSingleKey : CommonKey
    {
        internal static string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal static string SERVICE_TYPE_NAMEs = "SERVICE_TYPE_NAMEs";
        internal static string TRANSACTION_CODE_BAR = "TRANSACTION_CODE_BAR";
        internal static string AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST = "AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST";
        internal static string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";
        internal static string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";
        /// <summary>So tien da thu tach theo tung loai dich vu - CHI loai co tien > 0. VD: Kham(40.600); Thuoc(329.680)</summary>
        internal const string SERVICE_TYPE_AMOUNTs = "SERVICE_TYPE_AMOUNTs";
        /// <summary>So tien da thu tach theo tung loai dich vu - MOI loai co phat sinh, ke ca tien = 0</summary>
        internal const string SERVICE_TYPE_AMOUNT_ALLs = "SERVICE_TYPE_AMOUNT_ALLs";
        /// <summary>QR ma tra cuu hoa don dien tu - chua So bao mat (INVOICE_LOOKUP_CODE)</summary>
        internal const string INVOICE_LOOKUP_CODE_QR = "INVOICE_LOOKUP_CODE_QR";
        /// <summary>QR ma tra cuu hoa don dien tu - chua duong dan tra cuu (EINVOICE_URL)</summary>
        internal const string EINVOICE_URL_QR = "EINVOICE_URL_QR";
    }
}

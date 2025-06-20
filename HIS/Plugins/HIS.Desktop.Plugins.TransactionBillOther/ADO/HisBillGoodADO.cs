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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBillOther.ADO
{
    public class HisBillGoodADO : HIS_BILL_GOODS
    {
        public decimal TOTAL_PRICE { get; set; }
        public decimal TOTAL_PRICE_WITH_DISCOUNT { get; set; }
        public decimal TOTAL_PRICE_WITH_VAT { get; set; }

        public bool IsFromTreatmentNoneMediService { get; set; }

        public string ErrorText { get; set; }
        public bool Error { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorType { get; set; }
        public string ErrorColumnName { get; set; }
    }
}

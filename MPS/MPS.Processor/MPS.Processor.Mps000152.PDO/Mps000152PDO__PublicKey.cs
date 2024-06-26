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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000152.PDO
{
    public partial class Mps000152PDO : RDOBase
    {
        public V_HIS_INVOICE _invoice = null;
        public List<InvoiceDetailADO> _listInvoiceDetailsADOs = null;
        public List<HIS_INVOICE_DETAIL> _listInvoiceDetails = null;
        public List<TotalNextPage> _totalNextPageSdos = null;
        public List<string> _titles = null;
        public List<HIS_PAY_FORM> _payForm = null;
    }

    public class InvoiceDetailADO : MOS.EFMODEL.DataModels.HIS_INVOICE_DETAIL
    {
        public long? PageId { get; set; }
        public int NUM_ORDER { get; set; }
    }

    public class TotalNextPage
    {
        public long id { get; set; }
        public string Name { get; set; }
    }
}

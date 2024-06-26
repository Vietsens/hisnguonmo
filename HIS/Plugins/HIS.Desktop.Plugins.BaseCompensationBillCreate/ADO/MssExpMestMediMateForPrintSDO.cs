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

namespace HIS.Desktop.Plugins.BaseCompensationBillCreate.ADO
{
    public class MssExpMestMediMateForPrintSDO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        public string EXPIRED_DATE_STR { get; set; }
        public string EXP_TIME_STR { get; set; }
        public decimal VAT_RATIO_100 { get; set; }
        public decimal? IMP_VAT_RATIO_100 { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public decimal SUM_TOTAL_IMP_PRICE { get; set; }
        public decimal SUM_TOTAL_PRICE { get; set; }
    }
}

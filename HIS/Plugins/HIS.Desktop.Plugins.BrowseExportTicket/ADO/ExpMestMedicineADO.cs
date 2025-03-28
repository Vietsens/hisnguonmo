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

namespace HIS.Desktop.Plugins.BrowseExportTicket.ADO
{
    public class ExpMestMedicineADO : V_HIS_EXP_MEST_METY_REQ
    {
        public int Action { get; set; }
        public long? ID_GRID { get; set; }
        public decimal? SUM_IN_STOCK { get; set; }
        public decimal YC_AMOUNT { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public bool isCheck { get; set; }
        public decimal YCD_AMOUNT { get; set; }
        public bool IS_ALLOW_EXPORT_ODD { get; set; }
        public string REPLACE_MEDICINE_TYPE_NAME { get; set; }
        public long? REPLACE_MEDICINE_TYPE_ID { get; set; }
        public bool IsReplace { get; set; }
        public decimal CURRENT_DD_AMOUNT { get; set; }
        public decimal CURRENT_YC_AMOUNT { get; set; }
        public bool IsApproved { get; set; }
        public decimal? TT_AMOUNT { get; set; }
        public decimal TON_KHO { get; set; }
        public long? MEDICINE_ID { get; set; }
    }
}

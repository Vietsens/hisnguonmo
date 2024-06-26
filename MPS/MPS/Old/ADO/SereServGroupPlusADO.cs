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

namespace MPS.ADO
{
    public class SereServGroupPlusADO : V_HIS_SERE_SERV
    {
        public string KTC_FEE_GROUP_NAME { get; set; }
        public decimal TOTAL_PRICE_GROUP { get; set; }
        public string EXECUTE_GROUP_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public decimal TOTAL_PRICE_SERVICE_GROUP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_SERVICE_GROUP { get; set; }
        public decimal TOTAL_HEIN_PRICE_SERVICE_GROUP { get; set; }

        public decimal TOTAL_PRICE_DEPARTMENT_GROUP { get; set; }
        public decimal TOTAL_PATIENT_PRICE_DEPARTMENT_GROUP { get; set; }
        public decimal TOTAL_HEIN_PRICE_DEPARTMENT_GROUP { get; set; }

        public decimal VIR_TOTAL_PRICE_KTC { get; set; }
        public decimal VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_SUM_END { get; set; }

        public decimal VIR_TOTAL_PRICE_KTC_HIGHTTECH_GROUP { get; set; }
        public decimal VIR_TOTAL_HEIN_PRICE_HIGHTTECH_GROUP { get; set; }
        public decimal VIR_TOTAL_PATIENT_PRICE_HIGHTTECH_GROUP { get; set; }

        public decimal ROW_POS { get; set; }
        public decimal PRICE_BHYT { get; set; }
        public decimal PRICE_POLICY { get; set; }

        public long DEPARTMENT__GROUP_SERE_SERV { get; set; }
        public long DEPARTMENT__GROUP_SERVICE_REPORT { get; set; }
        public long SERE_SERV__GROUP_SERVICE_REPORT { get; set; }
        
    }
}

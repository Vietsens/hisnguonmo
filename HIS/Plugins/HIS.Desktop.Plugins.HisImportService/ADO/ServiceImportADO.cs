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

namespace HIS.Desktop.Plugins.HisImportService.ADO
{
    public class ServiceImportADO : V_HIS_SERVICE
    {
        public string NUM_ORDER_STR { get; set; }
        public string COGS_STR { get; set; }
        public string ESTIMATE_DURATION_STR { get; set; }
        public string HEIN_LIMIT_RATIO_OLD_STR { get; set; }
        public string HEIN_LIMIT_RATIO_STR { get; set; }
        public string HEIN_LIMIT_PRICE_STR { get; set; }
        public string HEIN_LIMIT_PRICE_OLD_STR { get; set; }
        public string PACKAGE_PRICE_STR { get; set; }

        public string CPNG { get; set; }
        public string MultiRequest { get; set; }
        public string ICD_CM_CODE { get; set; }
        public string PTTT_GROUP_CODE { get; set; }
        public string PTTT_METHOD_CODE { get; set; }
        public string PACKAGE_CODE { get; set; }
        public string BILL_PATIENT_TYPE_CODE { get; set; }
        public string HEIN_LIMIT_PRICE_IN_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_INTR_TIME_STR { get; set; }
        public string BILL_OPTION_STR { get; set; }
        public string PARENT_CODE { get; set; }

        public string GENDER_CODE { get; set; }
        public string GENDER_NAME { get; set; }

        public string ERROR { get; set; }
        public string ALLOW_SEND_PACS_STR { get; set; }
        public string EMR_FORM_CODES { get; set; }
        
        public ServiceImportADO()
        {
        }

        public ServiceImportADO(V_HIS_SERVICE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<ServiceImportADO>(this, data);
        }
    }
}

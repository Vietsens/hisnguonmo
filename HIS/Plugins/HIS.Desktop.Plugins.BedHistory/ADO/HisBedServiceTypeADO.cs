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

namespace HIS.Desktop.Plugins.BedHistory.ADO
{
    class HisBedServiceTypeADO : MOS.EFMODEL.DataModels.V_HIS_BED_LOG
    {
        public decimal AMOUNT { get; set; }
        //public string PATIENT_TYPE_CODE { get; set; }
        //public long PATIENT_TYPE_ID { get; set; }
        //public string PATIENT_TYPE_NAME { get; set; }
        public bool? IsExpend { get; set; }
        public bool? IsKHBHYT { get; set; }
        public bool? IsOutKtcFee { get; set; }
        public decimal TotalPrice { get; set; }
        public long? AmmoutNamGhep { get; set; }
        public string BED_SERVICE_TYPE_NAME { get; set; }
        //public long? PRIMARY_PATIENT_TYPE_ID { get; set; }
        //public string PRIMARY_PATIENT_TYPE_NAME { get; set; }
        public long? BILL_PATIENT_TYPE_ID { get; set; }
        public DateTime IntructionTime { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public long? OTHER_PAY_SOURCE_ID { get; set; }
        public bool HasConfigOtherSourcePay { get; set; }
        public bool IsContainAppliedPatientType { get; set; }
        public bool IsBedStretcher { get; set; }
        public bool IsSplitDayOrResult { get; set; }
    }
}

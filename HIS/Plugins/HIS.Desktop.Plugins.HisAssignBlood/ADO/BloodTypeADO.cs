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

namespace HIS.Desktop.Plugins.HisAssignBlood.ADO
{
    class BloodTypeADO : MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE
    {
        public BloodTypeADO()
        {

        }
        public BloodTypeADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD expMestMedicine)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<BloodTypeADO>(this, expMestMedicine);
                this.ID = expMestMedicine.BLOOD_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public long TDL_EXECUTE_BRANCH_ID { get; set; }
        public decimal? HEIN_LIMIT_PRICE_OLD { get; set; }
        public decimal? HEIN_LIMIT_PRICE
        {
            get;
            set;
        }

        public long? HEIN_LIMIT_PRICE_IN_TIME { get; set; }
        public long? HEIN_LIMIT_PRICE_INTR_TIME { get; set; }
        public double IdRow { get; set; }
        public decimal? AMOUNT { get; set; }
        public long? BLOOD_ABO_ID { get; set; }
        public long? BLOOD_RH_ID { get; set; }
        public decimal AMOUNT_CHECK { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? TOT_PRICE { get; set; }
        public decimal? VAT { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_RATIO { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public bool IsExpend { get; set; }
        public bool IsOutParentFee { get; set; }
        public decimal TotalPrice { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeAmount { get; set; }
        public string ErrorMessageAmount { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePatientTypeId { get; set; }
        public string ErrorMessagePatientTypeId { get; set; }
    }
}

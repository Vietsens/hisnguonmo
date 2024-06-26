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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier.ADO
{
    public class MediMateADO
    {
        public long SERVICE_ID { get; set; }
        //public long MEDI_MATE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public decimal? AVAILABLE_AMOUNT { get; set; }
        public decimal EXP_AMOUNT { get; set; }
        public decimal OLD_AMOUNT { get; set; }
        public string NOTE { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_RATIO { get; set; }
        public decimal? EXP_PRICE { get; set; }
        public decimal? EXP_VAT_RATIO { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? TOTAL_PRICE_PROFIT { get; set; }
        public string TUTORIAL { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public bool IsMedicine { get; set; }
        public bool IsMaterial { get; set; }
        public bool IsNotInStock { get; set; }
        public bool IsExceedsAvailable { get; set; }
        public bool IsNotHasServicePaty { get; set; }
        public bool IsCheckExpPrice { get; set; }
        public List<long> BeanIds { get; set; }
        public long? ExpMestDetailId { get; set; }
        public long? DayNum { get; set; }
        public long? USE_TIME_TO { get; set; }
        public decimal? Profit { get; set; }

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }

        public MediMateADO() { }

        public MediMateADO(MetyMatyInStockADO inStockAdo)
        {
            //ConstantUtil
            this.SERVICE_ID = inStockAdo.ServiceId;
            this.ACTIVE_INGR_BHYT_CODE = inStockAdo.ActiveIngrBhytCode;
            this.ACTIVE_INGR_BHYT_NAME = inStockAdo.ActiveIngrBhytName;
            this.AVAILABLE_AMOUNT = inStockAdo.AvailableAmount;
            this.CONCENTRA = inStockAdo.Concentra;
            this.IsMaterial = !inStockAdo.IsMedicine;
            this.IsMedicine = inStockAdo.IsMedicine;
            this.MANUFACTURER_NAME = inStockAdo.ManufacturerName;
            this.MEDI_MATE_TYPE_CODE = inStockAdo.MedicineTypeCode;
            this.MEDI_MATE_TYPE_ID = inStockAdo.Id;
            this.MEDI_MATE_TYPE_NAME = inStockAdo.MedicineTypeName;
            this.NATIONAL_NAME = inStockAdo.NationalName;
            this.REGISTER_NUMBER = inStockAdo.RegisterNumber;
            this.SERVICE_UNIT_NAME = inStockAdo.ServiceUnitName;
        }

        public MediMateADO(HIS_SERVICE_REQ_MATY maty)
        {
            try
            {
                this.IsMaterial = true;
                this.MEDI_MATE_TYPE_ID = maty.MATERIAL_TYPE_ID ?? 0;
                this.MEDI_MATE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                this.EXP_AMOUNT = maty.AMOUNT;                
                this.DayNum = 1;

                V_HIS_MATERIAL_TYPE type = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == maty.MATERIAL_TYPE_ID.Value);
                if (type != null)
                {
                    this.MEDI_MATE_TYPE_CODE = type.MATERIAL_TYPE_CODE;
                    this.SERVICE_UNIT_NAME = type.SERVICE_UNIT_CODE;
                    this.CONCENTRA = type.CONCENTRA;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMateADO(HIS_SERVICE_REQ_METY mety)
        {
            try
            {
                this.IsMedicine = true;
                this.MEDI_MATE_TYPE_ID = mety.MEDICINE_TYPE_ID ?? 0;
                this.MEDI_MATE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                this.EXP_AMOUNT = mety.AMOUNT;
                this.DayNum = 1;
                V_HIS_MEDICINE_TYPE type = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == mety.MEDICINE_TYPE_ID.Value);
                if (type != null)
                {
                    this.MEDI_MATE_TYPE_CODE = type.MEDICINE_TYPE_CODE;
                    this.SERVICE_UNIT_NAME = type.SERVICE_UNIT_CODE;
                    this.CONCENTRA = type.CONCENTRA;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

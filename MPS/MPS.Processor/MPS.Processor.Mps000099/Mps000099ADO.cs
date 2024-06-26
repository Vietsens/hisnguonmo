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

namespace MPS.Processor.Mps000099
{
    public class Mps000099ADO
    {

        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long? TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PRESCRIPTION_CODE { get; set; }
        public string TDL_SERVICE_REQ_CODE { get; set; }
        public string TDL_TREATMENT_CODE { get; set; }

        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string TUTORIAL { get; set; }
        public decimal AMOUNT { get; set; }

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MORNING { get; set; }
        public string NOON { get; set; }
        public string AFTERNOON { get; set; }
        public string EVENING { get; set; }
        public long? DAY_COUNT { get; set; }
        public string BID_NAME { get; set; }
        public string BID_NUMBER { get; set; }
        public string BREATH_SPEED { get; set; }
        public string BREATH_TIME { get; set; }
        public decimal? SPEED { get; set; }
        public string BYT_NUM_ORDER { get; set; }
        public string CONCENTRA { get; set; }
        public string DESCRIPTION { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public string HTU_NAME { get; set; }
        public short? IS_FUNCTIONAL_FOOD { get; set; }
        public short? IS_NOT_PRES { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MEDICINE_REGISTER_NUMBER { get; set; }
        public long? MEDICINE_TYPE_NUM_ORDER { get; set; }
        public string MEDICINE_USE_FORM_CODE { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public long? USE_TIME_TO { get; set; }

        public Mps000099ADO(V_HIS_EXP_MEST expMest, V_HIS_EXP_MEST_MEDICINE medicine)
        {
            this.AMOUNT = Convert.ToDecimal(String.Format("{0: #.##}", medicine.AMOUNT));
            this.MEDICINE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
            this.MEDICINE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
            this.TUTORIAL = medicine.TUTORIAL;

            this.TDL_PATIENT_ADDRESS = expMest.TDL_PATIENT_ADDRESS;
            this.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
            this.TDL_PATIENT_DOB = expMest.TDL_PATIENT_DOB;
            this.TDL_PATIENT_GENDER_NAME = expMest.TDL_PATIENT_GENDER_NAME;
            this.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
            this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
            this.TDL_PRESCRIPTION_CODE = expMest.TDL_PRESCRIPTION_CODE;
            this.TDL_SERVICE_REQ_CODE = expMest.TDL_SERVICE_REQ_CODE;
            this.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
            this.ACTIVE_INGR_BHYT_CODE = medicine.ACTIVE_INGR_BHYT_CODE;
            this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
            this.AFTERNOON = medicine.AFTERNOON;
            this.SERVICE_UNIT_CODE = medicine.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
            this.MORNING = medicine.MORNING;
            this.NOON = medicine.NOON;
            this.EVENING = medicine.EVENING;
            this.DAY_COUNT = medicine.DAY_COUNT;
            this.AMOUNT = Convert.ToDecimal(String.Format("{0: #.##}", medicine.AMOUNT));
            this.BID_NAME = medicine.BID_NAME;
            this.BID_NUMBER = medicine.BID_NUMBER;
            this.BREATH_SPEED = medicine.BREATH_SPEED;
            this.BREATH_TIME = medicine.BREATH_TIME;
            this.SPEED = medicine.SPEED;
            this.BYT_NUM_ORDER = medicine.BYT_NUM_ORDER;
            this.CONCENTRA = medicine.CONCENTRA;
            this.DESCRIPTION = medicine.DESCRIPTION;
            this.EXPIRED_DATE = medicine.EXPIRED_DATE;
            this.HTU_NAME = medicine.HTU_NAME;
            this.IS_FUNCTIONAL_FOOD = medicine.IS_FUNCTIONAL_FOOD;
            this.IS_NOT_PRES = medicine.IS_NOT_PRES;
            this.MANUFACTURER_CODE = medicine.MANUFACTURER_CODE;
            this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
            this.MEDICINE_REGISTER_NUMBER = medicine.MEDICINE_REGISTER_NUMBER;
            this.MEDICINE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
            this.MEDICINE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
            this.MEDICINE_TYPE_NUM_ORDER = medicine.MEDICINE_TYPE_NUM_ORDER;
            this.MEDICINE_USE_FORM_CODE = medicine.MEDICINE_USE_FORM_CODE;
            this.MEDICINE_USE_FORM_NAME = medicine.MEDICINE_USE_FORM_NAME;
            this.NATIONAL_NAME = medicine.NATIONAL_NAME;
            this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
            this.PATIENT_TYPE_CODE = medicine.PATIENT_TYPE_CODE;
            this.PATIENT_TYPE_NAME = medicine.PATIENT_TYPE_NAME;
            this.SUPPLIER_CODE = medicine.SUPPLIER_CODE;
            this.SUPPLIER_NAME = medicine.SUPPLIER_NAME;
            this.USE_TIME_TO = medicine.USE_TIME_TO;
        }

        public Mps000099ADO(V_HIS_EXP_MEST expMest, V_HIS_EXP_MEST_MATERIAL material)
        {
            this.AMOUNT = Convert.ToDecimal(String.Format ("{0: #.##}", material.AMOUNT));
            this.MEDICINE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
            this.MEDICINE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
            this.TUTORIAL = material.TUTORIAL;

            this.TDL_PATIENT_ADDRESS = expMest.TDL_PATIENT_ADDRESS;
            this.TDL_PATIENT_CODE = expMest.TDL_PATIENT_CODE;
            this.TDL_PATIENT_DOB = expMest.TDL_PATIENT_DOB;
            this.TDL_PATIENT_GENDER_NAME = expMest.TDL_PATIENT_GENDER_NAME;
            this.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = expMest.TDL_PATIENT_IS_HAS_NOT_DAY_DOB;
            this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
            this.TDL_PRESCRIPTION_CODE = expMest.TDL_PRESCRIPTION_CODE;
            this.TDL_SERVICE_REQ_CODE = expMest.TDL_SERVICE_REQ_CODE;
            this.TDL_TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
            this.SERVICE_UNIT_CODE = material.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
            this.BID_NAME = material.BID_NAME;
            this.IS_NOT_PRES = material.IS_NOT_PRES;
            this.MANUFACTURER_CODE = material.MANUFACTURER_CODE;
            this.AMOUNT = Convert.ToDecimal(String.Format ("{0: #.##}", material.AMOUNT));
            this.BID_NUMBER = material.BID_NUMBER;
            this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
            this.NATIONAL_NAME = material.NATIONAL_NAME;
            this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
            this.DESCRIPTION = material.DESCRIPTION;
            this.EXPIRED_DATE = material.EXPIRED_DATE;
            this.PATIENT_TYPE_NAME = material.PATIENT_TYPE_NAME;
            this.SUPPLIER_NAME = material.SUPPLIER_NAME;
            this.SUPPLIER_CODE = material.SUPPLIER_CODE;
            this.PATIENT_TYPE_CODE = material.PATIENT_TYPE_CODE;
        }
    }
}

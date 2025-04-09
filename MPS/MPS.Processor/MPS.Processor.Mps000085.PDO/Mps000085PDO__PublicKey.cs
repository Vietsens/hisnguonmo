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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000085.PDO
{
    public partial class Mps000085PDO : RDOBase
    {
        public V_HIS_BID _bid = null;
        public V_HIS_IMP_MEST _ImpMest = null;
        public HIS_SUPPLIER _supplier = null;
        public List<V_HIS_IMP_MEST_BLOOD> _ListImpMestBlood = null;
        public List<BLOODADO> _ListAdo = new List<BLOODADO>();

        public class BLOODADO : V_HIS_IMP_MEST_BLOOD
        {
            public decimal AMOUNT { get; set; }
            public decimal PRICE { get; set; }
            public string PRICE_SEPARATE { get; set; }

            public BLOODADO() { }

            public BLOODADO(V_HIS_IMP_MEST_BLOOD impBlood)
            {
                try
                {
                    if (impBlood != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<BLOODADO>(this, impBlood);
                        this.AMOUNT = 1;
                        this.PRICE = impBlood.IMP_PRICE;
                        this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }
    }

    public class Mps000085ADO
    {
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string CONCENTRA { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal IMP_VAT_RATIO_100 { get; set; }
        public decimal PRICE_AMOUNT { get; set; }
        public string PRICE_AMOUNT_SEPARATE { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string BID_NAME { get; set; }
        public string BID_NUMBER { get; set; }
        public string BID_YEAR { get; set; }
        public string TDL_BID_GROUP_CODE { get; set; }
        public string TDL_BID_NUM_ORDER { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public string TDL_BID_YEAR { get; set; }
        public string TDL_BID_PACKAGE_CODE { get; set; }
        public decimal? VIR_PRICE { get; set; }
        public decimal? VIR_IMP_PRICE { get; set; }
        public string RECORDING_TRANSACTION { get; set; }

        public string MEDICAL_CONTRACT_CODE { get; set; }
        public string MEDICAL_CONTRACT_NAME { get; set; }

        public string MEDICINE_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }

        public decimal? IMP_UNIT_AMOUNT { get; set; }
        public decimal? IMP_UNIT_PRICE { get; set; }
        public long? TDL_IMP_UNIT_ID { get; set; }
        public string IMP_UNIT_NAME { get; set; }

        public string DOCUMENT_SUPPLIER_NAME { get; set; }
        public string VENTURE_AGREENING { get; set; }

        public long? BID_VALID_FROM_TIME { get; set; }
        public long? BID_VALID_TO_TIME { get; set; }

        public string BATCH_REGISTER_NUMBER { get; set; }
        public string BATCH_MANUFACTURER_CODE { get; set; }
        public string BATCH_MANUFACTURER_NAME { get; set; }
        //public Mps000085ADO(V_HIS_IMP_MEST_MEDICINE medicine)
        //{
        //    try
        //    {
        //        if (medicine != null)
        //        {
        //            this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
        //            this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
        //            this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
        //            this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
        //            this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicine.EXPIRED_DATE ?? 0);
        //            this.AMOUNT = medicine.AMOUNT;
        //            this.PRICE = medicine.PRICE;
        //            this.VAT_RATIO_100 = (medicine.VAT_RATIO.HasValue) ? (medicine.VAT_RATIO.Value * 100) : 0;
        //            this.IMP_VAT_RATIO_100 = medicine.IMP_VAT_RATIO * 100;
        //            this.PRICE_AMOUNT = medicine.AMOUNT * (medicine.PRICE ?? 0) * (1 + (medicine.IMP_VAT_RATIO));
        //            this.PRICE_AMOUNT_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
        //            this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
        //            this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
        //            this.NATIONAL_NAME = medicine.NATIONAL_NAME;
        //            this.PACKING_TYPE_NAME = medicine.PACKING_TYPE_NAME;
        //            this.SUPPLIER_NAME = medicine.SUPPLIER_NAME;
        //            this.BID_NAME = medicine.BID_NAME;
        //            this.BID_NUMBER = medicine.BID_NUMBER;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}


        //public Mps000085ADO(V_HIS_IMP_MEST_MATERIAL material)
        //{
        //    try
        //    {
        //        if (material != null)
        //        {
        //            this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
        //            this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
        //            //this.REGISTER_NUMBER = material.REGISTER_NUMBER;
        //            this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
        //            this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(material.EXPIRED_DATE ?? 0);
        //            this.AMOUNT = material.AMOUNT;
        //            this.PRICE = material.PRICE;
        //            this.VAT_RATIO_100 = (material.VAT_RATIO.HasValue) ? (material.VAT_RATIO.Value * 100) : 0;
        //            this.IMP_VAT_RATIO_100 = material.IMP_VAT_RATIO * 100;
        //            this.PRICE_AMOUNT = material.AMOUNT * (material.PRICE ?? 0) * (1 + (material.IMP_VAT_RATIO));
        //            this.PRICE_AMOUNT_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
        //            this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
        //            this.NATIONAL_NAME = material.NATIONAL_NAME;
        //            this.PACKING_TYPE_NAME = material.PACKING_TYPE_NAME;
        //            this.SUPPLIER_NAME = material.SUPPLIER_NAME;
        //            this.BID_NAME = material.BID_NAME;
        //            this.BID_NUMBER = material.BID_NUMBER;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        public Mps000085ADO(ImpMestMedicineADO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                    this.CONCENTRA = medicine.CONCENTRA;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicine.EXPIRED_DATE ?? 0);
                    this.AMOUNT = medicine.AMOUNT;
                    this.PRICE = medicine.PRICE;
                    this.VAT_RATIO_100 = (medicine.VAT_RATIO.HasValue) ? (medicine.VAT_RATIO.Value * 100) : 0;
                    this.IMP_VAT_RATIO_100 = medicine.IMP_VAT_RATIO * 100;
                    this.PRICE_AMOUNT = medicine.AMOUNT * (medicine.PRICE ?? 0) * (1 + (medicine.IMP_VAT_RATIO));
                    this.PRICE_AMOUNT_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.PACKING_TYPE_NAME = medicine.PACKING_TYPE_NAME;
                    this.SUPPLIER_NAME = medicine.SUPPLIER_NAME;
                    this.BID_NAME = medicine.BID_NAME;
                    this.BID_NUMBER = medicine.BID_NUMBER;
                    this.TDL_BID_GROUP_CODE = medicine.TDL_BID_GROUP_CODE;
                    this.TDL_BID_NUM_ORDER = medicine.TDL_BID_NUM_ORDER;
                    this.TDL_BID_NUMBER = medicine.TDL_BID_NUMBER;
                    this.TDL_BID_PACKAGE_CODE = medicine.TDL_BID_PACKAGE_CODE;
                    this.TDL_BID_YEAR = medicine.TDL_BID_YEAR;
                    this.VIR_PRICE = medicine.VIR_PRICE;
                    this.VIR_IMP_PRICE = medicine.VIR_IMP_PRICE;
                    this.RECORDING_TRANSACTION = medicine.RECORDING_TRANSACTION;
                    this.MEDICAL_CONTRACT_CODE = medicine.MEDICAL_CONTRACT_CODE;
                    this.MEDICAL_CONTRACT_NAME = medicine.MEDICAL_CONTRACT_NAME;
                    this.MEDICINE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.IMP_UNIT_AMOUNT = medicine.IMP_UNIT_AMOUNT;
                    this.IMP_UNIT_PRICE = medicine.IMP_UNIT_PRICE;
                    this.IMP_UNIT_NAME = medicine.IMP_UNIT_NAME;
                    this.TDL_IMP_UNIT_ID = medicine.TDL_IMP_UNIT_ID;
                    this.DOCUMENT_SUPPLIER_NAME = medicine.DOCUMENT_SUPPLIER_NAME;
                    this.VENTURE_AGREENING = medicine.VENTURE_AGREENING;
                    this.BID_VALID_FROM_TIME = medicine.BID_VALID_FROM_TIME;
                    this.BID_VALID_FROM_TIME = medicine.BID_VALID_FROM_TIME;
                    this.BID_VALID_TO_TIME = medicine.BID_VALID_TO_TIME;
                    this.BATCH_REGISTER_NUMBER = medicine.MEDICINE_REGISTER_NUMBER;
                    this.BATCH_MANUFACTURER_CODE = medicine.MEDICINE_MANUFACTURER_CODE;
                    this.BATCH_MANUFACTURER_NAME = medicine.MEDICINE_MANUFACTURER_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public Mps000085ADO(ImpMestMaterialADO material)
        {
            try
            {
                if (material != null)
                {
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.REGISTER_NUMBER = material.MATERIAL_REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(material.EXPIRED_DATE ?? 0);
                    this.AMOUNT = material.AMOUNT;
                    this.PRICE = material.PRICE;
                    this.VAT_RATIO_100 = (material.VAT_RATIO.HasValue) ? (material.VAT_RATIO.Value * 100) : 0;
                    this.IMP_VAT_RATIO_100 = material.IMP_VAT_RATIO * 100;
                    this.PRICE_AMOUNT = material.AMOUNT * (material.PRICE ?? 0) * (1 + (material.IMP_VAT_RATIO));
                    this.PRICE_AMOUNT_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.PACKING_TYPE_NAME = material.PACKING_TYPE_NAME;
                    this.SUPPLIER_NAME = material.SUPPLIER_NAME;
                    this.BID_NAME = material.BID_NAME;
                    this.BID_NUMBER = material.BID_NUMBER;
                    this.TDL_BID_GROUP_CODE = material.TDL_BID_GROUP_CODE;
                    this.TDL_BID_NUM_ORDER = material.TDL_BID_NUM_ORDER;
                    this.TDL_BID_NUMBER = material.TDL_BID_NUMBER;
                    this.TDL_BID_PACKAGE_CODE = material.TDL_BID_PACKAGE_CODE;
                    this.TDL_BID_YEAR = material.TDL_BID_YEAR;
                    this.VIR_PRICE = material.VIR_PRICE;
                    this.VIR_IMP_PRICE = material.VIR_IMP_PRICE;
                    this.RECORDING_TRANSACTION = material.RECORDING_TRANSACTION;
                    this.MEDICAL_CONTRACT_CODE = material.MEDICAL_CONTRACT_CODE;
                    this.MEDICAL_CONTRACT_NAME = material.MEDICAL_CONTRACT_NAME;
                    this.MATERIAL_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.IMP_UNIT_AMOUNT = material.IMP_UNIT_AMOUNT;
                    this.IMP_UNIT_PRICE = material.IMP_UNIT_PRICE;
                    this.IMP_UNIT_NAME = material.IMP_UNIT_NAME;
                    this.TDL_IMP_UNIT_ID = material.TDL_IMP_UNIT_ID;
                    this.DOCUMENT_SUPPLIER_NAME = material.DOCUMENT_SUPPLIER_NAME;
                    this.VENTURE_AGREENING = material.VENTURE_AGREENING;
                    this.BID_VALID_FROM_TIME = material.BID_VALID_FROM_TIME;
                    this.BID_VALID_TO_TIME = material.BID_VALID_TO_TIME;
                    this.BATCH_REGISTER_NUMBER = material.MATERIAL_REGISTER_NUMBER;
                    this.BATCH_MANUFACTURER_CODE = material.MATERIAL_MANUFACTURER_CODE;
                    this.BATCH_MANUFACTURER_NAME = material.MATERIAL_MANUFACTURER_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class RoleADO
    {
        public string Role1 { get; set; }
        public string Role2 { get; set; }
        public string Role3 { get; set; }
        public string Role4 { get; set; }
        public string Role5 { get; set; }
        public string Role6 { get; set; }
        public string Role7 { get; set; }
        public string Role8 { get; set; }
        public string Role9 { get; set; }
        public string Role10 { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string User3 { get; set; }
        public string User4 { get; set; }
        public string User5 { get; set; }
        public string User6 { get; set; }
        public string User7 { get; set; }
        public string User8 { get; set; }
        public string User9 { get; set; }
        public string User10 { get; set; }
    }

    public class ImpMestMedicineADO : V_HIS_IMP_MEST_MEDICINE
    {
        public string TDL_BID_GROUP_CODE { get; set; }
        public string TDL_BID_NUM_ORDER { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public string TDL_BID_YEAR { get; set; }
        public string TDL_BID_PACKAGE_CODE { get; set; }
        public decimal? VIR_IMP_PRICE { get; set; }
        public string MEDICAL_CONTRACT_CODE { get; set; }
        public string MEDICAL_CONTRACT_NAME { get; set; }
        public string DOCUMENT_SUPPLIER_NAME { get; set; }
        public string VENTURE_AGREENING { get; set; }
        public string BATCH_REGISTER_NUMBER { get; set; }
        public string BATCH_MANUFACTURER_CODE { get; set; }
        public string BATCH_MANUFACTURER_NAME { get; set; }

    }

    public class ImpMestMaterialADO : V_HIS_IMP_MEST_MATERIAL
    {
        public string TDL_BID_GROUP_CODE { get; set; }
        public string TDL_BID_NUM_ORDER { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public string TDL_BID_YEAR { get; set; }
        public string TDL_BID_PACKAGE_CODE { get; set; }
        public decimal? VIR_IMP_PRICE { get; set; }
        public string MEDICAL_CONTRACT_CODE { get; set; }
        public string MEDICAL_CONTRACT_NAME { get; set; }
        public string DOCUMENT_SUPPLIER_NAME { get; set; }
        public string VENTURE_AGREENING { get; set; }
        public string BATCH_REGISTER_NUMBER { get; set; }
        public string BATCH_MANUFACTURER_CODE { get; set; }
        public string BATCH_MANUFACTURER_NAME { get; set; }
    }

    public class MedicalContractADO : V_HIS_MEDICAL_CONTRACT
    {
        public long MEDICINE_ID { get; set; }
        public long MATERIAL_ID { get; set; }
    }

}

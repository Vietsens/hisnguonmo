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
using HIS.Desktop.Plugins.ExpMestSaleCreate.Config;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.ADO
{
    public class MediMateTypeADO
    {
        public long SERVICE_ID { get; set; }
        public long MEDI_MATE_ID { get; set; }
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
        public decimal? EXP_AMOUNT { get; set; }
        public decimal? OLD_AMOUNT { get; set; }
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

        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public long? EXP_MEST_ID { get; set; }
        public long? SERVICE_REQ_ID { get; set; }

        public string MEDICINE_TYPE_CODE__UNSIGN { get; set; }
        public string MEDICINE_TYPE_NAME__UNSIGN { get; set; }
        public string ACTIVE_INGR_BHYT_NAME__UNSIGN { get; set; }
        public long? NUM_ORDER { get; set; }

        public string CHILD_ID { get; set; }
        public string PARENT_ID { get; set; }
        public bool? IsClone { get; set; }

        public long? PresNumber { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public bool KEY_PRICE_PARENT { get; set; }

        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long TDL_PATIENT_ID { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public long? TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public long TREATMENT_ID { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_PHONE { get; set; }
        public string ClientSessionKey { get; set; }
        public MediMateTypeADO() { }

        public MediMateTypeADO(V_HIS_EMTE_MEDICINE_TYPE mediTemp)
        {
            try
            {
                if (mediTemp != null)
                {
                    this.IsMedicine = true;
                    
                    this.MEDI_MATE_TYPE_ID = mediTemp.MEDICINE_TYPE_ID ?? 0;
                    this.MEDI_MATE_TYPE_CODE = mediTemp.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = mediTemp.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = mediTemp.SERVICE_UNIT_NAME;
                    this.EXP_AMOUNT = mediTemp.AMOUNT;
                    this.SERVICE_REQ_CODE = "000000000000";
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + mediTemp.MEDICINE_TYPE_ID;
                    V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == mediTemp.MEDICINE_TYPE_ID);
                    if (medicineType != null)
                    {
                        this.CONCENTRA = medicineType.CONCENTRA;
                        if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                            this.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", this.MEDI_MATE_TYPE_NAME, this.CONCENTRA);
                        this.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMateTypeADO(HIS_SERVICE_REQ_METY mety, long? intructionTime)
        {
            try
            {
                if (mety != null)
                {
                    this.IsMedicine = true;
                    this.MEDI_MATE_TYPE_ID = mety.MEDICINE_TYPE_ID ?? 0;
                    this.MEDI_MATE_TYPE_NAME = mety.MEDICINE_TYPE_NAME;
                    this.EXP_AMOUNT = mety.AMOUNT;
                    this.USE_TIME_TO = mety.USE_TIME_TO;
                    this.DayNum = 1;
                    this.NUM_ORDER = mety.NUM_ORDER;
                    if (intructionTime.HasValue && mety.USE_TIME_TO.HasValue)
                    {
                        DateTime dtUserTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(mety.USE_TIME_TO.Value).Value;
                        DateTime dtIntrcutionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTime.Value).Value;
                        DateTime dtUT = new DateTime(dtUserTimeTo.Year, dtUserTimeTo.Month, dtUserTimeTo.Day, dtUserTimeTo.Hour, 0, 0);
                        DateTime dtIT = new DateTime(dtIntrcutionTime.Year, dtIntrcutionTime.Month, dtIntrcutionTime.Day, dtIntrcutionTime.Hour, 0, 0);
                        TimeSpan ts = (TimeSpan)(dtUT - dtIT);
                        this.DayNum = (long?)ts.TotalDays + 1;
                    }
                    V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == (mety.MEDICINE_TYPE_ID ?? 0));
                    if (medicineType != null)
                    {
                        this.CONCENTRA = medicineType.CONCENTRA;
                        if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                            this.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", this.MEDI_MATE_TYPE_NAME, this.CONCENTRA);
                        this.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMateTypeADO(HIS_SERVICE_REQ_MATY maty)
        {
            try
            {
                if (maty != null)
                {
                    this.IsMaterial = true;
                    this.MEDI_MATE_TYPE_ID = maty.MATERIAL_TYPE_ID ?? 0;
                    this.MEDI_MATE_TYPE_NAME = maty.MATERIAL_TYPE_NAME;
                    this.EXP_AMOUNT = maty.AMOUNT;
                    this.DayNum = 1;
                    this.NUM_ORDER = maty.NUM_ORDER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMateTypeADO(V_HIS_EMTE_MATERIAL_TYPE mateTemp)
        {
            try
            {
                if (mateTemp != null)
                {
                    this.IsMaterial = true;
                    this.MEDI_MATE_TYPE_ID = mateTemp.MATERIAL_TYPE_ID ?? 0;
                    this.MEDI_MATE_TYPE_CODE = mateTemp.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = mateTemp.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = mateTemp.SERVICE_UNIT_NAME;
                    this.EXP_AMOUNT = mateTemp.AMOUNT;
                    this.SERVICE_REQ_CODE = "000000000000";
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + mateTemp.MATERIAL_TYPE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MediMateTypeADO(HisMedicineTypeInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicine.ServiceId;
                    this.MEDI_MATE_TYPE_ID = medicine.Id;
                    this.MEDI_MATE_TYPE_CODE = medicine.MedicineTypeCode;
                    this.MEDI_MATE_TYPE_NAME = medicine.MedicineTypeName;
                    this.SERVICE_UNIT_NAME = medicine.ServiceUnitName;
                    this.NATIONAL_NAME = medicine.NationalName;
                    this.MANUFACTURER_NAME = medicine.ManufacturerName;
                    this.REGISTER_NUMBER = medicine.RegisterNumber;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.CONCENTRA = medicine.Concentra;
                    this.ACTIVE_INGR_BHYT_CODE = medicine.ActiveIngrBhytCode;
                    this.ACTIVE_INGR_BHYT_NAME = medicine.ActiveIngrBhytName;

                    this.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(this.MEDI_MATE_TYPE_CODE) + this.MEDI_MATE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(this.MEDI_MATE_TYPE_NAME) + this.MEDI_MATE_TYPE_NAME;
                    this.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(this.ACTIVE_INGR_BHYT_NAME) + this.ACTIVE_INGR_BHYT_NAME;
                    if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                        this.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", this.MEDI_MATE_TYPE_NAME, this.CONCENTRA);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisMaterialTypeInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMaterial = true;
                    this.SERVICE_ID = material.ServiceId;
                    this.MEDI_MATE_TYPE_ID = material.Id;
                    this.MEDI_MATE_TYPE_CODE = material.MaterialTypeCode;
                    this.MEDI_MATE_TYPE_NAME = material.MaterialTypeName;
                    this.SERVICE_UNIT_NAME = material.ServiceUnitName;
                    this.NATIONAL_NAME = material.NationalName;
                    this.MANUFACTURER_NAME = material.ManufacturerName;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    this.CONCENTRA = material.Concentra;

                    this.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(this.MEDI_MATE_TYPE_CODE) + this.MEDI_MATE_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(this.MEDI_MATE_TYPE_NAME) + this.MEDI_MATE_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_EXP_MEST_MEDICINE medicine, V_HIS_EXP_MEST expMest, List<HIS_MEDICINE_BEAN> medicineBeans)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = medicine.MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.EXP_AMOUNT = medicine.AMOUNT;
                    this.OLD_AMOUNT = medicine.AMOUNT;
                    this.IsCheckExpPrice = (medicine.IS_USE_CLIENT_PRICE ?? 0) == 1 ? true : false;
                    this.PATIENT_TYPE_ID = medicine.PATIENT_TYPE_ID;
                    if (medicine.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = medicine.DISCOUNT.Value * 100;
                    this.EXP_PRICE = medicine.PRICE;
                    if (medicine.VAT_RATIO.HasValue)
                        this.EXP_VAT_RATIO = medicine.VAT_RATIO;
                    if (medicine.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = medicine.DISCOUNT * 100;
                    this.TOTAL_PRICE = (this.EXP_AMOUNT * this.EXP_PRICE * (1 + (this.EXP_VAT_RATIO ?? 0)) * (1 - (medicine.DISCOUNT ?? 0)));
                    this.ExpMestDetailId = medicine.ID;
                    if (medicineBeans != null && medicineBeans.Count > 0)
                    {
                        List<HIS_MEDICINE_BEAN> medicineBeanByExpMestMedicines = medicineBeans.Where(o => o.EXP_MEST_MEDICINE_ID == medicine.ID).ToList();
                        this.BeanIds = medicineBeanByExpMestMedicines.Select(o => o.ID).ToList();
                    }

                    //Tinh so ngay
                    if (medicine.USE_TIME_TO.HasValue && expMest.TDL_INTRUCTION_TIME.HasValue)
                    {
                        DateTime dtUserTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(medicine.USE_TIME_TO.Value).Value;
                        DateTime dtIntrcutionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMest.TDL_INTRUCTION_TIME.Value).Value;
                        DateTime dtUT = new DateTime(dtUserTimeTo.Year, dtUserTimeTo.Month, dtUserTimeTo.Day, dtUserTimeTo.Hour, 0, 0);
                        DateTime dtIT = new DateTime(dtIntrcutionTime.Year, dtIntrcutionTime.Month, dtIntrcutionTime.Day, dtIntrcutionTime.Hour, 0, 0);
                        TimeSpan ts = (TimeSpan)(dtUT - dtIT);
                        this.DayNum = (long?)ts.TotalDays + 1;
                    }

                    this.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                    this.SERVICE_REQ_CODE = expMest.PRESCRIPTION_ID > 0 ? expMest.TDL_SERVICE_REQ_CODE : expMest.EXP_MEST_CODE;
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + medicine.MEDICINE_TYPE_ID + "_" + medicine.MEDICINE_ID;
                    this.EXP_MEST_ID = expMest.ID;
                    this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                    this.PresNumber = expMest.PRES_NUMBER;
                    this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                    this.NUM_ORDER = medicine.NUM_ORDER;
                    this.CONCENTRA = medicine.CONCENTRA;
                    this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
                    if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                        this.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", this.MEDI_MATE_TYPE_NAME, this.CONCENTRA);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_EXP_MEST_MATERIAL material, V_HIS_EXP_MEST expMest, List<HIS_MATERIAL_BEAN> materialBeans)
        {
            try
            {
                if (material != null)
                {
                    this.IsMaterial = true;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.EXP_AMOUNT = material.AMOUNT;
                    this.OLD_AMOUNT = material.AMOUNT;
                    this.IsCheckExpPrice = (material.IS_USE_CLIENT_PRICE ?? 0) == 1 ? true : false;
                    this.PATIENT_TYPE_ID = material.PATIENT_TYPE_ID;
                    if (material.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = material.DISCOUNT.Value * 100;
                    this.EXP_PRICE = material.PRICE;
                    if (material.VAT_RATIO.HasValue)
                        this.EXP_VAT_RATIO = material.VAT_RATIO;
                    if (material.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = material.DISCOUNT * 100;
                    this.TOTAL_PRICE = (this.EXP_AMOUNT * this.EXP_PRICE * (1 + (this.EXP_VAT_RATIO ?? 0)) * (1 - (material.DISCOUNT ?? 0)));
                    if (materialBeans != null && materialBeans.Count > 0)
                    {
                        List<HIS_MATERIAL_BEAN> materialBeanByExpMestMedicines = materialBeans.Where(o => o.EXP_MEST_MATERIAL_ID == material.ID).ToList();
                        this.BeanIds = materialBeanByExpMestMedicines.Select(o => o.ID).ToList();
                    }

                    this.ExpMestDetailId = material.ID;
                    this.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                    this.SERVICE_REQ_CODE = expMest.PRESCRIPTION_ID > 0 ? expMest.TDL_SERVICE_REQ_CODE : expMest.EXP_MEST_CODE;
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + material.MATERIAL_TYPE_ID + "_" + material.MATERIAL_ID;
                    this.EXP_MEST_ID = expMest.ID;
                    this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                    this.PresNumber = expMest.PRES_NUMBER;
                    this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                    this.NUM_ORDER = material.NUM_ORDER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(
            HIS_EXP_MEST_MATERIAL material,
            HIS_EXP_MEST expMest,
            List<HIS_MATERIAL_BEAN> materialBeans,
            List<V_HIS_MATERIAL_TYPE> _materialTypes
            )
        {
            try
            {
                if (material != null)
                {
                    this.IsMaterial = true;
                    this.MEDI_MATE_TYPE_ID = material.TDL_MATERIAL_TYPE_ID ?? 0;
                    var dataMaterial = _materialTypes.FirstOrDefault(p => p.ID == material.TDL_MATERIAL_TYPE_ID);
                    if (dataMaterial != null)
                    {
                        this.SERVICE_ID = dataMaterial.SERVICE_ID;
                        this.MEDI_MATE_TYPE_CODE = dataMaterial.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_NAME = dataMaterial.MATERIAL_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = dataMaterial.SERVICE_UNIT_NAME;
                        this.NATIONAL_NAME = dataMaterial.NATIONAL_NAME;
                        this.MANUFACTURER_NAME = dataMaterial.MANUFACTURER_NAME;
                    }
                    this.EXP_AMOUNT = material.AMOUNT;
                    this.OLD_AMOUNT = material.AMOUNT;
                    this.IsCheckExpPrice = (material.IS_USE_CLIENT_PRICE ?? 0) == 1 ? true : false;
                    this.PATIENT_TYPE_ID = material.PATIENT_TYPE_ID;
                    if (material.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = material.DISCOUNT.Value * 100;
                    this.EXP_PRICE = material.PRICE;
                    if (material.VAT_RATIO.HasValue)
                        this.EXP_VAT_RATIO = material.VAT_RATIO;
                    if (material.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = material.DISCOUNT * 100;
                    this.TOTAL_PRICE = (this.EXP_AMOUNT * this.EXP_PRICE * (1 + (this.EXP_VAT_RATIO ?? 0)) * (1 - (material.DISCOUNT ?? 0)));
                    if (materialBeans != null && materialBeans.Count > 0)
                    {
                        List<HIS_MATERIAL_BEAN> materialBeanByExpMestMedicines = materialBeans.Where(o => o.EXP_MEST_MATERIAL_ID == material.ID).ToList();
                        this.BeanIds = materialBeanByExpMestMedicines.Select(o => o.ID).ToList();
                    }

                    this.ExpMestDetailId = material.ID;
                    this.SERVICE_REQ_CODE = !string.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE) ? expMest.TDL_SERVICE_REQ_CODE : "000000000000";
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + material.TDL_MATERIAL_TYPE_ID + "_" + material.MATERIAL_ID;
                    this.EXP_MEST_ID = expMest.ID;
                    this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                    this.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                    this.PresNumber = expMest.PRES_NUMBER;
                    this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                    this.NUM_ORDER = material.NUM_ORDER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(
    V_HIS_EXP_MEST_MATERIAL material,
    V_HIS_EXP_MEST expMest,
    List<HIS_MATERIAL_BEAN> materialBeans,
    List<V_HIS_MATERIAL_TYPE> _materialTypes
    )
        {
            try
            {
                if (material != null)
                {
                    this.IsMaterial = true;
                    this.MEDI_MATE_TYPE_ID = material.TDL_MATERIAL_TYPE_ID ?? 0;
                    var dataMaterial = _materialTypes.FirstOrDefault(p => p.ID == material.TDL_MATERIAL_TYPE_ID);
                    if (dataMaterial != null)
                    {
                        this.SERVICE_ID = dataMaterial.SERVICE_ID;
                        this.MEDI_MATE_TYPE_CODE = dataMaterial.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_NAME = dataMaterial.MATERIAL_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = dataMaterial.SERVICE_UNIT_NAME;
                        this.NATIONAL_NAME = dataMaterial.NATIONAL_NAME;
                        this.MANUFACTURER_NAME = dataMaterial.MANUFACTURER_NAME;
                    }
                    this.EXP_AMOUNT = material.AMOUNT;
                    this.OLD_AMOUNT = material.AMOUNT;
                    this.IsCheckExpPrice = (material.IS_USE_CLIENT_PRICE ?? 0) == 1 ? true : false;
                    this.PATIENT_TYPE_ID = material.PATIENT_TYPE_ID;
                    if (material.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = material.DISCOUNT.Value * 100;
                    this.EXP_PRICE = material.PRICE;
                    if (material.VAT_RATIO.HasValue)
                        this.EXP_VAT_RATIO = material.VAT_RATIO;
                    if (material.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = material.DISCOUNT * 100;
                    this.TOTAL_PRICE = (this.EXP_AMOUNT * this.EXP_PRICE * (1 + (this.EXP_VAT_RATIO ?? 0)) * (1 - (material.DISCOUNT ?? 0)));
                    if (materialBeans != null && materialBeans.Count > 0)
                    {
                        List<HIS_MATERIAL_BEAN> materialBeanByExpMestMedicines = materialBeans.Where(o => o.EXP_MEST_MATERIAL_ID == material.ID).ToList();
                        this.BeanIds = materialBeanByExpMestMedicines.Select(o => o.ID).ToList();
                    }

                    this.ExpMestDetailId = material.ID;
                    this.SERVICE_REQ_CODE = !string.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE) ? expMest.TDL_SERVICE_REQ_CODE : "000000000000";
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + material.TDL_MATERIAL_TYPE_ID + "_" + material.MATERIAL_ID;
                    this.EXP_MEST_ID = expMest.ID;
                    this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                    this.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                    this.PresNumber = expMest.PRES_NUMBER;
                    this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                    this.NUM_ORDER = material.NUM_ORDER;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(
            HIS_EXP_MEST_MEDICINE medicine,
            HIS_EXP_MEST expMest,
            List<HIS_MEDICINE_BEAN> medicineBeans,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.MEDI_MATE_TYPE_ID = medicine.TDL_MEDICINE_TYPE_ID ?? 0;

                    var dataMedicine = _medicineTypes.FirstOrDefault(p => p.ID == medicine.TDL_MEDICINE_TYPE_ID);
                    if (dataMedicine != null)
                    {
                        this.SERVICE_ID = dataMedicine.SERVICE_ID;
                        this.MEDI_MATE_TYPE_CODE = dataMedicine.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_NAME = dataMedicine.MEDICINE_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = dataMedicine.SERVICE_UNIT_NAME;
                        this.NATIONAL_NAME = dataMedicine.NATIONAL_NAME;
                        this.MANUFACTURER_NAME = dataMedicine.MANUFACTURER_NAME;
                    }
                    this.EXP_AMOUNT = medicine.AMOUNT;
                    this.OLD_AMOUNT = medicine.AMOUNT;
                    this.IsCheckExpPrice = (medicine.IS_USE_CLIENT_PRICE ?? 0) == 1 ? true : false;
                    this.PATIENT_TYPE_ID = medicine.PATIENT_TYPE_ID;
                    if (medicine.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = medicine.DISCOUNT.Value * 100;
                    this.EXP_PRICE = medicine.PRICE;
                    if (medicine.VAT_RATIO.HasValue)
                        this.EXP_VAT_RATIO = medicine.VAT_RATIO;
                    if (medicine.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = medicine.DISCOUNT * 100;
                    this.TOTAL_PRICE = (this.EXP_AMOUNT * this.EXP_PRICE * (1 + (this.EXP_VAT_RATIO ?? 0)) * (1 - (medicine.DISCOUNT ?? 0)));
                    this.ExpMestDetailId = medicine.ID;
                    if (medicineBeans != null && medicineBeans.Count > 0)
                    {
                        List<HIS_MEDICINE_BEAN> medicineBeanByExpMestMedicines = medicineBeans.Where(o => o.EXP_MEST_MEDICINE_ID == medicine.ID).ToList();
                        this.BeanIds = medicineBeanByExpMestMedicines.Select(o => o.ID).ToList();
                    }

                    //Tinh so ngay
                    if (medicine.USE_TIME_TO.HasValue && expMest.TDL_INTRUCTION_TIME.HasValue)
                    {
                        DateTime dtUserTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(medicine.USE_TIME_TO.Value).Value;
                        DateTime dtIntrcutionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMest.TDL_INTRUCTION_TIME.Value).Value;
                        DateTime dtUT = new DateTime(dtUserTimeTo.Year, dtUserTimeTo.Month, dtUserTimeTo.Day, dtUserTimeTo.Hour, 0, 0);
                        DateTime dtIT = new DateTime(dtIntrcutionTime.Year, dtIntrcutionTime.Month, dtIntrcutionTime.Day, dtIntrcutionTime.Hour, 0, 0);
                        TimeSpan ts = (TimeSpan)(dtUT - dtIT);
                        this.DayNum = (long?)ts.TotalDays + 1;
                    }
                    this.SERVICE_REQ_CODE = !string.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE) ? expMest.TDL_SERVICE_REQ_CODE : "000000000000";
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + medicine.TDL_MEDICINE_TYPE_ID + "_" + medicine.MEDICINE_ID;
                    this.EXP_MEST_ID = expMest.ID;
                    this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                    this.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                    this.PresNumber = expMest.PRES_NUMBER;
                    this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                    this.NUM_ORDER = medicine.NUM_ORDER;
                    V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == (medicine.TDL_MEDICINE_TYPE_ID ?? 0));
                    if (medicineType != null)
                    {
                        this.CONCENTRA = medicineType.CONCENTRA;
                        if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                            this.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", this.MEDI_MATE_TYPE_NAME, this.CONCENTRA);
                        this.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(
    V_HIS_EXP_MEST_MEDICINE medicine,
    V_HIS_EXP_MEST expMest,
    List<HIS_MEDICINE_BEAN> medicineBeans,
    List<V_HIS_MEDICINE_TYPE> _medicineTypes)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.MEDI_MATE_TYPE_ID = medicine.TDL_MEDICINE_TYPE_ID ?? 0;

                    var dataMedicine = _medicineTypes.FirstOrDefault(p => p.ID == medicine.TDL_MEDICINE_TYPE_ID);
                    if (dataMedicine != null)
                    {
                        this.SERVICE_ID = dataMedicine.SERVICE_ID;
                        this.MEDI_MATE_TYPE_CODE = dataMedicine.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_NAME = dataMedicine.MEDICINE_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = dataMedicine.SERVICE_UNIT_NAME;
                        this.NATIONAL_NAME = dataMedicine.NATIONAL_NAME;
                        this.MANUFACTURER_NAME = dataMedicine.MANUFACTURER_NAME;
                    }
                    this.EXP_AMOUNT = medicine.AMOUNT;
                    this.OLD_AMOUNT = medicine.AMOUNT;
                    this.IsCheckExpPrice = (medicine.IS_USE_CLIENT_PRICE ?? 0) == 1 ? true : false;
                    this.PATIENT_TYPE_ID = medicine.PATIENT_TYPE_ID;
                    if (medicine.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = medicine.DISCOUNT.Value * 100;
                    this.EXP_PRICE = medicine.PRICE;
                    if (medicine.VAT_RATIO.HasValue)
                        this.EXP_VAT_RATIO = medicine.VAT_RATIO;
                    if (medicine.DISCOUNT.HasValue)
                        this.DISCOUNT_RATIO = medicine.DISCOUNT * 100;
                    this.TOTAL_PRICE = (this.EXP_AMOUNT * this.EXP_PRICE * (1 + (this.EXP_VAT_RATIO ?? 0)) * (1 - (medicine.DISCOUNT ?? 0)));
                    this.ExpMestDetailId = medicine.ID;
                    if (medicineBeans != null && medicineBeans.Count > 0)
                    {
                        List<HIS_MEDICINE_BEAN> medicineBeanByExpMestMedicines = medicineBeans.Where(o => o.EXP_MEST_MEDICINE_ID == medicine.ID).ToList();
                        this.BeanIds = medicineBeanByExpMestMedicines.Select(o => o.ID).ToList();
                    }

                    //Tinh so ngay
                    if (medicine.USE_TIME_TO.HasValue && expMest.TDL_INTRUCTION_TIME.HasValue)
                    {
                        DateTime dtUserTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(medicine.USE_TIME_TO.Value).Value;
                        DateTime dtIntrcutionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expMest.TDL_INTRUCTION_TIME.Value).Value;
                        DateTime dtUT = new DateTime(dtUserTimeTo.Year, dtUserTimeTo.Month, dtUserTimeTo.Day, dtUserTimeTo.Hour, 0, 0);
                        DateTime dtIT = new DateTime(dtIntrcutionTime.Year, dtIntrcutionTime.Month, dtIntrcutionTime.Day, dtIntrcutionTime.Hour, 0, 0);
                        TimeSpan ts = (TimeSpan)(dtUT - dtIT);
                        this.DayNum = (long?)ts.TotalDays + 1;
                    }
                    this.SERVICE_REQ_CODE = !string.IsNullOrEmpty(expMest.TDL_SERVICE_REQ_CODE) ? expMest.TDL_SERVICE_REQ_CODE : "000000000000";
                    this.PARENT_ID__IN_SETY = this.SERVICE_REQ_CODE;
                    this.CONCRETE_ID__IN_SETY = this.SERVICE_REQ_CODE + "_" + medicine.TDL_MEDICINE_TYPE_ID + "_" + medicine.MEDICINE_ID;
                    this.EXP_MEST_ID = expMest.ID;
                    this.EXP_MEST_CODE = expMest.EXP_MEST_CODE;
                    this.SERVICE_REQ_ID = expMest.PRESCRIPTION_ID;
                    this.PresNumber = expMest.PRES_NUMBER;
                    this.TDL_PATIENT_NAME = expMest.TDL_PATIENT_NAME;
                    this.NUM_ORDER = medicine.NUM_ORDER;
                    V_HIS_MEDICINE_TYPE medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == (medicine.TDL_MEDICINE_TYPE_ID ?? 0));
                    if (medicineType != null)
                    {
                        this.CONCENTRA = medicineType.CONCENTRA;
                        if (HisConfigCFG.IS_JOIN_NAME_WITH_CONCENTRA)
                            this.MEDI_MATE_TYPE_NAME = String.Format("{0} {1}", this.MEDI_MATE_TYPE_NAME, this.CONCENTRA);
                        this.ACTIVE_INGR_BHYT_NAME = medicineType.ACTIVE_INGR_BHYT_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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

namespace MPS.Processor.Mps000215
{
    public class Mps000215ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string IS_EXPEND_DISPLAY { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT_IN_EXECUTE { get; set; }
        public decimal TOTAL_AMOUNT_IN_REQUEST { get; set; }
        public decimal TOTAL_AMOUNT_IN_EXPORT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public string DESCRIPTION { get; set; }
        public string AMOUNT_EXPORT_STRING { get; set; }
        public string AMOUNT_EXECUTE_STRING { get; set; }
        public string AMOUNT_REQUEST_STRING { get; set; }
        public long MEDI_MATE_NUM_ORDER { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? MEDICINE_USE_FORM_NUM_ORDER { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }

        public string CONCENTRA { get; set; }

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string REPLACE_NAME { get; set; }
        public string REPLACE_CODE { get; set; }

        public long KEY_GROUP { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string NATIONAL_CODE { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string MEDICINE_GROUP_CODE { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public string MEDICINE_PARENT_CODE { get; set; }
        public long? MEDICINE_PARENT_ID { get; set; }
        public string MEDICINE_PARENT_NAME { get; set; }
        public string SCIENTIFIC_NAME { get; set; }

        public Mps000215ADO(V_HIS_EXP_MEST expMest, List<HIS_EXP_MEST_METY_REQ> req, List<V_HIS_MEDICINE_TYPE> _medicineTypes, List<V_HIS_EXP_MEST_MEDICINE> medicines, bool isReplace, bool IsGroup = false)
        {

            this.TYPE_ID = 1;
            List<string> des = req.Where(o => !string.IsNullOrWhiteSpace(o.DESCRIPTION)).Select(s => s.DESCRIPTION).ToList();
            this.DESCRIPTION = des != null && des.Count > 0 ? string.Join(";", des) : "";
            if (!isReplace)
            {
                var data = _medicineTypes.FirstOrDefault(p => p.ID == req.First().MEDICINE_TYPE_ID);
                if (data != null)
                {
                    this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = data.ID;
                    this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.CONCENTRA = data.CONCENTRA;
                    this.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                    this.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                    this.NATIONAL_CODE = data.MEDICINE_NATIONAL_CODE;
                    this.NATIONAL_NAME = data.NATIONAL_NAME;
                    this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                    this.MEDICINE_GROUP_ID = data.MEDICINE_GROUP_ID;
                    this.MEDICINE_GROUP_CODE = data.MEDICINE_GROUP_CODE;
                    this.MEDICINE_GROUP_NAME = data.MEDICINE_GROUP_NAME;
                    this.MEDICINE_PARENT_ID = data.PARENT_ID;
                    this.MEDICINE_PARENT_CODE = data.PARENT_CODE;
                    this.MEDICINE_PARENT_NAME = data.PARENT_NAME;
                    this.SCIENTIFIC_NAME = data.SCIENTIFIC_NAME;
                }

                if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    if (medicines != null && medicines.Count > 0)
                    {
                        this.TOTAL_AMOUNT_IN_EXECUTE = medicines.Sum(p => p.AMOUNT);
                        this.PACKAGE_NUMBER = medicines.First().PACKAGE_NUMBER;
                        this.SUPPLIER_CODE = medicines.First().SUPPLIER_CODE;
                        this.SUPPLIER_NAME = medicines.First().SUPPLIER_NAME;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicines.First().EXPIRED_DATE ?? 0);
                        this.PRICE = medicines.First().PRICE;
                        this.IMP_PRICE = medicines.First().IMP_PRICE;
                        this.IMP_VAT_RATIO = medicines.First().IMP_VAT_RATIO * 100;
                        this.DESCRIPTION = medicines.First().DESCRIPTION;
                        this.MEDI_MATE_NUM_ORDER = medicines.First().MEDICINE_NUM_ORDER ?? 0;
                        this.NUM_ORDER = medicines.First().NUM_ORDER;
                        this.MEDICINE_USE_FORM_NUM_ORDER = medicines.First().MEDICINE_USE_FORM_NUM_ORDER;
                        this.MEDICINE_TYPE_NAME = medicines.First().MEDICINE_TYPE_NAME;
                    }
                    if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE ||
                        (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && expMest.IS_EXPORT_EQUAL_APPROVE == 1))
                    {
                        this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                    }
                }
                this.TOTAL_AMOUNT_IN_REQUEST = req.Sum(s => s.AMOUNT);
                if(IsGroup)
				{
                    TOTAL_AMOUNT_IN_REQUEST = TOTAL_AMOUNT_IN_EXECUTE;
                }                    
            }
            else
            {
                var replace = _medicineTypes.FirstOrDefault(p => p.ID == medicines.First().MEDICINE_TYPE_ID);
                if (replace != null)
                {
                    this.MEDI_MATE_TYPE_CODE = replace.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = replace.ID;
                    this.MEDI_MATE_TYPE_NAME = replace.MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = replace.REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = replace.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = replace.SERVICE_UNIT_NAME;
                    this.CONCENTRA = replace.CONCENTRA;
                    this.ACTIVE_INGR_BHYT_CODE = replace.ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = replace.ACTIVE_INGR_BHYT_NAME;
                    this.MANUFACTURER_CODE = replace.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = replace.MANUFACTURER_NAME;
                    this.NATIONAL_CODE = replace.MEDICINE_NATIONAL_CODE;
                    this.NATIONAL_NAME = replace.NATIONAL_NAME;
                    this.PACKING_TYPE_NAME = replace.PACKING_TYPE_NAME;
                    this.MEDICINE_GROUP_ID = replace.MEDICINE_GROUP_ID;
                    this.MEDICINE_GROUP_CODE = replace.MEDICINE_GROUP_CODE;
                    this.MEDICINE_GROUP_NAME = replace.MEDICINE_GROUP_NAME;
                    this.MEDICINE_PARENT_ID = replace.PARENT_ID;
                    this.MEDICINE_PARENT_CODE = replace.PARENT_CODE;
                    this.MEDICINE_PARENT_NAME = replace.PARENT_NAME;
                    this.SCIENTIFIC_NAME = replace.SCIENTIFIC_NAME;
                }
                var data = _medicineTypes.FirstOrDefault(p => p.ID == req.First().MEDICINE_TYPE_ID);
                if (data != null)
                {
                    this.REPLACE_NAME = data.MEDICINE_TYPE_NAME;
                    this.REPLACE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDICINE_PARENT_ID = data.PARENT_ID;
                    this.MEDICINE_PARENT_CODE = data.PARENT_CODE;
                    this.MEDICINE_PARENT_NAME = data.PARENT_NAME;
                    this.SCIENTIFIC_NAME = data.SCIENTIFIC_NAME;

                }
                if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    if (medicines != null && medicines.Count > 0)
                    {
                        this.TOTAL_AMOUNT_IN_EXECUTE = medicines.Sum(p => p.AMOUNT);
                        this.PACKAGE_NUMBER = medicines.First().PACKAGE_NUMBER;
                        this.SUPPLIER_CODE = medicines.First().SUPPLIER_CODE;
                        this.SUPPLIER_NAME = medicines.First().SUPPLIER_NAME;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicines.First().EXPIRED_DATE ?? 0);
                        this.PRICE = medicines.First().PRICE;
                        this.IMP_PRICE = medicines.First().IMP_PRICE;
                        this.IMP_VAT_RATIO = medicines.First().IMP_VAT_RATIO * 100;
                        this.DESCRIPTION = medicines.First().DESCRIPTION;
                        this.MEDI_MATE_NUM_ORDER = medicines.First().MEDICINE_NUM_ORDER ?? 0;
                        this.NUM_ORDER = medicines.First().NUM_ORDER;
                        this.MEDICINE_USE_FORM_NUM_ORDER = medicines.First().MEDICINE_USE_FORM_NUM_ORDER;
                        this.MEDICINE_TYPE_NAME = medicines.First().MEDICINE_TYPE_NAME;
                    }
                    if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE ||
                        (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && expMest.IS_EXPORT_EQUAL_APPROVE == 1))
                    {
                        this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                    }
                }
                this.TOTAL_AMOUNT_IN_REQUEST = 0;
            }

            this.TOTAL_AMOUNT = this.TOTAL_AMOUNT_IN_REQUEST;
            this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_REQUEST)));
            this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXECUTE)));
            this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXPORT)));
        }

        public Mps000215ADO(V_HIS_EXP_MEST expMest, List<HIS_EXP_MEST_MATY_REQ> req, List<V_HIS_MATERIAL_TYPE> _materialTypes, List<V_HIS_EXP_MEST_MATERIAL> materials, bool isReplace,bool IsGroup = false)
        {

            this.TYPE_ID = 1;
            List<string> des = req.Where(o => !string.IsNullOrWhiteSpace(o.DESCRIPTION)).Select(s => s.DESCRIPTION).ToList();
            this.DESCRIPTION = des != null && des.Count > 0 ? string.Join(";", des) : "";
            if (!isReplace)
            {
                var data = _materialTypes.FirstOrDefault(p => p.ID == req.First().MATERIAL_TYPE_ID);
                if (data != null)
                {
                    this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = data.ID;
                    this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.CONCENTRA = data.CONCENTRA;
                    this.MANUFACTURER_CODE = data.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = data.NATIONAL_NAME;
                    this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                }

                if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    if (materials != null && materials.Count > 0)
                    {
                        this.TOTAL_AMOUNT_IN_EXECUTE = materials.Sum(p => p.AMOUNT);
                        this.PACKAGE_NUMBER = materials.First().PACKAGE_NUMBER;
                        this.SUPPLIER_CODE = materials.First().SUPPLIER_CODE;
                        this.SUPPLIER_NAME = materials.First().SUPPLIER_NAME;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(materials.First().EXPIRED_DATE ?? 0);
                        this.PRICE = materials.First().PRICE;
                        this.IMP_PRICE = materials.First().IMP_PRICE;
                        this.IMP_VAT_RATIO = materials.First().IMP_VAT_RATIO * 100;
                        this.DESCRIPTION = materials.First().DESCRIPTION;
                        this.MEDI_MATE_NUM_ORDER = materials.First().MEDICINE_NUM_ORDER ?? 0;
                        this.NUM_ORDER = materials.First().NUM_ORDER;
                        this.MEDICINE_TYPE_NAME = materials.First().MATERIAL_TYPE_NAME;
                    }
                    if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE ||
                        (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && expMest.IS_EXPORT_EQUAL_APPROVE == 1))
                    {
                        this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                    }
                }
                this.TOTAL_AMOUNT_IN_REQUEST = req.Sum(s => s.AMOUNT);
                if (IsGroup)
                {
                    TOTAL_AMOUNT_IN_REQUEST = TOTAL_AMOUNT_IN_EXECUTE;
                }
            }
            else
            {
                var replace = _materialTypes.FirstOrDefault(p => p.ID == materials.First().MATERIAL_TYPE_ID);
                if (replace != null)
                {
                    this.MEDI_MATE_TYPE_CODE = replace.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = replace.ID;
                    this.MEDI_MATE_TYPE_NAME = replace.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = replace.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = replace.SERVICE_UNIT_NAME;
                    this.CONCENTRA = replace.CONCENTRA;
                    this.MANUFACTURER_CODE = replace.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = replace.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = replace.NATIONAL_NAME;
                    this.PACKING_TYPE_NAME = replace.PACKING_TYPE_NAME;
                }
                var data = _materialTypes.FirstOrDefault(p => p.ID == req.First().MATERIAL_TYPE_ID);
                if (data != null)
                {
                    this.REPLACE_CODE = data.MATERIAL_TYPE_CODE;
                    this.REPLACE_NAME = data.MATERIAL_TYPE_NAME;
                }
                if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE || expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                {
                    if (materials != null && materials.Count > 0)
                    {
                        this.TOTAL_AMOUNT_IN_EXECUTE = materials.Sum(p => p.AMOUNT);
                        this.PACKAGE_NUMBER = materials.First().PACKAGE_NUMBER;
                        this.SUPPLIER_CODE = materials.First().SUPPLIER_CODE;
                        this.SUPPLIER_NAME = materials.First().SUPPLIER_NAME;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(materials.First().EXPIRED_DATE ?? 0);
                        this.PRICE = materials.First().PRICE;
                        this.IMP_PRICE = materials.First().IMP_PRICE;
                        this.IMP_VAT_RATIO = materials.First().IMP_VAT_RATIO * 100;
                        this.DESCRIPTION = materials.First().DESCRIPTION;
                        this.MEDI_MATE_NUM_ORDER = materials.First().MEDICINE_NUM_ORDER ?? 0;
                        this.NUM_ORDER = materials.First().NUM_ORDER;
                        this.MEDICINE_TYPE_NAME = materials.First().MATERIAL_TYPE_NAME;
                    }
                    if (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE ||
                        (expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE && expMest.IS_EXPORT_EQUAL_APPROVE == 1))
                    {
                        this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                    }
                }
                this.TOTAL_AMOUNT_IN_REQUEST = 0;
            }

            this.TOTAL_AMOUNT = this.TOTAL_AMOUNT_IN_REQUEST;
            this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_REQUEST)));
            this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXECUTE)));
            this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXPORT)));
        }
    }
}

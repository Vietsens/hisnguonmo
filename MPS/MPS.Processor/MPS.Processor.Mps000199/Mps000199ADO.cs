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

namespace MPS.Processor.Mps000199
{
    class Mps000199ADO
    {
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal PRICE_AMOUNT { get; set; }
        public string PRICE_SEPARATE { get; set; }
        public decimal? VIR_PRICE { get; set; }
        public decimal? IMP_UNIT_AMOUNT { get; set; }
        public decimal? IMP_UNIT_PRICE { get; set; }
        public long? TDL_IMP_UNIT_ID { get; set; }
        public string IMP_UNIT_NAME { get; set; }
        public string BID_NAME { get; set; }
        public string MEDICAL_CONTRACT_CODE { get; set; }
        public string MEDICAL_CONTRACT_NAME { get; set; }
        public string DOCUMENT_SUPPLIER_NAME { get; set; }
        public string VENTURE_AGREENING { get; set; }
        public string TDL_BID_NUMBER { get; set; }

        public string CONCENTRA { get; set; }
        public Mps000199ADO(V_HIS_IMP_MEST_MEDICINE _impMestMedicine, List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> listMedicalContract)
        {
            try
            {
                if (_impMestMedicine != null)
                {
                    this.MEDI_MATE_TYPE_NAME = _impMestMedicine.MEDICINE_TYPE_NAME;
                    this.CONCENTRA = _impMestMedicine.CONCENTRA;
                    this.MEDI_MATE_TYPE_CODE = _impMestMedicine.MEDICINE_TYPE_CODE;
                    this.SERVICE_UNIT_NAME = _impMestMedicine.SERVICE_UNIT_NAME;
                    this.REGISTER_NUMBER = _impMestMedicine.REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = _impMestMedicine.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_impMestMedicine.EXPIRED_DATE ?? 0);
                    this.AMOUNT = _impMestMedicine.AMOUNT;
                    this.PRICE = _impMestMedicine.PRICE;
                    this.MANUFACTURER_NAME = _impMestMedicine.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = _impMestMedicine.NATIONAL_NAME;
                    this.VAT_RATIO_100 = _impMestMedicine.VAT_RATIO * 100;
                    this.PRICE_AMOUNT = _impMestMedicine.AMOUNT * (_impMestMedicine.PRICE ?? 0) * (1 + (_impMestMedicine.VAT_RATIO ?? 0));
                    this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.VIR_PRICE = _impMestMedicine.VIR_PRICE;
                    this.IMP_PRICE = _impMestMedicine.IMP_PRICE;
                    this.IMP_UNIT_AMOUNT = _impMestMedicine.IMP_UNIT_AMOUNT;
                    this.IMP_UNIT_PRICE = _impMestMedicine.IMP_UNIT_PRICE;
                    this.IMP_UNIT_NAME = _impMestMedicine.IMP_UNIT_NAME;
                    this.TDL_IMP_UNIT_ID = _impMestMedicine.TDL_IMP_UNIT_ID;
                    this.BID_NAME = _impMestMedicine.BID_NAME;
                    this.TDL_BID_NUMBER = _impMestMedicine.TDL_BID_NUMBER;

                    if (listMedicalContract != null && listMedicalContract.Count > 0)
                    {
                        V_HIS_MEDICAL_CONTRACT MedicalContract = listMedicalContract.FirstOrDefault(o => o.MEDICINE_ID == _impMestMedicine.MEDICINE_ID);
                        if (MedicalContract != null)
                        {
                            this.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                            this.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                            this.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                            this.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000199ADO(V_HIS_IMP_MEST_MATERIAL impMestMaterial, List<MPS.Processor.Mps000199.PDO.Mps000199PDO.MedicalContractADO> listMedicalContract)
        {
            try
            {
                if (impMestMaterial != null)
                {
                    this.MEDI_MATE_TYPE_NAME = impMestMaterial.MATERIAL_TYPE_NAME;
                    this.MEDI_MATE_TYPE_CODE = impMestMaterial.MATERIAL_TYPE_CODE;
                    this.SERVICE_UNIT_NAME = impMestMaterial.SERVICE_UNIT_NAME;
                    
                    //this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = impMestMaterial.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(impMestMaterial.EXPIRED_DATE ?? 0);
                    this.MANUFACTURER_NAME = impMestMaterial.MANUFACTURER_NAME;
                    this.NATIONAL_NAME = impMestMaterial.NATIONAL_NAME;
                    this.VAT_RATIO_100 = impMestMaterial.VAT_RATIO * 100;
                    this.AMOUNT = impMestMaterial.AMOUNT;
                    this.PRICE = impMestMaterial.PRICE;
                    this.PRICE_AMOUNT = impMestMaterial.AMOUNT * (impMestMaterial.PRICE ?? 0) * (1 + (impMestMaterial.VAT_RATIO ?? 0));
                    this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.VIR_PRICE = impMestMaterial.VIR_PRICE;
                    this.IMP_PRICE = impMestMaterial.IMP_PRICE;
                    this.IMP_UNIT_AMOUNT = impMestMaterial.IMP_UNIT_AMOUNT;
                    this.IMP_UNIT_PRICE = impMestMaterial.IMP_UNIT_PRICE;
                    this.IMP_UNIT_NAME = impMestMaterial.IMP_UNIT_NAME;
                    this.TDL_IMP_UNIT_ID = impMestMaterial.TDL_IMP_UNIT_ID;
                    this.BID_NAME = impMestMaterial.BID_NAME;
                    this.TDL_BID_NUMBER = impMestMaterial.TDL_BID_NUMBER;

                    if (listMedicalContract != null && listMedicalContract.Count > 0)
                    {
                        V_HIS_MEDICAL_CONTRACT MedicalContract = listMedicalContract.FirstOrDefault(o => o.MATERIAL_ID == impMestMaterial.MATERIAL_ID);
                        if (MedicalContract != null)
                        {
                            this.MEDICAL_CONTRACT_CODE = MedicalContract.MEDICAL_CONTRACT_CODE;
                            this.MEDICAL_CONTRACT_NAME = MedicalContract.MEDICAL_CONTRACT_NAME;
                            this.DOCUMENT_SUPPLIER_NAME = MedicalContract.DOCUMENT_SUPPLIER_NAME;
                            this.VENTURE_AGREENING = MedicalContract.VENTURE_AGREENING;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000199ADO(V_HIS_IMP_MEST_BLOOD impMestBlood)
        {
            try
            {
                if (impMestBlood != null)
                {
                    this.MEDI_MATE_TYPE_NAME = impMestBlood.BLOOD_TYPE_NAME;
                    this.MEDI_MATE_TYPE_CODE = impMestBlood.BLOOD_CODE;
                    this.SERVICE_UNIT_NAME = impMestBlood.SERVICE_UNIT_NAME;
                    //this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                    this.PACKAGE_NUMBER = impMestBlood.PACKAGE_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(impMestBlood.EXPIRED_DATE ?? 0);
                    this.NATIONAL_NAME = "";
                    this.VAT_RATIO_100 = impMestBlood.VAT_RATIO * 100;
                    this.AMOUNT = 1;
                    this.PRICE = impMestBlood.PRICE;
                    this.PRICE_AMOUNT = (impMestBlood.PRICE ?? 0) * (1 + (impMestBlood.VAT_RATIO ?? 0));
                    this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.VIR_PRICE = impMestBlood.VIR_PRICE;
                    this.IMP_PRICE = impMestBlood.IMP_PRICE;
                    this.BID_NAME = impMestBlood.BID_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

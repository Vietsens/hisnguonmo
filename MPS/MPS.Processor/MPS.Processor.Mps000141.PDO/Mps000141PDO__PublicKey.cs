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
using MOS.SDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000141.PDO
{
    public partial class Mps000141PDO : RDOBase
    {
        public V_HIS_IMP_MEST _ManuImpMest = null;
        public List<HIS_MEDICINE> _Medicines = null;
        public List<HIS_MATERIAL> _Materials = null;
        public List<HIS_IMP_SOURCE> _listImpSource = null;
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines = null;
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials = null;
        public List<Mps000141ADO> _ListAdo = null;
        public List<MedicalContractADO> _ListMedicalContract = null;

        public class Mps000141ADO : V_HIS_IMP_MEST_MEDICINE
        {
            public string MEDI_MATE_TYPE_CODE { get; set; }
            public string MEDI_MATE_TYPE_NAME { get; set; }
            //public string SERVICE_UNIT_NAME { get; set; }
            //public string RECORDING_TRANSACTION { get; set; }
            //public string REGISTER_NUMBER { get; set; }
            //public string PACKAGE_NUMBER { get; set; }
            public string EXPIRED_DATE_STR { get; set; }
            //public decimal AMOUNT { get; set; }
            public decimal TOTAL_PRICE { get; set; }
            //public decimal? PRICE { get; set; }

            //public decimal? VAT_RATIO { get; set; }
            //public decimal IMP_VAT_RATIO { get; set; }
            //public string NATIONAL_NAME { get; set; }
            //public string MANUFACTURER_NAME { get; set; }
            //public string MANUFACTURER_CODE { get; set; }
            //public string BID_NAME { get; set; }
            //public string BID_NUMBER { get; set; }
            //public long? EXPIRED_DATE { get; set; }
            //public string PACKING_TYPE_NAME { get; set; }
            //public string CONCENTRA { get; set; }// hàm lượng nồng độ
            //public string ACTIVE_INGR_BHYT_CODE { get; set; } // hoạt chất
            //public string ACTIVE_INGR_BHYT_NAME { get; set; }
            //public decimal? VIR_PRICE { get; set; }

            public decimal? VIR_IMP_PRICE { get; set; }

            //public long? MEDICINE_ID { get; set; }

            public long? MATERIAL_ID { get; set; }
            public string IMP_SOURCE_CODE { get; set; }
            public string IMP_SOURCE_NAME { get; set; }

            public string MEDICAL_CONTRACT_CODE { get; set; }
            public string MEDICAL_CONTRACT_NAME { get; set; }
            public string DOCUMENT_SUPPLIER_NAME { get; set; }
            public string VENTURE_AGREENING { get; set; }

            public Mps000141ADO(V_HIS_IMP_MEST_MEDICINE medicine, List<HIS_MEDICINE> _medicines, List<HIS_IMP_SOURCE> _listImpSource)
            {
                try
                {
                    if (medicine != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000141ADO>(this, medicine);
                        this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                        //this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                        //this.RECORDING_TRANSACTION = medicine.RECORDING_TRANSACTION;
                        //this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                        //this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;

                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicine.EXPIRED_DATE ?? 0);
                        //this.AMOUNT = medicine.AMOUNT;
                        //this.PRICE = medicine.PRICE;
                        if (medicine.PRICE.HasValue)
                        {
                            this.TOTAL_PRICE = medicine.PRICE.Value * medicine.AMOUNT;
                        }

                        //this.VAT_RATIO = medicine.VAT_RATIO;
                        //this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;
                        //this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                        //this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                        //this.MANUFACTURER_CODE = medicine.MANUFACTURER_CODE;
                        //this.BID_NAME = medicine.BID_NAME;
                        //this.BID_NUMBER = medicine.BID_NUMBER;
                        //this.EXPIRED_DATE = medicine.EXPIRED_DATE;
                        //this.PACKING_TYPE_NAME = medicine.PACKING_TYPE_NAME;
                        //this.CONCENTRA = medicine.CONCENTRA;
                        //this.ACTIVE_INGR_BHYT_CODE = medicine.ACTIVE_INGR_BHYT_CODE;
                        //this.ACTIVE_INGR_BHYT_NAME = medicine.ACTIVE_INGR_BHYT_NAME;
                        //this.VIR_PRICE = medicine.VIR_PRICE;
                        //this.MEDICINE_ID = medicine.MEDICINE_ID;
                        var medi = _medicines.FirstOrDefault((HIS_MEDICINE p) => p.ID == medicine.MEDICINE_ID);
                        if (medi != null)
                        {
                            this.VIR_IMP_PRICE = medi.VIR_IMP_PRICE;
                            var impSource = _listImpSource.FirstOrDefault((HIS_IMP_SOURCE p) => p.ID == medi.IMP_SOURCE_ID);
                            if (impSource != null)
                            {
                                this.IMP_SOURCE_CODE = impSource.IMP_SOURCE_CODE;
                                this.IMP_SOURCE_NAME = impSource.IMP_SOURCE_NAME;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }

            public Mps000141ADO(V_HIS_IMP_MEST_MATERIAL material, List<HIS_MATERIAL> _materials, List<HIS_IMP_SOURCE> _listImpSource)
            {
                try
                {
                    if (material != null)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<Mps000141ADO>(this, material);
                        this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                        //this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                        //this.RECORDING_TRANSACTION = material.RECORDING_TRANSACTION;
                        //this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(material.EXPIRED_DATE ?? 0);
                        //this.AMOUNT = material.AMOUNT;
                        //this.PRICE = material.PRICE;
                        if (material.PRICE.HasValue)
                        {
                            this.TOTAL_PRICE = material.AMOUNT * material.PRICE.Value;
                        }

                        //this.VAT_RATIO = material.VAT_RATIO;
                        //this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;
                        //this.NATIONAL_NAME = material.NATIONAL_NAME;
                        //this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                        //this.MANUFACTURER_CODE = material.MANUFACTURER_CODE;
                        //this.BID_NAME = material.BID_NAME;
                        //this.BID_NUMBER = material.BID_NUMBER;
                        //this.EXPIRED_DATE = material.EXPIRED_DATE;
                        //this.PACKING_TYPE_NAME = material.PACKING_TYPE_NAME;
                        //this.CONCENTRA = "";
                        //this.ACTIVE_INGR_BHYT_CODE = "";
                        //this.ACTIVE_INGR_BHYT_NAME = "";
                        //this.VIR_PRICE = material.VIR_PRICE;
                        this.MATERIAL_ID = material.MATERIAL_ID;
                        var mate = _materials.FirstOrDefault((HIS_MATERIAL p) => p.ID == material.MATERIAL_ID);
                        if (mate != null)
                        {
                            this.VIR_IMP_PRICE = mate.VIR_IMP_PRICE;
                            var impSource = _listImpSource.FirstOrDefault((HIS_IMP_SOURCE p) => p.ID == mate.IMP_SOURCE_ID);
                            if (impSource != null)
                            {
                                this.IMP_SOURCE_CODE = impSource.IMP_SOURCE_CODE;
                                this.IMP_SOURCE_NAME = impSource.IMP_SOURCE_NAME;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
        }

        public class MedicalContractADO : V_HIS_MEDICAL_CONTRACT
        {
            public long MEDICINE_ID { get; set; }
            public long MATERIAL_ID { get; set; }
        }
    }
}

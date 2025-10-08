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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000145.PDO
{
    public partial class Mps000145PDO : RDOBase
    {
        public V_HIS_IMP_MEST _ChmsImpMest = null;
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines = null;
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials = null;
        public List<Mps000145ADO> _ListAdo = null;

        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes = null;
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes = null;

        public class Mps000145ADO
        {
            public string MEDI_MATE_TYPE_NAME { get; set; }
            public string SERVICE_UNIT_NAME { get; set; }
            public string REGISTER_NUMBER { get; set; }
            public string PACKAGE_NUMBER { get; set; }
            public string EXPIRED_DATE_STR { get; set; }
            public decimal AMOUNT { get; set; }
            public decimal? PRICE { get; set; }
            public long TYPE_ID { get; set; }
            public long MEDI_MATE_NUM_ORDER { get; set; }


            public long? PARENT_ID { get; set; }
            public string PARENT_CODE { get; set; }
            public string PARENT_NAME { get; set; }

            public long? MEDICINCE_GROUP_ID { get; set; }
            public string MEDICINCE_GROUP_CODE { get; set; }
            public string MEDICINCE_GROUP_NAME { get; set; }

            public string ACTIVE_INGR_BHYT_CODE { get; set; }
            public string ACTIVE_INGR_BHYT_NAME { get; set; }
             
            public Mps000145ADO(V_HIS_IMP_MEST_MEDICINE medicine, List<V_HIS_MEDICINE_TYPE> medicineTypes)
            {
                try
                {
                    if (medicine != null)
                    {
                        this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                        this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                        this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(medicine.EXPIRED_DATE ?? 0);
                        this.AMOUNT = medicine.AMOUNT;
                        this.PRICE = medicine.PRICE;
                        this.TYPE_ID = 1;
                        this.MEDI_MATE_NUM_ORDER = medicine.MEDICINE_NUM_ORDER ?? 0;


                        var type = medicineTypes?.FirstOrDefault(x => x.ID == medicine.MEDICINE_TYPE_ID);
                        if (type != null)
                        {
                            this.PARENT_ID = type.PARENT_ID;
                            this.PARENT_CODE = type.PARENT_CODE;
                            this.PARENT_NAME = type.PARENT_NAME;

                            this.MEDICINCE_GROUP_ID = type.MEDICINE_GROUP_ID;
                            this.MEDICINCE_GROUP_CODE = type.MEDICINE_GROUP_CODE;
                            this.MEDICINCE_GROUP_NAME = type.MEDICINE_GROUP_NAME;

                            this.ACTIVE_INGR_BHYT_CODE = type.ACTIVE_INGR_BHYT_CODE;
                            this.ACTIVE_INGR_BHYT_NAME = type.ACTIVE_INGR_BHYT_NAME;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }

                
            public Mps000145ADO(V_HIS_IMP_MEST_MATERIAL material, List<V_HIS_MATERIAL_TYPE> materialTypes)
            {
                try
                {
                    if (material != null)  
                    {
                        this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                        this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                        //this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                        this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                        this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(material.EXPIRED_DATE ?? 0);
                        this.AMOUNT = material.AMOUNT;
                        this.PRICE = material.PRICE;
                        this.TYPE_ID = 2;
                        this.MEDI_MATE_NUM_ORDER = material.MEDICINE_NUM_ORDER ?? 0;


                        var type = materialTypes?.FirstOrDefault(x => x.ID == material.MATERIAL_TYPE_ID);
                        if (type != null)
                        {
                            this.PARENT_ID = type.PARENT_ID;
                            this.PARENT_CODE = type.PARENT_CODE;
                            this.PARENT_NAME = type.PARENT_NAME;
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
}

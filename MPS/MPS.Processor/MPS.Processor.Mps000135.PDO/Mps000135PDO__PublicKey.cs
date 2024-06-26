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

namespace MPS.Processor.Mps000135.PDO
{
    public partial class Mps000135PDO : RDOBase
    {
        public List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs = null;
        public List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs = null;
        public List<HIS_EXP_MEST_BLTY_REQ> _ExpMestBltyReqs = null;
        public V_HIS_EXP_MEST _ExpMest = null;
        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất
        public keyTitles _Title;

        public List<Mps000135ADO> listAdo = new List<Mps000135ADO>();

    }

    public class Mps000135ADO
    {
        private List<HIS_EXP_MEST_METY_REQ> group;
        private List<V_HIS_MEDICINE_TYPE> list;

        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string BID_NUMBER { get; set; }
        public string BID_NAME { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public decimal TOTAL_PRICE_AFTER_VAT { get; set; }
        public decimal? VIR_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public long? NUM_ORDER { get; set; }
        public decimal REQ_AMOUNT { get; set; }
        public decimal DD_AMOUNT { get; set; }

        public long BLOOD_TYPE_ID { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public decimal VOLUME { get; set; }
        public string CONCENTRA { get; set; }
        public string MEDICINE_GROUP_CODE { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public string MEDICINE_PARENT_CODE { get; set; }
        public long? MEDICINE_PARENT_ID { get; set; }
        public string MEDICINE_PARENT_NAME { get; set; }

        public Mps000135ADO()
        {
        }

        public Mps000135ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine, List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes, decimal REQ_AMOUNT, decimal DD_AMOUNT)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.BID_NAME = listMedicine.First().BID_NAME;
                    this.BID_NUMBER = listMedicine.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    this.DESCRIPTION = listMedicine.First().DESCRIPTION;
                    this.IMP_PRICE = listMedicine.First().IMP_PRICE;
                    this.IMP_VAT_RATIO = listMedicine.First().IMP_VAT_RATIO;
                    this.VIR_PRICE = listMedicine.First().VIR_PRICE;
                    this.TOTAL_PRICE = this.IMP_PRICE * this.AMOUNT;
                    this.REQ_AMOUNT = REQ_AMOUNT;
                    this.DD_AMOUNT = DD_AMOUNT;
                    this.CONCENTRA = listMedicine.First().CONCENTRA;
                    this.MEDICINE_GROUP_ID = listMedicine.First().MEDICINE_GROUP_ID;
                    this.MEDICINE_GROUP_CODE = listMedicine.First().MEDICINE_GROUP_CODE;
                    this.MEDICINE_GROUP_NAME = listMedicine.First().MEDICINE_GROUP_NAME;

                    if (vHisMedicineTypes != null && vHisMedicineTypes.Count > 0)
                    {
                        V_HIS_MEDICINE_TYPE MedicineType = vHisMedicineTypes.FirstOrDefault(o => o.ID == listMedicine.First().MEDICINE_TYPE_ID);

                        if (MedicineType != null)
                        {
                            this.MEDICINE_PARENT_ID = MedicineType.PARENT_ID;
                            this.MEDICINE_PARENT_CODE = MedicineType.PARENT_CODE;
                            this.MEDICINE_PARENT_NAME = MedicineType.PARENT_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs, List<V_HIS_MEDICINE_TYPE> _medicineTypes)
        {
            try
            {
                if (_expMestMetyReqs != null && _expMestMetyReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    var data = _medicineTypes.FirstOrDefault(p => p.ID == _expMestMetyReqs.First().MEDICINE_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = data.ID;
                        this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                        this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.CONCENTRA = data.CONCENTRA;
                        this.MEDICINE_GROUP_ID = data.MEDICINE_GROUP_ID;
                        this.MEDICINE_GROUP_CODE = data.MEDICINE_GROUP_CODE;
                        this.MEDICINE_GROUP_NAME = data.MEDICINE_GROUP_NAME;
                        this.MEDICINE_PARENT_ID = data.PARENT_ID;
                        this.MEDICINE_PARENT_CODE = data.PARENT_CODE;
                        this.MEDICINE_PARENT_NAME = data.PARENT_NAME;
                    }
                    this.AMOUNT = _expMestMetyReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestMetyReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestMetyReqs.First().DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs, List<V_HIS_MEDICINE_TYPE> _medicineTypes, decimal REQ_AMOUNT, decimal DD_AMOUNT)
        {
            try
            {
                if (_expMestMetyReqs != null && _expMestMetyReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    var data = _medicineTypes.FirstOrDefault(p => p.ID == _expMestMetyReqs.First().MEDICINE_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = data.ID;
                        this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                        this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.CONCENTRA = data.CONCENTRA;

                        this.MEDICINE_GROUP_ID = data.MEDICINE_GROUP_ID;
                        this.MEDICINE_GROUP_CODE = data.MEDICINE_GROUP_CODE;
                        this.MEDICINE_GROUP_NAME = data.MEDICINE_GROUP_NAME;
                        this.MEDICINE_PARENT_ID = data.PARENT_ID;
                        this.MEDICINE_PARENT_CODE = data.PARENT_CODE;
                        this.MEDICINE_PARENT_NAME = data.PARENT_NAME;
                    }
                    this.AMOUNT = _expMestMetyReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestMetyReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestMetyReqs.First().DESCRIPTION;
                    this.REQ_AMOUNT = REQ_AMOUNT;
                    this.DD_AMOUNT = DD_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<V_HIS_EXP_MEST_BLOOD> listBlood, decimal REQ_AMOUNT, decimal DD_AMOUNT)
        {
            try
            {
                if (listBlood != null && listBlood.Count > 0)
                {
                    this.TYPE_ID = 3;
                    this.BLOOD_TYPE_CODE = listBlood.First().BLOOD_TYPE_CODE;
                    this.BLOOD_TYPE_ID = listBlood.First().BLOOD_TYPE_ID;
                    this.BLOOD_TYPE_NAME = listBlood.First().BLOOD_TYPE_NAME;
                    this.PACKAGE_NUMBER = listBlood.First().PACKAGE_NUMBER;
                    this.VOLUME = listBlood.First().VOLUME;
                    //         this.REGISTER_NUMBER = listBlood.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listBlood.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listBlood.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listBlood.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listBlood.First().SUPPLIER_NAME;
                    this.BID_NAME = listBlood.First().BID_NAME;
                    this.BID_NUMBER = listBlood.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listBlood.First().EXPIRED_DATE ?? 0);
                    //         this.AMOUNT = listBlood.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listBlood.First().NUM_ORDER;
                    this.DESCRIPTION = listBlood.First().DESCRIPTION;
                    this.IMP_PRICE = listBlood.First().IMP_PRICE;
                    this.IMP_VAT_RATIO = listBlood.First().IMP_VAT_RATIO;
                    this.VIR_PRICE = listBlood.First().VIR_PRICE;
                    this.AMOUNT = listBlood.Count();
                    this.TOTAL_PRICE = this.IMP_PRICE * this.AMOUNT;
                    this.REQ_AMOUNT = REQ_AMOUNT;
                    this.DD_AMOUNT = DD_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<V_HIS_EXP_MEST_BLOOD> listBlood)
        {
            try
            {
                if (listBlood != null && listBlood.Count > 0)
                {
                    this.TYPE_ID = 3;
                    this.BLOOD_TYPE_CODE = listBlood.First().BLOOD_TYPE_CODE;
                    this.BLOOD_TYPE_ID = listBlood.First().BLOOD_TYPE_ID;
                    this.BLOOD_TYPE_NAME = listBlood.First().BLOOD_TYPE_NAME;
                    this.PACKAGE_NUMBER = listBlood.First().PACKAGE_NUMBER;
                    this.VOLUME = listBlood.First().VOLUME;
                    //         this.REGISTER_NUMBER = listBlood.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listBlood.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listBlood.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listBlood.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listBlood.First().SUPPLIER_NAME;
                    this.BID_NAME = listBlood.First().BID_NAME;
                    this.BID_NUMBER = listBlood.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listBlood.First().EXPIRED_DATE ?? 0);
                    //         this.AMOUNT = listBlood.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listBlood.First().NUM_ORDER;
                    this.DESCRIPTION = listBlood.First().DESCRIPTION;
                    this.IMP_PRICE = listBlood.First().IMP_PRICE;
                    this.IMP_VAT_RATIO = listBlood.First().IMP_VAT_RATIO;
                    this.VIR_PRICE = listBlood.First().VIR_PRICE;
                    this.AMOUNT = listBlood.Count();
                    this.TOTAL_PRICE = this.IMP_PRICE * this.AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<HIS_EXP_MEST_BLTY_REQ> _expMestBloodReqs, List<V_HIS_BLOOD_TYPE> _BloodTypes)
        {
            try
            {
                if (_expMestBloodReqs != null && _expMestBloodReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    var data = _BloodTypes.FirstOrDefault(p => p.ID == _expMestBloodReqs.First().BLOOD_TYPE_ID);
                    if (data != null)
                    {
                        this.BLOOD_TYPE_CODE = data.BLOOD_TYPE_CODE;
                        this.BLOOD_TYPE_ID = data.ID;
                        this.BLOOD_TYPE_NAME = data.BLOOD_TYPE_NAME;
                        this.VOLUME = data.VOLUME;
                        //     this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }
                    this.AMOUNT = _expMestBloodReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestBloodReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestBloodReqs.First().DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<HIS_EXP_MEST_BLTY_REQ> _expMestBloodReqs, List<V_HIS_BLOOD_TYPE> _BloodTypes, decimal REQ_AMOUNT, decimal DD_AMOUNT)
        {
            try
            {
                if (_expMestBloodReqs != null && _expMestBloodReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    var data = _BloodTypes.FirstOrDefault(p => p.ID == _expMestBloodReqs.First().BLOOD_TYPE_ID);
                    if (data != null)
                    {
                        this.BLOOD_TYPE_CODE = data.BLOOD_TYPE_CODE;
                        this.BLOOD_TYPE_ID = data.ID;
                        this.BLOOD_TYPE_NAME = data.BLOOD_TYPE_NAME;
                        this.VOLUME = data.VOLUME;
                        //     this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }
                    this.AMOUNT = _expMestBloodReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestBloodReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestBloodReqs.First().DESCRIPTION;
                    this.REQ_AMOUNT = REQ_AMOUNT;
                    this.DD_AMOUNT = DD_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs, List<V_HIS_MATERIAL_TYPE> _materialTypes)
        {
            try
            {
                if (_expMestMatyReqs != null && _expMestMatyReqs.Count > 0)
                {
                    this.TYPE_ID = 2;
                    var data = _materialTypes.FirstOrDefault(p => p.ID == _expMestMatyReqs.First().MATERIAL_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = data.ID;
                        this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                        //this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.CONCENTRA = data.CONCENTRA;
                    }
                    this.AMOUNT = _expMestMatyReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestMatyReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestMatyReqs.First().DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs, List<V_HIS_MATERIAL_TYPE> _materialTypes, decimal REQ_AMOUNT, decimal DD_AMOUNT)
        {
            try
            {
                if (_expMestMatyReqs != null && _expMestMatyReqs.Count > 0)
                {
                    this.TYPE_ID = 2;
                    var data = _materialTypes.FirstOrDefault(p => p.ID == _expMestMatyReqs.First().MATERIAL_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = data.ID;
                        this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                        //this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.CONCENTRA = data.CONCENTRA;
                    }
                    this.AMOUNT = _expMestMatyReqs.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = _expMestMatyReqs.First().NUM_ORDER;
                    this.DESCRIPTION = _expMestMatyReqs.First().DESCRIPTION;
                    this.REQ_AMOUNT = REQ_AMOUNT;
                    this.DD_AMOUNT = DD_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000135ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial, decimal REQ_AMOUNT, decimal DD_AMOUNT)
        {
            try
            {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 2;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.BID_NAME = listMaterial.First().BID_NAME;
                    this.BID_NUMBER = listMaterial.First().BID_NUMBER;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMaterial.First().EXPIRED_DATE ?? 0);
                    this.AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    this.DESCRIPTION = listMaterial.First().DESCRIPTION;
                    this.IMP_PRICE = listMaterial.First().IMP_PRICE;
                    this.IMP_VAT_RATIO = listMaterial.First().IMP_VAT_RATIO;
                    this.VIR_PRICE = listMaterial.First().VIR_PRICE;
                    this.TOTAL_PRICE = this.IMP_PRICE * this.AMOUNT;
                    this.REQ_AMOUNT = REQ_AMOUNT;
                    this.DD_AMOUNT = DD_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

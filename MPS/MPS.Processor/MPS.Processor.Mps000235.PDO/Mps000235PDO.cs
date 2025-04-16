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

namespace MPS.Processor.Mps000235.PDO
{
    public class Mps000235PDO : RDOBase
    {
        public List<Mps000235ADO> listAdo = new List<Mps000235ADO>();
        public V_HIS_EXP_MEST AggrExpMest { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public long _ExpMestSttId__Approved { get; set; }
        public long _ExpMestSttId__Exported { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        public List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        public keyTitles keyName;
        public long _ConfigKeyMERGER_DATA { get; set; }
        public V_HIS_PATIENT _Patient { get; set; }
        public List<HIS_PATIENT_TYPE_ALTER> _PatientTYpeAlters { get; set; }
        public List<HIS_SERVICE_REQ> ServiceReq_Remedy { get; set; }
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_TREATMENT_BED_ROOM _BedRoom { get; set; }
        public List<HIS_MEDI_STOCK> ListMediStock { get; set; }

        public Mps000235PDO() { }

        public Mps000235PDO(
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            long HisExpMestSttId__Approved,
            long HisExpMestSttId__Exported,
            List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes,
            keyTitles _key,
            long _configKeyMERGER_DATA,
            V_HIS_PATIENT _patient,
            List<HIS_PATIENT_TYPE_ALTER> _patientTYpeAlters,
            List<HIS_SERVICE_REQ> _ServiceReq,
            HIS_TREATMENT treatment
            )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this._ExpMestMaterials = _expMestMaterials;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this._ExpMestSttId__Approved = HisExpMestSttId__Approved;
                this._ExpMestSttId__Exported = HisExpMestSttId__Exported;
                this._MedicineTypes = vHisMedicineTypes;
                this.keyName = _key;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
                this._Patient = _patient;
                this._PatientTYpeAlters = _patientTYpeAlters;
                this.ServiceReq_Remedy = _ServiceReq;
                this._Treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000235PDO(
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            long HisExpMestSttId__Approved,
            long HisExpMestSttId__Exported,
            List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes,
            keyTitles _key,
            long _configKeyMERGER_DATA,
            V_HIS_PATIENT _patient,
            List<HIS_PATIENT_TYPE_ALTER> _patientTYpeAlters,
            List<HIS_SERVICE_REQ> _ServiceReq,
            HIS_TREATMENT treatment,
            V_HIS_TREATMENT_BED_ROOM bedRoom
            )
        {
            try
            {
                this._ExpMestMedicines = _expMestMedicines;
                this._ExpMestMaterials = _expMestMaterials;
                this.AggrExpMest = aggrExpMest;
                this._ExpMests_Print = _expMests_Print;
                this.Department = department;
                this._ExpMestSttId__Approved = HisExpMestSttId__Approved;
                this._ExpMestSttId__Exported = HisExpMestSttId__Exported;
                this._MedicineTypes = vHisMedicineTypes;
                this.keyName = _key;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
                this._Patient = _patient;
                this._PatientTYpeAlters = _patientTYpeAlters;
                this.ServiceReq_Remedy = _ServiceReq;
                this._Treatment = treatment;
                this._BedRoom = bedRoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public enum keyTitles
    {
        phieuLinhTongHop,
        phieuLinhThuocThuong,
        GayNghienHuongThan,
        GayNghien,
        HuongThan,
        VatTu,
        ThuocDoc,
        PhongXa,
        Corticoid,
        KhangSinh,
        Lao,
        DichTruyen,
        TienChat
    }

    public class Mps000235ADO
    {
        //Dangth
        public string TUTORIAL {  get; set; }
        public string HTU_TEXT { get; set; }
        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }
        public string REQ_USER_TITLE { get; set; }
        public long TYPE_ID { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }

        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string DESCRIPTION { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string IS_EXPEND_DISPLAY { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public long? NUM_ORDER { get; set; }

        public decimal AMOUNT_EXPORTED { get; set; }
        public decimal AMOUNT_EXCUTE { get; set; }
        public decimal AMOUNT_REQUEST { get; set; }

        public string AMOUNT_EXPORT_STRING { get; set; }
        public string AMOUNT_EXECUTE_STRING { get; set; }
        public string AMOUNT_REQUEST_STRING { get; set; }
        public long MEDI_MATE_NUM_ORDER { get; set; }
        public string CONCENTRA { get; set; }

        public long? REMEDY_COUNT { get; set; }

        public Mps000235ADO(
            V_HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            List<HIS_SERVICE_REQ> ServiceReq_Remedy
            )
        {
            try
            {
                if (_expMestMedicines != null && _expMestMedicines.Count > 0)
                {
                    this.TYPE_ID = 1;
                    if (_expMestMedicines[0].IS_EXPEND == 1)
                    {
                        this.IS_EXPEND_DISPLAY = "X";
                    }
                    //Dangth
                    this.TUTORIAL = _expMestMedicines[0].TUTORIAL;
                    this.HTU_TEXT = _expMestMedicines[0].HTU_TEXT;
                    this.REQ_LOGINNAME = _expMestMedicines[0].REQ_LOGINNAME;
                    this.REQ_USERNAME = _expMestMedicines[0].REQ_USERNAME;
                    this.REQ_USER_TITLE = _expMestMedicines[0].REQ_USER_TITLE;
                    this.DESCRIPTION = _expMestMedicines[0].DESCRIPTION;
                    this.MEDICINE_TYPE_CODE = _expMestMedicines[0].MEDICINE_TYPE_CODE;
                    this.MEDICINE_TYPE_ID = _expMestMedicines[0].MEDICINE_TYPE_ID;
                    this.MEDICINE_TYPE_NAME = _expMestMedicines[0].MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = _expMestMedicines[0].REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = _expMestMedicines[0].SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = _expMestMedicines[0].SERVICE_UNIT_NAME;
                    this.PACKAGE_NUMBER = _expMestMedicines[0].PACKAGE_NUMBER;
                    this.SUPPLIER_NAME = _expMestMedicines[0].SUPPLIER_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_expMestMedicines[0].EXPIRED_DATE ?? 0);
                    this.PRICE = _expMestMedicines[0].PRICE;
                    this.IMP_PRICE = _expMestMedicines[0].IMP_PRICE;
                    this.IMP_VAT_RATIO = _expMestMedicines[0].IMP_VAT_RATIO * 100;
                    this.DESCRIPTION = _expMestMedicines[0].DESCRIPTION;
                    this.MEDI_MATE_NUM_ORDER = _expMestMedicines[0].MEDICINE_NUM_ORDER ?? 0;
                    this.NUM_ORDER = _expMestMedicines[0].NUM_ORDER;

                    this.CONCENTRA = _expMestMedicines[0].CONCENTRA;

                    if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                    {
                        this.AMOUNT_EXCUTE = _expMestMedicines.Sum(p => p.AMOUNT);
                        if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.AMOUNT_EXPORTED = this.AMOUNT_EXCUTE;
                        }
                    }
                    this.AMOUNT_REQUEST = _expMestMedicines.Sum(o => o.AMOUNT);
                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXPORTED)));

                    if (ServiceReq_Remedy != null && ServiceReq_Remedy.Count > 0)
                    {
                        var reqIds = _expMestMedicines.Select(s => s.TDL_SERVICE_REQ_ID ?? 0).Distinct().ToList();
                        if (reqIds != null && reqIds.Count > 0)
                        {
                            var reqs = ServiceReq_Remedy.Where(o => reqIds.Contains(o.ID)).ToList();
                            if (reqs != null && reqs.Count > 0)
                            {
                                var count = reqs.Sum(s => s.REMEDY_COUNT ?? 0);
                                if (count > 0) this.REMEDY_COUNT = count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000235ADO(
            V_HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            List<HIS_SERVICE_REQ> ServiceReq_Remedy
            )
        {
            try
            {
                if (_expMestMaterials != null && _expMestMaterials.Count > 0)
                {
                    this.TYPE_ID = 2;
                    if (_expMestMaterials[0].IS_EXPEND == 1)
                    {
                        this.IS_EXPEND_DISPLAY = "X";
                    }
                    //Dangth
                    this.TUTORIAL = _expMestMaterials[0].TUTORIAL;
                    this.HTU_TEXT = _expMestMaterials[0].HTU_TEXT;
                    this.REQ_LOGINNAME = _expMestMaterials[0].REQ_LOGINNAME;
                    this.REQ_USERNAME = _expMestMaterials[0].REQ_USERNAME;
                    this.REQ_USER_TITLE = _expMestMaterials[0].REQ_USER_TITLE;
                    this.DESCRIPTION = _expMestMaterials[0].DESCRIPTION;
                    this.MEDICINE_TYPE_CODE = _expMestMaterials[0].MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_ID = _expMestMaterials[0].MATERIAL_TYPE_ID;
                    this.MEDICINE_TYPE_NAME = _expMestMaterials[0].MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = _expMestMaterials[0].SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = _expMestMaterials[0].SERVICE_UNIT_NAME;
                    this.PACKAGE_NUMBER = _expMestMaterials[0].PACKAGE_NUMBER;
                    this.SUPPLIER_NAME = _expMestMaterials[0].SUPPLIER_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_expMestMaterials[0].EXPIRED_DATE ?? 0);
                    this.PRICE = _expMestMaterials[0].PRICE;
                    this.IMP_PRICE = _expMestMaterials[0].IMP_PRICE;
                    this.IMP_VAT_RATIO = _expMestMaterials[0].IMP_VAT_RATIO * 100;
                    this.MEDI_MATE_NUM_ORDER = _expMestMaterials[0].MEDICINE_NUM_ORDER ?? 0;
                    this.NUM_ORDER = _expMestMaterials[0].NUM_ORDER;
                    if (_expMestMaterials[0].IS_CHEMICAL_SUBSTANCE != null)
                    {
                        this.TYPE_ID = 3;
                    }

                    if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                    {
                        this.AMOUNT_EXCUTE = _expMestMaterials.Sum(p => p.AMOUNT);

                        if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.AMOUNT_EXPORTED = this.AMOUNT_EXCUTE;
                        }
                    }
                    this.AMOUNT_REQUEST = _expMestMaterials.Sum(o => o.AMOUNT);

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXPORTED)));

                    if (ServiceReq_Remedy != null && ServiceReq_Remedy.Count > 0)
                    {
                        var reqIds = _expMestMaterials.Select(s => s.TDL_SERVICE_REQ_ID ?? 0).Distinct().ToList();
                        if (reqIds != null && reqIds.Count > 0)
                        {
                            var reqs = ServiceReq_Remedy.Where(o => reqIds.Contains(o.ID)).ToList();
                            if (reqs != null && reqs.Count > 0)
                            {
                                var count = reqs.Sum(s => s.REMEDY_COUNT ?? 0);
                                if (count > 0) this.REMEDY_COUNT = count;
                            }
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
}

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

namespace MPS.Processor.Mps000245.PDO
{
    public class Mps000245PDO : RDOBase
    {
        public List<Mps000245ADO> listAdo = new List<Mps000245ADO>();
        public V_HIS_IMP_MEST AggrImpMest { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public long _ImpMestSttId__Approved { get; set; }
        public long _ImpMestSttId__Imported { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines { get; set; }
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials { get; set; }
        public List<HIS_IMP_MEST> _ImpMests_Print { get; set; }
        public keyTitles keyName;
        public long _ConfigKeyMERGER_DATA { get; set; }
        public V_HIS_PATIENT _Patient { get; set; }
        public List<HIS_PATIENT_TYPE_ALTER> _PatientTYpeAlters { get; set; }
        public List<HIS_EXP_MEST> _MobaExpMests { get; set; }
        public HIS_TREATMENT _Treatment { get; set; }

        public Mps000245PDO() { }

        public Mps000245PDO(
    List<V_HIS_IMP_MEST_MEDICINE> _impMestMedicines,
    List<V_HIS_IMP_MEST_MATERIAL> _impMestMaterials,
    V_HIS_IMP_MEST aggrImpMest,
    List<HIS_IMP_MEST> _impMests_Print,
    HIS_DEPARTMENT department,
    long HisImpMestSttId__Approved,
    long HisImpMestSttId__Exported,
    List<V_HIS_MEDICINE_TYPE> vHisMedicineTypes,
    keyTitles _key,
    long _configKeyMERGER_DATA,
    V_HIS_PATIENT _patient,
    List<HIS_PATIENT_TYPE_ALTER> _patientTYpeAlters,
    List<HIS_EXP_MEST> _mobaExpMests,
    HIS_TREATMENT treatment
    )
        {
            try
            {
                this._ImpMestMedicines = _impMestMedicines;
                this._ImpMestMaterials = _impMestMaterials;
                this.AggrImpMest = aggrImpMest;
                this._ImpMests_Print = _impMests_Print;
                this.Department = department;
                this._ImpMestSttId__Approved = HisImpMestSttId__Approved;
                this._ImpMestSttId__Imported = HisImpMestSttId__Exported;
                this._MedicineTypes = vHisMedicineTypes;
                this.keyName = _key;
                this._ConfigKeyMERGER_DATA = _configKeyMERGER_DATA;
                this._Patient = _patient;
                this._PatientTYpeAlters = _patientTYpeAlters;
                this._MobaExpMests = _mobaExpMests;
                this._Treatment = treatment;
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

    public class Mps000245ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string DESCRIPTION { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public long? NUM_ORDER { get; set; }
        public long IMP_TIME { get; set; }

        public decimal AMOUNT_IMPORTED { get; set; }
        public decimal AMOUNT_EXCUTE { get; set; }
        public decimal AMOUNT_REQUEST { get; set; }

        public string AMOUNT_EXPORT_STRING { get; set; }
        public string AMOUNT_EXECUTE_STRING { get; set; }
        public string AMOUNT_REQUEST_STRING { get; set; }

        public long MEDI_MATE_NUM_ORDER { get; set; }

        public string CONCENTRA { get; set; }

        public Mps000245ADO(
            V_HIS_IMP_MEST _aggrImpMest,
            List<V_HIS_IMP_MEST_MEDICINE> _impMestMedicines,
            long _impMesttSttId__Approval,
            long _impMesttSttId__Import
            )
        {
            try
            {
                if (_impMestMedicines != null && _impMestMedicines.Count > 0)
                {
                    this.TYPE_ID = 1;

                    this.MEDI_MATE_TYPE_CODE = _impMestMedicines[0].MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = _impMestMedicines[0].MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = _impMestMedicines[0].MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = _impMestMedicines[0].REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = _impMestMedicines[0].SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = _impMestMedicines[0].SERVICE_UNIT_NAME;
                    this.PACKAGE_NUMBER = _impMestMedicines[0].PACKAGE_NUMBER;
                    this.SUPPLIER_NAME = _impMestMedicines[0].SUPPLIER_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_impMestMedicines[0].EXPIRED_DATE ?? 0);
                    this.EXPIRED_DATE = _impMestMedicines[0].EXPIRED_DATE;
                    this.PRICE = _impMestMedicines[0].PRICE;
                    this.IMP_PRICE = _impMestMedicines[0].IMP_PRICE;
                    this.IMP_VAT_RATIO = _impMestMedicines[0].IMP_VAT_RATIO * 100;
                    this.MEDI_MATE_NUM_ORDER = _impMestMedicines[0].MEDICINE_NUM_ORDER ?? 0;
                    this.NUM_ORDER = _impMestMedicines[0].NUM_ORDER;
                    this.IMP_TIME = _impMestMedicines[0].IMP_TIME ?? 0;
                    this.CONCENTRA = _impMestMedicines[0].CONCENTRA;

                    if (_aggrImpMest.IMP_MEST_STT_ID == _impMesttSttId__Approval || _aggrImpMest.IMP_MEST_STT_ID == _impMesttSttId__Import)
                    {
                        this.AMOUNT_EXCUTE = _impMestMedicines.Sum(p => p.AMOUNT);
                        if (_aggrImpMest.IMP_MEST_STT_ID == _impMesttSttId__Import)
                        {
                            this.AMOUNT_IMPORTED = this.AMOUNT_EXCUTE;
                        }
                    }
                    this.AMOUNT_REQUEST = _impMestMedicines.Sum(o => o.AMOUNT);
                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_IMPORTED)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000245ADO(
            V_HIS_IMP_MEST _aggrImpMest,
            List<V_HIS_IMP_MEST_MATERIAL> _impMestMaterials,
            long _impMesttSttId__Approval,
            long _impMesttSttId__Import
            )
        {
            try
            {
                if (_impMestMaterials != null && _impMestMaterials.Count > 0)
                {
                    this.TYPE_ID = 2;

                    this.MEDI_MATE_TYPE_CODE = _impMestMaterials[0].MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = _impMestMaterials[0].MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = _impMestMaterials[0].MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = _impMestMaterials[0].SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = _impMestMaterials[0].SERVICE_UNIT_NAME;
                    this.PACKAGE_NUMBER = _impMestMaterials[0].PACKAGE_NUMBER;
                    this.SUPPLIER_NAME = _impMestMaterials[0].SUPPLIER_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_impMestMaterials[0].EXPIRED_DATE ?? 0);
                    this.EXPIRED_DATE = _impMestMaterials[0].EXPIRED_DATE;
                    this.PRICE = _impMestMaterials[0].PRICE;
                    this.IMP_PRICE = _impMestMaterials[0].IMP_PRICE;
                    this.IMP_VAT_RATIO = _impMestMaterials[0].IMP_VAT_RATIO * 100;
                    this.MEDI_MATE_NUM_ORDER = _impMestMaterials[0].MEDICINE_NUM_ORDER ?? 0;
                    this.NUM_ORDER = _impMestMaterials[0].NUM_ORDER;

                    this.IMP_TIME = _impMestMaterials[0].IMP_TIME ?? 0;

                    if (_aggrImpMest.IMP_MEST_STT_ID == _impMesttSttId__Approval || _aggrImpMest.IMP_MEST_STT_ID == _impMesttSttId__Import)
                    {
                        this.AMOUNT_EXCUTE = _impMestMaterials.Sum(p => p.AMOUNT);

                        if (_aggrImpMest.IMP_MEST_STT_ID == _impMesttSttId__Import)
                        {
                            this.AMOUNT_IMPORTED = this.AMOUNT_EXCUTE;
                        }
                    }
                    this.AMOUNT_REQUEST = _impMestMaterials.Sum(o => o.AMOUNT);

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_IMPORTED)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

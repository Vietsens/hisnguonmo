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

namespace MPS.Processor.Mps000175.PDO
{
    public partial class Mps000175PDO : RDOBase
    {
        public V_HIS_EXP_MEST AggrExpMest { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public List<long> ServiceUnitIds { get; set; }
        public List<long> RoomIds { get; set; }
    }
    public enum keyTitles
    {
        phieuLinhVatTu,
        phieuLinhHoaChat
    }

    public class Mps000175Config
    {
        public long _ExpMestSttId__Approved { get; set; }
        public long _ExpMestSttId__Exported { get; set; }
        public long _ConfigKeyMERGER_DATA { get; set; }
        public long PatientTypeId__BHYT { get; set; }
    }

    public class Mps000175ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

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

        public decimal? HEIN_AMOUNT { get; set; }

        public string CONCENTRA { get; set; }

        public decimal? VAT_RATIO { get; set; }

        public string MANUFACTURER_CODE { get; set; }
        public long? MANUFACTURER_ID { get; set; }
        public string MANUFACTURER_NAME { get; set; }

        public Mps000175ADO() { }

        public Mps000175ADO(
            V_HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            long PatientTypeId__BHYT,
            List<V_HIS_MATERIAL_TYPE> _materialTypes
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
                    this.VAT_RATIO = _expMestMaterials[0].VAT_RATIO;
                    this.DESCRIPTION = _expMestMaterials[0].DESCRIPTION;
                    this.MEDICINE_TYPE_CODE = _expMestMaterials[0].MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = _expMestMaterials[0].MATERIAL_TYPE_ID;
                    this.MEDICINE_TYPE_NAME = _expMestMaterials[0].MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = _expMestMaterials[0].SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = _expMestMaterials[0].SERVICE_UNIT_NAME;
                    this.PACKAGE_NUMBER = _expMestMaterials[0].PACKAGE_NUMBER;
                    this.SUPPLIER_NAME = _expMestMaterials[0].SUPPLIER_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_expMestMaterials[0].EXPIRED_DATE ?? 0);
                    this.PRICE = _expMestMaterials[0].PRICE;
                    this.IMP_PRICE = _expMestMaterials[0].IMP_PRICE;
                    this.IMP_VAT_RATIO = _expMestMaterials[0].IMP_VAT_RATIO * 100;
                    this.DESCRIPTION = _expMestMaterials[0].DESCRIPTION;
                    this.MEDI_MATE_NUM_ORDER = _expMestMaterials[0].MEDICINE_NUM_ORDER ?? 0;
                    this.NUM_ORDER = _expMestMaterials[0].NUM_ORDER;

                    this.MANUFACTURER_CODE = _expMestMaterials[0].MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = _expMestMaterials[0].MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = _expMestMaterials[0].MANUFACTURER_NAME;

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

                    var dataBHYTs = _expMestMaterials.Where(p => p.PATIENT_TYPE_ID == PatientTypeId__BHYT).ToList();
                    if (dataBHYTs != null && dataBHYTs.Count > 0)
                    {
                        this.HEIN_AMOUNT = dataBHYTs.Sum(p => p.AMOUNT);
                    }
                    else
                    {
                        this.HEIN_AMOUNT = null;
                    }

                    this.AMOUNT_REQUEST = _expMestMaterials.Where(o => o.AGGR_EXP_MEST_ID.HasValue).Sum(o => o.AMOUNT);

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXPORTED)));

                    var maty = _materialTypes != null ? _materialTypes.FirstOrDefault(o => o.ID == _expMestMaterials[0].MATERIAL_TYPE_ID) : null;
                    if (maty != null)
                    {
                        this.CONCENTRA = maty.CONCENTRA;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000175ADO(
            long mert,
            V_HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export,
            long PatientTypeId__BHYT,
            List<V_HIS_MATERIAL_TYPE> _materialTypes
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
                    this.VAT_RATIO = _expMestMaterials[0].VAT_RATIO;
                    this.DESCRIPTION = _expMestMaterials[0].DESCRIPTION;
                    this.MEDICINE_TYPE_CODE = _expMestMaterials[0].MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = _expMestMaterials[0].MATERIAL_TYPE_ID;
                    this.MEDICINE_TYPE_NAME = _expMestMaterials[0].MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = _expMestMaterials[0].SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = _expMestMaterials[0].SERVICE_UNIT_NAME;
                    this.PACKAGE_NUMBER = _expMestMaterials[0].PACKAGE_NUMBER;
                    this.SUPPLIER_NAME = _expMestMaterials[0].SUPPLIER_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_expMestMaterials[0].EXPIRED_DATE ?? 0);
                    this.PRICE = _expMestMaterials[0].PRICE;
                    this.IMP_PRICE = _expMestMaterials[0].IMP_PRICE;
                    this.IMP_VAT_RATIO = _expMestMaterials[0].IMP_VAT_RATIO * 100;
                    this.DESCRIPTION = _expMestMaterials[0].DESCRIPTION;
                    this.MEDI_MATE_NUM_ORDER = _expMestMaterials[0].MEDICINE_NUM_ORDER ?? 0;
                    this.NUM_ORDER = _expMestMaterials[0].NUM_ORDER;

                    this.MANUFACTURER_CODE = _expMestMaterials[0].MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = _expMestMaterials[0].MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = _expMestMaterials[0].MANUFACTURER_NAME;

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

                    var dataBHYTs = _expMestMaterials.Where(p => p.PATIENT_TYPE_ID == PatientTypeId__BHYT).ToList();
                    if (dataBHYTs != null && dataBHYTs.Count > 0)
                    {
                        this.HEIN_AMOUNT = dataBHYTs.Sum(p => p.AMOUNT);
                    }
                    else
                    {
                        this.HEIN_AMOUNT = null;
                    }

                    this.AMOUNT_REQUEST = _expMestMaterials.Where(o => o.AGGR_EXP_MEST_ID.HasValue).Sum(o => o.AMOUNT);

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXPORTED)));

                    var maty = _materialTypes != null ? _materialTypes.FirstOrDefault(o => o.ID == _expMestMaterials[0].MATERIAL_TYPE_ID) : null;
                    if (maty != null)
                    {
                        this.CONCENTRA = maty.CONCENTRA;
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

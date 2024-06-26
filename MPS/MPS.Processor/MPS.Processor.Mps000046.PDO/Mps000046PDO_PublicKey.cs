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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MOS.SDO;
using MPS.Processor.Mps000046.PDO;

namespace MPS.Processor.Mps000046.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000046PDO : RDOBase
    {
        public V_HIS_EXP_MEST AggrExpMest { get; set; }
        public HIS_DEPARTMENT Department { get; set; }
        public List<long> ServiceUnitIds { get; set; }
        public List<long> UseFormIds { get; set; }
        public List<long> RoomIds { get; set; }
        public bool IsMedicine { get; set; }
        public bool Ismaterial { get; set; }
        public bool IsChemicalSustance { get; set; }
        public long _ExpMestSttId__Approved { get; set; }
        public long _ExpMestSttId__Exported { get; set; }
    }
    public enum keyTitles
    {
        phieuLinhTongHop,
        phieuLinhThuocThuong,
        Corticoid,
        KhangSinh,
        Lao,
        DichTruyen,
        TienChat,
    }

    public class Mps000046ADO
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

        public string CONCENTRA { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }

        public string MEDICINE_USE_FORM_CODE { get; set; }
        public long? MEDICINE_USE_FORM_ID { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public long? MEDICINE_USE_FORM_NUM_ORDER { get; set; }

        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }

        public long? VAT_RATIO { get; set; }

        public Mps000046ADO(
            V_HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export
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
                    this.DESCRIPTION = _expMestMedicines[0].DESCRIPTION;
                    this.MEDICINE_TYPE_CODE = _expMestMedicines[0].MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(_expMestMedicines[0].MEDICINE_TYPE_ID.ToString() + this.TYPE_ID.ToString());
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

                    this.MEDICINE_USE_FORM_ID = _expMestMedicines[0].MEDICINE_USE_FORM_ID;
                    this.MEDICINE_USE_FORM_CODE = _expMestMedicines[0].MEDICINE_USE_FORM_CODE;
                    this.MEDICINE_USE_FORM_NAME = _expMestMedicines[0].MEDICINE_USE_FORM_NAME;
                    this.MEDICINE_USE_FORM_NUM_ORDER = _expMestMedicines[0].MEDICINE_USE_FORM_NUM_ORDER;

                    this.CONCENTRA = _expMestMedicines[0].CONCENTRA;

                    if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                    {
                        this.AMOUNT_EXCUTE = _expMestMedicines.Sum(p => p.AMOUNT);
                        if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.AMOUNT_EXPORTED = this.AMOUNT_EXCUTE;
                        }
                    }

                    this.AMOUNT_REQUEST = _expMestMedicines.Where(o => o.AGGR_EXP_MEST_ID.HasValue).Sum(o => o.AMOUNT);

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXPORTED)));
                    this.ACTIVE_INGR_BHYT_CODE = _expMestMedicines[0].ACTIVE_INGR_BHYT_CODE;
                    this.ACTIVE_INGR_BHYT_NAME = _expMestMedicines[0].ACTIVE_INGR_BHYT_NAME;
                    this.REQ_LOGINNAME = _expMestMedicines[0].REQ_LOGINNAME;
                    this.REQ_USERNAME = _expMestMedicines[0].REQ_USERNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000046ADO(
            V_HIS_EXP_MEST _expMest,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export
            )
        {
            try
            {
                if (_expMestMaterials != null && _expMestMaterials.Count > 0)
                {
                    this.TYPE_ID = 2;
                    if (_expMestMaterials[0].IS_CHEMICAL_SUBSTANCE != null)
                    {
                        this.TYPE_ID = 3;
                    }
                    if (_expMestMaterials[0].IS_EXPEND == 1)
                    {
                        this.IS_EXPEND_DISPLAY = "X";
                    }
                    this.DESCRIPTION = _expMestMaterials[0].DESCRIPTION;
                    this.MEDICINE_TYPE_CODE = _expMestMaterials[0].MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(_expMestMaterials[0].MATERIAL_TYPE_ID.ToString() + this.TYPE_ID.ToString());
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

                    this.REQ_LOGINNAME = _expMestMaterials[0].REQ_LOGINNAME;
                    this.REQ_USERNAME = _expMestMaterials[0].REQ_USERNAME;

                    if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                    {

                        this.AMOUNT_EXCUTE = _expMestMaterials.Sum(p => p.AMOUNT);
                        if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.AMOUNT_EXPORTED = this.AMOUNT_EXCUTE;
                        }
                    }
                    this.AMOUNT_REQUEST = _expMestMaterials.Where(o => o.AGGR_EXP_MEST_ID.HasValue).Sum(o => o.AMOUNT);

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXCUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT_EXPORTED)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

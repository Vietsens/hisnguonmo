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

namespace MPS.Processor.Mps000086.PDO
{
    public partial class Mps000086PDO : RDOBase
    {
        public V_HIS_EXP_MEST _ChmsExpMest;
        public List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = null;
        public List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods = null;
        public string Req_Department_Name = "";
        public string Req_Room_Name = "";
        public string Exp_Department_Name = "";
        public string KeyNames = "";

        public long expMesttSttId__Approval = 4; // duyệt
        public long expMesttSttId__Export = 5;// đã xuất

        public List<Mps000086ADO> listAdo = new List<Mps000086ADO>();

        public List<HIS_MEDICINE> _Medicines = null;
        public List<HIS_MATERIAL> _Materials = null;
        public List<HIS_BLOOD> _Bloods = null;

    }

    public class Mps000086ADO
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
        public decimal? IMP_PRICE_RATIO { get; set; }
        public string DESCRIPTION { get; set; }
        public string AMOUNT_EXPORT_STRING { get; set; }
        public string AMOUNT_EXECUTE_STRING { get; set; }
        public string AMOUNT_REQUEST_STRING { get; set; }
        public long MEDI_MATE_NUM_ORDER { get; set; }
        public long? NUM_ORDER { get; set; }
        public string BLOOD_ADO_CODE { get; set; }
        public string BLOOD_RH_CODE { get; set; }
        public decimal? VOLUME { get; set; }
        public decimal? VIR_PRICE { get; set; }
        public decimal? VIR_IMP_PRICE { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string PARENT_MEDICINE_TYPE_NAME { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }

        public string PACKAGE_NUMBER_TOTAL { get; set; }

        public long? MEDICINE_USE_FORM_NUM_ORDER { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }

        public string BID_NUMBER { get; set; }
        public string BID_PACKAGE_CODE { get; set; }
        public string BID_GROUP_CODE { get; set; }
        public string BID_YEAR { get; set; }
        public decimal EXP_PRICE_VP { get; set; }
        public decimal EXP_VAT_RATIO_VP { get; set; }

        public Mps000086ADO()
        {
        }

        public Mps000086ADO(
            V_HIS_EXP_MEST _ChmsExpMest,
            List<HIS_EXP_MEST_METY_REQ> _expMestMetyReqs,
            List<V_HIS_MEDICINE_TYPE> _medicineTypes,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMeidicines,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export
            )
        {
            try
            {
                if (_expMestMetyReqs != null && _expMestMetyReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.DESCRIPTION = _expMestMetyReqs[0].DESCRIPTION;
                    var data = _medicineTypes.FirstOrDefault(p => p.ID == _expMestMetyReqs[0].MEDICINE_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(_expMestMetyReqs.First().MEDICINE_TYPE_ID.ToString() + this.TYPE_ID.ToString());
                        this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                        this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }

                    if (_ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Export
                        || _ChmsExpMest.IS_REQUEST_BY_PACKAGE == 1)
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> listMedicines = (
                            _expMestMeidicines != null && _expMestMeidicines.Count > 0)
                            ? _expMestMeidicines.Where(p =>
                                p.MEDICINE_TYPE_ID == _expMestMetyReqs[0].MEDICINE_TYPE_ID).ToList()
                                : null;
                        if (listMedicines != null && listMedicines.Count > 0)
                        {
                            this.TOTAL_AMOUNT_IN_EXECUTE = listMedicines.Sum(p => p.AMOUNT);
                            this.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                            this.SUPPLIER_CODE = listMedicines.First().SUPPLIER_CODE;
                            this.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                            this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                            this.PRICE = listMedicines.First().PRICE;
                            this.IMP_PRICE = listMedicines.First().IMP_PRICE;
                            this.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO;
                            this.IMP_PRICE_RATIO = this.IMP_PRICE + this.IMP_PRICE * this.IMP_VAT_RATIO;
                            this.DESCRIPTION = listMedicines.First().DESCRIPTION;
                            this.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                            this.NUM_ORDER = listMedicines.First().NUM_ORDER;
                            this.VIR_PRICE = listMedicines.Sum(p => p.VIR_PRICE);
                            this.ACTIVE_INGR_BHYT_CODE = listMedicines.First().ACTIVE_INGR_BHYT_CODE;
                            this.ACTIVE_INGR_BHYT_NAME = listMedicines.First().ACTIVE_INGR_BHYT_NAME;
                            this.MEDICINE_USE_FORM_NUM_ORDER = listMedicines.First().MEDICINE_USE_FORM_NUM_ORDER;
                            this.MEDICINE_TYPE_NAME = listMedicines.First().MEDICINE_TYPE_NAME;
                        }
                        if (_ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                        }
                    }
                    this.TOTAL_AMOUNT_IN_REQUEST = _expMestMetyReqs.Sum(o => o.AMOUNT);

                    this.TOTAL_AMOUNT = this.TOTAL_AMOUNT_IN_REQUEST;
                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXECUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXPORT)));

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000086ADO(
            V_HIS_EXP_MEST _ChmsExpMest,
            List<HIS_EXP_MEST_MATY_REQ> _expMestMatyReqs,
            List<V_HIS_MATERIAL_TYPE> _materialTypes,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export
            )
        {
            try
            {
                if (_expMestMatyReqs != null && _expMestMatyReqs.Count > 0)
                {
                    this.TYPE_ID = 2;
                    this.DESCRIPTION = _expMestMatyReqs[0].DESCRIPTION;
                    var data = _materialTypes.FirstOrDefault(p => p.ID == _expMestMatyReqs[0].MATERIAL_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(_expMestMatyReqs.First().MATERIAL_TYPE_ID.ToString() + this.TYPE_ID.ToString());
                        this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    }

                    if (_ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Export
                        || _ChmsExpMest.IS_REQUEST_BY_PACKAGE == 1)
                    {
                        List<V_HIS_EXP_MEST_MATERIAL> listMedicines = (
                            _expMestMaterials != null && _expMestMaterials.Count > 0)
                            ? _expMestMaterials.Where(p =>
                                p.MATERIAL_TYPE_ID == _expMestMatyReqs[0].MATERIAL_TYPE_ID).ToList()
                                : null;
                        if (listMedicines != null && listMedicines.Count > 0)
                        {
                            this.TOTAL_AMOUNT_IN_EXECUTE = listMedicines.Sum(p => p.AMOUNT);
                            this.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                            this.SUPPLIER_CODE = listMedicines.First().SUPPLIER_CODE;
                            this.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                            this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                            this.PRICE = listMedicines.First().PRICE;
                            this.IMP_PRICE = listMedicines.First().IMP_PRICE;
                            this.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO * 100;
                            this.DESCRIPTION = listMedicines.First().DESCRIPTION;
                            this.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                            this.NUM_ORDER = listMedicines.First().NUM_ORDER;
                            this.VIR_PRICE = listMedicines.Sum(p => p.VIR_PRICE);
                            this.MEDICINE_TYPE_NAME = listMedicines.First().MATERIAL_TYPE_NAME;
                        }
                        if (_ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                        }
                    }
                    this.TOTAL_AMOUNT_IN_REQUEST = _expMestMatyReqs.Sum(o => o.AMOUNT);

                    this.TOTAL_AMOUNT = this.TOTAL_AMOUNT_IN_REQUEST;
                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXECUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXPORT)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000086ADO(
           V_HIS_EXP_MEST _ChmsExpMest,
           List<HIS_EXP_MEST_BLTY_REQ> _expMestBltyReqs,
           List<V_HIS_BLOOD_TYPE> _bloodTypes,
           List<V_HIS_EXP_MEST_BLOOD> _expMestBloods,
            List<HIS_BLOOD_ABO> _bloodABOs,
            List<HIS_BLOOD_RH> _bloodRHs,
           long _expMesttSttId__Approval,
           long _expMesttSttId__Export
           )
        {
            try
            {
                if (_expMestBltyReqs != null && _expMestBltyReqs.Count > 0)
                {
                    this.TYPE_ID = 3;
                    if (_expMestBltyReqs[0].IS_EXPEND == 1)
                    {
                        this.IS_EXPEND_DISPLAY = "X";
                    }
                    this.DESCRIPTION = _expMestBltyReqs[0].DESCRIPTION;
                    var abo = _bloodABOs.FirstOrDefault(p => p.ID == _expMestBltyReqs[0].BLOOD_ABO_ID);
                    if (abo != null)
                    {
                        this.BLOOD_ADO_CODE = abo.BLOOD_ABO_CODE;
                    }
                    var rh = _bloodRHs.FirstOrDefault(p => p.ID == _expMestBltyReqs[0].BLOOD_RH_ID);
                    if (rh != null)
                    {
                        this.BLOOD_RH_CODE = rh.BLOOD_RH_CODE;
                    }
                    var data = _bloodTypes.FirstOrDefault(p => p.ID == _expMestBltyReqs[0].BLOOD_TYPE_ID);
                    if (data != null)
                    {
                        this.MEDI_MATE_TYPE_CODE = data.BLOOD_TYPE_CODE;
                        this.MEDI_MATE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(_expMestBltyReqs.First().BLOOD_TYPE_ID.ToString() + this.TYPE_ID.ToString());
                        this.MEDI_MATE_TYPE_NAME = data.BLOOD_TYPE_NAME;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.VOLUME = data.VOLUME;
                        this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                    }

                    if (_ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                    {
                        List<V_HIS_EXP_MEST_BLOOD> listBloods = (
                            _expMestBloods != null && _expMestBloods.Count > 0)
                            ? _expMestBloods.Where(p =>
                                p.BLOOD_TYPE_ID == _expMestBltyReqs[0].BLOOD_TYPE_ID).ToList()
                                : null;
                        if (listBloods != null && listBloods.Count > 0)
                        {
                            this.TOTAL_AMOUNT_IN_EXECUTE = listBloods.Count;
                            this.PACKAGE_NUMBER = listBloods.First().PACKAGE_NUMBER;
                            this.SUPPLIER_CODE = listBloods.First().SUPPLIER_CODE;
                            this.SUPPLIER_NAME = listBloods.First().SUPPLIER_NAME;
                            this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listBloods.First().EXPIRED_DATE ?? 0);
                            this.PRICE = listBloods.First().PRICE;
                            this.IMP_PRICE = listBloods.First().IMP_PRICE;
                            this.IMP_VAT_RATIO = listBloods.First().IMP_VAT_RATIO * 100;
                            this.DESCRIPTION = listBloods.First().DESCRIPTION;
                            this.MEDI_MATE_NUM_ORDER = 9999;
                            this.NUM_ORDER = listBloods.First().NUM_ORDER;
                            this.VIR_PRICE = listBloods.Sum(p => p.VIR_PRICE);
                            this.MEDICINE_TYPE_NAME = listBloods.First().BLOOD_TYPE_NAME;
                            this.BID_NUMBER = string.Join(",", listBloods.Select(p => p.BID_NUMBER).Distinct().ToList());
                        }
                        if (_ChmsExpMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                        }
                    }
                    this.TOTAL_AMOUNT_IN_REQUEST = _expMestBltyReqs.Sum(o => o.AMOUNT);

                    this.TOTAL_AMOUNT = this.TOTAL_AMOUNT_IN_REQUEST;
                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_REQUEST)));
                    this.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXECUTE)));
                    this.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.TOTAL_AMOUNT_IN_EXPORT)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

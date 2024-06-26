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

namespace MPS.Processor.Mps000198.PDO
{
    public partial class Mps000198PDO : RDOBase
    {
        public V_HIS_EXP_MEST _ChmsExpMest;
    }

    public class Mps000198ADO
    {
        public long TYPE_ID { get; set; }
        public long BLOOD_TYPE_ID { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string IS_EXPEND_DISPLAY { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal TOTAL_AMOUNT_IN_EXECUTE { get; set; }
        public decimal TOTAL_AMOUNT_IN_EXPORT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public decimal? IMP_VAT_RATIO { get; set; }
        public string DESCRIPTION { get; set; }
        public string AMOUNT_EXPORT_STRING { get; set; }
        public string AMOUNT_EXECUTE_STRING { get; set; }
        public string AMOUNT_REQUEST_STRING { get; set; }
        public long BLOOD_NUM_ORDER { get; set; }
        public long? NUM_ORDER { get; set; }
        public string BLOOD_CODE { get; set; }
        public string BLOOD_ABO_CODE { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
        public string BLOOD_RH_CODE { get; set; }
        public decimal? VOLUME { get; set; }

        public Mps000198ADO()
        {
        }

        public Mps000198ADO(
            V_HIS_EXP_MEST _expMest,
            List<HIS_EXP_MEST_BLTY_REQ> _expMestBltyReqs,
            List<V_HIS_EXP_MEST_BLOOD> _expMestBloods,
            List<V_HIS_BLOOD_TYPE> _bloodTypes,
             List<HIS_BLOOD_ABO> _bloodABOs,
            List<HIS_BLOOD_RH> _bloodRHs,
            long _expMesttSttId__Approval,
            long _expMesttSttId__Export)
        {
            try
            {
                if (_expMestBltyReqs != null && _expMestBltyReqs.Count > 0)
                {
                    this.TYPE_ID = 1;
                    if (_expMestBltyReqs[0].IS_EXPEND == 1)
                    {
                        this.IS_EXPEND_DISPLAY = "X";
                    }
                    this.AMOUNT = _expMestBltyReqs.Sum(p => p.AMOUNT);
                    var abo = _bloodABOs.FirstOrDefault(p => p.ID == _expMestBltyReqs[0].BLOOD_ABO_ID);
                    if (abo != null)
                    {
                        this.BLOOD_ABO_CODE = abo.BLOOD_ABO_CODE;
                    }
                    var rh = _bloodRHs.FirstOrDefault(p => p.ID == _expMestBltyReqs[0].BLOOD_RH_ID);
                    if (rh != null)
                    {
                        this.BLOOD_RH_CODE = rh.BLOOD_RH_CODE;
                    }
                    var data = _bloodTypes.FirstOrDefault(p => p.ID == _expMestBltyReqs[0].BLOOD_TYPE_ID);
                    if (data != null)
                    {
                        this.BLOOD_TYPE_CODE = data.BLOOD_TYPE_CODE;
                        this.BLOOD_TYPE_ID = data.ID;
                        this.BLOOD_TYPE_NAME = data.BLOOD_TYPE_NAME;
                        this.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                        this.VOLUME = data.VOLUME;
                    }

                    if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Approval || _expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
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
                            this.BLOOD_NUM_ORDER = 9999;
                            this.BLOOD_CODE = listBloods.First().BLOOD_CODE;
                            this.NUM_ORDER = listBloods.First().NUM_ORDER;
                        }
                        if (_expMest.EXP_MEST_STT_ID == _expMesttSttId__Export)
                        {
                            this.TOTAL_AMOUNT_IN_EXPORT = this.TOTAL_AMOUNT_IN_EXECUTE;
                        }
                    }

                    this.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(this.AMOUNT)));
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

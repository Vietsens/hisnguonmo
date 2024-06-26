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

namespace MPS.Processor.Mps000344
{
    public class Mps000344ADO
    {
        public long TYPE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }

        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }

        public decimal? DISCOUNT { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? PRICE_VAT { get; set; }
        public decimal? VAT_RATIO_100 { get; set; }
        public decimal SUM_TOTAL_PRICE { get; set; }
        public decimal SUM_TOTAL_PRICE_ROUND { get; set; }

        public long? NUM_ORDER { get; set; }

        public Mps000344ADO()
        {
        }

        public Mps000344ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
        {
            try
            {
                if (listMedicine != null && listMedicine.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMedicine.First().MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMedicine.First().MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMedicine.First().MEDICINE_TYPE_NAME;
                    this.REGISTER_NUMBER = listMedicine.First().REGISTER_NUMBER;
                    this.SERVICE_UNIT_CODE = listMedicine.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMedicine.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMedicine.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMedicine.First().SUPPLIER_NAME;
                    this.PRICE = listMedicine.First().PRICE;
                    this.TUTORIAL = listMedicine.First().TUTORIAL;
                    this.PACKAGE_NUMBER = listMedicine.First().PACKAGE_NUMBER;
                    if (listMedicine.First().VAT_RATIO.HasValue)
                    {
                        this.VAT_RATIO_100 = listMedicine.First().VAT_RATIO.Value * 100;
                    }
                    this.DISCOUNT = listMedicine.Sum(o => o.DISCOUNT ?? 0);
                    this.TOTAL_AMOUNT = listMedicine.Sum(s => s.AMOUNT - (s.TH_AMOUNT ?? 0));
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.PRICE_VAT = this.PRICE.Value * (1 + vat);
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat) - (this.DISCOUNT ?? 0);
                        this.SUM_TOTAL_PRICE_ROUND = Math.Round(this.SUM_TOTAL_PRICE, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000344ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
        {
            try
            {
                if (listMaterial != null && listMaterial.Count > 0)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_CODE = listMaterial.First().MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_ID = listMaterial.First().MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_NAME = listMaterial.First().MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_CODE = listMaterial.First().SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_NAME = listMaterial.First().SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = listMaterial.First().SUPPLIER_CODE;
                    this.SUPPLIER_NAME = listMaterial.First().SUPPLIER_NAME;
                    this.PRICE = listMaterial.First().PRICE;
                    this.PACKAGE_NUMBER = listMaterial.First().PACKAGE_NUMBER;
                    if (listMaterial.First().VAT_RATIO.HasValue)
                    {
                        this.VAT_RATIO_100 = listMaterial.First().VAT_RATIO.Value * 100;
                    }
                    this.DISCOUNT = listMaterial.Sum(o => o.DISCOUNT ?? 0);
                    this.TOTAL_AMOUNT = listMaterial.Sum(s => s.AMOUNT - (s.TH_AMOUNT ?? 0));
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.PRICE_VAT = this.PRICE.Value * (1 + vat);
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat) - (this.DISCOUNT ?? 0);
                        this.SUM_TOTAL_PRICE_ROUND = Math.Round(this.SUM_TOTAL_PRICE, 0, MidpointRounding.AwayFromZero);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
        public Mps000344ADO(HIS_BILL_GOODS billGoods)
        {
            try
            {
                if (billGoods != null)
                {
                    this.TYPE_ID = 1;
                    this.MEDI_MATE_TYPE_NAME = billGoods.GOODS_NAME;
                    this.SERVICE_UNIT_NAME = billGoods.GOODS_UNIT_NAME;
                    this.PRICE = billGoods.PRICE;
                    this.DISCOUNT = billGoods.DISCOUNT;
                    this.TOTAL_AMOUNT = billGoods.AMOUNT;
                    if (this.PRICE.HasValue)
                    {
                        var vat = this.VAT_RATIO_100.HasValue ? this.VAT_RATIO_100.Value / 100 : 0;
                        this.PRICE_VAT = this.PRICE.Value * (1 + vat);
                        this.SUM_TOTAL_PRICE = this.TOTAL_AMOUNT * this.PRICE.Value * (1 + vat) - (this.DISCOUNT ?? 0);
                        this.SUM_TOTAL_PRICE_ROUND = Math.Round(this.SUM_TOTAL_PRICE, 0, MidpointRounding.AwayFromZero);
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

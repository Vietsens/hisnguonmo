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

namespace MPS.ADO
{
    public class Mps000084ADO
    {
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string BID_NUMBER { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal VAT_RATIO_100 { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        
        public decimal EXP_PRICE { get; set; }
        public decimal EXP_VAT_RATIO { get; set; }
        public decimal EXP_TOTAL_PRICE { get; set; }
        public decimal? DISCOUNT { get; set; }


        public Mps000084ADO() { }

        public Mps000084ADO(V_HIS_IMP_MEST_MEDICINE data)
        {
            try
            {
                if (data != null)
                {
                    this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = data.SUPPLIER_CODE;
                    this.SUPPLIER_NAME = data.SUPPLIER_NAME;
                    this.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                    this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.BID_NUMBER = data.BID_NUMBER;
                    this.AMOUNT = data.AMOUNT;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                    this.VAT_RATIO_100 = data.IMP_VAT_RATIO * 100;
                    this.TOTAL_PRICE = (1 + data.IMP_VAT_RATIO) * data.AMOUNT * data.IMP_PRICE;
                    this.EXP_TOTAL_PRICE = (1 + (data.VAT_RATIO ?? 0)) * data.AMOUNT * (data.PRICE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000084ADO(V_HIS_IMP_MEST_MATERIAL data)
        {
            try
            {
                if (data != null)
                {
                    this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = data.SUPPLIER_CODE;
                    this.SUPPLIER_NAME = data.SUPPLIER_NAME;
                    this.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                    this.BID_NUMBER = data.BID_NUMBER;
                    this.AMOUNT = data.AMOUNT;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                    this.VAT_RATIO_100 = data.IMP_VAT_RATIO * 100;
                    this.TOTAL_PRICE = (1 + data.IMP_VAT_RATIO) * data.AMOUNT * data.IMP_PRICE;
                    this.EXP_TOTAL_PRICE = (1 + (data.VAT_RATIO ?? 0)) * data.AMOUNT * (data.PRICE ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

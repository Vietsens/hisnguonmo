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

namespace MPS.Processor.Mps000155.PDO
{
    public partial class Mps000155PDO : RDOBase
    {
        public V_HIS_EXP_MEST _LiquExpMest = null;
        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;

        public List<Mps000155ADO> listAdo = new List<Mps000155ADO>();
    }

    public class Mps000155ADO
    {
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
        public decimal TOTAL_AMOUNT { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal? DISCOUNT { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public string EXP_TIME_STR { get; set; }
        public string IMP_TIME_STR { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public decimal TOTAL_MONEY { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? VAT_RATIO { get; set; }
        public long? NUM_ORDER { get; set; }
        public string VAT_RATIO_100 { get; set; }
        public decimal TOTAL_EXP_MONEY { get; set; }

        public Mps000155ADO()
        {
        }

        public Mps000155ADO(List<V_HIS_EXP_MEST_MEDICINE> listMedicine)
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
                    this.BID_NAME = listMedicine.First().BID_NAME;
                    this.MANUFACTURER_NAME = listMedicine.First().MANUFACTURER_NAME;
                    this.BID_NUMBER = listMedicine.First().BID_NUMBER;
                    this.NATIONAL_NAME = listMedicine.First().NATIONAL_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().EXPIRED_DATE ?? 0);
                    this.EXP_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().EXP_TIME ?? 0);
                    this.IMP_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicine.First().IMP_TIME ?? 0);
                    this.AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.TOTAL_AMOUNT = listMedicine.Sum(s => s.AMOUNT);
                    this.DESCRIPTION = listMedicine.First().DESCRIPTION;
                    this.DISCOUNT = listMedicine.First().DISCOUNT;
                    this.EXP_MEST_CODE = listMedicine.First().EXP_MEST_CODE;
                    this.IMP_PRICE = listMedicine.First().IMP_PRICE;
                    this.IMP_VAT_RATIO = listMedicine.First().IMP_VAT_RATIO;
                    this.PRICE = listMedicine.First().PRICE;
                    this.VAT_RATIO = listMedicine.First().VAT_RATIO;
                    this.VAT_RATIO_100 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(listMedicine.First().IMP_VAT_RATIO * 100);
                    this.NUM_ORDER = listMedicine.First().NUM_ORDER;
                    this.TOTAL_MONEY = (listMedicine.First().AMOUNT * (listMedicine.First().IMP_PRICE) * (listMedicine.First().IMP_VAT_RATIO + 1)) - (listMedicine.First().DISCOUNT ?? 0);
                    this.TOTAL_EXP_MONEY = (listMedicine.First().AMOUNT * (listMedicine.First().PRICE ?? 0) * ((listMedicine.First().VAT_RATIO ?? 0) + 1)) - (listMedicine.First().DISCOUNT ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000155ADO(List<V_HIS_EXP_MEST_MATERIAL> listMaterial)
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
                    this.MANUFACTURER_NAME = listMaterial.First().MANUFACTURER_NAME;
                    this.BID_NAME = listMaterial.First().BID_NAME;
                    this.BID_NUMBER = listMaterial.First().BID_NUMBER;
                    this.NATIONAL_NAME = listMaterial.First().NATIONAL_NAME;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMaterial.First().EXPIRED_DATE ?? 0);
                    this.EXP_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMaterial.First().EXP_TIME ?? 0);
                    this.AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.TOTAL_AMOUNT = listMaterial.Sum(s => s.AMOUNT);
                    this.NUM_ORDER = listMaterial.First().NUM_ORDER;
                    this.DESCRIPTION = listMaterial.First().DESCRIPTION;
                    this.DISCOUNT = listMaterial.First().DISCOUNT;
                    this.EXP_MEST_CODE = listMaterial.First().EXP_MEST_CODE;
                    this.IMP_PRICE = listMaterial.First().IMP_PRICE;
                    this.IMP_VAT_RATIO = listMaterial.First().IMP_VAT_RATIO;
                    this.VAT_RATIO_100 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(listMaterial.First().IMP_VAT_RATIO * 100);
                    this.PRICE = listMaterial.First().PRICE;
                    this.VAT_RATIO = listMaterial.First().VAT_RATIO;
                    this.TOTAL_MONEY = (listMaterial.First().AMOUNT * (listMaterial.First().IMP_PRICE) * (listMaterial.First().IMP_VAT_RATIO + 1)) - (listMaterial.First().DISCOUNT ?? 0);
                    this.TOTAL_EXP_MONEY = (listMaterial.First().AMOUNT * (listMaterial.First().PRICE ?? 0) * ((listMaterial.First().VAT_RATIO ?? 0) + 1)) - (listMaterial.First().DISCOUNT ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

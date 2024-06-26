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

namespace MPS.Processor.Mps000130.PDO
{
    public partial class Mps000130PDO : RDOBase
    {
        public HIS_EXP_MEST expMest { get; set; }
        public List<Mps000130ADO> listMrs000130ADO;

        public List<V_HIS_EXP_MEST_MEDICINE> _Medicines = null;
        public List<V_HIS_EXP_MEST_MATERIAL> _Materials = null;
        public List<V_HIS_EXP_MEST_BLOOD> _Bloods = null;
        public List<HIS_SUPPLIER> _Suppliers = null;
        public List<V_HIS_MEDI_STOCK> _MediStocks = null;
    }

    public class Mps000130ADO
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
        public decimal PRICE { get; set; }
        public string PRICE_SEPARATE { get; set; }
        public decimal TOTAL_PRICE { get; set; }
        public string TOTAL_PRICE_SEPARATE { get; set; }

        public Mps000130ADO() { }

        public Mps000130ADO(V_HIS_EXP_MEST_BLOOD data)
        {
            try
            {
                if (data != null)
                {
                    this.MEDI_MATE_TYPE_CODE = data.BLOOD_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.BLOOD_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = data.SUPPLIER_CODE;
                    this.SUPPLIER_NAME = data.SUPPLIER_NAME;
                    this.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                    //this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.BID_NUMBER = data.BID_NUMBER;
                    //this.AMOUNT = data.AMOUNT;
                    this.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                    this.VAT_RATIO_100 = data.IMP_VAT_RATIO * 100;
                    this.PRICE = data.IMP_PRICE;
                    this.TOTAL_PRICE = (1 + data.IMP_VAT_RATIO) * data.IMP_PRICE;
                    this.TOTAL_PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.TOTAL_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000130ADO(V_HIS_EXP_MEST_MEDICINE data)
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
                    this.PRICE = data.IMP_PRICE;
                    this.TOTAL_PRICE = (1 + data.IMP_VAT_RATIO) * data.AMOUNT * data.IMP_PRICE;
                    this.TOTAL_PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.TOTAL_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000130ADO(V_HIS_EXP_MEST_MATERIAL data)
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
                    this.PRICE = data.IMP_PRICE;
                    this.TOTAL_PRICE = (1 + data.IMP_VAT_RATIO) * data.AMOUNT * data.IMP_PRICE;
                    this.TOTAL_PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.TOTAL_PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    this.PRICE_SEPARATE = Inventec.Common.Number.Convert.NumberToString(this.PRICE, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

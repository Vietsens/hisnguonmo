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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.ADO
{
    public class MaterialInStockExportADO : MOS.EFMODEL.DataModels.V_HIS_MATERIAL_BEAN
    {
        public decimal? AVAILABLE_AMOUNT { get; set; }
        public string IS_CHEMICAL_SUBSTANCE_STR { get; set; }//la hoa chat
        public string EXPIRED_DATE_STR { get; set; }
        public string ALERT_EXPIRED_DATE_STR { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }//Đối tuowjgn thanh toán
        public decimal IMP_PRICE_VAT { get; set; } //gia nhap sau VAT
        public decimal EXP_PRICE { get; set; }
        public decimal EXP_VAT_RATIO { get; set; }
        public string PATIENT_TYPE_NAME_1 { get; set; }
        public string PATIENT_TYPE_NAME_2 { get; set; }
        public string PATIENT_TYPE_NAME_3 { get; set; }
        public string PATIENT_TYPE_NAME_4 { get; set; }
        public string PATIENT_TYPE_NAME_5 { get; set; }
        public string PATIENT_TYPE_NAME_6 { get; set; }
        public string PATIENT_TYPE_NAME_7 { get; set; }
        public string PATIENT_TYPE_NAME_8 { get; set; }
        public string PATIENT_TYPE_NAME_9 { get; set; }
        public string PATIENT_TYPE_NAME_10 { get; set; }
        public decimal EXP_PRICE_1 { get; set; }
        public decimal EXP_PRICE_2 { get; set; }
        public decimal EXP_PRICE_3 { get; set; }
        public decimal EXP_PRICE_4 { get; set; }
        public decimal EXP_PRICE_5 { get; set; }
        public decimal EXP_PRICE_6 { get; set; }
        public decimal EXP_PRICE_7 { get; set; }
        public decimal EXP_PRICE_8 { get; set; }
        public decimal EXP_PRICE_9 { get; set; }
        public decimal EXP_PRICE_10 { get; set; }

        public Dictionary<string, string> DicPatientTypeName { get; set; }
        public Dictionary<string, decimal> DicExpPrice { get; set; }
        public Dictionary<string, decimal> DicExpVatRatio { get; set; }
    }
}

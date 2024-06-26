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
using MPS.Processor.Mps000259.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000259
{
    public class Mps000259SDO : V_HIS_SERE_SERV_BILL
    {
        public long Type { get; set; }//1:dvkt, 2:thuoc

        public decimal? VIR_HEIN_PRICE { get; set; }
        public decimal? VIR_PATIENT_PRICE { get; set; }
        public decimal? VIR_PRICE { get; set; }

        public decimal? PRICE_FEE { get; set; }
        public decimal? PRICE_SERVICE { get; set; }
        public decimal? TOTAL_PRICE_SERVICE { get; set; }
        public decimal? TOTAL_PRICE_FEE { get; set; }

        public Mps000259SDO() { }

        public Mps000259SDO(V_HIS_SERE_SERV sereServ, Mps000259ADO Mps000259ADO)
        {
            this.TDL_ADD_PRICE = sereServ.ADD_PRICE;
            this.TDL_AMOUNT = sereServ.AMOUNT;
            this.AMOUNT = sereServ.AMOUNT;
            this.TDL_DISCOUNT = sereServ.DISCOUNT;
            this.TDL_EXECUTE_DEPARTMENT_ID = sereServ.TDL_EXECUTE_DEPARTMENT_ID;
            this.TDL_HEIN_LIMIT_PRICE = sereServ.HEIN_LIMIT_PRICE;
            this.TDL_HEIN_LIMIT_RATIO = sereServ.HEIN_LIMIT_RATIO;
            this.TDL_HEIN_NORMAL_PRICE = sereServ.HEIN_NORMAL_PRICE;
            this.TDL_HEIN_PRICE = sereServ.HEIN_PRICE;
            this.TDL_HEIN_RATIO = sereServ.HEIN_RATIO;
            this.TDL_HEIN_SERVICE_TYPE_ID = sereServ.TDL_HEIN_SERVICE_TYPE_ID;
            this.TDL_IS_OUT_PARENT_FEE = sereServ.IS_OUT_PARENT_FEE;
            this.TDL_LIMIT_PRICE = sereServ.LIMIT_PRICE;
            this.TDL_ORIGINAL_PRICE = sereServ.ORIGINAL_PRICE;
            this.TDL_OTHER_SOURCE_PRICE = sereServ.OTHER_SOURCE_PRICE;
            this.TDL_OVERTIME_PRICE = sereServ.OVERTIME_PRICE;
            this.TDL_PATIENT_TYPE_ID = sereServ.PATIENT_TYPE_ID;
            this.TDL_PRICE = sereServ.PRICE;
            this.TDL_PRIMARY_PRICE = sereServ.PRIMARY_PRICE;
            this.TDL_REQUEST_DEPARTMENT_ID = sereServ.TDL_REQUEST_DEPARTMENT_ID;
            this.TDL_SERE_SERV_PARENT_ID = sereServ.PARENT_ID;
            this.TDL_SERVICE_CODE = sereServ.TDL_SERVICE_CODE;
            this.TDL_SERVICE_ID = sereServ.SERVICE_ID;
            this.TDL_SERVICE_NAME = sereServ.TDL_SERVICE_NAME;
            this.TDL_SERVICE_TYPE_ID = sereServ.TDL_SERVICE_TYPE_ID;
            this.TDL_SERVICE_UNIT_ID = sereServ.TDL_SERVICE_UNIT_ID;
            this.TDL_TOTAL_HEIN_PRICE = sereServ.VIR_TOTAL_HEIN_PRICE;
            this.TDL_TOTAL_PATIENT_PRICE = sereServ.VIR_TOTAL_PATIENT_PRICE;
            this.TDL_TOTAL_PATIENT_PRICE_BHYT = sereServ.VIR_TOTAL_PATIENT_PRICE_BHYT;
            this.TDL_USER_PRICE = sereServ.USER_PRICE;
            this.TDL_VAT_RATIO = sereServ.VAT_RATIO;
            this.TDL_REAL_PATIENT_PRICE = sereServ.VIR_PATIENT_PRICE;
            this.TDL_REAL_HEIN_PRICE = sereServ.VIR_HEIN_PRICE;
            this.TDL_REAL_PRICE = sereServ.VIR_PRICE;
            this.SERVICE_UNIT_CODE = sereServ.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_NAME = sereServ.SERVICE_UNIT_NAME;
            this.SERVICE_TYPE_CODE = sereServ.SERVICE_TYPE_CODE;
            this.SERVICE_TYPE_NAME = sereServ.SERVICE_TYPE_NAME;
            this.MEDICINE_ID = sereServ.MEDICINE_ID;
            this.MATERIAL_ID = sereServ.MATERIAL_ID;
            this.BLOOD_ID = sereServ.BLOOD_ID;
            this.VIR_HEIN_PRICE = sereServ.VIR_HEIN_PRICE;
            this.VIR_PATIENT_PRICE = sereServ.VIR_PATIENT_PRICE;
            this.VIR_PRICE = sereServ.VIR_PRICE;
            this.PRICE = sereServ.PRICE;

            if (sereServ.HEIN_LIMIT_PRICE.HasValue && (sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH))
            {
                this.PRICE_FEE = sereServ.HEIN_LIMIT_PRICE;
            }
            else if (sereServ.LIMIT_PRICE.HasValue)
            {
                this.PRICE_FEE = sereServ.LIMIT_PRICE;
            }
            else if (sereServ.PATIENT_TYPE_ID == Mps000259ADO.PatientTypeBHYT
                || sereServ.PATIENT_TYPE_ID == Mps000259ADO.PatientTypeVP)
            {
                this.PRICE_FEE = sereServ.VIR_PRICE ?? 0;
            }
            else
            {
                this.PRICE_FEE = 0;
            }

            this.PRICE_SERVICE = (sereServ.VIR_PRICE ?? 0) - (this.PRICE_FEE ?? 0);

            //Y nghia la co chech lech thi tach ra
            if (this.PRICE_SERVICE == 0 && sereServ.HEIN_LIMIT_PRICE.HasValue && (sereServ.VIR_PRICE ?? 0) > sereServ.HEIN_LIMIT_PRICE)
            {
                //khi có chênh lệch thì phần chênh lệch chỉ dồn sang khi là dịch vụ khám hoặc giường.
                if (sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G || sereServ.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                {
                    this.PRICE_FEE = sereServ.HEIN_LIMIT_PRICE;
                    this.PRICE_SERVICE = (sereServ.VIR_PRICE ?? 0) - this.PRICE_FEE;
                }
            }

            this.TOTAL_PRICE_SERVICE = (this.PRICE_SERVICE ?? 0) * sereServ.AMOUNT;
            this.TOTAL_PRICE_FEE = (this.PRICE_FEE ?? 0) * sereServ.AMOUNT;
        }
    }
}

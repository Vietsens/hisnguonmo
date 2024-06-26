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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.SereServTree
{
    public class SereServADO : V_HIS_SERE_SERV_5
    {
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public decimal? AMOUNT_PLUS { get; set; }
        public string AMOUNT_DISPLAY { get; set; }
        //public string VIR_TOTAL_PRICE_DISPLAY { get; set; }// thành tiền
        //public string VIR_TOTAL_HEIN_PRICE_DISPLAY { get; set; }// đồng chi trả
        //public string VIR_TOTAL_PATIENT_PRICE_DISPLAY { get; set; }// bệnh nhân trả
        public decimal VAT { get; set; }
        public bool? IsLeaf { get; set; }
        public bool? IsExpend { get; set; }
        public bool? IsFather { get; set; }
        public SereServADO()
        {
        }

        public SereServADO(V_HIS_SERE_SERV_5 service)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
            this.AMOUNT_PLUS = service.AMOUNT;
            this.VAT = service.VAT_RATIO * 100;
            this.AMOUNT_DISPLAY = Inventec.Common.Number.Convert.NumberToString(service.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
            //this.VIR_TOTAL_PRICE_DISPLAY = Inventec.Common.Number.Convert.NumberToString(service.VIR_TOTAL_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
            //this.VIR_TOTAL_HEIN_PRICE_DISPLAY = Inventec.Common.Number.Convert.NumberToString(service.VIR_TOTAL_HEIN_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
            //this.VIR_TOTAL_PATIENT_PRICE_DISPLAY = Inventec.Common.Number.Convert.NumberToString(service.VIR_TOTAL_PATIENT_PRICE ?? 0, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);

            //this.CONCRETE_ID__IN_SETY = (service.SERVICE_TYPE_ID + "." + service.CONCRETE_ID);
            //this.PARENT_ID__IN_SETY = (service.SERVICE_TYPE_ID + "." + service.PARENT_ID);

        }

        public SereServADO(V_HIS_SERE_SERV_DEPOSIT service)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
            IsExpend = (service.TDL_IS_EXPEND == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
            this.PATIENT_TYPE_ID = service.TDL_PATIENT_TYPE_ID;
            this.PATIENT_TYPE_NAME = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == service.TDL_PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
            this.TDL_SERVICE_CODE = service.TDL_SERVICE_CODE;
            this.TDL_SERVICE_REQ_CODE = service.SERVICE_REQ_CODE;
            this.VIR_PRICE = service.TDL_VIR_PRICE;
            this.VIR_TOTAL_PRICE = service.TDL_VIR_TOTAL_PRICE;
            this.VIR_TOTAL_PATIENT_PRICE = service.TDL_VIR_TOTAL_PATIENT_PRICE;
            this.VIR_TOTAL_HEIN_PRICE = service.TDL_VIR_TOTAL_HEIN_PRICE;
            this.AMOUNT_PLUS = service.TDL_AMOUNT;
            this.AMOUNT_DISPLAY = Inventec.Common.Number.Convert.NumberToString(service.TDL_AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);

        }

        public SereServADO(V_HIS_SERE_SERV service, int patyId)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
            this.PARENT_ID__IN_SETY = patyId + "." + service.TDL_SERVICE_TYPE_ID;
            this.CONCRETE_ID__IN_SETY = patyId + "." + service.TDL_SERVICE_TYPE_ID + "." + service.SERVICE_ID;
        }
    }
}

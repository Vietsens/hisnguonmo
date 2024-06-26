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
using HIS.Desktop.Plugins.PharmacyCashier.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier.ADO
{
    public class SereServADO : V_HIS_SERE_SERV_5
    {
        public bool IsInvoiced { get; set; }
        public bool IsReciepted { get; set; }

        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public decimal? AMOUNT_PLUS { get; set; }
        public decimal VAT { get; set; }
        public bool? IsExpend { get; set; }
        public bool? IsLeaf { get; set; }
        public bool IsAdd { get; set; }
        public bool IsExists { get; set; }

        public SereServADO()
        {
        }

        public SereServADO(V_HIS_SERE_SERV_5 service)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, service);
            IsExpend = (service.IS_EXPEND == 1);
            this.AMOUNT_PLUS = service.AMOUNT;
            this.VAT = service.VAT_RATIO * 100;
            this.IsLeaf = true;

            V_HIS_MEDICINE_TYPE medicineType = null;
            if (this.MEDICINE_ID.HasValue)
            {
                medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == this.SERVICE_ID);
            }

            if (medicineType != null
                && medicineType.IS_VACCINE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
            {
                this.IsInvoiced = true;
            }
            else if (this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                || this.PATIENT_TYPE_ID == HisConfigCFG.PATIENT_TYPE_ID__IS_FEE)
            {
                this.IsReciepted = true;
            }
            else
            {
                this.IsInvoiced = true;
            }
        }

        public SereServADO(HIS.Desktop.LocalStorage.BackendData.ADO.ServiceADO service, HIS_PATIENT_TYPE patientType)
        {
            this.SERVICE_ID = service.ID;
            this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
            this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
            this.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
            this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
            this.TDL_ACTIVE_INGR_BHYT_CODE = service.ACTIVE_INGR_BHYT_CODE;
            this.TDL_ACTIVE_INGR_BHYT_NAME = service.ACTIVE_INGR_BHYT_NAME;
            this.TDL_BILL_OPTION = service.BILL_OPTION;
            this.TDL_HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
            this.TDL_HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
            this.TDL_SERVICE_CODE = service.SERVICE_CODE;
            this.TDL_SERVICE_NAME = service.SERVICE_NAME;
            this.TDL_SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
            this.TDL_SERVICE_UNIT_ID = service.SERVICE_UNIT_ID;
            this.TDL_SPECIALITY_CODE = service.SPECIALITY_CODE;
            this.PRICE = service.PRICE ?? 0;
            this.VAT_RATIO = service.VAT_RATIO ?? 0;
            this.VIR_PRICE = service.PRICE_VAT;
            this.IsAdd = true;

            this.PATIENT_TYPE_ID = patientType.ID;
            this.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;

            V_HIS_MEDICINE_TYPE medicineType = null;
            if (this.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
            {
                medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == this.SERVICE_ID);
            }

            if (medicineType != null
                && medicineType.IS_VACCINE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
            {
                this.IsInvoiced = true;
            }
            else if (this.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                || this.PATIENT_TYPE_ID == HisConfigCFG.PATIENT_TYPE_ID__IS_FEE)
            {
                this.IsReciepted = true;
            }
            else
            {
                this.IsInvoiced = true;
            }
        }
    }
}

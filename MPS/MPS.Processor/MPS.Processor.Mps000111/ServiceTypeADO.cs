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

namespace MPS.Processor.Mps000111
{
    class ServiceTypeADO
    {
        public long TYPE { get; set; }

        public long SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }

        public string SERVICE_UNIT_NAME { get; set; }

        public decimal? AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? TOTAL_PRICE { get; set; }
        public decimal? VIR_TOTAL_PATIENT_PRICE_BHYT { get; set; }
        public decimal? VIR_TOTAL_PATIENT_PRICE { get; set; }
        public decimal? VIR_TOTAL_HEIN_PRICE { get; set; }
        public decimal? TOTAL_OTHER_SOURCE_PRICE { get; set; }
        public decimal? OTHER_SOURCE_PRICE { get; set; }
        public decimal? VAT_RATIO { get; set; }
        public ServiceTypeADO() { }

        public ServiceTypeADO(HIS_SERE_SERV data)
        {
            if (data != null)
            {
                this.SERVICE_TYPE_ID = data.TDL_SERVICE_TYPE_ID;
                this.AMOUNT = data.AMOUNT;
                this.PRICE = data.VIR_PRICE;
                this.TOTAL_PRICE = data.VIR_TOTAL_PATIENT_PRICE;
                this.OTHER_SOURCE_PRICE = data.OTHER_SOURCE_PRICE;
                this.VAT_RATIO = data.VAT_RATIO;
                this.VIR_TOTAL_PATIENT_PRICE = data.VIR_TOTAL_PATIENT_PRICE;
                this.VIR_TOTAL_HEIN_PRICE = data.VIR_TOTAL_HEIN_PRICE;
                this.VIR_TOTAL_PATIENT_PRICE_BHYT = data.VIR_TOTAL_PATIENT_PRICE_BHYT;
                // nhóm cùng chi trả
                if ((data.VIR_TOTAL_HEIN_PRICE ?? 0) > 0 && (data.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0) > 0)
                {
                    this.TYPE = 1;
                }

                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == data.TDL_SERVICE_TYPE_ID);
                if (serviceType != null)
                {
                    this.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                    this.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                    this.SERVICE_NUM_ORDER = serviceType.NUM_ORDER;
                }
            }
        }

        public ServiceTypeADO(ServiceTypeADO data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceTypeADO>(this, data);
            }
        }
    }
}

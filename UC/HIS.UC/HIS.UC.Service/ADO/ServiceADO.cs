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

namespace HIS.UC.Service
{
    public class ServiceADO : MOS.EFMODEL.DataModels.V_HIS_SERVICE
    {
        public ServiceADO() { }
        public ServiceADO(V_HIS_SERVICE data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceADO>(this, data);
            }
        }
        public ServiceADO(V_HIS_SERVICE data,
            long isChooseService,
            List<long> serviceIdInServiceMetys,
            long serviceIdCheckByService,
            List<long> serviceIdTemps
            )
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceADO>(this, data);
                if (isChooseService == 1)
                {
                    this.isKeyChooseService = true;
                }
                if (serviceIdInServiceMetys != null && serviceIdInServiceMetys.Count > 0 && serviceIdInServiceMetys.Contains(this.ID))
                {
                    this.checkService = true;
                }
                if (serviceIdCheckByService != 0 && isChooseService == 1)
                {
                    this.radioService = true;
                }
                if (serviceIdTemps != null && serviceIdTemps.Count > 1)
                    this.checkService = (serviceIdTemps.Contains(this.ID));
            }
        }
        public bool checkService { get; set; }
        public bool checkServiceNotUse { get; set; }
        public bool checkWarning { get; set; }
        public bool isKeyChooseService { get; set; }
        public bool radioService { get; set; }
        public long ROOM_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public long? ACTIVE_INGREDIENT_ID { get; set; }
        public string ACTIVE_INGREDIENT_CODE { get; set; }
        public string ACTIVE_INGREDIENT_NAME { get; set; }
        public bool ABOUT_REQUEST { get; set; }
        public bool ABOUT_EXECUTE { get; set; }
        public decimal? PRICE { get; set; }
        public decimal? VAT_RATIO { get; set; }
        public decimal? PRICE_KSK { get; set; }
        public decimal? VAT_RATIO_KSK { get; set; }
        public long? MIN_DURATION_STR { get; set; }

    }
}

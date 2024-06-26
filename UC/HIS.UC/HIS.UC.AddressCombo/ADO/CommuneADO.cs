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

namespace HIS.UC.AddressCombo.ADO
{
    public class CommuneADO : SDA.EFMODEL.DataModels.V_SDA_COMMUNE
    {
        public string PROVINCE_CODE { get; set; }
        public long PROVINCE_ID { get; set; }
        public string PROVINCE_NAME { get; set; }
        public string SEARCH_CODE_COMMUNE { get; set; }

        public CommuneADO()
        {

        }

        public CommuneADO(SDA.EFMODEL.DataModels.V_SDA_COMMUNE data, List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> districts, List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> provinces)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<CommuneADO>(this, data);

                var districtOne = districts.FirstOrDefault(o => o.ID == data.DISTRICT_ID);
                if (districtOne != null)
                {
                    this.PROVINCE_ID = districtOne.PROVINCE_ID;
                    this.PROVINCE_CODE = districtOne.PROVINCE_CODE;
                    this.PROVINCE_NAME = districtOne.PROVINCE_NAME;

                    var provinceOne = provinces.FirstOrDefault(o => o.ID == districtOne.PROVINCE_ID);
                    if (provinceOne != null)
                    {
                        this.SEARCH_CODE_COMMUNE += provinceOne.SEARCH_CODE;
                    }
                    this.SEARCH_CODE_COMMUNE += districtOne.SEARCH_CODE;
                }
                this.SEARCH_CODE_COMMUNE += data.SEARCH_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

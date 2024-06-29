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
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ConfigApp
{
    public class ConfigAppADO : SDA.EFMODEL.DataModels.SDA_CONFIG_APP
    {
        public ConfigAppADO() { }
        public ConfigAppADO(SDA_CONFIG_APP data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ConfigAppADO>(this, data);
            }
        }
        public ConfigAppADO(SDA_CONFIG_APP data,
            long isChooseConfigApp,
            List<long> ConfigAppIdInConfigAppMetys,
            long ConfigAppIdCheckByConfigApp,
            List<long> ConfigAppIdTemps
            )
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ConfigAppADO>(this, data);
                if (isChooseConfigApp == 1)
                {
                    this.isKeyChooseConfigApp = true;
                }
                if (ConfigAppIdInConfigAppMetys != null && ConfigAppIdInConfigAppMetys.Count > 0 && ConfigAppIdInConfigAppMetys.Contains(this.ID))
                {
                    this.checkConfigApp = true;
                }
                if (ConfigAppIdCheckByConfigApp != 0 && isChooseConfigApp == 1)
                {
                    this.radioConfigApp = true;
                }
                if (ConfigAppIdTemps != null && ConfigAppIdTemps.Count > 1)
                    this.checkConfigApp = (ConfigAppIdTemps.Contains(this.ID));
            }
        }
        public bool checkConfigApp { get; set; }
        public bool isKeyChooseConfigApp { get; set; }
        public bool radioConfigApp { get; set; }
        public string VALUE_STR { get; set; }
    }
}

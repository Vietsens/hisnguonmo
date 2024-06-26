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

namespace HIS.Desktop.Plugins.InfantInformation.ADO
{
    public class DistrictADO : V_SDA_DISTRICT
    {
        public string RENDERER_DISTRICT_NAME { get; set; }
        public DistrictADO(V_SDA_DISTRICT item)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<DistrictADO>(this, item);
                RENDERER_DISTRICT_NAME = string.Format("{0} {1}", INITIAL_NAME, DISTRICT_NAME);
            }
            catch (Exception  ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

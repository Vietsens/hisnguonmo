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

namespace HIS.Desktop.Plugins.AllergyCardCreate
{
    public class AllergenicADO : HIS_ALLERGENIC
    {
        public bool NghiNgo { get; set; }
        public bool ChacChan { get; set; }

        public AllergenicADO() { }

        public AllergenicADO(HIS_ALLERGENIC data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<AllergenicADO>(this, data);
                if (data.IS_DOUBT == 1)
                    this.NghiNgo = true;
                if (data.IS_SURE == 1)
                    this.ChacChan = true;
            }
        }
    }
}

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

namespace HIS.Desktop.Plugins.HistoryMaterial
{
    class MaterialADO : V_HIS_MATERIAL_TYPE
    {
        public string MATERIAL_TYPE_NAME_UNSIGN { get; set; }

        public MaterialADO(V_HIS_MATERIAL_TYPE data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<MaterialADO>(this, data);
                this.MATERIAL_TYPE_NAME_UNSIGN = Inventec.Common.String.Convert.UnSignVNese(data.MATERIAL_TYPE_NAME);
            }
        }
    }
}

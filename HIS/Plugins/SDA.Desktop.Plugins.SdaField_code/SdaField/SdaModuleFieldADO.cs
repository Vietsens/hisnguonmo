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

namespace SDA.Desktop.Plugins.SdaField_code.SdaField
{
    class SdaModuleFieldADO : SDA_MODULE_FIELD
    {
        public bool IS_VISIBLE_STR { get; set; }

        public SdaModuleFieldADO(SDA_MODULE_FIELD data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SdaModuleFieldADO>(this, data);
            if (data.IS_VISIBLE == 1)
                this.IS_VISIBLE_STR = true;
            else
                this.IS_VISIBLE_STR = false;

        }
    }
}

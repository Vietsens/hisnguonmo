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

namespace HIS.Desktop.Plugins.HisAdr
{
    public class MedicineTypeADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public string PACKAGE_NUMBER { get; set; }

        public MedicineTypeADO() { }

        public MedicineTypeADO(V_HIS_MEDICINE_TYPE dataType, List<HIS_MEDICINE> _medicines)
        {
            try
            {
                if (dataType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeADO>(this, dataType);
                    if (_medicines != null && _medicines.Count > 0)
                    {
                        var data = _medicines.FirstOrDefault(p => p.MEDICINE_TYPE_ID == dataType.ID);
                        this.PACKAGE_NUMBER = data != null ? data.PACKAGE_NUMBER : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

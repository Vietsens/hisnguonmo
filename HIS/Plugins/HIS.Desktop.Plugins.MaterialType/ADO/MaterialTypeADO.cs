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

namespace HIS.Desktop.Plugins.MaterialType.ADO
{
    public class MaterialTypeADO : V_HIS_MATERIAL_TYPE
    {
        public bool IsChemicalSubstance { get; set; }
        public bool IsStopImp { get; set; }

        public MaterialTypeADO(V_HIS_MATERIAL_TYPE _data)
        {
            try
            {
                if (_data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeADO>(this, _data);

                    this.IsChemicalSubstance = (_data.IS_CHEMICAL_SUBSTANCE == IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_CHEMICAL_SUBSTANCE);
                    this.IsStopImp = (_data.IS_STOP_IMP == IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_STOP_IMP__TRUE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

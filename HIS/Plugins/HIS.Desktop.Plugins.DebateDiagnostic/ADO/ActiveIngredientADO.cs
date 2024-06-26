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
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DebateDiagnostic.ADO
{
    internal class ActiveIngredientADO : MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT
    {
        public bool IsChecked { get; set; }
        public string ACTIVE_INGREDIENT_NAME__UNSIGN { get; set; }

        public ActiveIngredientADO(MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT item)
        {
            Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT, ActiveIngredientADO>();
            Mapper.Map<MOS.EFMODEL.DataModels.HIS_ACTIVE_INGREDIENT, ActiveIngredientADO>(item, this);
            this.ACTIVE_INGREDIENT_NAME__UNSIGN = Inventec.Common.String.Convert.UnSignVNese(this.ACTIVE_INGREDIENT_NAME);
        }
    }
}

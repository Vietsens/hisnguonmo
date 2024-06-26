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

namespace HIS.Desktop.Plugins.HisMedicineTypeAcin
{
    public class ActiveIngredientADO : HIS_ACTIVE_INGREDIENT
    {
        public ActiveIngredientADO() { }
        public ActiveIngredientADO(HIS_ACTIVE_INGREDIENT data)
        {
            if (data != null)
            {
                this.ID = data.ID;
                this.ACTIVE_INGREDIENT_CODE = data.ACTIVE_INGREDIENT_CODE;
                this.ACTIVE_INGREDIENT_NAME = data.ACTIVE_INGREDIENT_NAME;
                this.APP_CREATOR = data.APP_CREATOR;
                this.APP_MODIFIER = data.APP_MODIFIER;
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.MODIFIER = data.MODIFIER;
                this.MODIFY_TIME = data.MODIFY_TIME;
            }
        }

        public bool check2 { get; set; }
    }
}

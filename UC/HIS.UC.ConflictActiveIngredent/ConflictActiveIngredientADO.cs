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

namespace HIS.UC.ConflictActiveIngredient
{
    public class ConflictActiveIngredientADO : HIS_ACTIVE_INGREDIENT
    {
        public ConflictActiveIngredientADO() { }
        public ConflictActiveIngredientADO(HIS_ACTIVE_INGREDIENT data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ConflictActiveIngredientADO>(this, data);
            }
        }

        public bool check2 { get; set; }
        public bool isKeyChoose1 { get; set; }
        public bool radio2 { get; set; }
        public string DESCRIPTION { get; set; }
        public string INSTRUCTION { get; set; }
        public long? INTERACTIVE_GRADE_ID { get; set; }
        public string CONSEQUENCE { get; set; }
        public string MECHANISM { get; set; }

        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeConsequence { get; set; }
        public string ErrorMessageConsequence { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeMechanism { get; set; }
        public string ErrorMessageMechanism { get; set; }

    }
}
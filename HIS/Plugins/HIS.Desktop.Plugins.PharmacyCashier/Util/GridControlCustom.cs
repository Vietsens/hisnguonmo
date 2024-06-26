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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier.Util
{
    public class GridControlCustom : CustomGridViewWithFilterMultiColumn
    {
        public GridControlCustom() : base() { }

        protected override string OnCreateLookupDisplayFilter(string text, string displayMember)
        {
            List<CriteriaOperator> subStringOperators = new List<CriteriaOperator>();
            string sString = text.Trim();
            string exp = LikeData.CreateContainsPattern(sString);
            List<CriteriaOperator> columnsOperators = new List<CriteriaOperator>();
            foreach (GridColumn col in Columns)
            {
                if (col.ColumnType == typeof(string))
                    columnsOperators.Add(new BinaryOperator(col.FieldName, exp, BinaryOperatorType.Like));
            }
            subStringOperators.Add(new GroupOperator(GroupOperatorType.Or, columnsOperators));
            return new GroupOperator(GroupOperatorType.And, subStringOperators).ToString();
        }
    }
}

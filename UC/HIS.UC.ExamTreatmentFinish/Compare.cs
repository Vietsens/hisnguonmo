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
using HIS.UC.ExamTreatmentFinish.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ExamTreatmentFinish
{
    public class Compare : IEqualityComparer<IcdADO>
    {
        public bool Equals(IcdADO x, IcdADO y)
        {
            return
                x.ICD_CODE == y.ICD_CODE &&
                x.ICD_NAME == y.ICD_NAME &&
                x.IS_ACTIVE == y.IS_ACTIVE;
        }

        public int GetHashCode(IcdADO x)
        {
            return (!string.IsNullOrEmpty(x.ICD_CODE) ? x.ICD_CODE.GetHashCode() : 0) +
                (!string.IsNullOrEmpty(x.ICD_NAME) ? x.ICD_NAME.GetHashCode() : 0) +
                (x.IS_ACTIVE != null ? x.IS_ACTIVE.GetHashCode() : 0);
        }
    }
}

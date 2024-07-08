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

using His.UC.UCHein.Base;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein
{
    public class Compare : IEqualityComparer<PatientTypeAlterADO>
    {
        public bool Equals(PatientTypeAlterADO x, PatientTypeAlterADO y)
        {
            return
                x.HEIN_CARD_NUMBER == y.HEIN_CARD_NUMBER &&
                x.HEIN_CARD_FROM_TIME == y.HEIN_CARD_FROM_TIME &&
                x.HEIN_CARD_TO_TIME == y.HEIN_CARD_TO_TIME;
        }

        public int GetHashCode(PatientTypeAlterADO x)
        {
            return (!string.IsNullOrEmpty(x.HEIN_CARD_NUMBER) ? x.HEIN_CARD_NUMBER.GetHashCode() : 0) +
                (x.HEIN_CARD_FROM_TIME != null ? x.HEIN_CARD_FROM_TIME.GetHashCode() : 0) +
                (x.HEIN_CARD_TO_TIME != null ? x.HEIN_CARD_TO_TIME.GetHashCode() : 0);
        }
    }
}

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.Data
{
    /// <summary>
    /// The three form values derived from a BHXH co-payment lookup, plus the context
    /// needed to explain them to the user.
    /// </summary>
    public class CoPaidMcctADO
    {
        public CoPaidMcctADO() { }

        /// <summary>
        /// True when the gateway returned at least one record and the accumulated amount
        /// could be derived. False means: leave every form value untouched.
        /// </summary>
        public bool HasAccumulate { get; set; }

        /// <summary>
        /// True when HIS_BHYT_PARAM supplied a usable base salary, so the six-months
        /// threshold could be evaluated. False means: fill the amount only.
        /// </summary>
        public bool HasThreshold { get; set; }

        /// <summary>Accumulated co-payment of the financial year, already rounded.</summary>
        public long CoPaidAccumulateAmount { get; set; }

        /// <summary>Accumulated amount is strictly greater than six months of base salary.</summary>
        public bool IsPaid6Month { get; set; }

        /// <summary>
        /// Exemption start date as yyyyMMdd - the discharge date of the first episode that
        /// pushed the accumulated amount past the threshold. Null when it could not be derived.
        /// </summary>
        public long? FreeCoPaidTime { get; set; }

        /// <summary>
        /// The threshold was passed but no episode carried a usable discharge date, so the
        /// exemption date stays unknown and the user has to enter it from the BHXH certificate.
        /// </summary>
        public bool IsMissingFreeCoPaidTime { get; set; }

        /// <summary>
        /// Total amount eligible for exemption across the returned episodes.
        /// Shown for reference only - HIS has no column for it.
        /// </summary>
        public decimal TotalMcctAmount { get; set; }

        /// <summary>Source and cut-off timestamp of the gateway data, shown to the user.</summary>
        public string DataNote { get; set; }
    }
}

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
using MOS.SDO;
using System;

namespace HIS.Desktop.Plugins.FundAllocation.ADO
{
    /// <summary>
    /// One cost line shown in both grids: the insurer side (FUND_*) and the patient side (PATIENT_*)
    /// </summary>
    public class FundAllocationLineADO : HisFundAllocationLineSDO
    {
        /// <summary>
        /// 0: unchanged, 1: new line, 2: changed line, 3: line no longer a cost of the treatment
        /// </summary>
        internal const short CHANGE_TYPE__NEW = 1;
        internal const short CHANGE_TYPE__CHANGED = 2;
        internal const short CHANGE_TYPE__REMOVED = 3;

        public string INTRUCTION_TIME_DISPLAY { get; set; }
        public string NOTE { get; set; }

        public FundAllocationLineADO()
        {
        }

        public FundAllocationLineADO(HisFundAllocationLineSDO data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<FundAllocationLineADO>(this, data);
                this.INTRUCTION_TIME_DISPLAY = data.TDL_INTRUCTION_TIME > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool IsRemoved
        {
            get { return this.ChangeType == CHANGE_TYPE__REMOVED; }
        }

        public bool IsNewOrChanged
        {
            get { return this.ChangeType == CHANGE_TYPE__NEW || this.ChangeType == CHANGE_TYPE__CHANGED; }
        }

        /// <summary>
        /// Line can only be moved as a whole: quantity 1 (or less) or a negative amount (package balancing line)
        /// </summary>
        public bool IsWholeOnly
        {
            get { return this.CurrentAmount <= 1 || this.CurrentTotalPrice < 0; }
        }
    }
}

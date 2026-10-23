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

namespace HIS.Desktop.Plugins.FundAllocation.ADO
{
    /// <summary>
    /// One allocation version on the history tab
    /// </summary>
    public class FundAllocationHistoryADO : V_HIS_FUND_ALLOCATION
    {
        public string STATUS_DISPLAY { get; set; }
        public string FINISH_TIME_DISPLAY { get; set; }
        public string CANCEL_TIME_DISPLAY { get; set; }
        public string TRANSACTION_CODES { get; set; }

        public FundAllocationHistoryADO()
        {
        }

        public FundAllocationHistoryADO(V_HIS_FUND_ALLOCATION data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<FundAllocationHistoryADO>(this, data);
                this.FINISH_TIME_DISPLAY = data.FINISH_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME.Value) : "";
                this.CANCEL_TIME_DISPLAY = data.CANCEL_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CANCEL_TIME.Value) : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

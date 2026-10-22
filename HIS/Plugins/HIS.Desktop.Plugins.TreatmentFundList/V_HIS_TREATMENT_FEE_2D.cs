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

namespace HIS.Desktop.Plugins.TreatmentFundList
{
    class V_HIS_TREATMENT_FEE_2D : V_HIS_TREATMENT_FEE_2
    {
        public bool Check { get; set; }

        #region Fund allocation (current version from V_HIS_FUND_ALLOCATION)
        /// <summary>Allocation status text (not allocated / saved / finalized [+ costs changed])</summary>
        public string FUND_ALLOCATION_STATUS { get; set; }

        /// <summary>Finalized by (FINISH_USERNAME)</summary>
        public string FUND_ALLOCATION_FINISH_USERNAME { get; set; }

        /// <summary>Finalized time (FINISH_TIME) formatted dd/MM/yyyy HH:mm:ss</summary>
        public string FUND_ALLOCATION_FINISH_TIME_STR { get; set; }

        /// <summary>Allocated fund amount (TOTAL_FUND_PRICE)</summary>
        public decimal? FUND_ALLOCATION_TOTAL_FUND_PRICE { get; set; }

        /// <summary>True when costs of the treatment changed after the allocation was saved (IS_COST_CHANGED = 1)</summary>
        public bool IS_FUND_ALLOCATION_COST_CHANGED { get; set; }
        #endregion
    }
}

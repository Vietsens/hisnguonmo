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
using His.Bhyt.InsuranceExpertise.LDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.CheckHeinGOV
{
    /// <summary>
    /// Outcome of a co-payment lookup on the BHXH gateway.
    ///
    /// Carries the gateway payload unchanged. Deriving the three form values
    /// (accumulated amount, six-months flag, exemption date) is done by the caller,
    /// which already holds the cached HIS_BHYT_PARAM needed for the threshold.
    /// </summary>
    public class ResultMCCTADO
    {
        public ResultMCCTADO() { }

        /// <summary>Gateway result code: 200, 204, 400 or 500. Empty when the call never happened.</summary>
        public string MaKetQua { get; set; }

        /// <summary>
        /// On success, the source and cut-off timestamp of the gateway data - show it to the user
        /// so the staleness of the figures is visible. On failure, the gateway error text.
        /// </summary>
        public string GhiChu { get; set; }

        /// <summary>Co-payment records as returned, sorted descending by ngayRa. May be null.</summary>
        public List<DataCCTLDO> DataCCT { get; set; }

        /// <summary>Card information of the person looked up. May be null.</summary>
        public ThongTinSoTheCCTLDO ThongTinSoThe { get; set; }

        /// <summary>Raw response payload, kept for tracing.</summary>
        public string JsonBHYT { get; set; }

        /// <summary>
        /// Set when the request was rejected locally, before reaching the gateway
        /// (invalid card number length, missing name or date of birth, missing configuration).
        /// </summary>
        public bool IsBlockedLocally { get; set; }

        /// <summary>The lookup succeeded and brought back at least one record.</summary>
        public bool HasData
        {
            get
            {
                return MaKetQua == ResultMCCTLDO.MaKetQuaStore.SUCCESS
                    && DataCCT != null
                    && DataCCT.Count > 0;
            }
        }

        /// <summary>
        /// The gateway answered but holds no co-payment record for this card.
        /// The caller must leave the current form values untouched - this is not a zero.
        /// </summary>
        public bool IsNoData
        {
            get { return MaKetQua == ResultMCCTLDO.MaKetQuaStore.NO_DATA; }
        }
    }
}

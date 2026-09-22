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
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult
{
    /// <summary>
    /// Request body for POST api/LabResult/RequestView (asks the gateway to send an OTP).
    /// </summary>
    public class RequestViewRequestADO
    {
        /// <summary>Patient identification number — required</summary>
        [JsonProperty("SoDinhDanhBenhNhan")]
        public string SoDinhDanhBenhNhan { get; set; }

        /// <summary>Filter by a single test index code (optional)</summary>
        [JsonProperty("MaDungChung")]
        public string MaDungChung { get; set; }

        /// <summary>Ask OTP for a whole batch by lab request code (optional)</summary>
        [JsonProperty("MaPhieuXN")]
        public string MaPhieuXN { get; set; }

        /// <summary>Looking-up facility code (optional, defaults to the token owner)</summary>
        [JsonProperty("MaCSKCB")]
        public string MaCSKCB { get; set; }

        /// <summary>Looking-up doctor code (optional)</summary>
        [JsonProperty("MaBacSi")]
        public string MaBacSi { get; set; }

        /// <summary>Reason for accessing the data (optional)</summary>
        [JsonProperty("LyDoTruyCap")]
        public string LyDoTruyCap { get; set; }
    }
}

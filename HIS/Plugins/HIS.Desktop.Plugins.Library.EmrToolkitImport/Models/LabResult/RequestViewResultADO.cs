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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult
{
    /// <summary>
    /// Data field of the POST api/LabResult/RequestView response.
    /// </summary>
    public class RequestViewResultADO
    {
        /// <summary>Verification session id — required by the download step</summary>
        [JsonProperty("TransactionId")]
        public string TransactionId { get; set; }

        /// <summary>Channels the OTP was sent through (VNeID, Email, SMS...)</summary>
        [JsonProperty("KenhGuiOTP")]
        public List<string> KenhGuiOTP { get; set; }

        /// <summary>OTP expiry time</summary>
        [JsonProperty("ThoiHanOTP")]
        public string ThoiHanOTP { get; set; }
    }
}

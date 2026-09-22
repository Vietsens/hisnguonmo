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
    /// One still-valid lab result record on the gateway
    /// (one element of Data in GET api/LabResult/CheckValidity).
    /// DuLieu/KeyGiaiMa may be present in the response but are never used on the HIS side.
    /// </summary>
    public class LabResultValidityADO
    {
        /// <summary>Record id on the gateway</summary>
        [JsonProperty("_id")]
        public string Id { get; set; }

        /// <summary>Health facility code that submitted the result</summary>
        [JsonProperty("MaCSKCB")]
        public string MaCSKCB { get; set; }

        /// <summary>Patient identification number</summary>
        [JsonProperty("SoDinhDanhBenhNhan")]
        public string SoDinhDanhBenhNhan { get; set; }

        /// <summary>Technical service code</summary>
        [JsonProperty("MaDichVu")]
        public string MaDichVu { get; set; }

        /// <summary>Common catalog codes of the test indices in this record</summary>
        [JsonProperty("DanhSachMaDungChung")]
        public List<string> DanhSachMaDungChung { get; set; }

        /// <summary>Test group (HuyetHoc, HoaSinh...)</summary>
        [JsonProperty("LoaiXetNghiem")]
        public string LoaiXetNghiem { get; set; }

        /// <summary>Lab request code</summary>
        [JsonProperty("MaPhieuXN")]
        public string MaPhieuXN { get; set; }

        /// <summary>Print form type</summary>
        [JsonProperty("LoaiPhieu")]
        public string LoaiPhieu { get; set; }

        /// <summary>Expiry time of the shared result</summary>
        [JsonProperty("ValidUntil")]
        public string ValidUntil { get; set; }

        /// <summary>Whether internal quality control passed</summary>
        [JsonProperty("QCDatChuan")]
        public bool? QCDatChuan { get; set; }

        /// <summary>Creation time on the gateway</summary>
        [JsonProperty("NgayTao")]
        public string NgayTao { get; set; }
    }
}

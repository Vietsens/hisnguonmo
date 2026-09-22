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

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult
{
    /// <summary>
    /// Outcome of the download step: local pdf files extracted from the gateway response.
    /// The gateway returns a single pdf, or a zip when the batch spans several print forms.
    /// </summary>
    public class LabResultPdfResultADO
    {
        public LabResultPdfResultADO()
        {
            this.PdfFilePaths = new List<string>();
        }

        /// <summary>Whether at least one pdf file was obtained</summary>
        public bool Success { get; set; }

        /// <summary>Failure reason, empty when successful</summary>
        public string Message { get; set; }

        /// <summary>Local paths of the downloaded pdf files, in gateway order</summary>
        public List<string> PdfFilePaths { get; set; }

        /// <summary>Folder holding the downloaded files</summary>
        public string OutputFolder { get; set; }
    }
}

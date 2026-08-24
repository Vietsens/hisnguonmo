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

namespace HIS.Desktop.Plugins.TreatmentInspectionV2.ADO
{
    /// <summary>
    /// One row read from the import Excel file.
    /// TREATMENT_CODE is the only column filled by the user; every other member is resolved
    /// from the treatment record so the operator can check the batch before saving.
    /// </summary>
    public class RecordInspectionImportADO
    {
        /// <summary>Row number inside the file, 1-based.</summary>
        public long STT { get; set; }

        /// <summary>Treatment code typed in the Excel file, normalised to 12 characters.</summary>
        public string TREATMENT_CODE { get; set; }

        /// <summary>
        /// Optional note typed by the appraiser, shown while checking the batch.
        /// Displayed only — table HIS_RECORD_INSPECTION_IMP carries no note column, so the value
        /// is not persisted. The column also satisfies Inventec.Common.ExcelImport, which needs at
        /// least two tagged columns in the template (Import.CheckIndex).
        /// </summary>
        public string NOTE { get; set; }

        /// <summary>Resolved treatment id; 0 when the code was not found.</summary>
        public long TREATMENT_ID { get; set; }

        public string PATIENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string IN_TIME_STR { get; set; }
        public string OUT_TIME_STR { get; set; }
        public string END_DEPARTMENT_NAME { get; set; }
        public string ICD { get; set; }

        /// <summary>All validation messages of the row, one per line. Empty when the row is valid.</summary>
        public string ERROR { get; set; }

        /// <summary>
        /// Non blocking notice, one per line — currently "this treatment was already received on
        /// {date}". The row still saves: the appraiser sending a treatment again for a new round is
        /// normal business, the operator only needs to see it.
        /// </summary>
        public string WARNING { get; set; }

        /// <summary>Display status of the row: valid / invalid / saved.</summary>
        public string STATUS { get; set; }

        public bool HasError
        {
            get { return !string.IsNullOrWhiteSpace(this.ERROR); }
        }

        public bool HasWarning
        {
            get { return !string.IsNullOrWhiteSpace(this.WARNING); }
        }
    }
}

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

namespace HIS.Desktop.Plugins.TreatmentInspectionV2.ADO
{
    /// <summary>
    /// One record of the inspection list received from the health insurance appraiser:
    /// "this treatment was requested for appraisal, received at this moment".
    /// Mirrors table HIS_RECORD_INSPECTION_IMP.
    ///
    /// TEMPORARY BRIDGE: MOS.EFMODEL is shipped as a pre-built assembly, so the generated
    /// entity is not available on the desktop side yet. This class carries the same member
    /// names, therefore the JSON payload matches the backend entity.
    ///
    /// TODO: delete this class and use MOS.EFMODEL.DataModels.HIS_RECORD_INSPECTION_IMP
    /// once the backend release generates it.
    /// </summary>
    public class RecordInspectionImpADO
    {
        public long ID { get; set; }
        public Nullable<long> CREATE_TIME { get; set; }
        public Nullable<long> MODIFY_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public Nullable<short> IS_ACTIVE { get; set; }
        public Nullable<short> IS_DELETE { get; set; }
        public string GROUP_CODE { get; set; }

        /// <summary>Treatment requested for appraisal.</summary>
        public long TREATMENT_ID { get; set; }

        /// <summary>Moment the record was imported, format yyyyMMddHHmmss.</summary>
        public long IMPORT_TIME { get; set; }

        /// <summary>Source Excel file name, kept to tell two batches of the same day apart.</summary>
        public string FILE_NAME { get; set; }
    }

    /// <summary>
    /// Search filter for the inspection list records.
    ///
    /// TEMPORARY BRIDGE — see <see cref="RecordInspectionImpADO"/>.
    /// TODO: replace with MOS.Filter.HisRecordInspectionImpFilter once released.
    /// </summary>
    public class RecordInspectionImpFilter : MOS.Filter.FilterBase
    {
        public List<long> TREATMENT_IDs { get; set; }
        public long? IMPORT_TIME_FROM { get; set; }
        public long? IMPORT_TIME_TO { get; set; }

        public RecordInspectionImpFilter()
            : base()
        {
        }
    }
}

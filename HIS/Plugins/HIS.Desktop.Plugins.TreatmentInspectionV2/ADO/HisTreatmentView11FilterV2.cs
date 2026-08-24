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

namespace HIS.Desktop.Plugins.TreatmentInspectionV2.ADO
{
    /// <summary>
    /// Search filter of the treatment inspection list, extended with the parameters
    /// required by the "Giam dinh bao hiem y te v2" module.
    ///
    /// TEMPORARY BRIDGE: MOS.Filter is shipped as a pre-built assembly, so these three
    /// parameters cannot be declared in MOS.Filter.HisTreatmentView11Filter from the
    /// desktop side. They are serialized together with the inherited members, therefore
    /// the backend binds them as soon as the same parameters are added to
    /// MOS.Filter.HisTreatmentView11Filter. Until then the backend ignores them and the
    /// list behaves exactly like the existing inspection screen.
    ///
    /// TODO: delete this class and use MOS.Filter.HisTreatmentView11Filter directly once
    /// the backend release adds HAS_RECORD_INSPECTION_IMP, IMPORT_TIME_FROM and
    /// IMPORT_TIME_TO.
    /// </summary>
    public class HisTreatmentView11FilterV2 : MOS.Filter.HisTreatmentView11Filter
    {
        /// <summary>
        /// true  = only treatments that belong to an imported inspection list.
        /// false = only treatments that have never been imported.
        /// null  = no restriction (behaviour of the existing inspection screen).
        /// </summary>
        public bool? HAS_RECORD_INSPECTION_IMP { get; set; }

        /// <summary>Import time lower bound, format yyyyMMddHHmmss.</summary>
        public long? IMPORT_TIME_FROM { get; set; }

        /// <summary>Import time upper bound, format yyyyMMddHHmmss.</summary>
        public long? IMPORT_TIME_TO { get; set; }

        /// <summary>Treatment codes looked up in batch while validating an import file.</summary>
        public List<string> TREATMENT_CODEs { get; set; }

        public HisTreatmentView11FilterV2()
            : base()
        {
        }
    }
}

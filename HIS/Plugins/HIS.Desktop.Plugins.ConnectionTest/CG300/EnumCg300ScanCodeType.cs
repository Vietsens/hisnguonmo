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

namespace HIS.Desktop.Plugins.ConnectionTest.CG300
{
    /// <summary>
    /// Kind of code scanned into the CG300 scan box, resolved by probing LIS_SAMPLE
    /// in the order Barcode -> Service request code -> Treatment code -> Patient code.
    /// </summary>
    public enum EnumCg300ScanCodeType
    {
        /// <summary>Not resolved: search by keyword (KEY_WORD) like the normal search box</summary>
        None = 0,

        /// <summary>Sample barcode (LIS_SAMPLE.BARCODE)</summary>
        Barcode = 1,

        /// <summary>Service request code, padded to 12 digits</summary>
        ServiceReqCode = 2,

        /// <summary>Treatment code, padded to 12 digits</summary>
        TreatmentCode = 3,

        /// <summary>Patient code, padded to 10 digits</summary>
        PatientCode = 4
    }
}

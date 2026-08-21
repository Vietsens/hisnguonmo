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
namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking
{
    /// <summary>
    /// Document status of a single order on the medical record review screen (task 53180 - rule QT-09).
    /// Not available in IMSys.DbConfig: this status is computed on the client from the
    /// V_EMR_DOCUMENT rows linked to the order through HIS_CODE.
    /// </summary>
    public enum EnumRecordDocumentStatus
    {
        /// <summary>The order has not produced any document yet.</summary>
        NoDocument = 0,

        /// <summary>Documents exist but none of them has been signed.</summary>
        NotSigned = 1,

        /// <summary>Partially signed - at least one signer is still pending.</summary>
        Signing = 2,

        /// <summary>Every document of the order is fully signed (QT-10).</summary>
        FullySigned = 3
    }
}

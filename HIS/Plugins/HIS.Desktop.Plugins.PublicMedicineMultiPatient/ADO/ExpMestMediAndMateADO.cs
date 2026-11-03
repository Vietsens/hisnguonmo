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
namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO
{
    /// <summary>
    /// One disclosure row (medicine, material, chemical or blood) of a treatment.
    /// Material and blood rows reuse MEDICINE_TYPE_ID/MEDICINE_TYPE_NAME to hold their type, told apart by Service_Type_Id
    /// (same convention as HIS.Desktop.Plugins.PublicMedicineByPhased).
    /// </summary>
    public class ExpMestMediAndMateADO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        public long Service_Type_Id { get; set; }
        public short? IS_CHEMICAL_SUBSTANCE { set; get; }
        public long INTRUCTION_DATE { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public long TYPE { get; set; }

        /// <summary>
        /// Treatment of the prescription the row belongs to (HIS_SERVICE_REQ.TREATMENT_ID)
        /// </summary>
        public long TreatmentId { get; set; }
    }

    /// <summary>
    /// Kind of a disclosure row, stored in ExpMestMediAndMateADO.TYPE
    /// </summary>
    public enum DisclosureRowType : long
    {
        /// <summary>Medicine</summary>
        Medicine = 1,
        /// <summary>Material</summary>
        Material = 2,
        /// <summary>Chemical (material with IS_CHEMICAL_SUBSTANCE = 1)</summary>
        Chemical = 3,
        /// <summary>Blood</summary>
        Blood = 4
    }
}

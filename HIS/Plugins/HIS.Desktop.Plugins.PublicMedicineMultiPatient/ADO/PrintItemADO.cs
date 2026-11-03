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
using MPS.Processor.Mps000522.PDO;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO
{
    /// <summary>
    /// One page (day page) of the disclosure form of one patient, ready to print
    /// </summary>
    public class PrintItemADO
    {
        public long TreatmentId { get; set; }
        public string TreatmentCode { get; set; }
        public string PatientName { get; set; }
        public Mps000522PDO Pdo { get; set; }
    }
}

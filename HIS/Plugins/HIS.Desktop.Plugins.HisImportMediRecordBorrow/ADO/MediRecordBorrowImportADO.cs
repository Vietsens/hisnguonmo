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
namespace HIS.Desktop.Plugins.HisImportMediRecordBorrow.ADO
{
    public class MediRecordBorrowImportADO
    {
        // Excel input columns
        public string MEDI_RECORD_STORE_CODE { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string BORROW_LOGINNAME { get; set; }
        public string APPOINTMENT_TIME_STR { get; set; }

        // Resolved values (filled after lookup)
        public long? MEDI_RECORD_ID { get; set; }
        public long? DEPARTMENT_ID { get; set; }
        public string BORROW_USERNAME { get; set; }
        public long? APPOINTMENT_TIME { get; set; }

        // Display-only enrichment fields (from lookup)
        public string TREATMENT_CODE { get; set; }
        public string PATIENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string DATA_STORE_NAME { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string BORROW_PHONE { get; set; }

        // UI fields
        public long STT { get; set; }
        public string ERROR { get; set; }
    }
}

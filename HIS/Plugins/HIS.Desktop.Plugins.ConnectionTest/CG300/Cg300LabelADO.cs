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
    /// One tube record sent to FOYEN CG300 (vendor model "FyLabelResponseModel", Method 2 - file exchange).
    /// Property names MUST match the vendor spec exactly; every value is a string.
    /// </summary>
    public class Cg300LabelADO
    {
        /// <summary>Sample barcode</summary>
        public string LabNum { get; set; }
        /// <summary>Patient code</summary>
        public string HospNum { get; set; }
        public string PatientType { get; set; }
        /// <summary>Patient name</summary>
        public string CnName { get; set; }
        public string Sex { get; set; }
        public string AgeDesc { get; set; }
        /// <summary>Requesting room</summary>
        public string Location { get; set; }
        /// <summary>Instruction time, yyyy-MM-ddTHH:mm:ss</summary>
        public string OrderDate { get; set; }
        /// <summary>Urgent / Normal</summary>
        public string Priority { get; set; }
        /// <summary>Specimen source</summary>
        public string Source { get; set; }
        /// <summary>Tube code = HIS_TEST_SAMPLE_TYPE.TEST_SAMPLE_TYPE_CODE</summary>
        public string ContainerCode { get; set; }
        /// <summary>Tube color</summary>
        public string Container { get; set; }
        /// <summary>Yes / No</summary>
        public string RePrint { get; set; }
        public string TestItemDesc { get; set; }
        public string DeptDesc { get; set; }
        /// <summary>Service request code</summary>
        public string ExamineRequestID { get; set; }
        public string PaymentStatus { get; set; }
        /// <summary>yyyy-MM-ddTHH:mm:ss</summary>
        public string PrintTime { get; set; }
        public string PrintIP { get; set; }
        public string TestType { get; set; }
        public string TestItemType { get; set; }
        public string PrintUser { get; set; }
        public string TakeReportDesc { get; set; }
        public string TakeReportAddress { get; set; }
        public string BedNum { get; set; }
        /// <summary>0 = unconfirmed, 1 = confirmed</summary>
        public string BloodConfirmState { get; set; }
    }
}

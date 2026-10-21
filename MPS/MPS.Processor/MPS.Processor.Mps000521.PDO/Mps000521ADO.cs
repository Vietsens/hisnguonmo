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
namespace MPS.Processor.Mps000521.PDO
{
    /// <summary>
    /// One staff card. Template keys: &lt;#LOGINNAME;&gt;, &lt;#USERNAME;&gt;, &lt;#EMPLOYEE_CODE;&gt;, &lt;#DEPARTMENT_NAME;&gt;,
    /// barcode image named &lt;#FlFuncCreateBarcode(&lt;#BARCODE_TEXT;&gt;;500;100)&gt;.
    /// </summary>
    public class Mps000521ADO
    {
        public string LOGINNAME { get; set; }
        public string USERNAME { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        /// <summary>Text encoded in the barcode, read by the scanner connection screen ("TK-" + LOGINNAME).</summary>
        public string BARCODE_TEXT { get; set; }
    }
}

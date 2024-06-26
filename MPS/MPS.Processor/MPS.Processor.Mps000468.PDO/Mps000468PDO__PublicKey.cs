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
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000468.PDO
{
    public partial class Mps000468PDO
    {
        public HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public List<V_LIS_RESULT> lstLisResult { get; set; }
        public List<V_HIS_TEST_INDEX> lstTestIndex { get; set; }
        public V_LIS_SAMPLE currentSample { get; set; }//K được ghi đè //sx theo thứ tự
        public HIS_SERVICE_REQ currentServiceReq { get; set; }//K được ghi đè
        public HIS_TREATMENT currentTreatment { get; set; }  //K được ghi đè
        public List<V_HIS_TEST_INDEX_RANGE> testIndexRangeAll { get; set; }
        public V_HIS_SERVICE ServiceParent { get; set; }
        public long genderId { get; set; }
        public List<V_HIS_SERVICE> ListTestService { get; set; }
        public HIS_PATIENT currentPatient { get; set; }
    }
    public class PrintTypeCode
    {
        public const string Mps000468 = "Mps000468";
    }
}

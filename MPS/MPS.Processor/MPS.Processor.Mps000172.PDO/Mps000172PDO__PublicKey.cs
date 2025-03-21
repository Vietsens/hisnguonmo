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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000172.PDO
{
    public partial class Mps000172PDO : RDOBase
    {
        public V_HIS_TRANSACTION _Transaction = null;
        public V_HIS_PATIENT_TYPE_ALTER _BatyAlterBhyt = null;
        public V_HIS_DEPARTMENT_TRAN _DepartmentTran = null;
        public HIS_PATIENT _Patient = null;
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTranAllByTreatments = null;
        public V_HIS_TREATMENT treatment = null;
        public List<HIS_TREATMENT_TYPE> _TreatmentType = null;

    }

    public class PrintTypeCode
    {
        public const string Mps000172 = "Mps000172";
    }
}

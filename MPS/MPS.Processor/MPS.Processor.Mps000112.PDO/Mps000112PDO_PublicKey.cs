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

namespace MPS.Processor.Mps000112.PDO
{
	public partial class Mps000112PDO : RDOBase
	{
		public V_HIS_TRANSACTION _Transaction = null;
		public V_HIS_PATIENT _Patient = null;
		public V_HIS_PATIENT_TYPE_ALTER _PatyalterBHYT = null;
		public List<V_HIS_DEPARTMENT_TRAN> _DepartmentTranAllByTreatment = null;
		public Mps000112ADO MpsADO = null;
		public V_HIS_TREATMENT treatment = null;
		public List<HIS_TREATMENT_TYPE> _TreatmentType = null;
	}

	public class Mps000112ADO
	{
		public long DEPOSIT_NUM_ORDER { get; set; }

		public string IN_TREATMENT_TYPE_NAME { get; set; }

		public string PATIENT_CLASSIFY_NAME { get; set; }

        public string DEPOSIT_SERVICE_NUM_ORDER { get; set; }

	}
}

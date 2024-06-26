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
		public const string printTypeCode = "Mps000112";

		public decimal ratio = 0;

		public Mps000112PDO() { }

		public Mps000112PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient)
		{
			this._Transaction = transaction;
			this._Patient = patient;
		}

		public Mps000112PDO(V_HIS_PATIENT patient, decimal ratio)
		{
			this._Patient = patient;
			this.ratio = ratio;
		}

		public Mps000112PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ratio, V_HIS_PATIENT_TYPE_ALTER PatyAlterBHYT)
		{
			this._Transaction = transaction;
			this._Patient = patient;
			this.ratio = ratio;
			this._PatyalterBHYT = PatyAlterBHYT;
		}

		public Mps000112PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ratio, V_HIS_PATIENT_TYPE_ALTER PatyAlterBHYT, List<V_HIS_DEPARTMENT_TRAN> departmentTrans)
		{
			this._Transaction = transaction;
			this._Patient = patient;
			this.ratio = ratio;
			this._PatyalterBHYT = PatyAlterBHYT;
			this._DepartmentTranAllByTreatment = departmentTrans;
		}

		public Mps000112PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ratio, V_HIS_PATIENT_TYPE_ALTER PatyAlterBHYT, List<V_HIS_DEPARTMENT_TRAN> departmentTrans, Mps000112ADO ado)
		{
			this._Transaction = transaction;
			this._Patient = patient;
			this.ratio = ratio;
			this._PatyalterBHYT = PatyAlterBHYT;
			this._DepartmentTranAllByTreatment = departmentTrans;
			this.MpsADO = ado;
		}

		public Mps000112PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ratio, V_HIS_PATIENT_TYPE_ALTER PatyAlterBHYT, List<V_HIS_DEPARTMENT_TRAN> departmentTrans, Mps000112ADO ado, V_HIS_TREATMENT treatment, List<HIS_TREATMENT_TYPE> lst)
		{
			this._Transaction = transaction;
			this._Patient = patient;
			this.ratio = ratio;
			this._PatyalterBHYT = PatyAlterBHYT;
			this._DepartmentTranAllByTreatment = departmentTrans;
			this.MpsADO = ado;
			this.treatment = treatment;
			this._TreatmentType = lst;
		}


	}
}

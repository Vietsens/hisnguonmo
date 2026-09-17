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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.ADO
{
	public class AntibioticRequestADO
	{
		public V_HIS_ANTIBIOTIC_REQUEST AntibioticRequest { get; set; }
		public string PatientCode { get; set; }
		public string PatientName { get; set; }
		public long Dob { get; set; }
		public bool IsHasNotDayDob { get; set; }
		public string GenderName { get; set; }
		public decimal? Temperature { get; set; }
		public decimal? Weight { get; set; }
		public decimal? Height { get; set; }
		public string IcdSubCode { get; set; }
		public string IcdText { get; set; }
		public long ExpMestId { get; set; }
		public long InstructionDate { get; set; }
		public List<HIS_ANTIBIOTIC_NEW_REG> NewRegimen { get; set; }

		public ProcessType? processType { get; set; }
		public enum ProcessType
		{
			Request,
			Approval
		}

	}
}

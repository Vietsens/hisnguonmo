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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.Processor.Mps000066.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000066.PDO
{
	public partial class Mps000066PDO : RDOBase
	{
		public Mps000066PDO(
						PatientADO patient,
						PatyAlterBhytADO PatyAlterBhyt,
						DepartmentTranADO departmentTran,
						List<HIS_DEBATE_USER> lstHisDebateUser,
						List<HIS_DEBATE_USER> lstHisDebateUserTGia,
						HIS_DEBATE HisDebateRow,
						string bedRoomName
						)
		{
			try
			{
				this.Patient = patient;
				this.PatyAlterBhyt = PatyAlterBhyt;
				this.departmentTran = departmentTran;
				this.lstHisDebateUser = lstHisDebateUser;
				this.lstHisDebateUserTGia = lstHisDebateUserTGia;
				this.HisDebateRow = HisDebateRow;
				this.bedRoomName = bedRoomName;
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}

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

namespace HIS.UC.Hospitalize.ADO
{
	class DepartmentADO : MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_1
	{
		public string NAME_STR { get; set; }
		public DepartmentADO() { }
		public DepartmentADO(MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_1 data)
		{
			try
			{
				if (data != null)
				{
					Inventec.Common.Mapper.DataObjectMapper.Map<DepartmentADO>(this, data);
					if (this.PATIENT_COUNT != null && this.PATIENT_COUNT > 0)
					{
						NAME_STR = this.DEPARTMENT_NAME + " (" + (long)(this.PATIENT_COUNT) + ")";
					}
					else
					{
						NAME_STR = this.DEPARTMENT_NAME;
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}
	}
}

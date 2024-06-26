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
namespace HIS.Desktop.Plugins.AntibioticRequest.ADO
{
	public class AntibioticNewRegADO : V_HIS_ANTIBIOTIC_NEW_REG
	{
		public int Action { get; set; }

		public bool ANTIBIOTIC_NEW_ADD { get; set; }
		public bool ANTIBIOTIC_DELETE { get; set; }
		public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeConcentra { get; set; }
		public string ErrorMessageConcentra { get; set; }
		public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeDosage { get; set; }
		public string ErrorMessageDosage { get; set; }
		public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypePeriod { get; set; }
		public string ErrorMessagePeriod { get; set; }
		public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeUseForm { get; set; }
		public string ErrorMessageUseForm { get; set; }
		public long? START_DATE { get; set; }
		public long? END_DATE { get; set; }
	}
}

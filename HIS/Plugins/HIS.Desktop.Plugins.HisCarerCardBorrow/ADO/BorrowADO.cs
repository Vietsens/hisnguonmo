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

namespace HIS.Desktop.Plugins.HisCarerCardBorrow.ADO
{
	public class BorrowADO
	{
		public string LOGINNAME { get; set; }
		public string CARER_CARD_NUMBER {get;set;}
		public long CARER_CARD_ID { get; set; }
		public long TIME_FROM { get; set; }
		public string SERVICE_NAME { get; set; }
		public long ID_ROW { get; set; }
		public V_HIS_CARER_CARD HIS_CARER_CARD { get; set; }
	}
}

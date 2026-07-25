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

namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
	internal class RequestUriStore
	{
		internal const string RETURN_AVAILABLE_MATERIAL = "api/HisMaterial/ReturnAvailable";

		internal const string RETURN_AVAILABLE_MEDICINE = "api/HisMedicine/ReturnAvailable";

		// vCong 52461 - Tao du tru v2: API tong hop so lieu du tru theo LOAI (gop toan vien).
		internal const string HIS_MEDI_STOCK_METY_GET_FOR_ANTICIPATE = "api/HisMediStockMety/GetForAnticipate";
		internal const string HIS_MEDI_STOCK_MATY_GET_FOR_ANTICIPATE = "api/HisMediStockMaty/GetForAnticipate";

		// Luu du tru (dung lai API cu cua man Tao du tru).
		internal const string HIS_ANTICIPATE_CREATE = "api/HisAnticipate/Create";
		internal const string HIS_ANTICIPATE_UPDATE = "api/HisAnticipate/Update";
	}
}

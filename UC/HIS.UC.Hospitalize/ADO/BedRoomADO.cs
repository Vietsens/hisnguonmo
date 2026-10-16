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
	/// <summary>
	/// Dong du lieu cua o chon Buong benh: them cot Tong BN/Tong giuong
	/// ghep tu PATIENT_COUNT va BED_COUNT cua V_HIS_BED_ROOM (api/HisBedRoom/GetView).
	/// </summary>
	class BedRoomADO : MOS.EFMODEL.DataModels.V_HIS_BED_ROOM
	{
		public string TT_PATIENT_BED_STR { get; set; }

		/// <summary>
		/// PATIENT_COUNT > BED_COUNT (rong tinh la 0): dong hien chu do, nguoc lai chu xanh duong.
		/// </summary>
		public bool IsOverBedCount { get; set; }

		public BedRoomADO() { }
		public BedRoomADO(MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data)
		{
			try
			{
				if (data != null)
				{
					Inventec.Common.Mapper.DataObjectMapper.Map<BedRoomADO>(this, data);
					this.TT_PATIENT_BED_STR = BuildPatientBedText(this.PATIENT_COUNT, this.BED_COUNT);
					this.IsOverBedCount = (this.PATIENT_COUNT ?? 0) > (this.BED_COUNT ?? 0);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		/// <summary>
		/// "so benh nhan dang nam/so giuong", gia tri rong tinh la 0 (buong chua khai giuong hien "0/0").
		/// </summary>
		internal static string BuildPatientBedText(decimal? patientCount, decimal? bedCount)
		{
			long patients = (long)(patientCount ?? 0);
			long beds = (long)(bedCount ?? 0);
			return patients.ToString() + "/" + beds.ToString();
		}
	}
}

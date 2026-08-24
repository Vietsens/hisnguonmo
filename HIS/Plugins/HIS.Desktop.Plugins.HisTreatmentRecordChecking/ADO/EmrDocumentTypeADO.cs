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
using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO
{
	internal class EmrDocumentTypeADO : EMR_DOCUMENT_TYPE
	{
		public bool IsHasDocument { get; set; }
		public bool IsHasDocumentNoPatientSign { get; set; }

		/// <summary>
		/// Loai van ban co du lieu de hien o luoi giua: co VAN BAN, hoac co Y LENH thuoc loai nay.
		/// Dung cho checkbox "Uu tien" (day dong co du lieu len dau).
		/// KHONG dung IsHasDocument cho viec nay: 9 loai trong ListTypeId hien y lenh chu khong hien
		/// van ban, nen y lenh chua tao van ban van phai duoc coi la co du lieu.
		/// IsHasDocument giu nguyen y nghia "co van ban" vi con dung de to mau luoi trai.
		/// </summary>
		public bool IsHasData { get; set; }
		public EmrDocumentTypeADO(EMR_DOCUMENT_TYPE data)
		{
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<EMR_DOCUMENT_TYPE>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

		public EmrDocumentTypeADO()
		{
		}
	}
}

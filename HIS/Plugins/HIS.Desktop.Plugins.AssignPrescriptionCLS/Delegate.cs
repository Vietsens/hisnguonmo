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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    public delegate void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time);
    public delegate bool ValidAddRow(object data);
    //public delegate MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeBySeTy(long patientTypeId, MediMatyTypeADO medimaty);
    public delegate long? CalulateUseTimeTo();
    public delegate bool ExistsAssianInDay(long serviceId);
    public delegate void ChonThuocTrongKhoCungHoatChat(HIS.Desktop.Plugins.AssignPrescriptionCLS.OptionChonThuocThayThe chonThuocThayThe);
    public delegate void ChonVTTrongKho(HIS.Desktop.Plugins.AssignPrescriptionCLS.EnumOptionChonVatTuThayThe chonVTThayThe);
}

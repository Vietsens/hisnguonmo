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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    public delegate void DelegateSelectMultiDate(List<DateTime?> datas, DateTime time);
    public delegate bool ValidAddRow(object data);
    public delegate long? CalulateUseTimeTo();
    public delegate bool ExistsAssianInDay(MediMatyTypeADO mediMatyType);
    public delegate void ChonThuocTrongKhoCungHoatChat(HIS.Desktop.Plugins.AssignPrescriptionPK.OptionChonThuocThayThe chonThuocThayThe);
    public delegate void ChonVTTrongKho(HIS.Desktop.Plugins.AssignPrescriptionPK.EnumOptionChonVatTuThayThe chonVTThayThe);
    public delegate void LyDoKeThuocTuongTac(string InteractionReason);
    public delegate void TuongTacKhongBoSung();
    public delegate void LyDoKeThuocQuaSoLuong(string ReasonMaxPrescription);

}

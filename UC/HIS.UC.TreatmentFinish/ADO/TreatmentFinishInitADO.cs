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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish.ADO
{
    public class TreatmentFinishInitADO
    {
        public LanguageInputADO LanguageInputADO { get; set; }
        public DataInputADO DataInputADO { get; set; }
        public HisTreatmentWithPatientTypeInfoSDO Treatment { get; set; }
        public bool IsTreatmentIn { get; set; }
        public bool? IsAutoWidth { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsValidate { get; set; }
        public bool IsCheckFinishTime { get; set; }
        public bool IsCheckBedService { get; set; }
        public List<long> AutoFinishServiceIds { get; set; }
        public bool MustFinishAllServicesBeforeFinishTreatment { get; set; }
        public AutoTreatmentFinish__Checked AutoTreatmentFinish__Checked { get; set; }
        public DelegateNextFocus DelegateNextFocus { get; set; }
        public DelegateGetDateADO DelegateGetDateADO { get; set; }
        public CheckedTreatmentFinish DelegateCheckedTreatmentFinish { get; set; }
        public DelegateGetStoreStateValue DelegateGetStoreStateValue { get; set; }
        public DelegateSetStoreStateValue DelegateSetStoreStateValue { get; set; }
        public Action<bool> DelegateTreatmentFinishCheckChange { get; set; }
        public long TreatmentEndAppointmentTimeDefault { get; set; }
        public bool TreatmentEndHasAppointmentTimeDefault { get; set; }
        public long? treatmentId { get; set; }
        public long? WorkingRoomId { get; set; }
        public long? WorkingDepartmentId { get; set; }
        public bool? NotAutoInitData { get; set; }
        public bool? UseCapSoBABNCT { get; set; }
        public bool IsBlockOrder { get; set; }
        public bool IsShowButtonIcd { get; set; }
    }
}

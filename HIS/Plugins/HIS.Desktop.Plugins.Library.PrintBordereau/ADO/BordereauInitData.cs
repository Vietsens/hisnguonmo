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
using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.ADO
{
    public class BordereauInitData
    {
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        public List<V_HIS_TREATMENT_FEE> TreatmentFees { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> SereServDeposits { get; set; }
        public List<HIS_SESE_DEPO_REPAY> SeseDepoRepays { get; set; }
        public string UserNameReturnResult { get; set; }
        public long? CurrentDepartmentId { get; set; }
        public long RoomId { get; set; }
        public long RoomTypeId { get; set; }
    }
}

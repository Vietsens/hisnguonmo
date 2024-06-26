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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Base
{
    public abstract class MpsDataBase
    {
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        public List<V_HIS_TREATMENT_FEE> TreatmentFees { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> SereServDeposits { get; set; }
        public List<HIS_SESE_DEPO_REPAY> SeseDepoRepays { get; set; }
        public List<V_HIS_TRANSACTION> Transactions { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatientTypeAlter { get; set; }
        public List<HIS_CONFIG> lstConfig { get; set; }
        public HIS_TRANS_REQ transReq { get; set; }
        public HIS_TRANS_REQ transReq2 { get; set; }
        public long TotalDayTreatment { get; set; }
        public string StatusTreatmentOut { get; set; }
        public string DepartmentName { get; set; }
        public string RoomName { get; set; }
        public string UserNameReturnResult { get; set; }
        public long? CurrentDepartmentId { get; set; }
        public long? RoomId { get; set; }
        public bool isPreview { get; set; }
        public PrintOption.PayType? PayOption;
        public bool IsActionButtonPrintBill { get; set; }
        public List<HIS_SERE_SERV_BILL> SereServBills { get; set; }
        public List<HIS_TRANSACTION> ListTransaction { get; set; }
        public MpsDataBase() { }

        public MpsDataBase(long? roomId, V_HIS_TREATMENT treatment)
        {
            this.RoomId = roomId;
            this.Treatment = treatment;

            bool isCheckTreatment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKey.OnlyAllowPrintingIfTreatmentIsFinishedCFG) == 1 ? true : false;
            if (isCheckTreatment)
            {
                this.isPreview = true;
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && treatment != null && treatment.IS_PAUSE == 1 && treatment.END_DEPARTMENT_ID == room.DEPARTMENT_ID)
                {
                    this.isPreview = false;
                }
            }
        }

    }
}

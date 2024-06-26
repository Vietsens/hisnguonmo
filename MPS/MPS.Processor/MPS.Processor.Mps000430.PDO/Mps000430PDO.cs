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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000430.PDO
{
    public partial class Mps000430PDO : RDOBase
    {

        public const string printTypeCode = "Mps000430";

        public V_HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> _SereServDeposit { get; set; }
        public List<HIS_SESE_DEPO_REPAY> _SeseDepoRepay { get; set; }
        public List<HIS_SERE_SERV_BILL> _SereServBill { get; set; }
        public List<V_HIS_TRANSACTION> _Transaction { get; set; }
        public List<HIS_SERE_SERV> _SereServ { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> ListHeinServiceType { get; set; }
        public List<V_HIS_ROOM> ListRoom { get; set; }

        public Mps000430PDO(
            V_HIS_TREATMENT Treatment,
            V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter,
            List<HIS_SERE_SERV_DEPOSIT> SereServDeposit,
            List<HIS_SESE_DEPO_REPAY> SeseDepoRepay,
            List<HIS_SERE_SERV_BILL> SereServBill,
            List<V_HIS_TRANSACTION> Transaction
            )
        {
            try
            {
                this._Treatment = Treatment;
                this._PatientTypeAlter = PatientTypeAlter;
                this._SereServDeposit = SereServDeposit;
                this._SeseDepoRepay = SeseDepoRepay;
                this._SereServBill = SereServBill;
                this._Transaction = Transaction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000430PDO(
            V_HIS_TREATMENT Treatment,
            V_HIS_PATIENT_TYPE_ALTER PatientTypeAlter,
            List<HIS_SERE_SERV_DEPOSIT> SereServDeposit,
            List<HIS_SESE_DEPO_REPAY> SeseDepoRepay,
            List<HIS_SERE_SERV_BILL> SereServBill,
            List<V_HIS_TRANSACTION> Transaction,
            List<HIS_SERE_SERV> SereServ,
            List<HIS_HEIN_SERVICE_TYPE> _HeinServiceType,
            List<V_HIS_ROOM> _Rooms
            )
        {
            try
            {
                this._Treatment = Treatment;
                this._PatientTypeAlter = PatientTypeAlter;
                this._SereServDeposit = SereServDeposit;
                this._SeseDepoRepay = SeseDepoRepay;
                this._SereServBill = SereServBill;
                this._Transaction = Transaction;
                this._SereServ = SereServ;
                this.ListHeinServiceType = _HeinServiceType;
                this.ListRoom = _Rooms;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

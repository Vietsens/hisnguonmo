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
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000432.PDO
{
    public class Mps000432PDO : RDOBase
    {
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }

        public Mps000432PDO(
            HIS_TREATMENT currentHisTreatment,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            List<V_HIS_SERVICE_REQ> ServiceReqPrints,
            List<V_HIS_SERE_SERV> sereServs,
            Mps000432ADO mps000432ADO,
            List<V_HIS_SERVICE> _listService,
            List<HIS_SERVICE_CONDITION> _listServiceCondition,
            List<HisServiceReqMaxNumOrderSDO> reqMaxNumOrderSDO,
            V_HIS_BED_LOG bedLog,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
              List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
              List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction
            )
        {
            try
            {
                this.ServiceReqPrints = ServiceReqPrints;
                this.SereServs = sereServs;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.HisTreatment = currentHisTreatment;
                this.Mps000432ADO = mps000432ADO;
                this.ListService = _listService;
                this.ListServiceCondition = _listServiceCondition;
                this.ReqMaxNumOrderSDO = reqMaxNumOrderSDO;
                this.BedLog = bedLog;
                this.Dhst = _dhst;
                this.WorkPlace = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000432PDO(
            HIS_TREATMENT currentHisTreatment,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            List<V_HIS_SERVICE_REQ> ServiceReqPrints,
            List<V_HIS_SERE_SERV> sereServs,
            Mps000432ADO mps000432ADO,
            List<V_HIS_SERVICE> _listService,
            List<HIS_SERVICE_CONDITION> _listServiceCondition,
            List<HisServiceReqMaxNumOrderSDO> reqMaxNumOrderSDO,
            V_HIS_BED_LOG bedLog,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE
            )
        {
            try
            {
                this.ServiceReqPrints = ServiceReqPrints;
                this.SereServs = sereServs;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.HisTreatment = currentHisTreatment;
                this.Mps000432ADO = mps000432ADO;
                this.ListService = _listService;
                this.ListServiceCondition = _listServiceCondition;
                this.ReqMaxNumOrderSDO = reqMaxNumOrderSDO;
                this.BedLog = bedLog;
                this.Dhst = _dhst;
                this.WorkPlace = _wORK_PLACE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<V_HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public HIS_TREATMENT HisTreatment { get; set; }
        public Mps000432ADO Mps000432ADO { get; set; }
        public List<V_HIS_SERVICE> ListService { get; set; }
        public List<V_HIS_SERVICE_REQ> ServiceReqPrints { get; set; }
        public List<HIS_SERVICE_CONDITION> ListServiceCondition { get; set; }
        public List<HisServiceReqMaxNumOrderSDO> ReqMaxNumOrderSDO { get; set; }
        public V_HIS_BED_LOG BedLog { get; set; }
        public HIS_DHST Dhst { get; set; }
        public HIS_WORK_PLACE WorkPlace { get; set; }
    }

    public class Mps000432ADO
    {
        public string bebRoomName { get; set; }
        public string firstExamRoomName { get; set; }
        public decimal ratio { get; set; }
        public string PARENT_NAME { get; set; }
        public long PatientTypeId__Bhyt { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }
}

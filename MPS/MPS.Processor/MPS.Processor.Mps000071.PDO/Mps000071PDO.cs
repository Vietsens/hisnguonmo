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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000071.PDO
{
    public partial class Mps000071PDO : RDOBase
    {
        public decimal ratio { get; set; }
        public string firstExamRoomName { get; set; }
        public V_HIS_SERVICE_REQ serviceReqPrevios { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListConfigs { get; set; }

        public HIS_DHST HisDhst { get; set; }

        public HIS_TREATMENT hisTreatment { get; set; }

        public V_HIS_TREATMENT_BED_ROOM vTreatmentBedRoom { get; set; }
        public Mps000071PDO() { }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio
                )
        {
            try
            {
                this.patientADO = patientADO;
                //this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction
                )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            HIS_DHST _HisDhst
                )
        {
            try
            {
                this.patientADO = patientADO;
                //this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
                this.HisDhst = _HisDhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction,
            HIS_DHST _HisDhst
                )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }

                this.HisDhst = _HisDhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
                )
        {
            try
            {
                this.patientADO = patientADO;
                //this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
                )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            HIS_DHST _HisDhst,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
                )
        {
            try
            {
                this.patientADO = patientADO;
                //this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
                this.HisDhst = _HisDhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            List<ExeSereServSdo> sereServExamServiceReqs,
            V_HIS_SERE_SERV sereServExamSerivceReq,
            V_HIS_SERVICE_REQ vHisServiceReq,
            V_HIS_SERVICE_REQ serviceReqPrevios,
            string firstExamRoomName,
            decimal ratio,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction,
            HIS_DHST _HisDhst,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
                )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }

                this.HisDhst = _HisDhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000071PDO(
           PatientADO patientADO,
           V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
           List<ExeSereServSdo> sereServExamServiceReqs,
           V_HIS_SERE_SERV sereServExamSerivceReq,
           V_HIS_SERVICE_REQ vHisServiceReq,
           V_HIS_SERVICE_REQ serviceReqPrevios,
           string firstExamRoomName,
           decimal ratio,
           List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
           List<HIS_SERE_SERV_BILL> _listSereServBill,
             List<V_HIS_TRANSACTION> _listTransaction,
           HIS_DHST _HisDhst,
           HIS_TRANS_REQ transReq,
           List<HIS_CONFIG> listConfigs,
           V_HIS_TREATMENT_BED_ROOM vTreatmentBedRoom,
           HIS_TREATMENT hisTreatment
               )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.sereServExamSerivceReq = sereServExamSerivceReq;
                this.vHisServiceReq = vHisServiceReq;
                this.ratio = ratio;
                this.firstExamRoomName = firstExamRoomName;
                this.serviceReqPrevios = serviceReqPrevios;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServExamServiceReqs != null && sereServExamServiceReqs.Count > 0)
                {
                    this.sereServExamServiceReqs = sereServExamServiceReqs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
                else
                {
                    this.sereServExamServiceReqs = new List<ExeSereServSdo>();
                }
                this.hisTreatment = hisTreatment;
                this.vTreatmentBedRoom = vTreatmentBedRoom;
                this.HisDhst = _HisDhst;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

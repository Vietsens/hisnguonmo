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

namespace MPS.Processor.Mps000040.PDO
{
    public partial class Mps000040PDO : RDOBase
    {
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListConfigs { get; set; }

        public Mps000040PDO() { }

        public Mps000040PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000040_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000040ADO mps000040ADO,
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
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000040ADO = mps000040ADO;
                this.BedLog = bedLog;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000040PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000040_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000040ADO mps000040ADO,
            V_HIS_BED_LOG bedLog
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000040ADO = mps000040ADO;
                this.BedLog = bedLog;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000040PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000040_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000040ADO mps000040ADO,
            V_HIS_BED_LOG bedLog,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000040ADO = mps000040ADO;
                this.BedLog = bedLog;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000040PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000040_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000040ADO mps000040ADO,
            V_HIS_BED_LOG bedLog,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
              List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
              List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000040ADO = mps000040ADO;
                this.BedLog = bedLog;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000040PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000040_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000040ADO mps000040ADO,
            V_HIS_BED_LOG bedLog,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000040ADO = mps000040ADO;
                this.BedLog = bedLog;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000040PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000040_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000040ADO mps000040ADO,
            V_HIS_BED_LOG bedLog,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000040ADO = mps000040ADO;
                this.BedLog = bedLog;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.sereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

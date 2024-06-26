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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000167.PDO
{
    public partial class Mps000167PDO : RDOBase
    {
        public List<V_HIS_SERE_SERV> ListSereServ { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public List<HIS_CONFIG> lstConfig { get; set; }
        public HIS_TRANS_REQ transReq { get; set; }
        public Mps000167PDO() { }
        public Mps000167PDO(V_HIS_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, Mps000167ADO mps000167ADO)
        {
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
            this.Mps000167ADO = mps000167ADO;
        }

        public Mps000167PDO(V_HIS_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, Mps000167ADO mps000167ADO, List<V_HIS_SERE_SERV> listSereServ)
        {
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
            this.Mps000167ADO = mps000167ADO;
            this.ListSereServ = listSereServ;
        }

        public Mps000167PDO(V_HIS_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, Mps000167ADO mps000167ADO, List<V_HIS_SERE_SERV> listSereServ,
              List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
              List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction,
                HIS_TREATMENT HisTreatment)
        {
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
            this.Mps000167ADO = mps000167ADO;
            this.ListSereServ = listSereServ;
            this.ListSereServDeposit = _listSereServDeposit;
            this.ListSereServBill = _listSereServBill;
            this.ListTransaction = _listTransaction;
            this._HisTreatment = HisTreatment;
        }
        public Mps000167PDO(V_HIS_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, Mps000167ADO mps000167ADO,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq)
        {
            this.lstConfig = lstConfig;
            this.transReq = transReq;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
            this.Mps000167ADO = mps000167ADO;
        }

        public Mps000167PDO(V_HIS_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, Mps000167ADO mps000167ADO, List<V_HIS_SERE_SERV> listSereServ,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq)
        {
            this.lstConfig = lstConfig;
            this.transReq = transReq;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
            this.Mps000167ADO = mps000167ADO;
            this.ListSereServ = listSereServ;
        }

        public Mps000167PDO(V_HIS_SERVICE_REQ paanServiceReq, V_HIS_SERE_SERV_5 sereServ, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, Mps000167ADO mps000167ADO, List<V_HIS_SERE_SERV> listSereServ,
              List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
              List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction,
                HIS_TREATMENT HisTreatment,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq)
        {
            this.lstConfig = lstConfig;
            this.transReq = transReq;
            this._PatyAlterBhyt = patyAlterBhyt;
            this._SereServ = sereServ;
            this._PaanServiceReq = paanServiceReq;
            this.Mps000167ADO = mps000167ADO;
            this.ListSereServ = listSereServ;
            this.ListSereServDeposit = _listSereServDeposit;
            this.ListSereServBill = _listSereServBill;
            this.ListTransaction = _listTransaction;
            this._HisTreatment = HisTreatment;
        }
    }
}

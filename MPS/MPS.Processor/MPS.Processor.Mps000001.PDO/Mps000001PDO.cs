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

namespace MPS.Processor.Mps000001.PDO
{
    public partial class Mps000001PDO : RDOBase
    {
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListConfigs { get; set; }
        public string Gate { get; set; }
        public List<HIS_CARD> lstCard { get; set; }
        public Mps000001PDO() { }


        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
             V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
             List<Mps000001_ListSereServs> sereServs,
            HIS_TREATMENT treatment,
            Mps000001ADO Mps000001ADO,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
             V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
             List<Mps000001_ListSereServs> sereServs,
            HIS_TREATMENT treatment,
            Mps000001ADO Mps000001ADO,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
             V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
             List<Mps000001_ListSereServs> sereServs,
            HIS_TREATMENT treatment,
            Mps000001ADO Mps000001ADO)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
     V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
    V_HIS_PATIENT currentPatient,
     List<Mps000001_ListSereServs> sereServs,
    HIS_TREATMENT treatment,
    Mps000001ADO Mps000001ADO,
    HIS_DHST _dhst,
    HIS_WORK_PLACE _wORK_PLACE,
    List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
    List<HIS_SERE_SERV_BILL> _listSereServBill,
    List<V_HIS_TRANSACTION> _listTransaction,
    string Gate)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
                this.Gate = Gate;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
             V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
             List<Mps000001_ListSereServs> sereServs,
            HIS_TREATMENT treatment,
            Mps000001ADO Mps000001ADO,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;


                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
             V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
             List<Mps000001_ListSereServs> sereServs,
            HIS_TREATMENT treatment,
            Mps000001ADO Mps000001ADO,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
             V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            V_HIS_PATIENT currentPatient,
             List<Mps000001_ListSereServs> sereServs,
            HIS_TREATMENT treatment,
            Mps000001ADO Mps000001ADO,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listConfigs)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
     V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
    V_HIS_PATIENT currentPatient,
     List<Mps000001_ListSereServs> sereServs,
    HIS_TREATMENT treatment,
    Mps000001ADO Mps000001ADO,
    HIS_DHST _dhst,
    HIS_WORK_PLACE _wORK_PLACE,
    List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
    List<HIS_SERE_SERV_BILL> _listSereServBill,
    List<V_HIS_TRANSACTION> _listTransaction,
    string Gate,
    HIS_TRANS_REQ transReq,
    List<HIS_CONFIG> listConfigs)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;

                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
                this.Gate = Gate;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000001PDO(V_HIS_SERVICE_REQ serviceReq,
     V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
    V_HIS_PATIENT currentPatient,
     List<Mps000001_ListSereServs> sereServs,
    HIS_TREATMENT treatment,
    Mps000001ADO Mps000001ADO,
    HIS_DHST _dhst,
    HIS_WORK_PLACE _wORK_PLACE,
    List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
    List<HIS_SERE_SERV_BILL> _listSereServBill,
    List<V_HIS_TRANSACTION> _listTransaction,
    string Gate,
    HIS_TRANS_REQ transReq,
    List<HIS_CONFIG> listConfigs,
    List<HIS_CARD> lstCard)
        {
            try
            {
                this.currentPatient = currentPatient;
                this.sereServs = sereServs;
                this.ServiceReq = serviceReq;
                this.PatyAlterBhyt = patyAlterBhyt;
                this.currentTreatment = treatment;
                this.Mps000001ADO = Mps000001ADO;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.TransReq = transReq;
                this.ListConfigs = listConfigs;
                this.lstCard = lstCard;
                if (this.sereServs != null && this.sereServs.Count > 0)
                {
                    this.sereServs = this.sereServs.OrderBy(o => o.ID).ToList();
                }
                else
                {
                    this.sereServs = new List<Mps000001_ListSereServs>();
                }
                this.Gate = Gate;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

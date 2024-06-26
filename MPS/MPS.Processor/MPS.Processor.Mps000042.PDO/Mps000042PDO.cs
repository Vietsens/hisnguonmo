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

namespace MPS.Processor.Mps000042.PDO
{
    public partial class Mps000042PDO : RDOBase
    {
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }

        public Mps000042PDO() { }

        public Mps000042PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000042_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000042ADO Mps000042ADO,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
              List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
              List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000042ADO = Mps000042ADO;
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

        public Mps000042PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000042_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000042ADO Mps000042ADO)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000042ADO = Mps000042ADO;
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

        public Mps000042PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000042_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            HIS_TREATMENT treatment,
            Mps000042ADO Mps000042ADO,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = V_HIS_PATIENT_TYPE_ALTER;
                this.currentHisTreatment = treatment;
                this.Mps000042ADO = Mps000042ADO;
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

    }
}

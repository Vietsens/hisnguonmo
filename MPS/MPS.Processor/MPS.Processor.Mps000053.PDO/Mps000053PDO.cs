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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;
using MPS.Processor.Mps000053.PDO;
using MOS.SDO;

namespace MPS.Processor.Mps000053.PDO
{
    public partial class Mps000053PDO : RDOBase
    {
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }

        public Mps000053PDO() { }

        public Mps000053PDO(
           V_HIS_SERVICE_REQ ServiceReqPrint,
           List<Mps000053_ListSereServ> sereServs,
           V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
           HIS_TREATMENT treatment,
           Mps000053ADO mps000053ADO,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
              List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
              List<HIS_SERE_SERV_BILL> _listSereServBill,
              List<V_HIS_TRANSACTION> _listTransaction
            )
        {
            try
            {
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.ServiceReqPrint = ServiceReqPrint;
                this.currentTreatment = treatment;
                this.Mps000053ADO = mps000053ADO;
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

        public Mps000053PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000053_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT treatment,
            Mps000053ADO mps000053ADO)
        {
            try
            {
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.ServiceReqPrint = ServiceReqPrint;
                this.currentTreatment = treatment;
                this.Mps000053ADO = mps000053ADO;
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

        public Mps000053PDO(
           V_HIS_SERVICE_REQ ServiceReqPrint,
           List<Mps000053_ListSereServ> sereServs,
           V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
           HIS_TREATMENT treatment,
           Mps000053ADO mps000053ADO,
             HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE
            )
        {
            try
            {
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.ServiceReqPrint = ServiceReqPrint;
                this.currentTreatment = treatment;
                this.Mps000053ADO = mps000053ADO;
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

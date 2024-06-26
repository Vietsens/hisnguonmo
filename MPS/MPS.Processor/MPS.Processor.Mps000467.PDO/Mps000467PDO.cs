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
using System.Runtime.InteropServices;

namespace MPS.Processor.Mps000467.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000467PDO : RDOBase
    {
        public Mps000467PDO() { }

        public Mps000467PDO(
            HIS_TREATMENT currentHisTreatment,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000467_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            Mps000467ADO mps000467ADO,
            V_HIS_BED_LOG bedLog,
            V_HIS_SERVICE serviceParent,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<V_HIS_SERVICE> _listService
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.CurrentHisTreatment = currentHisTreatment;
                this.Mps000467ADO = mps000467ADO;
                this.BedLog = bedLog;
                this.ServiceParent = serviceParent;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListService = _listService;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.SereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000467PDO(
            HIS_TREATMENT currentHisTreatment,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000467_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            Mps000467ADO mps000467ADO,
            V_HIS_BED_LOG bedLog,
            V_HIS_SERVICE serviceParent,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<V_HIS_SERVICE> _listService,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.CurrentHisTreatment = currentHisTreatment;
                this.Mps000467ADO = mps000467ADO;
                this.BedLog = bedLog;
                this.ServiceParent = serviceParent;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListService = _listService;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.SereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000467PDO(
            HIS_TREATMENT currentHisTreatment,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<Mps000467_ListSereServ> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            Mps000467ADO mps000467ADO,
            V_HIS_BED_LOG bedLog,
            V_HIS_SERVICE serviceParent,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<V_HIS_SERVICE> _listService,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposit,
            List<HIS_SERE_SERV_BILL> _listSereServBill,
            List<V_HIS_TRANSACTION> _listTransaction,
            LIS.EFMODEL.DataModels.V_LIS_SAMPLE _lisSample)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.CurrentHisTreatment = currentHisTreatment;
                this.Mps000467ADO = mps000467ADO;
                this.BedLog = bedLog;
                this.ServiceParent = serviceParent;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.ListService = _listService;
                this.ListSereServDeposit = _listSereServDeposit;
                this.ListSereServBill = _listSereServBill;
                this.ListTransaction = _listTransaction;
                this.LisSample = _lisSample;
                if (sereServs != null && sereServs.Count > 0)
                {
                    this.SereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

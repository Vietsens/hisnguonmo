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

namespace MPS.Processor.Mps000364.PDO
{
    public class Mps000364PDO : RDOBase
    {
        public V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public HIS_TREATMENT currentHisTreatment { get; set; }
        public List<V_HIS_SERE_SERV> sereServADOs { get; set; }
        public V_HIS_BED_LOG BedLog { get; set; }
        public Mps000364ADO Mps000364ADO { get; set; }
        public HIS_DHST _HIS_DHST { get; set; }
        public HIS_WORK_PLACE _HIS_WORK_PLACE { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<HIS_SERE_SERV_EXT> SereServExts { get; set; }
        public List<HIS_CONFIG> Configs { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }

        public Mps000364PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServADOs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT treatment,
            Mps000364ADO mps000364ADO,
            V_HIS_BED_LOG bedLog,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<V_HIS_SERVICE> _services,
            List<HIS_SERE_SERV_EXT> _sereServExts)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServADOs = sereServADOs;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentHisTreatment = treatment;
                this.Mps000364ADO = mps000364ADO;
                this.BedLog = bedLog;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.Services = _services;
                this.SereServExts = _sereServExts;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000364PDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServADOs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            HIS_TREATMENT treatment,
            Mps000364ADO mps000364ADO,
            V_HIS_BED_LOG bedLog,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<V_HIS_SERVICE> _services,
            List<HIS_SERE_SERV_EXT> _sereServExts,
            List<HIS_CONFIG> _configs,
            HIS_TRANS_REQ _transReq)
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServADOs = sereServADOs;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentHisTreatment = treatment;
                this.Mps000364ADO = mps000364ADO;
                this.BedLog = bedLog;
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.Services = _services;
                this.SereServExts = _sereServExts;
                this.Configs = _configs;
                this.TransReq = _transReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000364ADO
    {
        public string bebRoomName { get; set; }
        public string firstExamRoomName { get; set; }
        public decimal ratio { get; set; }
        public long PatientTypeId__Bhyt { get; set; }
        public string TITLE { get; set; }
        public string PARENT_NAME { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }
}

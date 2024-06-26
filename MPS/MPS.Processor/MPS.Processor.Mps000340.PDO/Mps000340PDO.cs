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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000340.PDO
{
    public class Mps000340PDO : RDOBase
    {
        public List<SereServADO> SereServs { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        public HIS_TREATMENT CurrentHisTreatment { get; set; }  // được phép ghi đè
        public List<V_HIS_SERVICE_REQ> ListServiceReqPrint { get; set; }    // k ghi đè đc
        public Mps000340ADO Mps000340ADO { get; set; }
        public V_HIS_BED_LOG BedLog { get; set; }
        public List<ServiceType> ListServiceType { get; set; }
        public HIS_DHST _HIS_DHST { get; set; }
        public HIS_WORK_PLACE _HIS_WORK_PLACE { get; set; }
        public List<HisServiceReqMaxNumOrderSDO> MaxNumOrderSDO { get; set; }

        public Mps000340PDO(
            HIS_TREATMENT currentHisTreatment,
            List<V_HIS_SERVICE_REQ> ServiceReqPrint,
            List<SereServADO> sereServs,
            V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt,
            Mps000340ADO mps000340ADO,
            V_HIS_BED_LOG bedLog,
            HIS_DHST _dhst,
            HIS_WORK_PLACE _wORK_PLACE,
            List<ServiceType> _listServiceType,
            List<HisServiceReqMaxNumOrderSDO> maxNumOrderSDO
            )
        {
            try
            {
                this.ListServiceReqPrint = ServiceReqPrint;
                this.SereServs = sereServs.OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.CurrentHisTreatment = currentHisTreatment;
                this.Mps000340ADO = mps000340ADO;
                this.BedLog = bedLog;
                this.ListServiceType = _listServiceType.OrderBy(o => o.NUM_ORDER).ToList();
                this._HIS_DHST = _dhst;
                this._HIS_WORK_PLACE = _wORK_PLACE;
                this.MaxNumOrderSDO = maxNumOrderSDO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SereServADO : V_HIS_SERE_SERV
    {
        public decimal? VIR_TOTAL_HEIN_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PATIENT_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_SUM { get; set; }
        public decimal? VIR_TOTAL_PRICE_NO_EXPEND_SUM { get; set; }
        public decimal? PRICE_BHYT { get; set; }
        public long? SERVICE_PACKAGE_ID { get; set; }
        public string DEPARTMENT__SERVICE_GROUP__ID { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }

        public string CONCLUDE { get; set; }
        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string NOTE { get; set; }
        public decimal? ESTIMATE_DURATION { get; set; }

        public string patientIdQr { get; set; }
        public byte[] bPatientQr { get; set; }

        public string patientNameQr { get; set; }
        public byte[] bPatientNameQr { get; set; }

        public string studyDescriptionQr { get; set; }
        public byte[] bStudyDescriptionQr { get; set; }

        public byte[] ServiceReqExecuteQr { get; set; }
        public byte[] SereServIdQr { get; set; }
        public byte[] QrDiimV2 { get; set; }
        public byte[] QrCT540 { get; set; }

        public long SERVICE_TYPE_GROUP_ID { get; set; }

        public SereServADO() { }

        public SereServADO(V_HIS_SERE_SERV data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(this, data);

                this.REQUEST_LOGINNAME = data.TDL_REQUEST_LOGINNAME;
                this.REQUEST_USERNAME = data.TDL_REQUEST_USERNAME;
                this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                this.SERVICE_NAME = data.TDL_SERVICE_NAME;

                VIR_TOTAL_PRICE_NO_EXPEND_SUM = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV_11 data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(this, data);

                this.REQUEST_LOGINNAME = data.TDL_REQUEST_LOGINNAME;
                this.REQUEST_USERNAME = data.TDL_REQUEST_USERNAME;
                this.SERVICE_CODE = data.TDL_SERVICE_CODE;
                this.SERVICE_NAME = data.TDL_SERVICE_NAME;

                VIR_TOTAL_PRICE_NO_EXPEND_SUM = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }

    public class ServiceType : HIS_SERVICE_TYPE
    {
        public long? SERVICE_REQ_ID { get; set; }
        public long SERVICE_TYPE_GROUP_ID { get; set; }
        public string PARENT_NAME { get; set; }
    }

    public class Mps000340ADO
    {
        public string bebRoomName { get; set; }
        public string firstExamRoomName { get; set; }
        public decimal ratio { get; set; }
        public long PatientTypeId__Bhyt { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }
}

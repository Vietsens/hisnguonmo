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

namespace MPS.Processor.Mps000316.PDO
{
    public class Mps000316PDO : RDOBase
    {
        public V_HIS_TREATMENT VHisTreatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter { get; set; }

        public V_HIS_SERVICE_REQ VHisServiceReqExam { get; set; } //thông tin khám
        public HIS_DHST HisDhst { get; set; }

        public List<V_HIS_SERVICE_REQ> VHisServiceReqTests { get; set; } //thông tin xét nghiệm
        public List<V_HIS_SERE_SERV_TEIN> VHisSereServTeins { get; set; }
        public List<V_HIS_TEST_INDEX_RANGE> VHistTestIndexRanges { get; set; }

        public List<V_HIS_SERVICE> VHisServices { get; set; } //các dịch vụ xét nghiệm để gom nhóm

        public List<HIS_SERE_SERV> HisSereServs { get; set; } //tất cả dịch vụ kể cả thuốc, vật tư
        public List<HIS_SERE_SERV_EXT> HisSereServsExts { get; set; }
        public V_HIS_PATIENT VHisPatient { get; set; }
        public List<HIS_EXP_MEST_MEDICINE> ListMedicine { get; set; }
        public List<HIS_EXP_MEST_MATERIAL> ListMaterial { get; set; }

        public List<V_HIS_SERVICE_REQ> VHisServiceReqDonK { get; set; } // thông tin đơn khám
        public List<V_HIS_ROOM> VHisRooms { get; set; } //Các phòng hẹn khám

        public PrescriptionADO prescriptionADO = null;

        public List<V_HIS_SERVICE_REQ> VHisServiceReqHk { get; set; } // thông tin hẹn khám

        public Mps000316PDO(V_HIS_PATIENT _patient, V_HIS_TREATMENT _treatment, V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, V_HIS_SERVICE_REQ _serviceReqExam, HIS_DHST _dhst,
            List<V_HIS_SERVICE_REQ> _serviceReqTests, List<V_HIS_SERE_SERV_TEIN> _sereServTeins, List<V_HIS_TEST_INDEX_RANGE> _testIndexRange,
            List<V_HIS_SERVICE> _services, List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV_EXT> _sereServsExts)
        {
            try
            {
                this.HisDhst = _dhst;
                this.HisSereServs = _sereServs;
                this.HisSereServsExts = _sereServsExts;
                this.VHisPatientTypeAlter = _patientTypeAlter;
                this.VHisSereServTeins = _sereServTeins;
                this.VHisServiceReqExam = _serviceReqExam;
                this.VHisServiceReqTests = _serviceReqTests;
                this.VHisServices = _services;
                this.VHisTreatment = _treatment;
                this.VHistTestIndexRanges = _testIndexRange;
                this.VHisPatient = _patient;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000316PDO(V_HIS_PATIENT _patient, V_HIS_TREATMENT _treatment, V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, V_HIS_SERVICE_REQ _serviceReqExam, HIS_DHST _dhst,
            List<V_HIS_SERVICE_REQ> _serviceReqTests, List<V_HIS_SERE_SERV_TEIN> _sereServTeins, List<V_HIS_TEST_INDEX_RANGE> _testIndexRange,
            List<V_HIS_SERVICE> _services, List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV_EXT> _sereServsExts, List<HIS_EXP_MEST_MEDICINE> _listMedi, List<HIS_EXP_MEST_MATERIAL> _listMaterial)
        {
            try
            {
                this.HisDhst = _dhst;
                this.HisSereServs = _sereServs;
                this.HisSereServsExts = _sereServsExts;
                this.VHisPatientTypeAlter = _patientTypeAlter;
                this.VHisSereServTeins = _sereServTeins;
                this.VHisServiceReqExam = _serviceReqExam;
                this.VHisServiceReqTests = _serviceReqTests;
                this.VHisServices = _services;
                this.VHisTreatment = _treatment;
                this.VHistTestIndexRanges = _testIndexRange;
                this.VHisPatient = _patient;
                this.ListMedicine = _listMedi;
                this.ListMaterial = _listMaterial;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000316PDO(V_HIS_PATIENT _patient, V_HIS_TREATMENT _treatment, V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, V_HIS_SERVICE_REQ _serviceReqExam, HIS_DHST _dhst,
            List<V_HIS_SERVICE_REQ> _serviceReqTests, List<V_HIS_SERE_SERV_TEIN> _sereServTeins, List<V_HIS_TEST_INDEX_RANGE> _testIndexRange,
            List<V_HIS_SERVICE> _services, List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV_EXT> _sereServsExts, List<V_HIS_SERVICE_REQ> _ServiceReqDonK, List<V_HIS_ROOM> _Rooms, List<V_HIS_SERVICE_REQ> _ServiceReqHk)
        {
            try
            {
                this.HisDhst = _dhst;
                this.HisSereServs = _sereServs;
                this.HisSereServsExts = _sereServsExts;
                this.VHisPatientTypeAlter = _patientTypeAlter;
                this.VHisSereServTeins = _sereServTeins;
                this.VHisServiceReqExam = _serviceReqExam;
                this.VHisServiceReqTests = _serviceReqTests;
                this.VHisServices = _services;
                this.VHisTreatment = _treatment;
                this.VHistTestIndexRanges = _testIndexRange;
                this.VHisPatient = _patient;
                this.VHisServiceReqDonK = _ServiceReqDonK;
                this.VHisRooms = _Rooms;
                this.VHisServiceReqHk = _ServiceReqHk;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000316PDO(V_HIS_PATIENT _patient, V_HIS_TREATMENT _treatment, V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, V_HIS_SERVICE_REQ _serviceReqExam, HIS_DHST _dhst,
            List<V_HIS_SERVICE_REQ> _serviceReqTests, List<V_HIS_SERE_SERV_TEIN> _sereServTeins, List<V_HIS_TEST_INDEX_RANGE> _testIndexRange,
            List<V_HIS_SERVICE> _services, List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV_EXT> _sereServsExts, List<HIS_EXP_MEST_MEDICINE> _listMedi, List<HIS_EXP_MEST_MATERIAL> _listMaterial, List<V_HIS_SERVICE_REQ> _ServiceReqDonK, List<V_HIS_ROOM> _Rooms, List<V_HIS_SERVICE_REQ> _ServiceReqHk)
        {
            try
            {
                this.HisDhst = _dhst;
                this.HisSereServs = _sereServs;
                this.HisSereServsExts = _sereServsExts;
                this.VHisPatientTypeAlter = _patientTypeAlter;
                this.VHisSereServTeins = _sereServTeins;
                this.VHisServiceReqExam = _serviceReqExam;
                this.VHisServiceReqTests = _serviceReqTests;
                this.VHisServices = _services;
                this.VHisTreatment = _treatment;
                this.VHistTestIndexRanges = _testIndexRange;
                this.VHisPatient = _patient;
                this.ListMedicine = _listMedi;
                this.ListMaterial = _listMaterial;
                this.VHisServiceReqDonK = _ServiceReqDonK;
                this.VHisRooms = _Rooms;
                this.VHisServiceReqHk = _ServiceReqHk;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000316PDO(V_HIS_PATIENT _patient, V_HIS_TREATMENT _treatment, V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, V_HIS_SERVICE_REQ _serviceReqExam, HIS_DHST _dhst,
            List<V_HIS_SERVICE_REQ> _serviceReqTests, List<V_HIS_SERE_SERV_TEIN> _sereServTeins, List<V_HIS_TEST_INDEX_RANGE> _testIndexRange,
            List<V_HIS_SERVICE> _services, List<HIS_SERE_SERV> _sereServs, List<HIS_SERE_SERV_EXT> _sereServsExts, List<HIS_EXP_MEST_MEDICINE> _listMedi, List<HIS_EXP_MEST_MATERIAL> _listMaterial, List<V_HIS_SERVICE_REQ> _ServiceReqDonK, List<V_HIS_ROOM> _Rooms, List<V_HIS_SERVICE_REQ> _ServiceReqHk, PrescriptionADO _prescriptionADO)
        {
            try
            {
                this.HisDhst = _dhst;
                this.HisSereServs = _sereServs;
                this.HisSereServsExts = _sereServsExts;
                this.VHisPatientTypeAlter = _patientTypeAlter;
                this.VHisSereServTeins = _sereServTeins;
                this.VHisServiceReqExam = _serviceReqExam;
                this.VHisServiceReqTests = _serviceReqTests;
                this.VHisServices = _services;
                this.VHisTreatment = _treatment;
                this.VHistTestIndexRanges = _testIndexRange;
                this.VHisPatient = _patient;
                this.ListMedicine = _listMedi;
                this.ListMaterial = _listMaterial;
                this.VHisServiceReqDonK = _ServiceReqDonK;
                this.VHisRooms = _Rooms;
                this.VHisServiceReqHk = _ServiceReqHk;
                this.prescriptionADO = _prescriptionADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

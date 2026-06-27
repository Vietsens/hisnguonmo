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
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000517.PDO
{
    public partial class Mps000517PDO : RDOBase
    {
        /// <summary>
        /// Constructor tối thiểu: đa mẫu + danh mục dịch vụ (cần cho phân nhóm cha và loại XN).
        /// </summary>
        public Mps000517PDO(
            HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT currentTreatment,
            List<V_LIS_SAMPLE> currentSamples,
            List<HIS_SERVICE_REQ> currentServiceReqs,
            List<V_HIS_TEST_INDEX> lstTestIndexs,
            List<V_LIS_RESULT> lstLisResult,
            List<V_HIS_TEST_INDEX_RANGE> testIndexRanges,
            long genderId,
            List<V_HIS_SERVICE> listService,
            [Optional] V_HIS_SERVICE serviceParent)
        {
            this.PatientTypeAlter = patientTypeAlter;
            this.currentTreatment = currentTreatment;
            this.currentSamples = currentSamples;
            this.currentServiceReqs = currentServiceReqs;
            this.lstTestIndex = lstTestIndexs;
            this.lstLisResult = lstLisResult;
            this.testIndexRangeAll = testIndexRanges;
            this.genderId = genderId;
            this.ListTestService = listService;
            this.ServiceParent = serviceParent;
        }

        /// <summary>
        /// Constructor mở rộng: thêm bệnh nhân + giường + sere_serv + loại bệnh phẩm.
        /// </summary>
        public Mps000517PDO(
            HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT currentTreatment,
            List<V_LIS_SAMPLE> currentSamples,
            List<HIS_SERVICE_REQ> currentServiceReqs,
            List<V_HIS_TEST_INDEX> lstTestIndexs,
            List<V_LIS_RESULT> lstLisResult,
            List<V_HIS_TEST_INDEX_RANGE> testIndexRanges,
            long genderId,
            List<V_HIS_SERVICE> listService,
            HIS_PATIENT patient,
            V_HIS_TREATMENT_BED_ROOM currentTreatBedRoom,
            List<HIS_SERE_SERV> listSereServ,
            List<LIS_SAMPLE_TYPE> lstSampleType,
            List<HIS_TEST_SAMPLE_TYPE> lstTestSampleType,
            [Optional] V_HIS_SERVICE serviceParent)
            : this(patientTypeAlter, currentTreatment, currentSamples, currentServiceReqs,
                   lstTestIndexs, lstLisResult, testIndexRanges, genderId, listService, serviceParent)
        {
            this.currentPatient = patient;
            this.currentTreatBedRoom = currentTreatBedRoom;
            this.ListSereServ = listSereServ;
            this.ListSampleType = lstSampleType;
            this.ListTestSampleType = lstTestSampleType;
        }

        /// <summary>
        /// Constructor đầy đủ: thêm danh sách chỉ số máu lọc (MLCT).
        /// </summary>
        public Mps000517PDO(
            HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            HIS_TREATMENT currentTreatment,
            List<V_LIS_SAMPLE> currentSamples,
            List<HIS_SERVICE_REQ> currentServiceReqs,
            List<V_HIS_TEST_INDEX> lstTestIndexs,
            List<V_LIS_RESULT> lstLisResult,
            List<V_HIS_TEST_INDEX_RANGE> testIndexRanges,
            long genderId,
            List<V_HIS_SERVICE> listService,
            HIS_PATIENT patient,
            V_HIS_TREATMENT_BED_ROOM currentTreatBedRoom,
            List<HIS_SERE_SERV> listSereServ,
            List<LIS_SAMPLE_TYPE> lstSampleType,
            List<HIS_TEST_SAMPLE_TYPE> lstTestSampleType,
            List<MLCTADO> lstMLCTADO,
            [Optional] V_HIS_SERVICE serviceParent)
            : this(patientTypeAlter, currentTreatment, currentSamples, currentServiceReqs,
                   lstTestIndexs, lstLisResult, testIndexRanges, genderId, listService,
                   patient, currentTreatBedRoom, listSereServ, lstSampleType, lstTestSampleType, serviceParent)
        {
            this.ListMlctado = lstMLCTADO;
        }
    }
}

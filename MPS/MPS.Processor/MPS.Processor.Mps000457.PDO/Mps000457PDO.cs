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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System.Runtime.InteropServices;

namespace MPS.Processor.Mps000457.PDO
{

    public partial class Mps000457PDO : RDOBase
    {
        public Mps000457PDO(HIS_PATIENT_TYPE_ALTER patientTypeAlter, HIS_TREATMENT currentTreatment, V_LIS_SAMPLE currentSample, HIS_SERVICE_REQ currentServiceReq, List<V_HIS_TEST_INDEX> lstTestIndexs, List<V_LIS_RESULT> lstLisResult, List<V_HIS_TEST_INDEX_RANGE> testIndexRanges, long gerderId, [Optional] V_HIS_SERVICE serviceParent)
        {
            this.PatientTypeAlter = patientTypeAlter;
            this.currentTreatment = currentTreatment;
            this.currentSample = currentSample;
            this.currentServiceReq = currentServiceReq;
            this.lstTestIndex = lstTestIndexs;
            this.lstLisResult = lstLisResult;
            this.testIndexRangeAll = testIndexRanges;
            this.ServiceParent = serviceParent;
            this.genderId = gerderId;
        }

        public Mps000457PDO(HIS_PATIENT_TYPE_ALTER patientTypeAlter, HIS_TREATMENT currentTreatment, V_LIS_SAMPLE currentSample, HIS_SERVICE_REQ currentServiceReq, List<V_HIS_TEST_INDEX> lstTestIndexs, List<V_LIS_RESULT> lstLisResult, List<V_HIS_TEST_INDEX_RANGE> testIndexRanges, long gerderId, List<V_HIS_SERVICE> listService, [Optional] V_HIS_SERVICE serviceParent)
        {
            this.PatientTypeAlter = patientTypeAlter;
            this.currentTreatment = currentTreatment;
            this.currentSample = currentSample;
            this.currentServiceReq = currentServiceReq;
            this.lstTestIndex = lstTestIndexs;
            this.lstLisResult = lstLisResult;
            this.testIndexRangeAll = testIndexRanges;
            this.ServiceParent = serviceParent;
            this.genderId = gerderId;
            this.ListTestService = listService;
        }

        public Mps000457PDO(HIS_PATIENT_TYPE_ALTER patientTypeAlter, HIS_TREATMENT currentTreatment, V_LIS_SAMPLE currentSample, HIS_SERVICE_REQ currentServiceReq, List<V_HIS_TEST_INDEX> lstTestIndexs, List<V_LIS_RESULT> lstLisResult, List<V_HIS_TEST_INDEX_RANGE> testIndexRanges, long gerderId, List<V_HIS_SERVICE> listService, HIS_PATIENT patient, [Optional] V_HIS_SERVICE serviceParent)
        {
            this.PatientTypeAlter = patientTypeAlter;
            this.currentTreatment = currentTreatment;
            this.currentSample = currentSample;
            this.currentServiceReq = currentServiceReq;
            this.lstTestIndex = lstTestIndexs;
            this.lstLisResult = lstLisResult;
            this.testIndexRangeAll = testIndexRanges;
            this.ServiceParent = serviceParent;
            this.genderId = gerderId;
            this.ListTestService = listService;
            this.currentPatient = patient;
        }

    }
}

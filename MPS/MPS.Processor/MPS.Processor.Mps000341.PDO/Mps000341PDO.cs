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
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000341.PDO
{
    public class Mps000341PDO : RDOBase
    {
        public V_LIS_SAMPLE _Sample { get; set; }
        public LIS_SAMPLE_SERVICE _SampleService { get; set; }
        public LIS_MACHINE _Machine { get; set; }
        public List<V_LIS_RESULT> _Results { get; set; }
        public List<LIS_PATIENT_CONDITION> _PatientConditions { get; set; }
        public HIS_SERVICE_REQ _serviceReq { get; set; }
        public HIS_TREATMENT _treatment { get; set; }
        public HIS_PATIENT _patient { get; set; }



        public Mps000341PDO(V_LIS_SAMPLE sample, LIS_SAMPLE_SERVICE service, LIS_MACHINE machine, List<V_LIS_RESULT> results)
        {
            this._Sample = sample;
            this._SampleService = service;
            this._Machine = machine;
            this._Results = results;
        }

        public Mps000341PDO(V_LIS_SAMPLE sample, LIS_SAMPLE_SERVICE service, LIS_MACHINE machine, List<V_LIS_RESULT> results, HIS_SERVICE_REQ serviceReq, HIS_TREATMENT treatment)
        {
            this._Sample = sample;
            this._SampleService = service;
            this._Machine = machine;
            this._Results = results;
            this._serviceReq = serviceReq;
            this._treatment = treatment;
        }

        public Mps000341PDO(V_LIS_SAMPLE sample, LIS_SAMPLE_SERVICE service, LIS_MACHINE machine, List<V_LIS_RESULT> results, HIS_SERVICE_REQ serviceReq, HIS_TREATMENT treatment, HIS_PATIENT patient)
        {
            this._Sample = sample;
            this._SampleService = service;
            this._Machine = machine;
            this._Results = results;
            this._serviceReq = serviceReq;
            this._treatment = treatment;
            this._patient = patient;
        }
        public Mps000341PDO(V_LIS_SAMPLE sample, LIS_SAMPLE_SERVICE service, LIS_MACHINE machine, List<V_LIS_RESULT> results, HIS_SERVICE_REQ serviceReq, HIS_TREATMENT treatment, HIS_PATIENT patient, List<LIS_PATIENT_CONDITION> conditions)
        {
            this._Sample = sample;
            this._SampleService = service;
            this._Machine = machine;
            this._Results = results;
            this._serviceReq = serviceReq;
            this._treatment = treatment;
            this._patient = patient;
            this._PatientConditions = conditions;
        }
    }
}

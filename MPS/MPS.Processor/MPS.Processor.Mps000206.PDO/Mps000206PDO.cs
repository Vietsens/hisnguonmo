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
using SCN.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000206.PDO
{
    public class Mps000206PDO : RDOBase
    {
        public V_HIS_SERVICE_REQ _ServiceReqExam { get; set; }
        public HIS_DHST _Dhst { get; set; }
        public List<V_HIS_SERE_SERV_7> _SereServTests { get; set; }
        public V_HIS_PATIENT _Patient { get; set; }
        public List<V_SCN_ALLERGIC> _ScnAllergic { get; set; }
        public List<V_SCN_DISABILITY> _ScnDisability { get; set; }
        public List<V_SCN_DISEASE> _ScnDisease { get; set; }
        public List<V_SCN_HEALTH_RISK> _ScnHealthRisk { get; set; }
        public List<SCN_SURGERY> _ScnSurgery { get; set; }
        public V_HIS_TREATMENT_4 _Treatment { get; set; }
        public List<AllergicTypeRelative> _ScnAllergicTypeRelatives { get; set; }
        public List<DiseaseTypeRelative> _ScnDiseaseTypeRelatives { get; set; }

        public Mps000206PDO(
            V_HIS_TREATMENT_4 _treatment,
            V_HIS_PATIENT _patient,
            V_HIS_SERVICE_REQ serviceReqExam,
            HIS_DHST dhst,
            List<V_HIS_SERE_SERV_7> _serviceReqTests,
            List<V_SCN_ALLERGIC> _scnAllergic,
            List<V_SCN_DISABILITY> _scnDisability,
            List<V_SCN_DISEASE> _scnDisease,
            List<V_SCN_HEALTH_RISK> _scnHealthRisk,
            List<SCN_SURGERY> _scnSurgery,
            List<AllergicTypeRelative> _scnAllergicTypeRelatives,
            List<DiseaseTypeRelative> _scnDiseaseTypeRelatives
            )
        {
            this._Treatment = _treatment;
            this._Patient = _patient;
            this._ServiceReqExam = serviceReqExam;
            this._Dhst = dhst;
            this._SereServTests = _serviceReqTests;
            this._ScnAllergic = _scnAllergic;
            this._ScnDisability = _scnDisability;
            this._ScnDisease = _scnDisease;
            this._ScnHealthRisk = _scnHealthRisk;
            this._ScnSurgery = _scnSurgery;
            this._ScnAllergicTypeRelatives = _scnAllergicTypeRelatives;
            this._ScnDiseaseTypeRelatives = _scnDiseaseTypeRelatives;
        }

        public class AllergicTypeRelative : SCN_ALLERGIC_TYPE
        {
            public string Father { get; set; }
            public string Mother { get; set; }
            public string Description_Father { get; set; }
            public string Description_Mother { get; set; }
        }

        public class DiseaseTypeRelative : SCN_DISEASE_TYPE
        {
            public string Father { get; set; }
            public string Mother { get; set; }
            public string Description_Father { get; set; }
            public string Description_Mother { get; set; }
        }

    }
}

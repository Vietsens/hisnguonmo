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

namespace MPS.Processor.Mps000222.PDO
{
    public class Mps000222PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000222";

        public V_HIS_TREATMENT HisTreatment { get; set; }
        public List<V_HIS_SERVICE_REQ> VHisServiceReqExams { get; set; } //thông tin khám
        public List<V_HIS_SERVICE_REQ> VHisServiceReqTests { get; set; } //thông tin xét nghiệm
        public List<V_HIS_TEST_INDEX_RANGE> testIndexRange { get; set; }
        public List<V_HIS_SERVICE> HisServices { get; set; } //các dịch vụ xét nghiệm để gom nhóm
        public List<HIS_SERE_SERV> HisSereServs { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> VHisSereServTeins { get; set; }
        public Mps000222SDO Mps000222SDO { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter { get; set; }
        public List<ExpMestMedicineSDO> ListExpMestMedcine { get; set; }
        public List<HIS_DIIM_TYPE> lstDiimType { get; set; }
        public List<HIS_FUEX_TYPE> lstFuexType { get; set; }
        public Mps000222PDO(V_HIS_TREATMENT _HisTreatment,
            List<V_HIS_SERVICE_REQ> _VHisServiceReqExam,
            List<V_HIS_SERVICE_REQ> _VHisServiceReqTest,
            List<V_HIS_TEST_INDEX_RANGE> _testIndexRange,
            List<V_HIS_SERVICE> _HisService,
            List<HIS_SERE_SERV> _HisSereServs,
            List<V_HIS_SERE_SERV_TEIN> _VHisSereServTeins,
            Mps000222SDO _Mps000222SDO,
            HIS_DHST _HisDhst,
            V_HIS_PATIENT_TYPE_ALTER _VHisPatientTypeAlter,
            List<ExpMestMedicineSDO> _listExpMestMedcine,
            List<HIS_DIIM_TYPE> _lstDiimType,
             List<HIS_FUEX_TYPE> _lstFuexType
            )
        {
            this.HisSereServs = _HisSereServs;
            this.HisServices = _HisService;
            this.HisTreatment = _HisTreatment;
            this.VHisSereServTeins = _VHisSereServTeins;
            this.VHisServiceReqExams = _VHisServiceReqExam;
            this.VHisServiceReqTests = _VHisServiceReqTest;
            this.testIndexRange = _testIndexRange;
            this.Mps000222SDO = _Mps000222SDO;
            this.HisDhst = _HisDhst;
            this.VHisPatientTypeAlter = _VHisPatientTypeAlter;
            this.ListExpMestMedcine = _listExpMestMedcine;
            this.lstDiimType = _lstDiimType;
            this.lstFuexType = _lstFuexType;
        }

        public class ExpMestMedicineSDO : V_HIS_EXP_MEST_MEDICINE
        {
            public short? IS_ADDICTIVE { get; set; }
            public short? IS_NEUROLOGICAL { get; set; }
            public string MEDICINE_USE_FORM_NAME { get; set; }
            public int Type { get; set; }//1: thuoc // 2: vat tu, 3: thuoc trong kho, 4: thuoc ngoai kho, 5: tu tuc
        }
    }
}

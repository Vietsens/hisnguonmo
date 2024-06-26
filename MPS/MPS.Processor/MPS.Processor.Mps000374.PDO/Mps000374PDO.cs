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
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000374.PDO
{
    public class Mps000374PDO : RDOBase
    {
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }
        public List<HIS_TRACKING> _Trackings { get; set; }
        public List<HIS_DHST> _Dhsts { get; set; }
        public Dictionary<long, HIS_SERVICE_REQ> _DicServiceReqs { get; set; }
        public Dictionary<long, List<HIS_SERE_SERV>> _DicSereServs { get; set; }
        public Dictionary<long, HIS_EXP_MEST> _DicHisExpMests { get; set; }
        public Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> _DicExpMestMedicines { get; set; }
        public Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> _DicExpMestMaterials { get; set; }
        public Dictionary<long, List<HIS_SERVICE_REQ_METY>> _DicServiceReqMetys { get; set; }
        public Dictionary<long, List<HIS_SERVICE_REQ_MATY>> _DicServiceReqMatys { get; set; }
        public List<HIS_CARE> _Cares { get; set; }
        public List<V_HIS_CARE_DETAIL> _CareDetails { get; set; }
        public Mps000374SingleKey _WorkPlaceSDO { get; set; }
        public List<HIS_ICD> _Icds { get; set; }
        public long _KeyVienTim { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<HIS_SERVICE_TYPE> _ServiceTypes { get; set; }

        public List<HIS_IMP_MEST> _ImpMests_input { get; set; }
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedis { get; set; }
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMates { get; set; }

        public List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }

        public List<HIS_MEDICINE_USE_FORM> _MedicineUseForms { get; set; }

        // 07
         public V_HIS_PATIENT_TYPE_ALTER PatyAlter { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        public HIS_TREATMENT Treatment { get; set; }
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_SERVICE_REQ ExamServiceReq { get; set; }
        public V_HIS_PATIENT _currentPatient { get; set; }
        public HIS_DHST DHST { get; set; }
        public List<V_HIS_EXP_MEST_BLOOD> ExpMestBloodList { get; set; }
        public List<V_HIS_EXP_MEST_BLTY_REQ> ExpMestBltyReqList { get; set; }
        public List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicineList { get; set; }
        public List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterialList { get; set; }
        public SingleKeyValue SingleKeyValue { get; set; }

        #region -------------
        //public Mps000374PDO(
        //    HIS_TREATMENT _treatment,
        //    V_HIS_TREATMENT_BED_ROOM _treatmentBedRoom,
        //    List<HIS_TRACKING> _trackings,
        //    List<HIS_DHST> _dhsts,
        //    Dictionary<long, HIS_SERVICE_REQ> _dicServiceReqs,
        //    Dictionary<long, List<HIS_SERE_SERV>> _dicSereServs,
        //    Dictionary<long, HIS_EXP_MEST> _dicHisExpMests,
        //    Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> _dicExpMestMedicines,
        //    Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> _dicExpMestMaterials,
        //    Dictionary<long, List<HIS_SERVICE_REQ_METY>> _dicHisServiceReqMetys,
        //    List<HIS_CARE> _cares,
        //    List<V_HIS_CARE_DETAIL> _careDetails,
        //    Mps000374SingleKey _workPlaceSDO,
        //    List<HIS_ICD> _icds,
        //    List<V_HIS_MEDICINE_TYPE> _medicineTypes,
        //    List<V_HIS_MATERIAL_TYPE> _materialTypes,
        //    List<HIS_SERVICE_TYPE> _serviceTypes,
        //    long keyVienTim,
        //    List<HIS_IMP_MEST> _impMests_input,
        //    List<V_HIS_IMP_MEST_MEDICINE> _impMestMedis,
        //    List<V_HIS_IMP_MEST_MATERIAL> _impMestMates
        //    )
        //{
        //    try
        //    {
        //        this._Treatment = _treatment;
        //        this._TreatmentBedRoom = _treatmentBedRoom;
        //        this._Trackings = _trackings;
        //        this._Dhsts = _dhsts;
        //        this._DicServiceReqs = _dicServiceReqs;
        //        this._DicSereServs = _dicSereServs;
        //        this._DicHisExpMests = _dicHisExpMests;
        //        this._DicExpMestMedicines = _dicExpMestMedicines;
        //        this._DicExpMestMaterials = _dicExpMestMaterials;
        //        this._DicServiceReqMetys = _dicHisServiceReqMetys;
        //        this._Cares = _cares;
        //        this._CareDetails = _careDetails;
        //        this._Icds = _icds;
        //        this._MedicineTypes = _medicineTypes;
        //        this._MaterialTypes = _materialTypes;
        //        this._ServiceTypes = _serviceTypes;
        //        this._WorkPlaceSDO = _workPlaceSDO;
        //        this._KeyVienTim = keyVienTim;
        //        this._ImpMests_input = _impMests_input;
        //        this._ImpMestMedis = _impMestMedis;
        //        this._ImpMestMates = _impMestMates;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
        #endregion

        public Mps000374PDO(
           HIS_TREATMENT _treatment,
           V_HIS_TREATMENT_BED_ROOM _treatmentBedRoom,
           List<HIS_TRACKING> _trackings,
           List<HIS_DHST> _dhsts,
           Dictionary<long, HIS_SERVICE_REQ> _dicServiceReqs,
           Dictionary<long, List<HIS_SERE_SERV>> _dicSereServs,
           Dictionary<long, HIS_EXP_MEST> _dicHisExpMests,
           Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> _dicExpMestMedicines,
           Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> _dicExpMestMaterials,
           Dictionary<long, List<HIS_SERVICE_REQ_METY>> _dicHisServiceReqMetys,
             Dictionary<long, List<HIS_SERVICE_REQ_MATY>> _dicHisServiceReqMatys,
           List<HIS_CARE> _cares,
           List<V_HIS_CARE_DETAIL> _careDetails,
           Mps000374SingleKey _workPlaceSDO,
           List<HIS_ICD> _icds,
           List<V_HIS_MEDICINE_TYPE> _medicineTypes,
           List<V_HIS_MATERIAL_TYPE> _materialTypes,
           List<HIS_SERVICE_TYPE> _serviceTypes,
           long keyVienTim,
           List<HIS_IMP_MEST> _impMests_input,
           List<V_HIS_IMP_MEST_MEDICINE> _impMestMedis,
           List<V_HIS_IMP_MEST_MATERIAL> _impMestMates,
              List<HIS_SERE_SERV_EXT> _sereServExts,
            List<HIS_MEDICINE_USE_FORM> _medicineUseForms,
            // 07
             V_HIS_PATIENT _currentPatient,
           V_HIS_PATIENT_TYPE_ALTER _patyAlter,
           List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
           V_HIS_SERVICE_REQ _examServiceReq,
           HIS_DHST _dhst,
           List<HIS_SERE_SERV> _sereServs,
           SingleKeyValue _singleKeyValue,
            List<V_HIS_EXP_MEST_BLOOD> _expMestBloodList,
            List<V_HIS_EXP_MEST_BLTY_REQ> _expMestBltyReqlist,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicineList,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterialList
           )
        {
            try
            {
                this._Treatment = _treatment;
                this._TreatmentBedRoom = _treatmentBedRoom;
                this._Trackings = _trackings;
                this._Dhsts = _dhsts;
                this._DicServiceReqs = _dicServiceReqs;
                this._DicSereServs = _dicSereServs;
                this._DicHisExpMests = _dicHisExpMests;
                this._DicExpMestMedicines = _dicExpMestMedicines;
                this._DicExpMestMaterials = _dicExpMestMaterials;
                this._DicServiceReqMetys = _dicHisServiceReqMetys;
                this._DicServiceReqMatys = _dicHisServiceReqMatys;
                this._Cares = _cares;
                this._CareDetails = _careDetails;
                this._Icds = _icds;
                this._MedicineTypes = _medicineTypes;
                this._MaterialTypes = _materialTypes;
                this._ServiceTypes = _serviceTypes;
                this._WorkPlaceSDO = _workPlaceSDO;
                this._KeyVienTim = keyVienTim;
                this._ImpMests_input = _impMests_input;
                this._ImpMestMedis = _impMestMedis;
                this._ImpMestMates = _impMestMates;
                this._SereServExts = _sereServExts;
                this._MedicineUseForms = _medicineUseForms;

                // 07
                this.PatyAlter = _patyAlter;
                this.Treatment = _treatment;
                this.SereServs = _sereServs;
                this.DepartmentTrans = _departmentTrans;
                this.SingleKeyValue = _singleKeyValue;
                this.ExamServiceReq = _examServiceReq;
                this._currentPatient = _currentPatient;
                this.DHST = _dhst;
                this.ExpMestBloodList = _expMestBloodList;
                this.ExpMestBltyReqList = _expMestBltyReqlist;
                this.ExpMestMaterialList = _expMestMaterialList;
                this.ExpMestMedicineList = _expMestMedicineList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class Mps000374SingleKey : WorkPlaceSDO
    {
        public string LOGIN_NAME { get; set; }
        public string USER_NAME { get; set; }
        public bool IsShowMedicineLine { get; set; }
        public long IsOrderByType { get; set; }

        public Mps000374SingleKey(WorkPlaceSDO data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000374SingleKey>(this, data);
            }
        }

            
    }
}

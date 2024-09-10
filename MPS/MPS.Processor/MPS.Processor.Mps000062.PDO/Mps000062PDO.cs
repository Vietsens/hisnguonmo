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

namespace MPS.Processor.Mps000062.PDO
{
    public class Mps000062PDO : RDOBase
    {
        public HIS_TREATMENT _Treatment { get; set; }
        public List<V_HIS_TREATMENT_BED_ROOM> _TreatmentBedRooms { get; set; }
        public List<V_HIS_TRACKING> _Trackings { get; set; }
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
        public Mps000062SingleKey _WorkPlaceSDO { get; set; }
        public List<HIS_ICD> _Icds { get; set; }
        public List<V_HIS_MEDICINE_TYPE> _MedicineTypes { get; set; }
        public List<V_HIS_MATERIAL_TYPE> _MaterialTypes { get; set; }
        public List<HIS_SERVICE_TYPE> _ServiceTypes { get; set; }

        public List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }

        public List<HIS_MEDICINE_USE_FORM> _MedicineUseForms { get; set; }

        public List<V_HIS_SERE_SERV_RATION> _SereServRations { get; set; }

        public List<V_HIS_IMP_MEST_2> _MobaImpMests { get; set; }
        public List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines_TL { get; set; }
        public List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterial_TL { get; set; }
        public List<V_HIS_EXP_MEST_BLTY_REQ_2> _ExpMestBltyReq2 { get; set; }
        public List<V_HIS_SERVICE> _HisServiceViews { get; set; }
        public List<V_HIS_IMP_MEST_BLOOD> _ImpMestBlood_TL { get; set; }
        public HIS_PATIENT_TYPE_ALTER PatientTypeAlter { get; set; }
        public List<HIS_MEDICINE_LINE> MedicineLine { get; set; }
        public List<HIS_DOSAGE_FORM> DosageForm { get; set; }
        #region -------------
        //public Mps000062PDO(
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
        //    Mps000062SingleKey _workPlaceSDO,
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
          
        public Mps000062PDO() { }

        public Mps000062PDO(
           HIS_TREATMENT _treatment,
           List<V_HIS_TREATMENT_BED_ROOM> _treatmentBedRooms,
           List<V_HIS_TRACKING> _trackings,
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
           Mps000062SingleKey _workPlaceSDO,
           List<HIS_ICD> _icds,
           List<V_HIS_MEDICINE_TYPE> _medicineTypes,
           List<V_HIS_MATERIAL_TYPE> _materialTypes,
           List<HIS_SERVICE_TYPE> _serviceTypes,
           List<HIS_SERE_SERV_EXT> _sereServExts,
            List<HIS_MEDICINE_USE_FORM> _medicineUseForms,
            List<V_HIS_SERE_SERV_RATION> _sereServRations,
            List<V_HIS_IMP_MEST_2> MobaImpMests,
            List<V_HIS_IMP_MEST_MEDICINE> ImpMestMedicines_TL,
            List<V_HIS_IMP_MEST_MATERIAL> ImpMestMaterial_TL,
            List<V_HIS_EXP_MEST_BLTY_REQ_2> ExpMestBltyReq2,
            List<V_HIS_SERVICE> HisServiceViews,
            List<V_HIS_IMP_MEST_BLOOD> _ImpMestBlood_TL, 
            HIS_PATIENT_TYPE_ALTER PatientTypeAlter,
            List<HIS_MEDICINE_LINE> MedicineLine,
            List<HIS_DOSAGE_FORM> DosageForm
           )
        {
            try
            {
                this.MedicineLine = MedicineLine;
                this.DosageForm = DosageForm;
                this.PatientTypeAlter = PatientTypeAlter;
                this._Treatment = _treatment;
                this._TreatmentBedRooms = _treatmentBedRooms;
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
                this._SereServExts = _sereServExts;
                this._MedicineUseForms = _medicineUseForms;
                this._SereServRations = _sereServRations;
                this._MobaImpMests = MobaImpMests;
                this._ImpMestMedicines_TL = ImpMestMedicines_TL;
                this._ImpMestMaterial_TL = ImpMestMaterial_TL;
                this._ExpMestBltyReq2 = ExpMestBltyReq2;
                this._HisServiceViews = HisServiceViews;
                this._ImpMestBlood_TL = _ImpMestBlood_TL;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
    public class Mps000062SingleKey : WorkPlaceSDO
    {
        public string LOGIN_NAME { get; set; }
        public string USER_NAME { get; set; }
        public bool IsShowMedicineLine { get; set; }
        public long IsOrderByType { get; set; }
        public long keyVienTim { get; set; }
        public long UsedDayCountingOption { get; set; }
        public long UsedDayCountingFormatOption { get; set; }
        public long UsedDayCountingOutStockOption { get; set; }

        public Mps000062SingleKey() { }

        public Mps000062SingleKey(WorkPlaceSDO data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<Mps000062SingleKey>(this, data);
            }
        }
    }
}

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
using Inventec.Core;
using MPS.Processor.Mps000429.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MOS.EFMODEL.DataModels;
using System.Threading;
using FlexCel.Report;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using AutoMapper;

namespace MPS.Processor.Mps000429
{
    public class Mps000429Processor : AbstractProcessor
    {
        internal Mps000429ADOExt mps000429ADOExt = new Mps000429ADOExt();
        internal Mps000429PDO rdo;
        internal List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        internal HIS_TRACKING _tracking { get; set; }
        internal HIS_TREATMENT _treatment { get; set; }
        internal List<Mps000429ADO> _Mps000429ADOs { get; set; }
        internal List<Mps000429ExtADO> _Mps000429ExtADOs { get; set; }
        internal List<ServiceCLS> _ServiceCLSs { get; set; }
        internal List<ServiceCLS> _Bloods { get; set; }
        internal List<ServiceCLS> _ExamServices { get; set; }
        internal List<ServiceCLS> _TTServices { get; set; }
        internal List<RemedyCountADO> _RemedyCountADOs { get; set; }
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommons { get; set; }
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOs { get; set; }
        internal List<MedicineLineADO> MedicineLineADOs { get; set; }
        internal List<ExpMestMatyReqADO> _ExpMestMatyReqADOs { get; set; }
        internal List<ServiceReqMetyADO> _ServiceReqMetyADOs { get; set; }
        internal List<ServiceReqMatyADO> _ServiceReqMatyADOs { get; set; }
        internal List<MedicalInstruction> _MedicalInstructions { get; set; }
        Dictionary<long, List<NumberDate>> _DicCountNumbers = new Dictionary<long, List<NumberDate>>();
        Dictionary<long, List<NumberDate>> _DicCountNumberByTypes = new Dictionary<long, List<NumberDate>>();

        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOsV2 { get; set; }//Them ghi chu Y lệnh ngày 08/04, 09/04, 10/04

        List<SingleKeyTracking> _SingleKeyTamThans = new List<SingleKeyTracking>();
        SingleKeyTracking keyTamThan { get; set; }

        List<HIS_SERVICE_REQ> ServiceReq;

        //TH
        internal List<ImpMestMedicineADO> _ImpMestMedicineADOs { get; set; }
        internal List<ImpMestMaterialADO> _ImpMestMaterialADOs { get; set; }

        internal long _EXECUE_TIME_DHST { get; set; }

        string IS_SHOW = "X";

        long sum_medi_mate = 0;


        public Mps000429Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000429PDO)rdoBase;
        }

        /// <summary>
        /// ---- STT Ke Thuoc GN HT ----
        /// </summary>
        void CheckIntructionDate_GN_HT()
        {
            try
            {
                if (rdo._DicHisExpMests != null && rdo._DicHisExpMests.Count > 0)
                {
                    var ExpMests = rdo._DicHisExpMests.Values.OrderBy(p => p.TDL_INTRUCTION_DATE).ToList();
                    var ExpMestGroups = ExpMests.GroupBy(o => o.TDL_INTRUCTION_DATE).ToList();//group lại theo ngày chỉ định
                    int num = 1;
                    foreach (var itemGroups in ExpMestGroups)
                    {
                        List<long> _expMestIds = new List<long>();
                        _expMestIds = itemGroups.Select(p => p.ID).Distinct().ToList();
                        if (rdo._DicExpMestMedicines != null && rdo._DicExpMestMedicines.Count > 0)
                        {
                            List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                            foreach (var item in _expMestIds)
                            {
                                if (rdo._DicExpMestMedicines.ContainsKey(item))
                                {
                                    _ExpMestMedicines.AddRange(rdo._DicExpMestMedicines[item]);
                                }
                            }

                            if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                            {
                                var expMestGroup = _ExpMestMedicines.GroupBy(p => p.TDL_MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                                foreach (var expMedicine in expMestGroup)
                                {
                                    var check = rdo._MedicineTypes.FirstOrDefault(p => p.ID == expMedicine[0].TDL_MEDICINE_TYPE_ID);
                                    if (check == null)
                                    {
                                        continue;
                                    }
                                    //if (check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO
                                    //    )
                                    //{ }
                                    //else
                                    //    continue;

                                    string key = itemGroups.Key + "_" + expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID;
                                    if (!_DicCountNumberByTypes.ContainsKey(expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        ado.num = num;
                                        _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0] = new List<NumberDate>();
                                        _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        //ado.num = _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Count + 1;

                                        var _DataNumber = _DicCountNumberByTypes[check.ID].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                        if (_DataNumber != null)
                                        {
                                            var _DataNumbers = _DicCountNumberByTypes[check.ID].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count;
                                        }
                                        else
                                        {
                                            var _DataNumbers = _DicCountNumberByTypes[check.ID].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count + 1;
                                        }

                                        _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                }
                            }
                        }
                        //num++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// ---- STT Ke Thuoc GN HT KS TD Theo loai MEDICINE_GROUP #10061----
        /// </summary>
        void CheckSTTByMedicineGroup()
        {
            try
            {
                if (rdo._DicHisExpMests != null && rdo._DicHisExpMests.Count > 0)
                {
                    var ExpMests = rdo._DicHisExpMests.Values.OrderBy(p => p.TDL_INTRUCTION_DATE).ToList();
                    var ExpMestGroups = ExpMests.GroupBy(o => o.TDL_INTRUCTION_DATE).ToList();//group lại theo ngày chỉ định
                    int num = 1;
                    foreach (var itemGroups in ExpMestGroups)
                    {
                        List<long> _expMestIds = new List<long>();
                        _expMestIds = itemGroups.Select(p => p.ID).Distinct().ToList();
                        if (rdo._DicExpMestMedicines != null && rdo._DicExpMestMedicines.Count > 0)
                        {
                            List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                            foreach (var item in _expMestIds)
                            {
                                if (rdo._DicExpMestMedicines.ContainsKey(item))
                                {
                                    _ExpMestMedicines.AddRange(rdo._DicExpMestMedicines[item]);
                                }
                            }

                            if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                            {
                                var expMestGroup = _ExpMestMedicines.GroupBy(p => p.TDL_MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                                foreach (var expMedicine in expMestGroup)
                                {
                                    var check = rdo._MedicineTypes.FirstOrDefault(p => p.ID == expMedicine[0].TDL_MEDICINE_TYPE_ID);
                                    if (check == null)
                                    {
                                        continue;
                                    }
                                    //if (check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO
                                    //    || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN // mới thêm
                                    //    )
                                    //{ }
                                    //else
                                    //    continue;

                                    var medicinegroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();
                                    var medicinegroup = medicinegroups != null ? medicinegroups.FirstOrDefault(o => o.ID == check.MEDICINE_GROUP_ID) : null;

                                    if (medicinegroup == null)
                                    {
                                        continue;
                                    }
                                    if (medicinegroup.IS_NUMBERED_TRACKING == 1)
                                    { }
                                    else
                                        continue;

                                    if (!_DicCountNumbers.ContainsKey(check.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                        ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                        ado.num = num;
                                        _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0] = new List<NumberDate>();
                                        _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                        //ado.num = _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Count + 1;
                                        var _DataNumber = _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                        if (_DataNumber != null)
                                        {
                                            var _DataNumbers = _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count;
                                        }
                                        else
                                        {
                                            var _DataNumbers = _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count + 1;
                                        }
                                        ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                        _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                    }
                                }
                            }
                        }
                        //  num++;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorDataPrint()
        {
            try
            {
                _ExpMestMetyReqADOCommons = new List<ExpMestMetyReqADO>();
                _Mps000429ADOs = new List<Mps000429ADO>();
                _Mps000429ExtADOs = new List<Mps000429ExtADO>();
                _ExpMestMetyReqADOs = new List<ExpMestMetyReqADO>();
                _ServiceCLSs = new List<ServiceCLS>();
                _Bloods = new List<ServiceCLS>();
                _ExamServices = new List<ServiceCLS>();
                _TTServices = new List<ServiceCLS>();
                _RemedyCountADOs = new List<RemedyCountADO>();
                _ExpMestMatyReqADOs = new List<ExpMestMatyReqADO>();
                _ServiceReqMetyADOs = new List<ServiceReqMetyADO>();
                _ServiceReqMatyADOs = new List<ServiceReqMatyADO>();
                _ImpMestMedicineADOs = new List<ImpMestMedicineADO>();
                _ImpMestMaterialADOs = new List<ImpMestMaterialADO>();
                _MedicalInstructions = new List<MedicalInstruction>();

                _ExpMestMetyReqADOsV2 = new List<ExpMestMetyReqADO>();

                if (rdo._Trackings != null && rdo._Trackings.Count > 0)
                {
                    foreach (var itemTracking in rdo._Trackings)
                    {
                        this.IS_SHOW = "X";
                        this.sum_medi_mate = 0;
                        _tracking = new HIS_TRACKING();
                        _tracking = itemTracking;
                        keyTamThan = new SingleKeyTracking();

                        #region Thông tin chẩn đoán tờ điều trị
                        string icd_Name = "";
                        //Review
                        if (!string.IsNullOrEmpty(itemTracking.ICD_CODE))
                        {
                            var icd = rdo._Icds.Where(p => p.ICD_CODE == itemTracking.ICD_CODE).FirstOrDefault();
                            if (icd != null)
                            {
                                if (!string.IsNullOrEmpty(itemTracking.ICD_NAME) && icd.ICD_NAME.Trim() != itemTracking.ICD_NAME.Trim())
                                {
                                    icd_Name = itemTracking.ICD_NAME;
                                }
                                else
                                    icd_Name = icd.ICD_NAME;
                            }
                        }

                        #endregion

                        #region TrackingTime
                        Mps000429ADO _service = new Mps000429ADO();

                        Mapper.CreateMap<HIS_TRACKING, Mps000429ADO>();
                        _service = Mapper.Map<HIS_TRACKING, Mps000429ADO>(itemTracking);
                        _service.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(itemTracking.TRACKING_TIME);
                        _service.TRACKING_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemTracking.TRACKING_TIME);
                        _service.TRACKING_DATE_SEPARATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(itemTracking.TRACKING_TIME);
                        var User = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == itemTracking.CREATOR);

                        _service.TRACKING_USERNAME = User != null ? User.USERNAME : "";

                        keyTamThan.TRACKING_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemTracking.TRACKING_TIME);

                        if (!string.IsNullOrEmpty(itemTracking.MEDICAL_INSTRUCTION) && itemTracking.MEDICAL_INSTRUCTION.Length > 100)
                        {
                            var str = itemTracking.MEDICAL_INSTRUCTION.Split('\n');
                            foreach (var item in str)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    MedicalInstruction _Medical = new MedicalInstruction();
                                    _Medical.TRACKING_ID = _tracking.ID;
                                    _Medical.MEDICAL_INSTRUCTION += item;// +". ";

                                    _MedicalInstructions.Add(_Medical);
                                }
                            }
                        }
                        else
                        {
                            MedicalInstruction _Medical = new MedicalInstruction();
                            _Medical.TRACKING_ID = itemTracking.ID;
                            _Medical.MEDICAL_INSTRUCTION = itemTracking.MEDICAL_INSTRUCTION;
                            _MedicalInstructions.Add(_Medical);
                        }

                        if (rdo._Dhsts != null && rdo._Dhsts.Count > 0)
                        {
                            var dhst = rdo._Dhsts.OrderByDescending(o => o.MODIFY_TIME).FirstOrDefault(o => o.TRACKING_ID == itemTracking.ID);

                            if (dhst != null)
                            {
                                _service.BELLY = dhst.BELLY;
                                _service.BLOOD_PRESSURE_MAX = dhst.BLOOD_PRESSURE_MAX;
                                _service.BLOOD_PRESSURE_MIN = dhst.BLOOD_PRESSURE_MIN;
                                _service.BREATH_RATE = dhst.BREATH_RATE;
                                _service.CHEST = dhst.CHEST;
                                _service.HEIGHT = dhst.HEIGHT;
                                _service.PULSE = dhst.PULSE;
                                _service.TEMPERATURE = dhst.TEMPERATURE;
                                _service.VIR_BMI = dhst.VIR_BMI;
                                _service.VIR_BODY_SURFACE_AREA = dhst.VIR_BODY_SURFACE_AREA;
                                _service.WEIGHT = dhst.WEIGHT;
                                _service.CAPILLARY_BLOOD_GLUCOSE = dhst.CAPILLARY_BLOOD_GLUCOSE;
                                _service.SPO2 = dhst.SPO2;
                                _service.NOTE = dhst.NOTE;
                                if (rdo._Trackings != null && rdo._Trackings.Count == 1 && dhst.EXECUTE_TIME != null)
                                {
                                    this._EXECUE_TIME_DHST = dhst.EXECUTE_TIME ?? 0;
                                }
                            }
                        }
                        #endregion

                        #region ServiceReqIds
                        _ServiceReqs = new List<HIS_SERVICE_REQ>();
                        ServiceReq = new List<HIS_SERVICE_REQ>();
                        if (rdo._DicServiceReqs != null)
                        {
                            ServiceReq = rdo._DicServiceReqs.Values.ToList();
                            var ser = rdo._DicServiceReqs.Values.Where(p => p.TRACKING_ID == itemTracking.ID).ToList();
                            if (ser != null && ser.Count > 0)
                                _ServiceReqs = ser.ToList();
                        }
                        else
                        {
                            return;
                        }
                        Inventec.Common.Logging.LogSystem.Warn("ServiceReqIds: " + itemTracking.ID);
                        #endregion

                        CreateThread();

                        MediMateTH();

                        #region --- Tam Than ---
                        var rsCare = rdo._Cares.FirstOrDefault(p => p.TRACKING_ID == itemTracking.ID);
                        string care = "";
                        if (rsCare != null)
                        {
                            care = rsCare.NUTRITION;
                        }
                        var rsCareDetail = rdo._CareDetails.Where(p => p.TRACKING_ID == itemTracking.ID).ToList();
                        if (rsCareDetail != null && rsCareDetail.Count > 0)
                        {
                            foreach (var itemCareDetail in rsCareDetail)
                            {
                                if (care == "")
                                    keyTamThan.Y_LENH += " - " + itemCareDetail.CARE_TYPE_NAME + "\r\n";
                                else
                                    keyTamThan.Y_LENH += " - " + care + "/ " + itemCareDetail.CARE_TYPE_NAME + "\r\n";
                            }
                        }
                        #endregion

                        _SingleKeyTamThans.Add(keyTamThan);
                        _service.ICD_NAME_TRACKING = icd_Name;
                        _service.IS_SHOW_MEDICINE = this.IS_SHOW;
                        _service.SUM_MEDI_MATE = this.sum_medi_mate;
                        _Mps000429ADOs.Add(_service);

                        this.ProcessMedicineLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThread()
        {
            Thread threadMedicines = new Thread(Medicines);
            Thread threadMaterials = new Thread(Materials);
            Thread threadServices = new Thread(Services);
            // Thread threadServiceReqMetys = new Thread(ServiceReqMetys);
            //  Thread threadServiceReqMatys = new Thread(ServiceReqMatys);
            Thread threadBloos = new Thread(Bloods);

            try
            {
                threadMedicines.Start();
                threadMaterials.Start();
                threadServices.Start();
                //threadServiceReqMetys.Start();
                // threadServiceReqMatys.Start();
                threadBloos.Start();

                threadMedicines.Join();
                threadMaterials.Join();
                threadServices.Join();
                // threadServiceReqMetys.Join();
                // threadServiceReqMatys.Join();
                threadBloos.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicines.Abort();
                threadMaterials.Abort();
                threadServices.Abort();
                // threadServiceReqMetys.Abort();
                //threadServiceReqMatys.Abort();
                threadBloos.Abort();
            }
        }

        private void Medicines()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("Medicines() 1");
                //Gan mac dinh thang
                RemedyCountADO remedyCountAddNull = new RemedyCountADO();
                remedyCountAddNull.TRACKING_ID = _tracking.ID;
                remedyCountAddNull.EXP_MEST_ID = 0;
                remedyCountAddNull.REMEDY_COUNT = 0;
                _RemedyCountADOs.Add(remedyCountAddNull);
                Inventec.Common.Logging.LogSystem.Warn("Medicines() 2");
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Medicines() 3");
                    List<ExpMestMetyReqADO> __ExpMestMetyReqADO_V2s = new List<ExpMestMetyReqADO>();
                    //1.Lay tu danh sach yeu cau
                    foreach (var itemByServiceReq in _ServiceReqs)
                    {
                        var iiiiintruc = itemByServiceReq.INTRUCTION_DATE;
                        if (itemByServiceReq.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                            continue;
                        long SerId = itemByServiceReq.ID;
                        var itemServiceReq = rdo._DicServiceReqs.ContainsKey(SerId) ? rdo._DicServiceReqs[SerId] : null;
                        var itemExpMest = rdo._DicHisExpMests.ContainsKey(SerId) ? rdo._DicHisExpMests[SerId] : null;
                        if (itemServiceReq == null || itemExpMest == null)
                            continue;

                        List<HIS_EXP_MEST_MEDICINE> _expMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                        if (rdo._DicExpMestMedicines.ContainsKey(itemExpMest.ID))
                        {
                            _expMestMedicines = rdo._DicExpMestMedicines[itemExpMest.ID].ToList();
                        }
                        if (_expMestMedicines != null && _expMestMedicines.Count > 0)
                        {
                            _expMestMedicines = _expMestMedicines.OrderBy(p => p.NUM_ORDER).ToList();
                        }
                        else
                            continue;
                        if (rdo._WorkPlaceSDO != null && !rdo._WorkPlaceSDO.IsShowMedicineLine)
                        {
                            RemedyCountADO remedyCount = new RemedyCountADO();
                            remedyCount.TRACKING_ID = _tracking.ID;
                            remedyCount.EXP_MEST_ID = itemExpMest.ID;//EXP_MEST_ID
                            remedyCount.REMEDY_COUNT = itemServiceReq.REMEDY_COUNT;
                            remedyCount.PRESCRIPTION_TYPE_ID = itemServiceReq.PRESCRIPTION_TYPE_ID ?? 0;
                            var tutorial = _expMestMedicines.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.TUTORIAL) && itemExpMest.ID == o.EXP_MEST_ID);
                            remedyCount.TUTORIAL_REMEDY = tutorial != null ? tutorial.TUTORIAL : "";

                            long userTimeTo = _expMestMedicines.Max(p => p.USE_TIME_TO ?? 0);
                            long intructionTime = itemServiceReq.INTRUCTION_TIME;
                            if (userTimeTo > 0 && intructionTime > 0)
                            {
                                DateTime dtIntructionTime = System.DateTime.ParseExact(intructionTime.ToString(), "yyyyMMddHHmmss",
                                      System.Globalization.CultureInfo.InvariantCulture);
                                DateTime dtUserTimeTo = System.DateTime.ParseExact(userTimeTo.ToString(), "yyyyMMddHHmmss",
                                                       System.Globalization.CultureInfo.InvariantCulture);
                                TimeSpan ts = new TimeSpan();
                                ts = (TimeSpan)(dtUserTimeTo - dtIntructionTime);
                                if (ts != null && ts.Days >= 0)
                                {
                                    remedyCount.DAY_COUNT = ts.Days + 1;
                                }
                            }
                            _RemedyCountADOs.Add(remedyCount);
                        }


                        var _expMestMetyReqGroups = _expMestMedicines.GroupBy(p => p.TDL_MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                        this.sum_medi_mate += _expMestMetyReqGroups.Count();


                        int d = 0;


                        Inventec.Common.Logging.LogSystem.Info("_expMestMetyReqGroups: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _expMestMetyReqGroups), _expMestMetyReqGroups));
                        foreach (var itemExpMestMetyReq in _expMestMetyReqGroups)
                        {
                            ExpMestMetyReqADO group = new ExpMestMetyReqADO(itemExpMestMetyReq[0]);
                            group.TRACKING_ID = _tracking.ID;

                            group.AMOUNT = itemExpMestMetyReq.Sum(p => p.AMOUNT);
                            var medicineTypeName = rdo._MedicineTypes.FirstOrDefault(p => p.ID == group.TDL_MEDICINE_TYPE_ID);
                            if (medicineTypeName != null)
                            {
                                group.MEDICINE_TYPE_NAME = medicineTypeName.MEDICINE_TYPE_NAME;
                                group.MEDICINE_TYPE_CODE = medicineTypeName.MEDICINE_TYPE_CODE;
                                if (rdo._WorkPlaceSDO != null && rdo._WorkPlaceSDO.IsShowMedicineLine)
                                {
                                    group.MEDICINE_LINE_ID = medicineTypeName.MEDICINE_LINE_ID != null ? medicineTypeName.MEDICINE_LINE_ID : 0;
                                    group.MEDICINE_LINE_CODE = medicineTypeName.MEDICINE_LINE_CODE;
                                    group.MEDICINE_LINE_NAME = string.IsNullOrEmpty(medicineTypeName.MEDICINE_LINE_NAME) ? "Không xác định" : medicineTypeName.MEDICINE_LINE_NAME;
                                    group.MEDICINE_LINE_NUM_ORDER = medicineTypeName.MEDICINE_LINE_NUM_ORDER;
                                }

                                group.SERVICE_UNIT_CODE = medicineTypeName.SERVICE_UNIT_CODE;
                                group.SERVICE_UNIT_NAME = medicineTypeName.SERVICE_UNIT_NAME;
                                group.MEDICINE_USE_FORM_NAME = medicineTypeName.MEDICINE_USE_FORM_NAME;
                                group.CONCENTRA = medicineTypeName.CONCENTRA;

                                group.INTRUCTION_DATE = itemServiceReq.INTRUCTION_DATE;
                                group.INTRUCTION_TIME = itemServiceReq.INTRUCTION_TIME;

                                var userForm = rdo._MedicineUseForms.FirstOrDefault(p => p.ID == medicineTypeName.MEDICINE_USE_FORM_ID && p.IS_ACTIVE == 1);
                                group.MEDICINE_USE_FORM_ID = medicineTypeName.MEDICINE_USE_FORM_ID;
                                group.NUM_ORDER_BY_USE_FORM = userForm != null ? userForm.NUM_ORDER ?? 0 : 0;

                                #region ---- MEDICINE_GROUP_ID------

                                if (medicineTypeName.IS_TREATMENT_DAY_COUNT == 1 && _DicCountNumberByTypes != null && _DicCountNumberByTypes.Count > 0
                                   && _DicCountNumberByTypes.ContainsKey(medicineTypeName.ID))
                                {
                                    var _DataNumberByType = _DicCountNumberByTypes[medicineTypeName.ID].FirstOrDefault(p => p.INTRUCTION_DATE == itemServiceReq.INTRUCTION_DATE && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                    Inventec.Common.Logging.LogSystem.Debug("medicineTypeName.IS_TREATMENT_DAY_COUNT: " + medicineTypeName.IS_TREATMENT_DAY_COUNT);
                                    if (_DataNumberByType != null)
                                    {
                                        group.NUMBER_BY_TYPE = (long)_DataNumberByType.num;
                                    }
                                }

                                if (medicineTypeName.MEDICINE_GROUP_ID > 0)
                                {
                                    group.GROUP_BHYT = "X";

                                    group.MEDICINE_GROUP_NUM_ORDER = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == medicineTypeName.MEDICINE_GROUP_ID).NUM_ORDER;


                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _DicCountNumbers), _DicCountNumbers)
                                        + "____medicineTypeName.MEDICINE_GROUP_ID=" + medicineTypeName.MEDICINE_GROUP_ID);
                                    if (_DicCountNumbers != null && _DicCountNumbers.Count > 0
                                        && _DicCountNumbers.ContainsKey(medicineTypeName.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        var _DataNumber = _DicCountNumbers[medicineTypeName.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == itemServiceReq.INTRUCTION_DATE && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                        if (_DataNumber != null)
                                        {
                                            //group.Num_Order_by_group = _DataNumber.Num_Order;
                                            //group.Num_Order_by_group dùng để săp xếp thuốc Theo loại thuốc: Thuốc phóng xạ >> Thuốc độc >> Thuốc hướng thần, gây nghiện, corticoid, kháng sinh (sắp xếp theo đường dùng quy định ở TT23)>> Thuốc thường (sắp xếp theo đường dùng quy định ở TT23)
                                            group.NUMBER_H_N = (long)_DataNumber.num;

                                            //if (rdo._WorkPlaceSDO != null && rdo._WorkPlaceSDO.IsOrderByType)
                                            //{
                                            if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)//GayNghien..2.4
                                            {
                                                group.TYPE_ID = 4;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 3;
                                            }
                                            else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)//KhangSinh..2.1
                                            {
                                                group.TYPE_ID = 1;
                                                group.NUMBER_INTRUCTION_DATE = "(" + _DataNumber.num + ")";
                                                group.Num_Order_by_group = 3;
                                            }
                                            else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//HuongThan..2.5
                                            {
                                                group.TYPE_ID = 5;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 3;
                                            }
                                            else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)//corticod..2.2
                                            {
                                                group.TYPE_ID = 2;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 3;
                                            }
                                            else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)//Lao..2.6
                                            {
                                                group.TYPE_ID = 6;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 4;
                                            }
                                            else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX)//phongxa..2.8
                                            {
                                                group.TYPE_ID = 8;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 1;
                                            }
                                            else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC)//doc..2.9
                                            {
                                                group.TYPE_ID = 9;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 2;
                                            }
                                            else//ThuocKhac..Cho xg cuoi cung
                                            {
                                                group.TYPE_ID = 7;
                                                group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                group.Num_Order_by_group = 5;
                                            }
                                            //}
                                        }
                                    }
                                }
                                else//ThuocThuogn..2.3
                                {
                                    group.TYPE_ID = 3;
                                    group.Num_Order_by_group = 1000000000;
                                    group.MEDICINE_GROUP_NUM_ORDER = -1000000000;
                                }
                                #endregion
                            }

                            if (itemServiceReq.REMEDY_COUNT != null && itemServiceReq.REMEDY_COUNT > 0)
                            {
                                d++;
                                group.REMEDY_COUNT = itemServiceReq.REMEDY_COUNT;
                                group.Amount_By_Remedy_Count = Inventec.Common.Number.Convert.NumberToNumberRoundAuto((group.AMOUNT / group.REMEDY_COUNT) ?? 0, 2);
                                if (d == _expMestMetyReqGroups.Count)
                                {
                                    group.TUTORIAL_REMEDY = group.TUTORIAL;
                                }
                            }
                            this.IS_SHOW = "";

                            decimal totalNewAmount = group.AMOUNT - group.AMOUNT;
                            string amount_str = group.AMOUNT.ToString();
                            if ((Math.Ceiling(totalNewAmount) - totalNewAmount) == 0)
                            {
                                long _amount = Convert.ToInt64(group.AMOUNT);
                                if (_amount > 0 && _amount < 10)
                                {
                                    amount_str = "0" + _amount;
                                }
                            }
                            group.AMOUNT_STR = amount_str;
                            _ExpMestMetyReqADOs.Add(group);
                            __ExpMestMetyReqADO_V2s.Add(group);


                            keyTamThan.Y_LENH += " - " + group.MEDICINE_TYPE_NAME + " x" + amount_str + " " + group.SERVICE_UNIT_NAME + ", " + group.MEDICINE_USE_FORM_NAME + "\r\n" + "     " + group.TUTORIAL + "\r\n";
                        }
                    }

                    if (rdo._WorkPlaceSDO != null && rdo._WorkPlaceSDO.IsShowMedicineLine)
                    {
                        var medicineGroups = __ExpMestMetyReqADO_V2s.GroupBy(o => new { o.MEDICINE_LINE_ID, o.TDL_MEDICINE_TYPE_ID });//Sua lai gom theo dong thuoc và laoi thuoc de xem so ngay y lenh
                        foreach (var g in medicineGroups)
                        {
                            ExpMestMetyReqADO adoV2 = new ExpMestMetyReqADO();
                            adoV2 = g.FirstOrDefault();

                            adoV2.AMOUNT = g.Sum(p => p.AMOUNT);
                            string descriptionV2 = "Y lệnh ngày ";
                            var intr = g.OrderBy(p => p.INTRUCTION_DATE).Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                            adoV2.INTRUCTION_DAYS = intr.Count();
                            int ddd = 0;
                            foreach (var itemV2 in intr)
                            {
                                ddd++;
                                string date = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemV2);
                                descriptionV2 += date.Substring(0, 5) + (ddd == intr.Count() ? "" : ", ");
                            }
                            adoV2.DESCRIPTION_INTRUCTION_DAYS = descriptionV2;



                            _ExpMestMetyReqADOsV2.Add(adoV2);
                        }




                        #region ---- new -----
                        var medicineGroupsV2 = __ExpMestMetyReqADO_V2s.GroupBy(o => new { o.MEDICINE_LINE_ID, o.EXP_MEST_ID, o.TDL_SERVICE_REQ_ID });
                        foreach (var g in medicineGroupsV2)
                        {
                            RemedyCountADO remedyCount = new RemedyCountADO();
                            remedyCount.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                            remedyCount.TRACKING_ID = _tracking.ID;
                            remedyCount.EXP_MEST_ID = g.FirstOrDefault().EXP_MEST_ID ?? 0;
                            remedyCount.REMEDY_COUNT = g.FirstOrDefault().REMEDY_COUNT;
                            var tutorial = rdo._DicExpMestMedicines[g.FirstOrDefault().EXP_MEST_ID ?? 0].FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.TUTORIAL));
                            remedyCount.TUTORIAL_REMEDY = tutorial != null ? tutorial.TUTORIAL : "";

                            var listExpMesMedicines = rdo._DicExpMestMedicines[g.FirstOrDefault().EXP_MEST_ID ?? 0];
                            long userTimeTo = listExpMesMedicines != null && listExpMesMedicines.Count > 0 ? listExpMesMedicines.Max(p => p.USE_TIME_TO ?? 0) : 0;
                            var serviceReqTime = _ServiceReqs.FirstOrDefault(p => p.ID == g.FirstOrDefault().TDL_SERVICE_REQ_ID);
                            long intructionTime = serviceReqTime != null ? serviceReqTime.INTRUCTION_TIME : 0;
                            if (userTimeTo > 0 && intructionTime > 0)
                            {
                                DateTime dtIntructionTime = System.DateTime.ParseExact(intructionTime.ToString(), "yyyyMMddHHmmss",
                                      System.Globalization.CultureInfo.InvariantCulture);
                                DateTime dtUserTimeTo = System.DateTime.ParseExact(userTimeTo.ToString(), "yyyyMMddHHmmss",
                                                       System.Globalization.CultureInfo.InvariantCulture);
                                TimeSpan ts = new TimeSpan();
                                ts = (TimeSpan)(dtUserTimeTo - dtIntructionTime);
                                if (ts != null && ts.Days >= 0)
                                {
                                    remedyCount.DAY_COUNT = ts.Days + 1;
                                }
                            }
                            _RemedyCountADOs.Add(remedyCount);
                        }
                        #endregion

                    }


                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => __ExpMestMetyReqADO_V2s), __ExpMestMetyReqADO_V2s));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOsV2), _ExpMestMetyReqADOsV2));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tiennv
        /// </summary>
        private void ProcessMedicineLine()
        {
            try
            {
                this.MedicineLineADOs = new List<MedicineLineADO>();
                if (this._ExpMestMetyReqADOs != null && this._ExpMestMetyReqADOs.Count > 0)
                {
                    var groups = (from a in this._ExpMestMetyReqADOs
                                  join c in this._RemedyCountADOs on a.EXP_MEST_ID equals c.EXP_MEST_ID
                                  select new
                                  {
                                      MEDICINE_TYPE_ID = a.TDL_MEDICINE_TYPE_ID,
                                      MEDICINE_LINE_ID = a.MEDICINE_LINE_ID,
                                      MEDICINE_LINE_NAME = a.MEDICINE_LINE_NAME,
                                      MEDICINE_LINE_CODE = a.MEDICINE_LINE_CODE,
                                      MEDICINE_LINE_NUM_ORDER = a.MEDICINE_LINE_NUM_ORDER,
                                      EXP_MEST_ID = a.EXP_MEST_ID,
                                      TRACKING_ID = c.TRACKING_ID
                                  })
                                  .OrderBy(o => o.MEDICINE_LINE_NUM_ORDER ?? 999999999)
                                    .GroupBy(o => new { o.MEDICINE_LINE_ID, o.TRACKING_ID });
                    foreach (var g in groups)
                    {
                        MedicineLineADO m = new MedicineLineADO();
                        m.EXP_MEST_ID = g.First().EXP_MEST_ID;
                        m.ID = g.First().MEDICINE_LINE_ID;
                        m.MEDICINE_LINE_NAME = g.First().MEDICINE_LINE_NAME;
                        m.TRACKING_ID = g.First().TRACKING_ID;
                        this.MedicineLineADOs.Add(m);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ServiceReqMetys()
        {
            try
            {
                if (_ServiceReqs != null && _ServiceReqs.Count > 0 && rdo._DicServiceReqMetys != null && rdo._DicServiceReqMetys.Count > 0)
                {
                    List<HIS_SERVICE_REQ_METY> dataMety = new List<HIS_SERVICE_REQ_METY>();

                    foreach (var itemByServiceReq in _ServiceReqs)
                    {
                        //if (itemByServiceReq.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                        //    continue;
                        if (itemByServiceReq.IS_NOT_SHOW_OUT_MEDI_TRACKING == 1)
                            continue;
                        long item = itemByServiceReq.ID;
                        if (rdo._DicServiceReqMetys.ContainsKey(item))
                        {
                            dataMety.AddRange(rdo._DicServiceReqMetys[item]);
                        }
                    }
                    //  remedyCountAddNull.TRACKING_ID = _tracking.ID;//
                    if (dataMety != null && dataMety.Count > 0)
                    {
                        _ServiceReqMetyADOs.AddRange((from r in dataMety select new ServiceReqMetyADO(r, _tracking.ID)).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ServiceReqMatys()
        {
            try
            {
                if (_ServiceReqs != null
                    && _ServiceReqs.Count > 0
                    && rdo._DicServiceReqMatys != null
                    && rdo._DicServiceReqMatys.Count > 0)
                {
                    List<HIS_SERVICE_REQ_MATY> dataMaty = new List<HIS_SERVICE_REQ_MATY>();

                    foreach (var itemByServiceReq in _ServiceReqs)
                    {
                        if (itemByServiceReq.IS_NOT_SHOW_OUT_MATE_TRACKING == 1)
                            continue;
                        long item = itemByServiceReq.ID;
                        if (rdo._DicServiceReqMatys.ContainsKey(item))
                        {
                            dataMaty.AddRange(rdo._DicServiceReqMatys[item]);
                        }
                    }
                    if (dataMaty != null && dataMaty.Count > 0)
                    {
                        _ServiceReqMatyADOs.AddRange((from r in dataMaty select new ServiceReqMatyADO(r, this._tracking.ID)).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Materials()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("Materials() 1");
                Inventec.Common.Logging.LogSystem.Info("_ServiceReqs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ServiceReqs), _ServiceReqs));
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Materials() 2");
                    //1.Lay tu danh sach yeu cau

                    foreach (var itemByServiceReq in _ServiceReqs)
                    {
                        if (itemByServiceReq.IS_NOT_SHOW_MATERIAL_TRACKING == 1)
                            continue;
                        long SerId = itemByServiceReq.ID;
                        var itemServiceReq = rdo._DicServiceReqs.ContainsKey(SerId) ? rdo._DicServiceReqs[SerId] : null;
                        var itemExpMest = rdo._DicHisExpMests.ContainsKey(SerId) ? rdo._DicHisExpMests[SerId] : null;
                        if (itemServiceReq == null || itemExpMest == null)
                            continue;
                        //VT
                        Inventec.Common.Logging.LogSystem.Info("rdo._DicExpMestMaterials: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._DicExpMestMaterials), rdo._DicExpMestMaterials));
                        if (rdo._DicExpMestMaterials != null && rdo._DicExpMestMaterials.Count > 0)
                        {
                            List<HIS_EXP_MEST_MATERIAL> _expMestMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                            if (rdo._DicExpMestMaterials.ContainsKey(itemExpMest.ID))
                            {
                                _expMestMaterials = rdo._DicExpMestMaterials[itemExpMest.ID].OrderBy(p => p.NUM_ORDER).ToList();
                            }
                            if (_expMestMaterials != null && _expMestMaterials.Count > 0)
                            {
                                _expMestMaterials = _expMestMaterials.OrderBy(p => p.NUM_ORDER).ToList();
                                this.sum_medi_mate += _expMestMaterials.Count();
                            }
                            foreach (var itemMate in _expMestMaterials)
                            {
                                ExpMestMatyReqADO ado = new ExpMestMatyReqADO(itemMate);
                                ado.TRACKING_ID = _tracking.ID;
                                var materialTypeName = rdo._MaterialTypes.FirstOrDefault(p => p.ID == ado.TDL_MATERIAL_TYPE_ID);
                                if (materialTypeName != null)
                                {
                                    ado.MATERIAL_TYPE_NAME = materialTypeName.MATERIAL_TYPE_NAME;
                                    ado.MATERIAL_TYPE_CODE = materialTypeName.MATERIAL_TYPE_CODE;
                                    ado.SERVICE_UNIT_CODE = materialTypeName.SERVICE_UNIT_CODE;
                                    ado.SERVICE_UNIT_NAME = materialTypeName.SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = materialTypeName.CONCENTRA;
                                }
                                decimal totalNewAmount = ado.AMOUNT - ado.AMOUNT;
                                string amount_str = ado.AMOUNT.ToString();
                                if ((Math.Ceiling(totalNewAmount) - totalNewAmount) == 0)
                                {
                                    long _amount = Convert.ToInt64(ado.AMOUNT);
                                    if (_amount > 0 && _amount < 10)
                                    {
                                        amount_str = "0" + _amount;
                                    }
                                }
                                ado.AMOUNT_STR = amount_str;
                                _ExpMestMatyReqADOs.Add(ado);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Services()
        {
            try
            {
                List<HIS_SERE_SERV> sereServCLSByTracking = new List<HIS_SERE_SERV>();
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    foreach (var item in _ServiceReqs)
                    {
                        if (rdo._DicSereServs.ContainsKey(item.ID))
                        {
                            sereServCLSByTracking.AddRange(
                                rdo._DicSereServs[item.ID].Where(p =>
                                    p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                                    && (!p.IS_NO_EXECUTE.HasValue || p.IS_NO_EXECUTE != 1)).ToList());
                        }
                    }
                    if (sereServCLSByTracking != null && sereServCLSByTracking.Count > 0)
                    {
                    }
                    else
                    {
                        ServiceCLS group = new ServiceCLS();
                        group.TRACKING_ID = _tracking.ID;
                        _ServiceCLSs.Add(group);
                        return;
                    }
                    var CLSGroup = sereServCLSByTracking.GroupBy(p => p.TDL_SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();
                    foreach (var itemGroups in CLSGroup)
                    {
                        string serviceName = "";
                        var serviceType = rdo._ServiceTypes.FirstOrDefault(p => p.ID == itemGroups[0].TDL_SERVICE_TYPE_ID);
                        //var skip = 0;
                        //foreach (var item in itemGroups)
                        //{
                        //    //Moi dich vu 1 dong
                        //    ServiceCLS group = new ServiceCLS(itemGroups[0]);
                        //    if (skip == 0)
                        //        group.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                        //    group.TRACKING_ID = _tracking.ID;

                        //    group.SERVICE_NAME = item.TDL_SERVICE_NAME + "; ";
                        //    serviceName += item.TDL_SERVICE_NAME + "; ";
                        //    skip++;
                        //    _ServiceCLSs.Add(group);
                        //}
                        ServiceCLS group = new ServiceCLS(itemGroups[0]);
                        group.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";
                        foreach (var item in itemGroups)
                        {
                            var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                            {
                                group.SERVICE_NAME += item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                group.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                            }
                            else
                            {
                                group.SERVICE_NAME += item.TDL_SERVICE_NAME + "; ";
                            }
                            // group.SERVICE_NAME += item.TDL_SERVICE_NAME + "; ";
                            group.TRACKING_ID = _tracking.ID;
                            serviceName += item.TDL_SERVICE_NAME + "; ";
                        }
                        _ServiceCLSs.Add(group);
                        keyTamThan.Y_LENH += " -" + (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + " :" + serviceName + "\r\n";
                    }
                }
                else
                {
                    ServiceCLS group = new ServiceCLS();
                    group.TRACKING_ID = _tracking.ID;
                    _ServiceCLSs.Add(group);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Bloods()
        {
            try
            {
                ServiceReqMetys();
                ServiceReqMatys();
                List<HIS_SERE_SERV> bloodByTracking = new List<HIS_SERE_SERV>();
                List<HIS_SERE_SERV> examByTracking = new List<HIS_SERE_SERV>();
                List<HIS_SERE_SERV> TTByTracking = new List<HIS_SERE_SERV>();
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    foreach (var item in _ServiceReqs)
                    {
                        if (rdo._DicSereServs.ContainsKey(item.ID))
                        {
                            bloodByTracking.AddRange(
                                rdo._DicSereServs[item.ID].Where(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList());
                            examByTracking.AddRange(
                                rdo._DicSereServs[item.ID].Where(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList());
                            TTByTracking.AddRange(
                                rdo._DicSereServs[item.ID].Where(p => p.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT).ToList());
                        }
                    }
                    if (bloodByTracking != null && bloodByTracking.Count > 0)
                    {
                        var bloodGroups = bloodByTracking.GroupBy(p => p.BLOOD_ID).Select(x => x.ToList()).ToList();
                        foreach (var itemGroups in bloodGroups)
                        {
                            ServiceCLS group = new ServiceCLS(itemGroups[0]);
                            group.AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                            group.TRACKING_ID = _tracking.ID;
                            _Bloods.Add(group);
                        }
                    }

                    if (examByTracking != null && examByTracking.Count > 0)
                    {
                        var examGroups = examByTracking.GroupBy(p => p.SERVICE_ID).Select(x => x.ToList()).ToList();
                        foreach (var itemGroups in examGroups)
                        {
                            ServiceCLS group = new ServiceCLS(itemGroups[0]);
                            group.AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                            group.TRACKING_ID = _tracking.ID;
                            _ExamServices.Add(group);
                        }
                    }
                    if (TTByTracking != null && TTByTracking.Count > 0)
                    {
                        foreach (var item in TTByTracking)
                        {
                            ServiceCLS group = new ServiceCLS(item);
                            group.AMOUNT = item.AMOUNT;
                            group.TRACKING_ID = _tracking.ID;
                            var intruc = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                            if (intruc != null)
                            {
                                group.INSTRUCTION_NOTE = intruc.INSTRUCTION_NOTE;
                            }
                            _TTServices.Add(group);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MediMateTH()
        {
            try
            {
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    var expMestss = rdo._DicHisExpMests.Where(p => _ServiceReqs.Select(o => o.ID).ToList().Contains(p.Key)).Select(p => p.Value).ToList();
                    if (expMestss != null && expMestss.Count > 0 && rdo._ImpMests_input != null && rdo._ImpMests_input.Count > 0)
                    {
                        var impMestss = rdo._ImpMests_input.Where(p => expMestss.Select(o => o.ID).Contains(p.MOBA_EXP_MEST_ID ?? 0)).ToList();
                        if (impMestss != null && impMestss.Count > 0)
                        {
                            List<long> _impMestIds = impMestss.Select(p => p.ID).ToList();

                            //THUOC
                            if (rdo._ImpMestMedis != null && rdo._ImpMestMedis.Count > 0)
                            {
                                var medicines = rdo._ImpMestMedis.Where(p => _impMestIds.Contains(p.IMP_MEST_ID)).ToList();
                                if (medicines != null && medicines.Count > 0)
                                {
                                    var dataGroups = medicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                                    foreach (var item in dataGroups)
                                    {
                                        ImpMestMedicineADO ado = new ImpMestMedicineADO(item[0]);
                                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                                        ado.TRACKING_ID = this._tracking.ID;
                                        this._ImpMestMedicineADOs.Add(ado);
                                    }
                                }
                            }

                            //VT
                            if (rdo._ImpMestMates != null && rdo._ImpMestMates.Count > 0)
                            {
                                var materials = rdo._ImpMestMates.Where(p => _impMestIds.Contains(p.IMP_MEST_ID)).ToList();
                                if (materials != null && materials.Count > 0)
                                {
                                    var dataGroups = materials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                                    foreach (var item in dataGroups)
                                    {
                                        ImpMestMaterialADO ado = new ImpMestMaterialADO(item[0]);
                                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                                        ado.TRACKING_ID = this._tracking.ID;
                                        this._ImpMestMaterialADOs.Add(ado);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        internal void SetBarcodeKey()
        {
            try
            {
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Patient.PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000429ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                //Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                //barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                //barcodeTreatment.Width = 120;
                //barcodeTreatment.Height = 40;
                //barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                //barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                //barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                //barcodeTreatment.IncludeLabel = true;

                //dicImage.Add(Mps000429ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Mps000429 ------ ProcessData----1");

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                CheckSTTByMedicineGroup();

                CheckIntructionDate_GN_HT();

                ProcessorDataPrint();

                SetBarcodeKey();

                SetSingleKey();

                if (_RemedyCountADOs != null && _RemedyCountADOs.Count > 0)
                {
                    _RemedyCountADOs = _RemedyCountADOs.OrderBy(o => o.PRESCRIPTION_TYPE_ID).ToList();
                }

                if (_ExpMestMetyReqADOsV2 != null
                    && _ExpMestMetyReqADOsV2.Count > 0
                    && rdo._WorkPlaceSDO.IsShowMedicineLine)
                {
                    if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                    {
                        List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                        this._ExpMestMetyReqADOsV2 = this._ExpMestMetyReqADOsV2.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                        var dataGroups = this._ExpMestMetyReqADOsV2.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                            _dataNews1.AddRange(dtGroups);
                        }

                        _ExpMestMetyReqADOCommons = _dataNews1;
                        //objectTag.AddObjectData(store, "Medicines", _dataNews);
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOsV2.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();

                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                    {
                        this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOsV2.OrderBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                    {
                        this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOsV2.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();

                    }
                    //objectTag.AddObjectData(store, "Medicines", this._ExpMestMetyReqADOsV2);
                }
                else if (this._ExpMestMetyReqADOs != null
                    && this._ExpMestMetyReqADOs.Count > 0
                    && rdo._WorkPlaceSDO != null)
                {
                    if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                    {
                        List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                        this._ExpMestMetyReqADOs = this._ExpMestMetyReqADOs.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                        var dataGroups = this._ExpMestMetyReqADOs.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                            _dataNews1.AddRange(dtGroups);
                        }

                        this._ExpMestMetyReqADOCommons = _dataNews1;
                        //objectTag.AddObjectData(store, "Medicines", _dataNews);
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOs.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();

                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                    {
                        this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOs.OrderBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                        //objectTag.AddObjectData(store, "Medicines", this._ExpMestMetyReqADOs);
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                    {
                        //List<ExpMestMetyReqADO> _dataNews3 = new List<ExpMestMetyReqADO>();

                        //var dtGroups = this._ExpMestMetyReqADOs.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ToList();
                        //_dataNews3.AddRange(dtGroups);

                        //_ExpMestMetyReqADOCommons = _dataNews3;

                        this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOs.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                    }
                }
                else
                {
                    this._ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOs.OrderBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                }

                Inventec.Common.Logging.LogSystem.Info("_ExpMestMetyReqADOs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOs), _ExpMestMetyReqADOs));

                Inventec.Common.Logging.LogSystem.Info("_ExpMestMetyReqADOCommons: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOCommons), _ExpMestMetyReqADOCommons));

                result = (this.templateType == ProcessorBase.PrintConfig.TemplateType.Excel) ? ProcessDataExcel() : ((this.templateType == ProcessorBase.PrintConfig.TemplateType.Word) ? ProcessDataWord() : ProcessDataXtraReport());

                Inventec.Common.Logging.LogSystem.Debug("Mps000429 ------ ProcessData----2");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private bool ProcessDataExcel()
        {
            bool success = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);

                //var MedicinesData = _ExpMestMetyReqADOCommons.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ToList();
                //objectTag.AddObjectData(store, "Medicines", MedicinesData ?? new List<ExpMestMetyReqADO>());

                objectTag.AddObjectData(store, "Medicines", _ExpMestMetyReqADOCommons ?? new List<ExpMestMetyReqADO>());

                objectTag.AddObjectData(store, "TrackingADOs", this._Mps000429ADOs);
                objectTag.AddObjectData(store, "RemedyCount", this._RemedyCountADOs);
                objectTag.AddObjectData(store, "Materials", this._ExpMestMatyReqADOs);
                objectTag.AddObjectData(store, "ServiceReqMety", this._ServiceReqMetyADOs);
                objectTag.AddObjectData(store, "ServiceReqMaty", this._ServiceReqMatyADOs);
                objectTag.AddObjectData(store, "ServiceCLS", this._ServiceCLSs);
                objectTag.AddObjectData(store, "CARE", rdo._Cares);
                objectTag.AddObjectData(store, "CareDetail", rdo._CareDetails);
                objectTag.AddObjectData(store, "MedicalInstruction", this._MedicalInstructions);
                objectTag.AddObjectData(store, "Bloods", this._Bloods);
                objectTag.AddObjectData(store, "ExamServices", this._ExamServices);
                objectTag.AddObjectData(store, "TTServices", this._TTServices);
                //TH
                objectTag.AddObjectData(store, "ImpMestMedicine", this._ImpMestMedicineADOs);
                objectTag.AddObjectData(store, "ImpMestMaterial", this._ImpMestMaterialADOs);
                objectTag.AddObjectData(store, "MedicineLines", this.MedicineLineADOs);
                objectTag.SetUserFunction(store, "FuncMergeData", new CalculateMergerData() { _Mps000429ADOs = this._Mps000429ADOs });

                objectTag.AddRelationship(store, "TrackingADOs", "MedicineLines", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "MedicineLines", "Medicines", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLines", "RemedyCount", "ID", "MEDICINE_LINE_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "RemedyCount", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "RemedyCount", "Medicines", "EXP_MEST_ID", "EXP_MEST_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReqMety", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReqMaty", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLS", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "Bloods", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ExamServices", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "TTServices", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "CARE", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "CareDetail", "ID", "TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "Medicines", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "Materials", "ID", "TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "MedicalInstruction", "ID", "TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "ImpMestMedicine", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ImpMestMaterial", "ID", "TRACKING_ID");

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private bool ProcessDataXtraReport()
        {
            bool success = false;
            try
            {
                Inventec.Common.XtraReportExport.ProcessSingleTag singleTag = new Inventec.Common.XtraReportExport.ProcessSingleTag();
                Inventec.Common.XtraReportExport.ProcessObjectTag objectTag = new Inventec.Common.XtraReportExport.ProcessObjectTag();

                mps000429ADOExt.Mps000429ADOs = (from m in this._Mps000429ADOs select new Mps000429ExtADO(m)).ToList();

                int itemIndex = 0;

                var users = (mps000429ADOExt.Mps000429ADOs != null && mps000429ADOExt.Mps000429ADOs.Count > 0) ? BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>() : null;

                foreach (var item in mps000429ADOExt.Mps000429ADOs)
                {
                    if (!String.IsNullOrEmpty(item.PART_EXAM_EYE_TENSION_LEFT) && Inventec.Common.TypeConvert.Parse.ToDecimal(item.PART_EXAM_EYE_TENSION_LEFT) > 0)
                    {
                        item.PART_EXAM_EYE_TENSION_LEFT += "mmHg";
                    }
                    if (!String.IsNullOrEmpty(item.PART_EXAM_EYE_TENSION_RIGHT) && Inventec.Common.TypeConvert.Parse.ToDecimal(item.PART_EXAM_EYE_TENSION_RIGHT) > 0)
                    {
                        item.PART_EXAM_EYE_TENSION_RIGHT += "mmHg";
                    }
                    if (!String.IsNullOrEmpty(item.PART_EXAM_EYESIGHT_LEFT) && Inventec.Common.TypeConvert.Parse.ToDecimal(item.PART_EXAM_EYESIGHT_LEFT) > 0)
                    {
                        item.PART_EXAM_EYESIGHT_LEFT += "/10";
                    }
                    if (!String.IsNullOrEmpty(item.PART_EXAM_EYESIGHT_RIGHT) && Inventec.Common.TypeConvert.Parse.ToDecimal(item.PART_EXAM_EYESIGHT_RIGHT) > 0)
                    {
                        item.PART_EXAM_EYESIGHT_RIGHT += "/10";
                    }
                    if (!String.IsNullOrEmpty(item.PART_EXAM_EYESIGHT_GLASS_LEFT) && Inventec.Common.TypeConvert.Parse.ToDecimal(item.PART_EXAM_EYESIGHT_GLASS_LEFT) > 0)
                    {
                        item.PART_EXAM_EYESIGHT_GLASS_LEFT += "/10";
                    }
                    if (!String.IsNullOrEmpty(item.PART_EXAM_EYESIGHT_GLASS_RIGHT) && Inventec.Common.TypeConvert.Parse.ToDecimal(item.PART_EXAM_EYESIGHT_GLASS_RIGHT) > 0)
                    {
                        item.PART_EXAM_EYESIGHT_GLASS_RIGHT += "/10";
                    }

                    var medicines = _ExpMestMetyReqADOCommons != null && _ExpMestMetyReqADOCommons.Count > 0 ? _ExpMestMetyReqADOCommons.Where(o => o.TRACKING_ID == item.ID).ToList() : null;

                    if (medicines == null)
                    {
                        medicines = new List<ExpMestMetyReqADO>();
                    }

                    if (medicines.Count > 0 && rdo._WorkPlaceSDO != null && rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        medicines = medicines.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ToList();
                    }
                    else if (medicines.Count > 0 && rdo._WorkPlaceSDO.IsOrderByType == 2)
                    {
                        medicines = medicines.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ToList();
                    }
                    else if (medicines.Count > 0 && rdo._WorkPlaceSDO.IsOrderByType == 0)
                    {
                        medicines = medicines.OrderBy(p => p.NUM_ORDER).ToList();
                    }

                    //ServiceReqMety
                    var listServiceReqMetyADOs = this._ServiceReqMetyADOs != null && this._ServiceReqMetyADOs.Count > 0 ? this._ServiceReqMetyADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listServiceReqMetyADOs != null && listServiceReqMetyADOs.Count > 0)
                    {
                        foreach (var item1 in listServiceReqMetyADOs)
                        {
                            medicines.Insert(medicines.Count, new ExpMestMetyReqADO()
                            {
                                MEDICINE_TYPE_NAME = (item1.MEDICINE_TYPE_NAME + " (Tự túc)"),
                                AMOUNT = item1.AMOUNT,
                                MEDICINE_USE_FORM_NAME = item1.MEDICINE_USE_FORM_NAME,
                                MEDICINE_USE_FORM_ID = item1.MEDICINE_USE_FORM_ID,
                                TUTORIAL = item1.TUTORIAL,
                                NUM_ORDER = item1.NUM_ORDER,
                                SERVICE_UNIT_NAME = String.IsNullOrEmpty(item1.SERVICE_UNIT_NAME) ? item1.UNIT_NAME : item1.SERVICE_UNIT_NAME,
                                TRACKING_ID = item1.TRACKING_ID,
                                DAY_COUNT = item1.DAY_COUNT,
                            });
                        }
                    }

                    if (medicines != null && medicines.Count > 0)
                    {
                        item.MEDICINES___DATA = "";
                        item.MEDICINES___DATA1 = "";
                        item.MEDICINES___DATA2 = "";
                        item.MEDICINES___DATA3 = "";
                        int dem = 0;

                        Inventec.Common.Logging.LogSystem.Warn("dữ liệu medicines: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicines), medicines));

                        // var groupmedicines = medicines.ToList().OrderBy(o => o.Num_Order_by_group).GroupBy(o => o.TYPE_ID).ToList();

                        //foreach (var groupmedicine in groupmedicines)
                        //{
                        //  var mediInGroups = groupmedicine.ToList().OrderBy(o => o.NUM_ORDER_BY_USE_FORM).ToList();
                        var mediInGroups = medicines.OrderBy(o => o.Num_Order_by_group).ThenBy(o => o.NUM_ORDER_BY_USE_FORM).ToList();
                        foreach (var medi in mediInGroups)
                        {
                            string s1 = "- " + ((medi.NUMBER_H_N ?? 0) > 0 ? Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", medi.NUMBER_H_N) + "", FontStyle.Bold) : "");

                            if ((medi.REMEDY_COUNT ?? 0) == 0)
                            {
                                s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                            }
                            else
                            {
                                s1 += " " + medi.MEDICINE_TYPE_NAME;
                            }
                            s1 += " ";

                            if (medi.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT)
                            {
                                s1 += medi.CONCENTRA;
                            }

                            decimal? amount = 0;
                            string strAmount = "";
                            if ((medi.REMEDY_COUNT ?? 0) > 0)
                            {
                                amount = medi.Amount_By_Remedy_Count;
                            }
                            else if (medi.AMOUNT > 0)
                            {
                                amount = medi.AMOUNT;
                            }

                            if (medi.TYPE_ID == 4)//GayNghien..2.4
                            {
                                strAmount = Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0));
                            }
                            else if (medi.TYPE_ID == 5)//HuongThan..2.5
                            {
                                strAmount = (amount < 10 ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                            }
                            else
                                strAmount = Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0);

                            string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                            item.MEDICINES___DATA3 += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                            if ((medi.REMEDY_COUNT ?? 0) <= 0)
                            {
                                item.MEDICINES___DATA3 += medi.TUTORIAL;
                                if (dem < medicines.Count - 1)
                                {
                                    item.MEDICINES___DATA3 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }

                            dem++;
                        }
                        //}

                        foreach (var item1 in medicines)
                        {
                            item.MEDICINES___DATA1 += String.Format("{0}{1} {2} {3}", item1.NUMBER_H_N, item1.MEDICINE_TYPE_NAME, item1.CONCENTRA, ((item1.REMEDY_COUNT ?? 0) > 0 ? item1.Amount_By_Remedy_Count + " " + item1.SERVICE_UNIT_NAME : (item1.AMOUNT > 0 ? "   x" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) + " " + item1.SERVICE_UNIT_NAME + "   " + item1.MEDICINE_USE_FORM_NAME : "")));
                            if (item1.PRESCRIPTION_TYPE_ID != 2)
                            {
                                item.MEDICINES___DATA1 += item1.TUTORIAL;
                            }
                            if (dem < medicines.Count - 1)
                            {
                                item.MEDICINES___DATA1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }

                            item.MEDICINES___DATA2 += String.Format("{0}--{1} {2}", item1.NUMBER_H_N, item1.MEDICINE_TYPE_NAME, ((item1.REMEDY_COUNT ?? 0) > 0 ? item1.Amount_By_Remedy_Count + " (" + item1.REMEDY_COUNT + " thang) " + item1.SERVICE_UNIT_NAME : (item1.AMOUNT > 0 ? " x" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) + " " + item1.SERVICE_UNIT_NAME + "   " + item1.MEDICINE_USE_FORM_NAME : "")));
                            item.MEDICINES___DATA2 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            item.MEDICINES___DATA2 += "    " + item1.TUTORIAL;
                            if (dem < medicines.Count - 1)
                            {
                                item.MEDICINES___DATA2 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            //'<table><tr><td width="650" text-align="left" align="left"><b>- Parahasan Max 650mg</b></td></span><td text-align="right" align="right" width="150">1 Viên</td></tr></table>uống khi đau 01 viên'

                            string s1 = "- " + ((item1.NUMBER_H_N ?? 0) > 0 ? Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", item1.NUMBER_H_N) + "", FontStyle.Bold) : "");

                            if ((item1.REMEDY_COUNT ?? 0) == 0)
                            {
                                s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + item1.MEDICINE_TYPE_NAME, FontStyle.Bold);
                            }
                            else
                            {
                                s1 += " " + item1.MEDICINE_TYPE_NAME;
                            }
                            s1 += " ";

                            if (item1.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT)
                            {
                                s1 += item1.CONCENTRA;
                            }

                            decimal? amount = 0;
                            string strAmount = "";
                            if ((item1.REMEDY_COUNT ?? 0) > 0)
                            {
                                amount = item1.Amount_By_Remedy_Count;
                            }
                            else if (item1.AMOUNT > 0)
                            {
                                amount = item1.AMOUNT;
                            }

                            if (item1.TYPE_ID == 4)//GayNghien..2.4
                            {
                                strAmount = Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0));
                            }
                            else if (item1.TYPE_ID == 5)//HuongThan..2.5
                            {
                                strAmount = (amount < 10 ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                            }
                            else
                                strAmount = Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0);

                            string s2 = strAmount + " " + item1.SERVICE_UNIT_NAME;

                            item.MEDICINES___DATA += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                            if ((item1.REMEDY_COUNT ?? 0) <= 0)
                            {
                                item.MEDICINES___DATA += item1.TUTORIAL;
                                if (dem < medicines.Count - 1)
                                {
                                    item.MEDICINES___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }

                            dem++;
                        }
                    }



                    item.MOBA_IMP_MEST_MEDICINE__DATA = "";
                    item.MOBA_IMP_MEST_MATERIAL__DATA = "";

                    //Inventec.Common.Logging.LogSystem.Warn("dữ liệu mobaImpMests: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mobaImpMests), mobaImpMests));

                    if (rdo._MobaImpMests != null && rdo._MobaImpMests.Count > 0)
                    {
                        var serviceReqCodes = rdo._DicServiceReqs.Values.Where(p => p.TRACKING_ID == item.ID).Select(o => o.SERVICE_REQ_CODE).ToList();

                        //Inventec.Common.Logging.LogSystem.Warn("dữ liệu item.ID: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.ID), item.ID));
                        //Inventec.Common.Logging.LogSystem.Warn("dữ liệu serviceReqCodes: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqCodes), serviceReqCodes));

                        var _ImpMestMedicineADOByTrackings = rdo._MobaImpMests.Where(o => serviceReqCodes.Contains(o.TDL_SERVICE_REQ_CODE)).ToList();

                        //Inventec.Common.Logging.LogSystem.Warn("dữ liệu _ImpMestMedicineADOByTrackings: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ImpMestMedicineADOByTrackings), _ImpMestMedicineADOByTrackings));

                        if (_ImpMestMedicineADOByTrackings != null && _ImpMestMedicineADOByTrackings.Count > 0)
                        {

                            if (rdo._MobaImpMestMedicine != null && rdo._MobaImpMestMedicine.Count > 0)
                            {
                                var impMestMedicineAPIs = rdo._MobaImpMestMedicine.Where(o => _ImpMestMedicineADOByTrackings.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                                foreach (var impMestMedicine in impMestMedicineAPIs)
                                {
                                    string s1 = "", s2 = "";
                                    s1 = impMestMedicine.MEDICINE_TYPE_NAME;
                                    s2 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(impMestMedicine.AMOUNT) + " " + impMestMedicine.SERVICE_UNIT_NAME;
                                    item.MOBA_IMP_MEST_MEDICINE__DATA += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                }

                            }

                            if (rdo._MobaImpMestMaterial != null && rdo._MobaImpMestMaterial.Count > 0)
                            {
                                var impMestMaterialAPIs = rdo._MobaImpMestMaterial.Where(o => _ImpMestMedicineADOByTrackings.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                                foreach (var impMestMaterial in impMestMaterialAPIs)
                                {
                                    string s1 = "", s2 = "";
                                    s1 = impMestMaterial.MATERIAL_TYPE_NAME;
                                    s2 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(impMestMaterial.AMOUNT) + " " + impMestMaterial.SERVICE_UNIT_NAME;
                                    item.MOBA_IMP_MEST_MATERIAL__DATA += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                }
                            }
                        }
                    }

                    //Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MOBA_IMP_MEST_MEDICINE__DATA), item.MOBA_IMP_MEST_MEDICINE__DATA)
                    //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.MOBA_IMP_MEST_MATERIAL__DATA), item.MOBA_IMP_MEST_MATERIAL__DATA));


                    //- Lấy tất cả các đơn điều trị đã được chỉ định mà có ngày dùng lớn hơn hoặc bằng thời gian tờ điều trị:
                    //+ SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                    //+ INTRUCTION_DATE < TRACKING_DATE (từ tracking_time suy ra tracking_date)
                    //+ USE_TIME_TO (trong his_exp_mest_medicine) >= TRACKING_TIME
                    //+ SERVICE_REQ_STT_ID = 3
                    //- Nếu có thì đổ dữ liệu ra key với cấu trúc: Tên thuốc và hướng dẫn sử dụng (lấy từ TUTORIAL trong HIS_EXP_MEST_MEDICINE) hiển thị trên 2 dòng:
                    //vd:
                    //- Mỡ Vaselin
                    //Bôi da ngày 3 - 4 lần                 

                    item.PRE_MEDICINE = "";

                    var dtTrackingDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(item.TRACKING_TIME).Value;

                    Inventec.Common.Logging.LogSystem.Warn("dữ liệu dtTrackingDate: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dtTrackingDate), dtTrackingDate));

                    Inventec.Common.Logging.LogSystem.Warn("dữ liệu INTRUCTION_DATE: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.Common.TypeConvert.Parse.ToInt64(dtTrackingDate.ToString("yyyyMMdd") + "000000")), Inventec.Common.TypeConvert.Parse.ToInt64(dtTrackingDate.ToString("yyyyMMdd") + "000000")));

                    Inventec.Common.Logging.LogSystem.Warn("dữ liệu item.ID: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.ID), item.ID));

                    Inventec.Common.Logging.LogSystem.Warn("dữ liệu _ServiceReqs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ServiceReqs), _ServiceReqs));



                    var serviceReqDDTs = (ServiceReq != null && ServiceReq.Count > 0) ?
                       ServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                           && o.INTRUCTION_DATE < Inventec.Common.TypeConvert.Parse.ToInt64(dtTrackingDate.ToString("yyyyMMdd") + "000000")
                           && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList()
                       : null;

                    //var serviceReqDDTs = (_ServiceReqs != null && _ServiceReqs.Count > 0) ?
                    //    _ServiceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                    //        && o.INTRUCTION_DATE < Inventec.Common.TypeConvert.Parse.ToInt64(dtTrackingDate.ToString("yyyyMMdd") + "000000")
                    //        && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT).ToList()
                    //    : null;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqDDTs), serviceReqDDTs)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                    if (serviceReqDDTs != null && serviceReqDDTs.Count > 0)
                    {
                        //1.Lay tu danh sach yeu cau
                        foreach (var itemByServiceReq in serviceReqDDTs)
                        {
                            var itemExpMest = rdo._DicHisExpMests.ContainsKey(itemByServiceReq.ID) ? rdo._DicHisExpMests[itemByServiceReq.ID] : null;

                            Inventec.Common.Logging.LogSystem.Warn("dữ liệu itemExpMest: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => itemExpMest), itemExpMest));

                            if (itemExpMest == null)
                                continue;

                            List<HIS_EXP_MEST_MEDICINE> _expMestMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                            if (rdo._DicExpMestMedicines.ContainsKey(itemExpMest.ID))
                            {
                                _expMestMedicines = rdo._DicExpMestMedicines[itemExpMest.ID].ToList();
                                Inventec.Common.Logging.LogSystem.Warn("dữ liệu _expMestMedicines 1: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _expMestMedicines), _expMestMedicines));
                            }
                            if (_expMestMedicines != null && _expMestMedicines.Count > 0)
                            {
                                _expMestMedicines = _expMestMedicines.Where(o => o.USE_TIME_TO >= item.TRACKING_TIME).OrderBy(p => p.NUM_ORDER).ToList();

                                Inventec.Common.Logging.LogSystem.Warn("dữ liệu _expMestMedicines 2: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _expMestMedicines), _expMestMedicines));

                                foreach (var emmedi in _expMestMedicines)
                                {
                                    var medicineTypeName = rdo._MedicineTypes.FirstOrDefault(p => p.ID == emmedi.TDL_MEDICINE_TYPE_ID);

                                    Inventec.Common.Logging.LogSystem.Warn("dữ liệu medicineTypeName: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeName), medicineTypeName));

                                    if (medicineTypeName != null)
                                    {
                                        string s1 = "", s2 = "";
                                        s1 = medicineTypeName.MEDICINE_TYPE_NAME;
                                        s2 = emmedi.TUTORIAL;
                                        item.PRE_MEDICINE += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">- {0}</td></span></tr><tr><td text-align=\"right\" align=\"left\" width=\"650\">{1}</td></tr></table>", s1, s2);

                                    }
                                    Inventec.Common.Logging.LogSystem.Warn("dữ liệu PRE_MEDICINE: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.PRE_MEDICINE), item.PRE_MEDICINE));
                                }
                            }
                        }
                    }



                    //RemedyCount
                    var listRemedyCountADOs = this._RemedyCountADOs != null && this._RemedyCountADOs.Count > 0 ? this._RemedyCountADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listRemedyCountADOs != null && listRemedyCountADOs.Count > 0)
                    {
                        item.REMEDY_COUNT___DATA = "";
                        item.REMEDY_COUNT___DATA1 = "";
                        int dem = 0;
                        foreach (var item1 in listRemedyCountADOs)
                        {
                            if (item1.PRESCRIPTION_TYPE_ID == 2)
                            {
                                item.REMEDY_COUNT___DATA1 += String.Format("Đơn {0} thang: {1}", item1.REMEDY_COUNT, item1.TUTORIAL_REMEDY);
                            }
                            if ((item1.REMEDY_COUNT ?? 0) > 0)
                            {
                                item.REMEDY_COUNT___DATA += "Liều 1 thang x " + item1.DAY_COUNT;
                                item.REMEDY_COUNT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                item.REMEDY_COUNT___DATA += item1.TUTORIAL_REMEDY;
                            }
                            if (dem < listRemedyCountADOs.Count - 1)
                            {
                                item.REMEDY_COUNT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                item.REMEDY_COUNT___DATA1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    //MedicalInstruction
                    var listMedicalInstructions = this._MedicalInstructions != null && this._MedicalInstructions.Count > 0 ? this._MedicalInstructions.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    item.MEDICAL_INSTRUCTION___DATA = "";
                    if (listMedicalInstructions != null && listMedicalInstructions.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in listMedicalInstructions)
                        {
                            item.MEDICAL_INSTRUCTION___DATA += item1.MEDICAL_INSTRUCTION;
                            if (dem < listMedicalInstructions.Count - 1)
                            {
                                item.MEDICAL_INSTRUCTION___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    //_Bloods
                    var listBloods = this._Bloods != null && this._Bloods.Count > 0 ? this._Bloods.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    item.BLOOD___DATA = "";
                    if (listBloods != null && listBloods.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in listBloods)
                        {
                            item.BLOOD___DATA += item1.TDL_SERVICE_NAME;
                            if (dem < listBloods.Count - 1)
                            {
                                item.BLOOD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    //MEDICINE_LINE___DATA
                    var listMedicineLineADOs = this.MedicineLineADOs != null && this.MedicineLineADOs.Count > 0 ? this.MedicineLineADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    item.MEDICINE_LINE___DATA = "";
                    if (listMedicineLineADOs != null && listMedicineLineADOs.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in listMedicineLineADOs)
                        {
                            item.MEDICINE_LINE___DATA += item1.MEDICINE_LINE_NAME;
                            if (dem < listMedicineLineADOs.Count - 1)
                            {
                                item.MEDICINE_LINE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    var materials = this._ExpMestMatyReqADOs != null && this._ExpMestMatyReqADOs.Count > 0 ? this._ExpMestMatyReqADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    item.MATERIAL___DATA = "";

                    //ServiceReqMaty
                    var listServiceReqMatyADOs = this._ServiceReqMatyADOs != null && this._ServiceReqMatyADOs.Count > 0 ? this._ServiceReqMatyADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listServiceReqMatyADOs != null && listServiceReqMatyADOs.Count > 0)
                    {
                        if (materials == null)
                            materials = new List<ExpMestMatyReqADO>();
                        foreach (var item1 in listServiceReqMatyADOs)
                        {
                            materials.Insert(materials.Count, new ExpMestMatyReqADO()
                            {
                                MATERIAL_TYPE_NAME = item1.MATERIAL_TYPE_NAME,
                                AMOUNT = item1.AMOUNT,
                                TUTORIAL = item1.TUTORIAL,
                                NUM_ORDER = item1.NUM_ORDER,
                                SERVICE_UNIT_NAME = String.IsNullOrEmpty(item1.SERVICE_UNIT_NAME) ? item1.UNIT_NAME : item1.SERVICE_UNIT_NAME,
                                TRACKING_ID = item1.TRACKING_ID,
                            });
                        }
                    }
                    if (materials != null && materials.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in materials)
                        {
                            item.MATERIAL___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                            if (dem < materials.Count - 1)
                            {
                                item.MATERIAL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    item.IMP_MEST_MEDICINE___DATA = "";
                    var impMestMedicineADOs = this._ImpMestMedicineADOs != null && this._ImpMestMedicineADOs.Count > 0 ? this._ImpMestMedicineADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (impMestMedicineADOs != null && impMestMedicineADOs.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in impMestMedicineADOs)
                        {
                            item.IMP_MEST_MEDICINE___DATA += item1.MEDICINE_TYPE_NAME + " " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                            if (dem < impMestMedicineADOs.Count - 1)
                            {
                                item.IMP_MEST_MEDICINE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    item.IMP_MEST_MATERIAL___DATA = "";
                    var impMestMaterialADOs = this._ImpMestMaterialADOs != null && this._ImpMestMaterialADOs.Count > 0 ? this._ImpMestMaterialADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (impMestMaterialADOs != null && impMestMaterialADOs.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in impMestMaterialADOs)
                        {
                            item.IMP_MEST_MATERIAL___DATA += item1.MATERIAL_TYPE_NAME + " " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                            if (dem < impMestMaterialADOs.Count - 1)
                            {
                                item.IMP_MEST_MATERIAL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }
                    item.TT_SERVICE___DATA = "";
                    var ttServices = this._TTServices != null && this._TTServices.Count > 0 ? this._TTServices.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (ttServices != null && ttServices.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in ttServices)
                        {
                            item.TT_SERVICE___DATA += item1.TDL_SERVICE_NAME + "    -     " + item1.INSTRUCTION_NOTE;
                            if (dem < ttServices.Count - 1)
                            {
                                item.TT_SERVICE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    item.SERVICE_CLS___DATA = "";
                    var listServiceCLSs = this._ServiceCLSs != null && this._ServiceCLSs.Count > 0 ? this._ServiceCLSs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listServiceCLSs != null && listServiceCLSs.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in listServiceCLSs)
                        {
                            item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + item1.SERVICE_NAME;
                            if (dem < listServiceCLSs.Count - 1)
                            {
                                item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }


                    //CARE
                    var listCares = this.rdo._Cares != null && this.rdo._Cares.Count > 0 ? this.rdo._Cares.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listCares != null && listCares.Count > 0)
                    {
                        int dem = 0;
                        item.CARE___DATA = "";
                        foreach (var item1 in listCares)
                        {
                            item.CARE___DATA += String.Format("{0}", item1.NUTRITION);
                            if (dem < listCares.Count - 1)
                            {
                                item.CARE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    //CareDetail
                    var listCareDetails = this.rdo._CareDetails != null && this.rdo._CareDetails.Count > 0 ? this.rdo._CareDetails.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listCareDetails != null && listCareDetails.Count > 0)
                    {
                        int dem = 0;
                        item.CARE_DETAIL___DATA = "";
                        foreach (var item1 in listCareDetails)
                        {
                            item.CARE_DETAIL___DATA += String.Format("{0}", item1.CARE_TYPE_NAME);
                            if (dem < listCareDetails.Count - 1)
                            {
                                item.CARE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }

                    var user = users != null ? users.FirstOrDefault(o => o.LOGINNAME == item.CREATOR) : null;
                    item.TRACKING_USERNAME = user != null ? user.USERNAME : "";


                    item.PARENT_ORGANIZATION_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.PARENT_ORGANIZATION_NAME);
                    item.ORGANIZATION_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.ORGANIZATION_NAME);
                    item.DEPARTMENT_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.DEPARTMENT_NAME);

                    item.EXECUTE_TIME_DHST = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.EXECUTE_TIME_DHST);
                    item.NGAY_1 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.NGAY_1);
                    item.NGAY_2 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.NGAY_2);
                    item.NGAY_3 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.NGAY_3);
                    item.NGAY_4 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.NGAY_4);
                    item.Y_LENH_1 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.Y_LENH_1);
                    item.Y_LENH_2 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.Y_LENH_2);
                    item.Y_LENH_3 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.Y_LENH_3);
                    item.Y_LENH_4 = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.Y_LENH_4);
                    item.TDL_PATIENT_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_NAME");
                    item.AGE = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.AGE);
                    item.TDL_PATIENT_GENDER_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_GENDER_NAME");
                    item.TDL_PATIENT_ADDRESS = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_ADDRESS");
                    item.IN_CODE = GetStringValueInSingleValueDictionaryByKey("IN_CODE");
                    item.PHONE = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.PHONE);
                    item.ROOM_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.ROOM_NAME);
                    item.BED_NAME = GetStringValueInSingleValueDictionaryByKey("BED_NAME");
                    item.ICD_NAME_FULL = GetStringValueInSingleValueDictionaryByKey("ICD_NAME");
                    item.TREATMENT_CODE = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.TREATMENT_CODE);
                    item.TDL_PATIENT_CODE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_CODE");
                    item.ICD_CODE_BY_TRACKING = GetStringValueInSingleValueDictionaryByKey("ICD_CODE_BY_TRACKING");
                    item.ICD_NAME_BY_TRACKING = GetStringValueInSingleValueDictionaryByKey("ICD_NAME_BY_TRACKING");
                    item.ICD_SUB_CODE_BY_TRACKING = GetStringValueInSingleValueDictionaryByKey("ICD_SUB_CODE_BY_TRACKING");
                    item.ICD_TEXT_BY_TRACKING = GetStringValueInSingleValueDictionaryByKey("ICD_TEXT_BY_TRACKING");

                    item.ADVISE = GetStringValueInSingleValueDictionaryByKey("ADVISE");
                    item.APPOINTMENT_CODE = GetStringValueInSingleValueDictionaryByKey("APPOINTMENT_CODE");
                    item.APPOINTMENT_DESC = GetStringValueInSingleValueDictionaryByKey("APPOINTMENT_DESC");
                    item.APPOINTMENT_EXAM_ROOM_IDS = GetStringValueInSingleValueDictionaryByKey("APPOINTMENT_EXAM_ROOM_IDS");
                    item.APPOINTMENT_SURGERY = GetStringValueInSingleValueDictionaryByKey("APPOINTMENT_SURGERY");
                    item.APPOINTMENT_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("APPOINTMENT_TIME"));
                    item.AUTO_DISCOUNT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(GetStringValueInSingleValueDictionaryByKey("AUTO_DISCOUNT_RATIO"));
                    item.BRANCH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("BRANCH_ID"));
                    item.CLINICAL_IN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("CLINICAL_IN_TIME"));
                    item.DEATH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("DEATH_TIME"));
                    item.DEATH_WITHIN_ID = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("DEATH_WITHIN_ID"));
                    item.EMERGENCY_WTIME_ID = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("EMERGENCY_WTIME_ID"));
                    item.END_DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("END_DEPARTMENT_ID"));
                    item.CLINICAL_NOTE = GetStringValueInSingleValueDictionaryByKey("CLINICAL_NOTE");
                    item.DOCTOR_LOGINNAME = GetStringValueInSingleValueDictionaryByKey("DOCTOR_LOGINNAME");
                    item.DOCTOR_USERNAME = GetStringValueInSingleValueDictionaryByKey("DOCTOR_USERNAME");
                    item.END_CODE = GetStringValueInSingleValueDictionaryByKey("END_CODE");
                    item.END_LOGINNAME = GetStringValueInSingleValueDictionaryByKey("END_LOGINNAME");
                    item.END_CODE = GetStringValueInSingleValueDictionaryByKey("END_CODE");
                    item.END_USERNAME = GetStringValueInSingleValueDictionaryByKey("END_USERNAME");
                    item.EXTRA_END_CODE = GetStringValueInSingleValueDictionaryByKey("EXTRA_END_CODE");
                    item.HOSPITALIZATION_REASON = GetStringValueInSingleValueDictionaryByKey("HOSPITALIZATION_REASON");
                    item.HRM_KSK_CODE = GetStringValueInSingleValueDictionaryByKey("HRM_KSK_CODE");
                    item.ICD_CAUSE_NAME = GetStringValueInSingleValueDictionaryByKey("ICD_CAUSE_NAME");
                    item.ICD_CAUSE_CODE = GetStringValueInSingleValueDictionaryByKey("ICD_CAUSE_CODE");
                    item.ICD_CODE = GetStringValueInSingleValueDictionaryByKey("ICD_CODE");
                    item.ICD_NAME = GetStringValueInSingleValueDictionaryByKey("ICD_NAME");
                    item.ICD_SUB_CODE = GetStringValueInSingleValueDictionaryByKey("ICD_SUB_CODE");
                    item.ICD_TEXT = GetStringValueInSingleValueDictionaryByKey("ICD_TEXT");
                    item.IN_ICD_CODE = GetStringValueInSingleValueDictionaryByKey("IN_ICD_CODE");
                    item.IN_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("IN_DATE"));
                    item.IN_ICD_CODE = GetStringValueInSingleValueDictionaryByKey("IN_ICD_CODE");
                    item.IN_ICD_NAME = GetStringValueInSingleValueDictionaryByKey("IN_ICD_NAME");
                    item.IN_ICD_SUB_CODE = GetStringValueInSingleValueDictionaryByKey("IN_ICD_SUB_CODE");
                    item.IN_ICD_TEXT = GetStringValueInSingleValueDictionaryByKey("IN_ICD_TEXT");
                    item.IN_LOGINNAME = GetStringValueInSingleValueDictionaryByKey("IN_LOGINNAME");
                    item.IN_ICD_CODE = GetStringValueInSingleValueDictionaryByKey("IN_ICD_CODE");
                    item.IN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("IN_TIME"));
                    item.IN_USERNAME = GetStringValueInSingleValueDictionaryByKey("IN_USERNAME");
                    item.MAIN_CAUSE = GetStringValueInSingleValueDictionaryByKey("MAIN_CAUSE");
                    item.MEDI_ORG_CODE = GetStringValueInSingleValueDictionaryByKey("MEDI_ORG_CODE");
                    item.MEDI_ORG_NAME = GetStringValueInSingleValueDictionaryByKey("MEDI_ORG_NAME");
                    item.IN_USERNAME = GetStringValueInSingleValueDictionaryByKey("IN_USERNAME");
                    item.OUT_CODE = GetStringValueInSingleValueDictionaryByKey("OUT_CODE");
                    item.PATIENT_CONDITION = GetStringValueInSingleValueDictionaryByKey("PATIENT_CONDITION");
                    item.PROVISIONAL_DIAGNOSIS = GetStringValueInSingleValueDictionaryByKey("PROVISIONAL_DIAGNOSIS");
                    item.NEED_SICK_LEAVE_CERT = Inventec.Common.TypeConvert.Parse.ToInt16(GetStringValueInSingleValueDictionaryByKey("NEED_SICK_LEAVE_CERT"));
                    item.OUT_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("OUT_DATE"));
                    item.OUT_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("OUT_TIME"));
                    item.SICK_HEIN_CARD_NUMBER = GetStringValueInSingleValueDictionaryByKey("SICK_HEIN_CARD_NUMBER");
                    item.STORE_CODE = GetStringValueInSingleValueDictionaryByKey("STORE_CODE");
                    item.STORE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("STORE_TIME"));
                    item.SUBCLINICAL_RESULT = GetStringValueInSingleValueDictionaryByKey("SUBCLINICAL_RESULT");
                    item.SUBCLINICAL_RESULT = GetStringValueInSingleValueDictionaryByKey("SUBCLINICAL_RESULT");
                    item.TDL_HEIN_CARD_NUMBER = GetStringValueInSingleValueDictionaryByKey("TDL_HEIN_CARD_NUMBER");
                    item.TDL_HEIN_MEDI_ORG_CODE = GetStringValueInSingleValueDictionaryByKey("TDL_HEIN_MEDI_ORG_CODE");
                    item.TDL_HEIN_MEDI_ORG_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_HEIN_MEDI_ORG_NAME");
                    item.TDL_PATIENT_ACCOUNT_NUMBER = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_ACCOUNT_NUMBER");
                    item.TDL_PATIENT_CAREER_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_CAREER_NAME");
                    item.TDL_PATIENT_COMMUNE_CODE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_COMMUNE_CODE");
                    item.TDL_PATIENT_DISTRICT_CODE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_DISTRICT_CODE");
                    item.SUBCLINICAL_RESULT = GetStringValueInSingleValueDictionaryByKey("SUBCLINICAL_RESULT");
                    item.TDL_PATIENT_DOB = Inventec.Common.TypeConvert.Parse.ToInt64(GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_DOB"));
                    item.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = Inventec.Common.TypeConvert.Parse.ToInt16(GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_IS_HAS_NOT_DAY_DOB"));
                    item.TDL_PATIENT_FIRST_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_FIRST_NAME");
                    item.TDL_PATIENT_LAST_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_LAST_NAME");
                    item.TDL_PATIENT_MILITARY_RANK_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_MILITARY_RANK_NAME");
                    item.TDL_PATIENT_MOBILE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_MOBILE");
                    item.TDL_PATIENT_NATIONAL_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_NATIONAL_NAME");
                    item.TDL_PATIENT_PHONE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_PHONE");
                    item.TDL_PATIENT_PROVINCE_CODE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_PROVINCE_CODE");
                    item.TDL_PATIENT_RELATIVE_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_RELATIVE_NAME");
                    item.TDL_PATIENT_RELATIVE_TYPE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_RELATIVE_TYPE");
                    item.TDL_PATIENT_TAX_CODE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_TAX_CODE");
                    item.TDL_PATIENT_WORK_PLACE = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_WORK_PLACE");
                    item.TDL_PATIENT_WORK_PLACE_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_WORK_PLACE_NAME");
                    item.TRANSFER_IN_CODE = GetStringValueInSingleValueDictionaryByKey("TRANSFER_IN_CODE");
                    item.TRANSFER_IN_ICD_CODE = GetStringValueInSingleValueDictionaryByKey("TRANSFER_IN_ICD_CODE");
                    item.TRANSFER_IN_ICD_NAME = GetStringValueInSingleValueDictionaryByKey("TRANSFER_IN_ICD_NAME");
                    item.TRANSFER_IN_MEDI_ORG_CODE = GetStringValueInSingleValueDictionaryByKey("TRANSFER_IN_MEDI_ORG_CODE");
                    item.TRANSFER_IN_MEDI_ORG_NAME = GetStringValueInSingleValueDictionaryByKey("TRANSFER_IN_MEDI_ORG_NAME");
                    item.TREATMENT_DAY_COUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(GetStringValueInSingleValueDictionaryByKey("TREATMENT_DAY_COUNT"));
                    item.TREATMENT_DIRECTION = GetStringValueInSingleValueDictionaryByKey("TREATMENT_DIRECTION");
                    item.TREATMENT_METHOD = GetStringValueInSingleValueDictionaryByKey("TREATMENT_METHOD");
                    item.USED_MEDICINE = GetStringValueInSingleValueDictionaryByKey("USED_MEDICINE");
                    item.BED_CODE = GetStringValueInSingleValueDictionaryByKey("BED_CODE");
                    item.BED_ROOM_CODE = GetStringValueInSingleValueDictionaryByKey("BED_ROOM_CODE");
                    item.BED_ROOM_NAME = GetStringValueInSingleValueDictionaryByKey("BED_ROOM_NAME");
                    item.PATIENT_TYPE_NAME = GetStringValueInSingleValueDictionaryByKey("PATIENT_TYPE_NAME");
                    item.PATIENT_TYPE_CODE = GetStringValueInSingleValueDictionaryByKey("PATIENT_TYPE_CODE");
                    item.ORDER_SHEET = GetStringValueInSingleValueDictionaryByKey(Mps000429ExtendSingleKey.ORDER_SHEET);
                    if (itemIndex > 0)
                    {
                        item.PRIVIOUS_TRACKING_DATE_STR = GetPriviousTrackingDate(itemIndex - 1);
                    }
                    if (mps000429ADOExt.Mps000429ADOs.Count > itemIndex + 1)
                        item.NEXT_TRACKING_DATE_STR = GetPriviousTrackingDate(itemIndex + 1);
                    itemIndex++;
                }

                success = xtraReportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                success = success && singleTag.ProcessData(xtraReportStore, singleValueDictionary);
                success = success && objectTag.AddObjectData<Mps000429ExtADO>(xtraReportStore, mps000429ADOExt.Mps000429ADOs);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mps000429ADOExt.Mps000429ADOs), mps000429ADOExt.Mps000429ADOs));
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        string GetPriviousTrackingDate(int index)
        {
            string value = "";
            try
            {
                if (mps000429ADOExt != null && mps000429ADOExt.Mps000429ADOs != null && mps000429ADOExt.Mps000429ADOs.Count > index)
                    value = mps000429ADOExt.Mps000429ADOs[index].TRACKING_DATE_STR;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        object GetValueInSingleValueDictionaryByKey(string key)
        {
            object vl = null;
            try
            {
                if (singleValueDictionary.ContainsKey(key))
                    vl = singleValueDictionary[key];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vl;
        }

        string GetStringValueInSingleValueDictionaryByKey(string key)
        {
            string vl = "";
            try
            {
                var obj = GetValueInSingleValueDictionaryByKey(key);
                vl = (obj != null ? obj.ToString() : "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vl;
        }

        private bool ProcessDataWord()
        {
            bool success = false;
            try
            {
                Inventec.Common.TemplaterExport.ProcessSingleTag singleTag = new Inventec.Common.TemplaterExport.ProcessSingleTag();
                Inventec.Common.TemplaterExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.TemplaterExport.ProcessBarCodeTag();
                Inventec.Common.TemplaterExport.ProcessObjectTag objectTag = new Inventec.Common.TemplaterExport.ProcessObjectTag();

                mps000429ADOExt.Mps000429ADOs = (from m in this._Mps000429ADOs select new Mps000429ExtADO(m)).ToList();
                mps000429ADOExt.WorkPlaceSDO = rdo._WorkPlaceSDO;
                mps000429ADOExt.SingleValueDictionary = singleValueDictionary;

                foreach (var item in mps000429ADOExt.Mps000429ADOs)
                {
                    item.RemedyCountADOs = this._RemedyCountADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ExpMestMatyReqADOs = this._ExpMestMatyReqADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ServiceReqMetyADOs = this._ServiceReqMetyADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ServiceReqMatyADOs = this._ServiceReqMatyADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ServiceCLSs = this._ServiceCLSs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.Cares = rdo._Cares.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.CareDetails = rdo._CareDetails.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.MedicalInstructions = this._MedicalInstructions.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.Bloods = this._Bloods.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ExamServices = this._ExamServices.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.TTServices = this._TTServices.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ImpMestMedicineADOs = this._ImpMestMedicineADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ImpMestMaterialADOs = this._ImpMestMaterialADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.MedicineLineADOs = this.MedicineLineADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ExpMestMetyReqADOs = _ExpMestMetyReqADOCommons.Where(o => o.TRACKING_ID == item.ID).ToList();
                }

                success = templaterExportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                success = success && barCodeTag.ProcessData(templaterExportStore, dicImage);
                success = success && singleTag.ProcessData(templaterExportStore, singleValueDictionary);
                //List<Mps000429ADOExt> mps000429ADOExts = new List<Mps000429ADOExt>();
                //mps000429ADOExts.Add(mps000429ADOExt);
                success = success && objectTag.AddObjectData(templaterExportStore, mps000429ADOExt.Mps000429ADOs);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mps000429ADOExt.Mps000429ADOs), mps000429ADOExt.Mps000429ADOs));
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void SetSingleKey()
        {
            try
            {
                string phone = "";
                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.EXECUTE_TIME_DHST, this._EXECUE_TIME_DHST > 0));
                if (_SingleKeyTamThans != null && _SingleKeyTamThans.Count > 0)
                {
                    for (int i = 0; i < _SingleKeyTamThans.Count; i++)
                    {
                        if (i == 4)
                            break;
                        switch (i)
                        {
                            case 0:
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.NGAY_1, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.Y_LENH_1, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 1:
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.NGAY_2, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.Y_LENH_2, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 2:
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.NGAY_3, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.Y_LENH_3, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 3:
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.NGAY_4, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.Y_LENH_4, _SingleKeyTamThans[i].Y_LENH));
                                break;
                        }
                    }
                }

                if (rdo._Patient != null)
                {
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._Patient.DOB)));
                    phone = rdo._Patient.PHONE;
                }
                if (rdo._Trackings != null && rdo._Trackings.Count > 0)
                {
                    var dataTracking = rdo._Trackings.FirstOrDefault();
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ICD_CODE_BY_TRACKING, dataTracking.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ICD_NAME_BY_TRACKING, dataTracking.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ICD_SUB_CODE_BY_TRACKING, dataTracking.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ICD_TEXT_BY_TRACKING, dataTracking.ICD_TEXT));

                    //for (int i = 1; i <= rdo._Trackings.Count; i++)
                    //{
                    long? OrderSheet = rdo._Trackings.Min(o => o.SHEET_ORDER) != null ? rdo._Trackings.Min(o => o.SHEET_ORDER) : 1;

                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ORDER_SHEET, OrderSheet));
                    //}
                }

                if (rdo._Treatments != null)
                {
                    string icd_main_text = "", icd_text_by_treatment = "";
                    foreach (var item in rdo._Treatments)
                    {
                        if (!string.IsNullOrEmpty(item.ICD_NAME) && !icd_main_text.Contains(item.ICD_NAME))
                        {
                            icd_main_text += item.ICD_NAME;
                        }

                        if (!string.IsNullOrEmpty(item.ICD_TEXT) && !icd_text_by_treatment.Contains(item.ICD_TEXT))
                        {
                            icd_text_by_treatment += item.ICD_TEXT;
                        }
                    }
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ICD_MAIN_BY_TREATMENT, icd_main_text));
                    SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ICD_TEXT_BY_TREATMENT, icd_text_by_treatment));
                }

                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.PHONE, phone));
                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.DEPARTMENT_NAME, rdo._WorkPlaceSDO.DepartmentName));
                SetSingleKey(new KeyValue(Mps000429ExtendSingleKey.ROOM_NAME, rdo._WorkPlaceSDO.RoomName));
                AddObjectKeyIntoListkey<Mps000429SingleKey>(rdo._WorkPlaceSDO, false);
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo._Patient);
                AddObjectKeyIntoListkey<HIS_MEDI_RECORD>(rdo._MediRecord);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                log = "Mã bệnh nhân: " + rdo._Patient.PATIENT_CODE;
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo._Treatments != null)
                {
                    //string patientCode = "PATIENT_CODE:" + rdo._Patient.PATIENT_CODE;
                    List<string> treatmentIds = new List<string>();
                    foreach (var itemTr in rdo._Treatments.Select(s => s.ID).Distinct().ToList())
                    {
                        treatmentIds.Add("TREATMENT_CODE:" + itemTr);
                    }

                    List<string> trackingIds = new List<string>();
                    foreach (var item in rdo._Trackings.Select(s => s.ID).Distinct().ToList())
                    {
                        trackingIds.Add("HIS_TRACKING:" + item);
                    }

                    string trackingId = string.Join(",", trackingIds);
                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentIds, trackingId);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
    class CalculateMergerData : TFlexCelUserFunction
    {
        internal List<Mps000429ADO> _Mps000429ADOs;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                int index = int.Parse(parameters[0].ToString());

                if (_Mps000429ADOs != null && _Mps000429ADOs.Count > 0 && index < _Mps000429ADOs.Count - 1)
                {
                    if (_Mps000429ADOs[index].TRACKING_DATE_STR == _Mps000429ADOs[index + 1].TRACKING_DATE_STR)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}


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
using AutoMapper;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000007;
using MPS.Processor.Mps000374.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000374
{
    public class Mps000374Processor : AbstractProcessor
    {
        internal Mps000374ADOExts Mps000374ADOExt = new Mps000374ADOExts();
        internal Mps000374PDO rdo;
        internal List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        internal HIS_TRACKING _tracking { get; set; }
        internal List<Mps000374ADO> _Mps000374ADOs { get; set; }
        internal List<Mps000374ExtADO> _Mps000374ExtADOs { get; set; }
        internal List<ServiceCLS> _ServiceCLSs { get; set; }
        internal List<ServiceCLS> _Bloods { get; set; }
        internal List<ServiceCLS> _ExamServices { get; set; }
        internal List<ServiceCLS> _TTServices { get; set; }
        internal List<RemedyCountADO> _RemedyCountADOs { get; set; }
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommons { get; set; }
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOs { get; set; }
        internal List<MedicineLineADO> MedicineLineADOs { get; set; } //tiennv
        internal List<ExpMestMatyReqADO> _ExpMestMatyReqADOs { get; set; }
        internal List<ServiceReqMetyADO> _ServiceReqMetyADOs { get; set; }
        internal List<ServiceReqMatyADO> _ServiceReqMatyADOs { get; set; }
        internal List<MedicalInstruction> _MedicalInstructions { get; set; }//ppxl
        Dictionary<long, List<NumberDate>> _DicCountNumbers = new Dictionary<long, List<NumberDate>>();
        Dictionary<long, List<NumberDate>> _DicCountNumberByTypes = new Dictionary<long, List<NumberDate>>();

        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOsV2 { get; set; }//Them ghi chu Y lệnh ngày 08/04, 09/04, 10/04

        List<SingleKeyTracking> _SingleKeyTamThans = new List<SingleKeyTracking>();
        SingleKeyTracking keyTamThan { get; set; }

        //TH
        internal List<ImpMestMedicineADO> _ImpMestMedicineADOs { get; set; }
        internal List<ImpMestMaterialADO> _ImpMestMaterialADOs { get; set; }

        internal long _EXECUE_TIME_DHST { get; set; }

        string IS_SHOW = "X";

        long sum_medi_mate = 0;


        // begin 007
        private PatientADO patientADO { get; set; }
        private PatyAlterBhytADO patyAlter { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> ExpMestMaterials { get; set; }
        List<ExpMestBloods> ExpMestBloods { get; set; }
        // end 007

        public Mps000374Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000374PDO)rdoBase;
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
                                    if (check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO
                                        )
                                    { }
                                    else
                                        continue;

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
                                    if (check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO
                                        || check.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO
                                        )
                                    { }
                                    else
                                        continue;


                                    if (!_DicCountNumbers.ContainsKey(check.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
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
                _Mps000374ADOs = new List<Mps000374ADO>();
                _Mps000374ExtADOs = new List<Mps000374ExtADO>();
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
                        Mps000374ADO _service = new Mps000374ADO();

                        Mapper.CreateMap<HIS_TRACKING, Mps000374ADO>();
                        _service = Mapper.Map<HIS_TRACKING, Mps000374ADO>(itemTracking);
                        _service.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(itemTracking.TRACKING_TIME);
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
                            //int key = str.Count() / 4;
                            //var skip = 0;
                            //if (key > 0)
                            //{
                            //    while ((str.Count() - skip) > 0)
                            //    {
                            //        MedicalInstruction _Medical = new MedicalInstruction();
                            //        _Medical.TRACKING_ID = _tracking.ID;
                            //        var listNews = str.Skip(skip).Take(key).ToList();
                            //        skip = skip + key;
                            //        foreach (var itemN in listNews)
                            //        {
                            //            _Medical.MEDICAL_INSTRUCTION += itemN + ". ";
                            //        }
                            //        _MedicalInstructions.Add(_Medical);
                            //    }
                            //}
                            //else
                            //{
                            //    MedicalInstruction _Medical = new MedicalInstruction();
                            //    _Medical.TRACKING_ID = _tracking.ID;
                            //    foreach (var itemN in str)
                            //    {
                            //        _Medical.MEDICAL_INSTRUCTION += itemN + ". ";
                            //    }
                            //    _MedicalInstructions.Add(_Medical);
                            //}
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
                        if (rdo._DicServiceReqs != null)
                        {
                            var ser = rdo._DicServiceReqs.Values.Where(p => p.TRACKING_ID == itemTracking.ID).ToList();
                            if (ser != null && ser.Count > 0)
                                _ServiceReqs = ser.ToList();
                        }
                        else
                        {
                            return;
                            // Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._DicServiceReqs), rdo._DicServiceReqs));
                        }
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
                        _Mps000374ADOs.Add(_service);

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
                //Gan mac dinh thang
                RemedyCountADO remedyCountAddNull = new RemedyCountADO();
                remedyCountAddNull.TRACKING_ID = _tracking.ID;
                remedyCountAddNull.EXP_MEST_ID = 0;
                remedyCountAddNull.REMEDY_COUNT = 0;
                _RemedyCountADOs.Add(remedyCountAddNull);
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
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
                        foreach (var itemExpMestMetyReq in _expMestMetyReqGroups)
                        {
                            ExpMestMetyReqADO group = new ExpMestMetyReqADO(itemExpMestMetyReq[0]);
                            group.TRACKING_ID = _tracking.ID;
                            // group.PRESCRIPTION_TYPE_ID = itemServiceReq.PRESCRIPTION_TYPE_ID ?? 0;
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
                                if (medicineTypeName.MEDICINE_GROUP_ID > 0)
                                {
                                    group.GROUP_BHYT = "X";

                                    if (_DicCountNumberByTypes != null && _DicCountNumberByTypes.Count > 0
                                       && _DicCountNumberByTypes.ContainsKey(medicineTypeName.ID))
                                    {
                                        var _DataNumberByType = _DicCountNumberByTypes[medicineTypeName.ID].FirstOrDefault(p => p.INTRUCTION_DATE == itemServiceReq.INTRUCTION_DATE && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                        if (_DataNumberByType != null)
                                        {
                                            group.NUMBER_BY_TYPE = (long)_DataNumberByType.num;
                                        }
                                    }

                                    if (_DicCountNumbers != null && _DicCountNumbers.Count > 0
                                        && _DicCountNumbers.ContainsKey(medicineTypeName.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        var _DataNumber = _DicCountNumbers[medicineTypeName.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == itemServiceReq.INTRUCTION_DATE && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                        if (_DataNumber != null)
                                        {
                                            group.NUMBER_H_N = (long)_DataNumber.num;
                                            if (rdo._WorkPlaceSDO != null && rdo._WorkPlaceSDO.IsOrderByType == 2)
                                            {
                                                //var userForm = rdo._MedicineUseForms.FirstOrDefault(p => p.ID == medicineTypeName.MEDICINE_USE_FORM_ID && p.IS_ACTIVE == 1);
                                                //group.MEDICINE_USE_FORM_ID = medicineTypeName.MEDICINE_USE_FORM_ID;
                                                //group.NUM_ORDER_BY_USE_FORM = userForm != null ? userForm.NUM_ORDER ?? 0 : 0;
                                                if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)//GayNghien..2.4
                                                {
                                                    group.TYPE_ID = 4;
                                                    group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                }
                                                else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)//KhangSinh..2.1
                                                {
                                                    group.TYPE_ID = 1;
                                                    group.NUMBER_INTRUCTION_DATE = "(" + _DataNumber.num + ")";
                                                }
                                                else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//HuongThan..2.5
                                                {
                                                    group.TYPE_ID = 5;
                                                    group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                }
                                                else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)//corticod..2.2
                                                {
                                                    group.TYPE_ID = 2;
                                                    group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                }
                                                else if (medicineTypeName.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)//Lao..2.6
                                                {
                                                    group.TYPE_ID = 6;
                                                    group.NUMBER_INTRUCTION_DATE = "(" + ToRoman(_DataNumber.num) + ")";
                                                }
                                                else//ThuocKhac..Cho xg cuoi cung
                                                {
                                                    group.TYPE_ID = 7;
                                                }
                                            }
                                        }
                                    }
                                }
                                else//ThuocThuogn..2.3
                                {
                                    group.TYPE_ID = 3;
                                }
                                #endregion
                            }

                            //if (_DicCountNumbers != null && _DicCountNumbers.Count > 0
                            //    && _DicCountNumbers.ContainsKey(itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID))
                            //{
                            //    var lstData = _DicCountNumbers[itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID];
                            //    group.NUMBER_H_N = lstData.FirstOrDefault(p => p.INTRUCTION_DATE == itemServiceReq.INTRUCTION_DATE).num;
                            //}
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
                        // var medicineGroups = _ExpMestMetyReqADOs.GroupBy(o => new { o.MEDICINE_LINE_ID, o.EXP_MEST_ID, o.TDL_SERVICE_REQ_ID }); 
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

                            #region -------------old-------------
                            //RemedyCountADO remedyCount = new RemedyCountADO();
                            //remedyCount.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                            //remedyCount.TRACKING_ID = _tracking.ID;
                            //remedyCount.EXP_MEST_ID = g.FirstOrDefault().EXP_MEST_ID ?? 0;//EXP_MEST_ID
                            //remedyCount.REMEDY_COUNT = g.FirstOrDefault().REMEDY_COUNT;
                            ////remedyCount.PRESCRIPTION_TYPE_ID = itemServiceReq.PRESCRIPTION_TYPE_ID ?? 0;
                            //var tutorial = rdo._DicExpMestMedicines[g.FirstOrDefault().EXP_MEST_ID ?? 0].FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.TUTORIAL));
                            //remedyCount.TUTORIAL_REMEDY = tutorial != null ? tutorial.TUTORIAL : "";

                            //var listExpMesMedicines = rdo._DicExpMestMedicines[g.FirstOrDefault().EXP_MEST_ID ?? 0];
                            //long userTimeTo = listExpMesMedicines != null && listExpMesMedicines.Count > 0 ? listExpMesMedicines.Max(p => p.USE_TIME_TO ?? 0) : 0;
                            //var serviceReqTime = _ServiceReqs.FirstOrDefault(p => p.ID == g.FirstOrDefault().TDL_SERVICE_REQ_ID);
                            //long intructionTime = serviceReqTime != null ? serviceReqTime.INTRUCTION_TIME : 0;
                            //if (userTimeTo > 0 && intructionTime > 0)
                            //{
                            //    DateTime dtIntructionTime = System.DateTime.ParseExact(intructionTime.ToString(), "yyyyMMddHHmmss",
                            //          System.Globalization.CultureInfo.InvariantCulture);
                            //    DateTime dtUserTimeTo = System.DateTime.ParseExact(userTimeTo.ToString(), "yyyyMMddHHmmss",
                            //                           System.Globalization.CultureInfo.InvariantCulture);
                            //    TimeSpan ts = new TimeSpan();
                            //    ts = (TimeSpan)(dtUserTimeTo - dtIntructionTime);
                            //    if (ts != null && ts.Days >= 0)
                            //    {
                            //        remedyCount.DAY_COUNT = ts.Days + 1;
                            //    }
                            //}
                            //_RemedyCountADOs.Add(remedyCount);
                            #endregion
                        }

                        #region ---- new -----
                        var medicineGroupsV2 = __ExpMestMetyReqADO_V2s.GroupBy(o => new { o.MEDICINE_LINE_ID, o.EXP_MEST_ID, o.TDL_SERVICE_REQ_ID });
                        foreach (var g in medicineGroupsV2)
                        {
                            RemedyCountADO remedyCount = new RemedyCountADO();
                            remedyCount.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                            remedyCount.TRACKING_ID = _tracking.ID;
                            remedyCount.EXP_MEST_ID = g.FirstOrDefault().EXP_MEST_ID ?? 0;//EXP_MEST_ID
                            remedyCount.REMEDY_COUNT = g.FirstOrDefault().REMEDY_COUNT;
                            //remedyCount.PRESCRIPTION_TYPE_ID = itemServiceReq.PRESCRIPTION_TYPE_ID ?? 0;
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

                }
                //this.ProcessMedicineLine();
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
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
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
                Inventec.Common.BarcodeLib.Barcode barcodePatientCode = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TDL_PATIENT_CODE);
                barcodePatientCode.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodePatientCode.Width = 120;
                barcodePatientCode.Height = 40;
                barcodePatientCode.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodePatientCode.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodePatientCode.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodePatientCode.IncludeLabel = true;

                dicImage.Add(Mps000374ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000374ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                Inventec.Common.Logging.LogSystem.Debug("Mps000374 ------ ProcessData----1");

                CheckSTTByMedicineGroup();

                CheckIntructionDate_GN_HT();

                ProcessorDataPrint();

                SetBarcodeKey();

                SetSingleKey();

                DataInputProcess();

                ProcessSingleKey07();

                SetBarcodeKey07();

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
                        List<ExpMestMetyReqADO> _dataNews = new List<ExpMestMetyReqADO>();
                        this._ExpMestMetyReqADOs = this._ExpMestMetyReqADOsV2.OrderBy(p => p.TYPE_ID).ToList();
                        var dataGroups = this._ExpMestMetyReqADOsV2.GroupBy(p => p.TYPE_ID).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                            _dataNews.AddRange(dtGroups);
                        }

                        _ExpMestMetyReqADOCommons = _dataNews;
                        //objectTag.AddObjectData(store, "Medicines", _dataNews);
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        List<ExpMestMetyReqADO> _dataNews2 = new List<ExpMestMetyReqADO>();

                        var dtGroups = this._ExpMestMetyReqADOsV2.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                        _dataNews2.AddRange(dtGroups);

                        _ExpMestMetyReqADOCommons = _dataNews2;
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                    {
                        List<ExpMestMetyReqADO> _dataNews2 = new List<ExpMestMetyReqADO>();

                        var dtGroups = this._ExpMestMetyReqADOsV2.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                        _dataNews2.AddRange(dtGroups);

                        _ExpMestMetyReqADOCommons = _dataNews2;
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                        _ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOsV2.OrderBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                    //objectTag.AddObjectData(store, "Medicines", this._ExpMestMetyReqADOsV2);
                }
                else if (this._ExpMestMetyReqADOs != null
                    && this._ExpMestMetyReqADOs.Count > 0
                    && rdo._WorkPlaceSDO != null)
                {
                    if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                    {
                        List<ExpMestMetyReqADO> _dataNews = new List<ExpMestMetyReqADO>();
                        this._ExpMestMetyReqADOs = this._ExpMestMetyReqADOs.OrderBy(p => p.TYPE_ID).ToList();
                        var dataGroups = this._ExpMestMetyReqADOs.GroupBy(p => p.TYPE_ID).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                            _dataNews.AddRange(dtGroups);
                        }
                        _ExpMestMetyReqADOCommons = _dataNews;
                        //objectTag.AddObjectData(store, "Medicines", _dataNews);
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        List<ExpMestMetyReqADO> _dataNews2 = new List<ExpMestMetyReqADO>();

                        var dtGroups = this._ExpMestMetyReqADOs.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                        _dataNews2.AddRange(dtGroups);

                        _ExpMestMetyReqADOCommons = _dataNews2;
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                    {
                        List<ExpMestMetyReqADO> _dataNews2 = new List<ExpMestMetyReqADO>();

                        var dtGroups = this._ExpMestMetyReqADOs.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                        _dataNews2.AddRange(dtGroups);

                        _ExpMestMetyReqADOCommons = _dataNews2;
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                    {
                        _ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOs.OrderBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                        //objectTag.AddObjectData(store, "Medicines", this._ExpMestMetyReqADOs);
                    }
                }
                else
                {
                    _ExpMestMetyReqADOCommons = this._ExpMestMetyReqADOs.OrderBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ToList();
                    //objectTag.AddObjectData(store, "Medicines", this._ExpMestMetyReqADOs);
                }

                result = (this.templateType == ProcessorBase.PrintConfig.TemplateType.Excel) ? ProcessDataExcel() : ProcessDataWord();

                Inventec.Common.Logging.LogSystem.Debug("Mps000374 ------ ProcessData----2");
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOCommons), _ExpMestMetyReqADOCommons));
                objectTag.AddObjectData(store, "Medicines", _ExpMestMetyReqADOCommons ?? new List<ExpMestMetyReqADO>());
                objectTag.AddObjectData(store, "TrackingADOs", this._Mps000374ADOs);
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
                if (this.MedicineLineADOs == null)
                {
                    this.MedicineLineADOs = new List<MedicineLineADO>();
                }
                objectTag.AddObjectData(store, "MedicineLines", this.MedicineLineADOs);

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
                objectTag.AddObjectData(store, "ExpMestMedicines", this.ExpMestMedicines);
                objectTag.AddObjectData(store, "ExpMestMaterials", this.ExpMestMaterials);
                objectTag.AddObjectData(store, "ExpMesBloods", this.ExpMestBloods);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private bool ProcessDataWord()
        {
            bool success = false;
            try
            {
                Inventec.Common.SharpDocxExport.ProcessSingleTag singleTag = new Inventec.Common.SharpDocxExport.ProcessSingleTag();
                Inventec.Common.SharpDocxExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.SharpDocxExport.ProcessBarCodeTag();
                Inventec.Common.SharpDocxExport.ProcessObjectTag objectTag = new Inventec.Common.SharpDocxExport.ProcessObjectTag();

                Inventec.Common.SharpDocxExport.Store sharpDocxStore = new Inventec.Common.SharpDocxExport.Store();

                Mps000374ADOExt.Mps000374ADOs = (from m in this._Mps000374ADOs select new Mps000374ExtADO(m)).ToList();
                Mps000374ADOExt.WorkPlaceSDO = rdo._WorkPlaceSDO;
                Mps000374ADOExt.SingleValueDictionary = singleValueDictionary;

                foreach (var item in Mps000374ADOExt.Mps000374ADOs)
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

                ////barCodeTag.ProcessData(sharpDocxStore, dicImage);

                success = sharpDocxStore.ReadTemplate(System.IO.Path.GetFullPath(fileName), this.Mps000374ADOExt);
                success = success && singleTag.ProcessData(sharpDocxStore, singleValueDictionary);

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
                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.EXECUTE_TIME_DHST, this._EXECUE_TIME_DHST));
                if (_SingleKeyTamThans != null && _SingleKeyTamThans.Count > 0)
                {
                    for (int i = 0; i < _SingleKeyTamThans.Count; i++)
                    {
                        if (i == 4)
                            break;
                        switch (i)
                        {
                            case 0:
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.NGAY_1, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.Y_LENH_1, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 1:
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.NGAY_2, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.Y_LENH_2, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 2:
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.NGAY_3, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.Y_LENH_3, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 3:
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.NGAY_4, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.Y_LENH_4, _SingleKeyTamThans[i].Y_LENH));
                                break;
                        }
                    }
                }
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._Treatment.TDL_PATIENT_DOB)));
                }
                if (rdo._Trackings != null && rdo._Trackings.Count > 0)
                {
                    var dataTracking = rdo._Trackings.FirstOrDefault();
                    SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.ICD_CODE_BY_TRACKING, dataTracking.ICD_CODE));
                    SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.ICD_NAME_BY_TRACKING, dataTracking.ICD_NAME));
                    SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.ICD_SUB_CODE_BY_TRACKING, dataTracking.ICD_SUB_CODE));
                    SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.ICD_TEXT_BY_TRACKING, dataTracking.ICD_TEXT));

                }
                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.PHONE, ""));
                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.DEPARTMENT_NAME, rdo._WorkPlaceSDO.DepartmentName));
                SetSingleKey(new KeyValue(Mps000374ExtendSingleKey.ROOM_NAME, rdo._WorkPlaceSDO.RoomName));
                AddObjectKeyIntoListkey<Mps000374SingleKey>(rdo._WorkPlaceSDO, false);
                AddObjectKeyIntoListkey<V_HIS_TREATMENT_BED_ROOM>(rdo._TreatmentBedRoom, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._Treatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey07()
        {
            try
            {
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.RATIO_STR, rdo.SingleKeyValue.RatioText));
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, rdo.SingleKeyValue.ExecuteDepartmentName));
                SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.EXECUTE_ROOM_NAME, rdo.SingleKeyValue.ExecuteRoomName));
                if (rdo.SereServs != null)
                {
                    List<HIS_SERE_SERV> clsName = new List<HIS_SERE_SERV>();
                    clsName.AddRange(rdo.SereServs);
                    if (rdo.Treatment != null && rdo.Treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        clsName = clsName.Where(o => o.TDL_INTRUCTION_TIME <= rdo.Treatment.CLINICAL_IN_TIME.Value).ToList();
                    }

                    SetSingleKey(new KeyValue(Mps000007ExtendSingleKey.CLS_NAMES, string.Join("; ", clsName.Select(o => o.TDL_SERVICE_NAME).Distinct().ToList())));
                }

                if (rdo.Treatment != null)
                {
                    if (rdo.Treatment.CLINICAL_IN_TIME != null)
                    {

                        SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.CLINICAL_IN_TIME.Value))));

                    }
                    else
                    {
                        SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.Treatment.IN_TIME))));
                    }
                }

                if (rdo.ExamServiceReq != null)
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.ExamServiceReq.INTRUCTION_TIME))));
                if (!String.IsNullOrEmpty(rdo.Treatment.TRANSFER_IN_MEDI_ORG_CODE))
                {
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ICD_NGT_TEXT, rdo.Treatment.TRANSFER_IN_ICD_NAME)));
                }
                SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ICD_DEPARTMENT_TRAN, rdo.SingleKeyValue.Icd_Name)));

                if (rdo.PatyAlter != null)
                {
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.HEIN_CARD_FROM_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_FROM_TIME.ToString()))));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.HEIN_CARD_TO_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.PatyAlter.HEIN_CARD_TO_TIME.ToString()))));
                }

                SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.LOGIN_USER_NAME, rdo.SingleKeyValue.Username)));
                SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.LOGIN_NAME, rdo.SingleKeyValue.LoginName)));


                if (rdo._currentPatient != null)
                {
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._currentPatient.DOB))));
                }

                if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                {
                    rdo.DepartmentTrans = rdo.DepartmentTrans.OrderByDescending(o => o.DEPARTMENT_IN_TIME).ToList();
                    V_HIS_DEPARTMENT_TRAN departmentTran = null;
                    if (rdo.DepartmentTrans != null && rdo.DepartmentTrans.Count > 0)
                    {
                        var departmentTranTimeNull = rdo.DepartmentTrans.FirstOrDefault(o => o.DEPARTMENT_IN_TIME == null);
                        if (departmentTranTimeNull != null)
                            departmentTran = departmentTranTimeNull;
                        else
                            departmentTran = rdo.DepartmentTrans[0];
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.NEXT_DEPARTMENT_CODE, departmentTran.DEPARTMENT_CODE)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.NEXT_DEPARTMENT_NAME, departmentTran.DEPARTMENT_NAME)));

                }

                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo.Treatment, true);
                AddObjectKeyIntoListkey<PatientADO>(patientADO, true);
                AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlter, true);
                if (rdo.ExamServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.ExamServiceReq, false);
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.PROVISIONAL_DIAGNOSIS, rdo.ExamServiceReq.PROVISIONAL_DIAGNOSIS)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_CODE, rdo.ExamServiceReq.ICD_CODE)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_NAME, rdo.ExamServiceReq.ICD_NAME)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_SUB_CODE, rdo.ExamServiceReq.ICD_SUB_CODE)));
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.ORIGINAL_ICD_TEXT, rdo.ExamServiceReq.ICD_TEXT)));

                }
                AddObjectKeyIntoListkey<HIS_DHST>(rdo.DHST, false);
                if (rdo.DHST != null)
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.DHST_NOTE, rdo.DHST.NOTE)));
                //AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(rdo.TranPati, false);
                //AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.DepartmentTran,false);
                ExpMestBloods = new List<ExpMestBloods>();
                if (rdo.ExpMestBloodList != null && rdo.ExpMestBloodList.Count > 0)
                {
                    string blood = "";
                    var groupBlood = rdo.ExpMestBloodList.GroupBy(o => new { o.BLOOD_TYPE_ID, o.VOLUME }).ToList();
                    foreach (var item in groupBlood)
                    {
                        ExpMestBloods expBlood = new ExpMestBloods();
                        List<V_HIS_EXP_MEST_BLTY_REQ> blty = null;
                        decimal amount = 0;
                        if (rdo.ExpMestBltyReqList != null && rdo.ExpMestBltyReqList.Count > 0)
                        {
                            blty = rdo.ExpMestBltyReqList.Where(o => item.Select(p => p.EXP_MEST_BLTY_REQ_ID).Contains(o.ID)).ToList();
                        }
                        amount = blty != null && blty.Count > 0 ? blty.Sum(o => o.AMOUNT) : 0;

                        expBlood.BLOOD_TYPE_NAME = item.FirstOrDefault().BLOOD_TYPE_NAME;
                        expBlood.VOLUME = item.FirstOrDefault().VOLUME;
                        expBlood.AMOUNT = amount;

                        expBlood.DESCRIPTION = String.Format("Tên: {0} - DT: {1} - SL: {2}; ", item.FirstOrDefault().BLOOD_TYPE_NAME, item.FirstOrDefault().VOLUME, amount);

                        ExpMestBloods.Add(expBlood);
                        blood += String.Format("Tên: {0} - DT: {1} - SL: {2}; ", item.FirstOrDefault().BLOOD_TYPE_NAME, item.FirstOrDefault().VOLUME, amount);
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.EXP_MEST_BLOOD_LIST, blood)));
                }
                this.ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                this.ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                if (rdo.ExpMestMedicineList != null && rdo.ExpMestMedicineList.Count > 0)
                {
                    string medicine = "";
                    var groupMedicine = rdo.ExpMestMedicineList.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.CONCENTRA }).ToList();
                    foreach (var item in groupMedicine)
                    {
                        V_HIS_EXP_MEST_MEDICINE expMedicine = new V_HIS_EXP_MEST_MEDICINE();
                        expMedicine.MEDICINE_TYPE_NAME = item.FirstOrDefault().MEDICINE_TYPE_NAME;
                        expMedicine.CONCENTRA = item.FirstOrDefault().CONCENTRA;
                        expMedicine.AMOUNT = item.Sum(o => o.AMOUNT);
                        expMedicine.SERVICE_UNIT_NAME = item.FirstOrDefault().SERVICE_UNIT_NAME;
                        expMedicine.TUTORIAL = item.FirstOrDefault().TUTORIAL;

                        expMedicine.DESCRIPTION = String.Format("Tên: {0} - HL: {1} - SL: {2} - ĐVT: {3} - HDSD: {4}; ", item.FirstOrDefault().MEDICINE_TYPE_NAME
                            , item.FirstOrDefault().CONCENTRA
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME
                            , item.FirstOrDefault().TUTORIAL);

                        this.ExpMestMedicines.Add(expMedicine);

                        medicine += String.Format("Tên: {0} - HL: {1} - SL: {2} - ĐVT: {3} - HDSD: {4}; ", item.FirstOrDefault().MEDICINE_TYPE_NAME
                            , item.FirstOrDefault().CONCENTRA
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME
                            , item.FirstOrDefault().TUTORIAL);
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.EXP_MEST_MEDICINE_LIST, medicine)));
                }


                if (rdo.ExpMestMaterialList != null && rdo.ExpMestMaterialList.Count > 0)
                {
                    string material = "";
                    var groupMaterial = rdo.ExpMestMaterialList.GroupBy(o => new { o.TDL_MATERIAL_TYPE_ID }).ToList();
                    foreach (var item in groupMaterial)
                    {
                        V_HIS_EXP_MEST_MATERIAL expMaterial = new V_HIS_EXP_MEST_MATERIAL();
                        expMaterial.MATERIAL_TYPE_NAME = item.FirstOrDefault().MATERIAL_TYPE_NAME;
                        expMaterial.AMOUNT = item.Sum(o => o.AMOUNT);
                        expMaterial.SERVICE_UNIT_NAME = item.FirstOrDefault().SERVICE_UNIT_NAME;

                        expMaterial.DESCRIPTION = String.Format("Tên: {0} - SL: {1} - ĐVT: {2}; ", item.FirstOrDefault().MATERIAL_TYPE_NAME
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME);
                        this.ExpMestMaterials.Add(expMaterial);

                        material += String.Format("Tên: {0} - SL: {1} - ĐVT: {2}; ", item.FirstOrDefault().MATERIAL_TYPE_NAME
                            , item.Sum(o => o.AMOUNT)
                            , item.FirstOrDefault().SERVICE_UNIT_NAME);
                    }
                    SetSingleKey((new KeyValue(Mps000007ExtendSingleKey.EXP_MEST_MATERIAL_LIST, material)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void DataInputProcess()
        {
            try
            {
                patientADO = DataRawProcess.PatientRawToADO(rdo._currentPatient);
                patyAlter = DataRawProcess.PatyAlterBHYTRawToADO(rdo.PatyAlter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetBarcodeKey07()
        {
            try
            {

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo.Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.IncludeLabel = false;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000007ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

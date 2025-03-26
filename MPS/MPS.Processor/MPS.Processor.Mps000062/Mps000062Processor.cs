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
using FlexCel.Report;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000062.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000062
{
    public class Mps000062Processor : AbstractProcessor
    {
        internal Mps000062ADOExt mps000062ADOExt = new Mps000062ADOExt();
        internal Mps000062PDO rdo;
        internal List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        internal List<HIS_SERVICE_REQ> lstServiceReqs { get; set; }
        internal V_HIS_TRACKING _tracking { get; set; }
        internal List<Mps000062ADO> _Mps000062ADOs { get; set; }
        internal List<Mps000062ExtADO> _Mps000062ExtADOs { get; set; }
        internal List<ServiceCLS> _ServiceCLSs { get; set; } //Dịch vụ không sắp xếp
        internal List<ServiceCLS> _ServiceCLSOrders { get; set; } //Dịch vụ sắp xếp
        internal List<ServiceCLS> _ServiceCLSSplitXNs { get; set; } //Dịch vụ XN được tách theo loại XN
        internal List<ServiceCLS> _ServiceCLSSplits { get; set; }
        internal List<ServiceCLS> _Bloods { get; set; }
        internal List<ServiceCLS> _ExamServices { get; set; }
        internal List<ServiceCLS> _TTServices { get; set; }
        internal List<RemedyCountADO> _RemedyCountADOs { get; set; }
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommons { get; set; }
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommonsMix { get; set; } // thuốc pha truyền
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommons_Merge { get; set; } // thuốc pha truyền và không pha truyền
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommonsDuTru { get; set; } //thuốc dự trù
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommonsTHDT { get; set; } //thuốc thực hiện dự trù
        internal List<ServiceCLS> _ServiceClsDuTru { get; set; } //thuốc dự trù cls
        internal List<ServiceCLS> _ServiceClsTHDT { get; set; } //thuốc cls thực hiện dự trù
        internal List<ServiceCLS> _ServiceTtDuTru { get; set; } //thuốc dự trù cls
        internal List<ServiceCLS> _ServiceTtTHDT { get; set; } //thuốc cls thực hiện dự trù
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOCommonsDuTru_Merge { get; set; } //thuốc dự trù pha truyền và không pha truyền
        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOs { get; set; }
        internal List<MedicineLineADO> MedicineLineADOs { get; set; }
        internal List<ExpMestMatyReqADO> _ExpMestMatyReqADOs { get; set; }
        internal List<ExpMestMatyReqADO> _ExpMestMatyReqADOsDuTru { get; set; } // Vật tư dự trù
        internal List<ExpMestMatyReqADO> _ExpMestMatyReqADOsTHDT { get; set; } // Vật tư thực hiện dự trù
        internal List<ServiceReqMetyADO> _ServiceReqMetyADOs { get; set; }
        internal List<ServiceReqMatyADO> _ServiceReqMatyADOs { get; set; }
        internal List<MedicalInstruction> _MedicalInstructions { get; set; }
        Dictionary<long, List<NumberDate>> _DicCountNumbers = new Dictionary<long, List<NumberDate>>();
        Dictionary<long, List<NumberDate>> _DicCountNumberByTypes = new Dictionary<long, List<NumberDate>>();
        Dictionary<long, List<NumberDate>> _DicCountNumberByGroup = new Dictionary<long, List<NumberDate>>();//số cả thuốc trong kho và ngoài kho
        Dictionary<long, List<NumberDate>> _DicCountNumberByTypes_InOut = new Dictionary<long, List<NumberDate>>(); //Số theo loại cả thuốc trong kho và ngoài kho
        Dictionary<string, List<NumberDate>> _DicCountNumberActives = new Dictionary<string, List<NumberDate>>(); // đánh số theo hoạt chất

        internal List<ExpMestMetyReqADO> _MediInfusionDutru { get; set; } //thuốc pha truyền dự trù
        internal List<ExpMestMetyReqADO> _MediInfusionTHDT { get; set; } //thuốc pha truyền thực hiện dự trù

        internal List<ExpMestMetyReqADO> _ExpMestMetyReqADOsV2 { get; set; }//Them ghi chu Y lệnh ngày 08/04, 09/04, 10/04

        List<SingleKeyTracking> _SingleKeyTamThans = new List<SingleKeyTracking>();
        SingleKeyTracking keyTamThan { get; set; }

        List<HIS_SERVICE_REQ> ServiceReq;

        List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicinesAll = new List<HIS_EXP_MEST_MEDICINE>();

        internal List<HIS_SERVICE_REQ> _ServiceReqDuTrus { get; set; } // y lệnh dự trù
        internal List<HIS_SERVICE_REQ> _ServiceReqTHDT { get; set; } // y lệnh thực hiện dự trù

        //TH
        internal List<ImpMestMedicineADO> _ImpMestMedicineADOs { get; set; }
        internal List<ImpMestMaterialADO> _ImpMestMaterialADOs { get; set; }
        internal List<ImpMestBloodADO> _ImpMestBloodADOs { get; set; }

        //suat an
        internal List<SereServRationADO> _SereServRationADO { get; set; }

        List<ExpMestADO> _ExpMestADOs { get; set; }

        internal long _EXECUE_TIME_DHST { get; set; }

        string IS_SHOW = "X";

        long sum_medi_mate = 0;

        public Mps000062Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000062PDO)rdoBase;

            Inventec.Common.Logging.LogSystem.Info("rdo._DicExpMestMedicines: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._DicExpMestMedicines.Values), rdo._DicExpMestMedicines.Values));
            Inventec.Common.Logging.LogSystem.Info("rdo._DicExpMestMaterials: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._DicExpMestMaterials.Values), rdo._DicExpMestMaterials.Values));
            Inventec.Common.Logging.LogSystem.Info("rdo._DicServiceReqs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rdo._DicServiceReqs.Values), rdo._DicServiceReqs.Values));

        }
        void SetDataExpMestADO()
        {
            try
            {
                _ExpMestADOs = new List<ExpMestADO>();
                if (rdo._DicHisExpMests != null && rdo._DicHisExpMests.Count > 0)
                {
                    foreach (var item in rdo._DicHisExpMests.Values)
                    {
                        HIS_SERVICE_REQ Servicereq = new HIS_SERVICE_REQ();
                        if (rdo._DicServiceReqs != null && rdo._DicServiceReqs.Count > 0)
                        {
                            Servicereq = rdo._DicServiceReqs.Values.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                        }
                        _ExpMestADOs.Add(new ExpMestADO(item, Servicereq));
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
                if (_ExpMestADOs != null && _ExpMestADOs.Count > 0)
                {
                    _ExpMestMedicinesAll = new List<HIS_EXP_MEST_MEDICINE>();
                    var ExpMests = _ExpMestADOs.OrderBy(p => p.TDL_INTRUCTION_DATE).ThenBy(o => o.USE_TIME).ToList();
                    var ExpMestGroups = ExpMests.GroupBy(o => new { o.TDL_INTRUCTION_DATE, o.USE_TIME }).ToList();//group lại theo ngày chỉ định và ngày dự trù
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
                                _ExpMestMedicinesAll.AddRange(_ExpMestMedicines);
                                var expMestGroup = _ExpMestMedicines.Where(o => o.MIXED_INFUSION == null).GroupBy(p => p.TDL_MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                                foreach (var expMedicine in expMestGroup)
                                {
                                    var check = rdo._MedicineTypes.FirstOrDefault(p => p.ID == expMedicine[0].TDL_MEDICINE_TYPE_ID);
                                    if (check == null)
                                    {
                                        continue;
                                    }

                                    #region STT Ke Thuoc GN HT KS TD Theo loai MEDICINE_GROUP #10061
                                    var medicinegroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();
                                    var medicinegroup = medicinegroups != null ? medicinegroups.FirstOrDefault(o => o.ID == check.MEDICINE_GROUP_ID) : null;

                                    if (medicinegroup != null && medicinegroup.IS_NUMBERED_TRACKING == 1)
                                    {
                                        if (!_DicCountNumbers.ContainsKey(check.MEDICINE_GROUP_ID ?? 0))
                                        {
                                            NumberDate ado = new NumberDate();
                                            ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                            ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                            ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                            ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                            ado.num = num;
                                            ado.MIXED_INFUSION = null;
                                            _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0] = new List<NumberDate>();
                                            _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                        }
                                        else
                                        {
                                            NumberDate ado = new NumberDate();
                                            ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                            ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                            ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
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
                                            ado.MIXED_INFUSION = null;
                                            _DicCountNumbers[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                        }
                                    }
                                    #endregion

                                    #region STT theo loại thuốc cả trong và ngoài kho (việc 47388)
                                    if (!_DicCountNumberByTypes_InOut.ContainsKey(expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        ado.num = num;
                                        ado.MIXED_INFUSION = null;
                                        _DicCountNumberByTypes_InOut[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0] = new List<NumberDate>();
                                        _DicCountNumberByTypes_InOut[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;

                                        var _DataNumber = _DicCountNumberByTypes_InOut[check.ID].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                        if (_DataNumber != null)
                                        {
                                            var _DataNumbers = _DicCountNumberByTypes_InOut[check.ID].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count;
                                        }
                                        else
                                        {
                                            var _DataNumbers = _DicCountNumberByTypes_InOut[check.ID].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count + 1;
                                        }
                                        ado.MIXED_INFUSION = null;

                                        _DicCountNumberByTypes_InOut[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    #endregion

                                    #region STT Ke Thuoc trong kho và ngoài kho theo checkbox đánh số thứ tự 26055
                                    if (medicinegroup != null && medicinegroup.IS_NUMBERED_TRACKING == 1)
                                    {
                                        if (!_DicCountNumberByGroup.ContainsKey(check.MEDICINE_GROUP_ID ?? 0))
                                        {
                                            NumberDate ado = new NumberDate();
                                            ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                            ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                            ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                            ado.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                                            ado.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                                            ado.CONCENTRA = check.CONCENTRA;
                                            ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                            ado.num = num;
                                            ado.MIXED_INFUSION = null;
                                            _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0] = new List<NumberDate>();
                                            _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                        }
                                        else
                                        {
                                            NumberDate ado = new NumberDate();
                                            ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                            ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                            ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                            ado.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                                            ado.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                                            ado.CONCENTRA = check.CONCENTRA;
                                            var _DataNumber = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                            if (_DataNumber != null)
                                            {
                                                var _DataNumbers = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                                ado.num = _DataNumbers.Count;
                                            }
                                            else
                                            {
                                                var _DataNumbers = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                                ado.num = _DataNumbers.Count + 1;
                                            }
                                            ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                            ado.MIXED_INFUSION = null;
                                            _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                        }
                                    }
                                    #endregion

                                    #region STT Ke Thuoc GN HT
                                    if (!_DicCountNumberByTypes.ContainsKey(expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;
                                        ado.num = num;
                                        ado.MIXED_INFUSION = null;
                                        _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0] = new List<NumberDate>();
                                        _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.TDL_INTRUCTION_DATE ?? 0;
                                        ado.MEDICINE_TYPE_ID = expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0;

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
                                        ado.MIXED_INFUSION = null;

                                        _DicCountNumberByTypes[expMedicine.FirstOrDefault().TDL_MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    #endregion
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

        /// <summary>
        /// ---- đánh số thứ tự theo hoạt chất V+ 127097----
        /// </summary>
        void CheckSTTByActive()
        {
            try
            {
                if (_ExpMestADOs != null && _ExpMestADOs.Count > 0)
                {
                    var ExpMests = _ExpMestADOs.OrderBy(p => p.USE_TIME_AND_INTRUCTION_TIME).ToList();
                    var ExpMestGroups = ExpMests.GroupBy(o => o.USE_TIME_AND_INTRUCTION_TIME).ToList();
                    int num = 1;
                    foreach (var itemGroups in ExpMestGroups)
                    {
                        List<long> _expMestIds = new List<long>();
                        _expMestIds = itemGroups.Select(p => p.ID).Distinct().ToList();
                        if (rdo._DicExpMestMedicines != null && rdo._DicExpMestMedicines.Count > 0)
                        {
                            List<ExpMestMedicineADO> _ExpMestMedicineADOs = new List<ExpMestMedicineADO>();
                            foreach (var item in _expMestIds)
                            {
                                if (rdo._DicExpMestMedicines.ContainsKey(item))
                                {
                                    foreach (var itemDic in rdo._DicExpMestMedicines[item])
                                    {
                                        ExpMestMedicineADO Ado = new ExpMestMedicineADO(itemDic, rdo._MedicineTypes);
                                        _ExpMestMedicineADOs.Add(Ado);
                                    }
                                }
                            }

                            if (_ExpMestMedicineADOs != null && _ExpMestMedicineADOs.Count > 0)
                            {
                                var expMestGroup = _ExpMestMedicineADOs.GroupBy(p => p.ACTIVE_INGR_BHYT_CODE).Select(p => p.ToList()).ToList();
                                foreach (var expMedicine in expMestGroup)
                                {
                                    if (String.IsNullOrEmpty(expMedicine[0].ACTIVE_INGR_BHYT_CODE))
                                    {
                                        continue;
                                    }

                                    if (!_DicCountNumberActives.ContainsKey(expMedicine[0].ACTIVE_INGR_BHYT_CODE))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.ACTIVE_INGR_BHYT_CODE = expMedicine[0].ACTIVE_INGR_BHYT_CODE;
                                        ado.ACTIVE_INGR_BHYT_NAME = expMedicine[0].ACTIVE_INGR_BHYT_NAME;
                                        ado.num = num;
                                        ado.MIXED_INFUSION = null;
                                        _DicCountNumberActives[expMedicine[0].ACTIVE_INGR_BHYT_CODE] = new List<NumberDate>();
                                        _DicCountNumberActives[expMedicine[0].ACTIVE_INGR_BHYT_CODE].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key ?? 0;
                                        ado.ACTIVE_INGR_BHYT_CODE = expMedicine[0].ACTIVE_INGR_BHYT_CODE;
                                        ado.ACTIVE_INGR_BHYT_NAME = expMedicine[0].ACTIVE_INGR_BHYT_NAME;
                                        var _DataNumber = _DicCountNumberActives[expMedicine[0].ACTIVE_INGR_BHYT_CODE].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                        if (_DataNumber != null)
                                        {
                                            var _DataNumbers = _DicCountNumberActives[expMedicine[0].ACTIVE_INGR_BHYT_CODE].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count;
                                        }
                                        else
                                        {
                                            var _DataNumbers = _DicCountNumberActives[expMedicine[0].ACTIVE_INGR_BHYT_CODE].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count + 1;
                                        }
                                        ado.MIXED_INFUSION = null;
                                        _DicCountNumberActives[expMedicine[0].ACTIVE_INGR_BHYT_CODE].Add(ado);
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

        /// <summary>
        /// ---- STT Ke Thuoc trong kho và ngoài kho theo checkbox đánh số thứ tự 26055----
        /// </summary>
        private void CheckSTTMedicineOutStock()
        {
            try
            {
                if (rdo._DicServiceReqs != null && rdo._DicServiceReqs.Count > 0)
                {
                    var ServiceReqs = rdo._DicServiceReqs.Values.OrderBy(p => p.INTRUCTION_DATE).ThenBy(o => o.USE_TIME).ToList();
                    var ServiceReqGroups = ServiceReqs.GroupBy(o => new { o.INTRUCTION_DATE, o.USE_TIME }).ToList();//group lại theo ngày chỉ định và ngày dự trù
                    int num = 1;
                    foreach (var itemGroups in ServiceReqGroups)
                    {
                        List<long> _ServiceReqIds = new List<long>();
                        _ServiceReqIds = itemGroups.Select(p => p.ID).Distinct().ToList();
                        if (rdo._DicServiceReqMetys != null && rdo._DicServiceReqMetys.Count > 0)
                        {
                            List<HIS_SERVICE_REQ_METY> _ServiceReqMetys = new List<HIS_SERVICE_REQ_METY>();
                            foreach (var item in _ServiceReqIds)
                            {
                                if (rdo._DicServiceReqMetys.ContainsKey(item))
                                {
                                    _ServiceReqMetys.AddRange(rdo._DicServiceReqMetys[item]);
                                }
                            }

                            if (_ServiceReqMetys != null && _ServiceReqMetys.Count > 0)
                            {

                                var ServiceReqMetyGroup = _ServiceReqMetys.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                                foreach (var ReqMety in ServiceReqMetyGroup)
                                {
                                    var check = rdo._MedicineTypes.FirstOrDefault(p => p.ID == ReqMety[0].MEDICINE_TYPE_ID);
                                    if (check == null)
                                    {
                                        continue;
                                    }

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

                                    #region STT Ke Thuoc trong kho và ngoài kho theo checkbox đánh số thứ tự 26055
                                    if (!_DicCountNumberByGroup.ContainsKey(check.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.INTRUCTION_DATE;
                                        ado.MEDICINE_TYPE_ID = ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0;
                                        ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                        ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                        ado.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                                        ado.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                                        ado.CONCENTRA = check.CONCENTRA;
                                        ado.num = num;
                                        _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0] = new List<NumberDate>();
                                        _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.INTRUCTION_DATE;
                                        ado.MEDICINE_TYPE_ID = ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0;
                                        ado.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID ?? 0;
                                        ado.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                                        ado.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                                        ado.CONCENTRA = check.CONCENTRA;
                                        var _DataNumber = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                        if (_DataNumber != null)
                                        {
                                            var _DataNumbers = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Where(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE).OrderByDescending(p => p.num).FirstOrDefault();
                                            ado.num = _DataNumbers != null ? _DataNumbers.num + 1 : 0;
                                        }
                                        else
                                        {
                                            var _DataNumbers = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Where(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE).OrderByDescending(p => p.num).FirstOrDefault();
                                            ado.num = _DataNumbers != null ? _DataNumbers.num + 1 : 0;
                                        }
                                        ado.Num_Order = (short)(medicinegroup != null ? medicinegroup.NUM_ORDER ?? 0 : 0);
                                        _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Add(ado);

                                        var _DataNumberDate = _DicCountNumberByGroup[check.MEDICINE_GROUP_ID ?? 0].Where(p => p.INTRUCTION_DATE > (itemGroups.Key.USE_TIME ?? itemGroups.Key.INTRUCTION_DATE)).OrderBy(o => o.INTRUCTION_DATE).ToList();

                                        if (_DataNumberDate != null && _DataNumberDate.Count() > 0)
                                        {
                                            foreach (var data in _DataNumberDate)
                                            {
                                                data.num += 1;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region STT theo loại thuốc cả trong và ngoài kho (việc 47388)
                                    if (!_DicCountNumberByTypes_InOut.ContainsKey(ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0))
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.INTRUCTION_DATE;
                                        ado.MEDICINE_TYPE_ID = ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0;
                                        ado.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                                        ado.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                                        ado.CONCENTRA = check.CONCENTRA;
                                        ado.num = num;
                                        ado.MIXED_INFUSION = null;
                                        _DicCountNumberByTypes_InOut[ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0] = new List<NumberDate>();
                                        _DicCountNumberByTypes_InOut[ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    else
                                    {
                                        NumberDate ado = new NumberDate();
                                        ado.INTRUCTION_DATE = itemGroups.Key.USE_TIME ?? itemGroups.Key.INTRUCTION_DATE;
                                        ado.MEDICINE_TYPE_ID = ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0;
                                        ado.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                                        ado.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                                        ado.CONCENTRA = check.CONCENTRA;
                                        var _DataNumber = _DicCountNumberByTypes_InOut[check.ID].FirstOrDefault(p => p.INTRUCTION_DATE == ado.INTRUCTION_DATE);
                                        if (_DataNumber != null)
                                        {
                                            var _DataNumbers = _DicCountNumberByTypes_InOut[check.ID].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count;
                                        }
                                        else
                                        {
                                            var _DataNumbers = _DicCountNumberByTypes_InOut[check.ID].Select(p => p.INTRUCTION_DATE).Distinct().ToList();
                                            ado.num = _DataNumbers.Count + 1;
                                        }
                                        ado.MIXED_INFUSION = null;

                                        _DicCountNumberByTypes_InOut[ReqMety.FirstOrDefault().MEDICINE_TYPE_ID ?? 0].Add(ado);
                                    }
                                    #endregion
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

        private void ProcessorDataPrint()
        {
            try
            {
                _ExpMestMetyReqADOCommons = new List<ExpMestMetyReqADO>();
                _ExpMestMetyReqADOCommonsMix = new List<ExpMestMetyReqADO>();
                _Mps000062ADOs = new List<Mps000062ADO>();
                _Mps000062ExtADOs = new List<Mps000062ExtADO>();
                _ExpMestMetyReqADOs = new List<ExpMestMetyReqADO>();
                _ServiceCLSs = new List<ServiceCLS>();
                _ServiceCLSSplits = new List<ServiceCLS>();
                _ServiceCLSOrders = new List<ServiceCLS>();
                _ServiceCLSSplitXNs = new List<ServiceCLS>();
                _Bloods = new List<ServiceCLS>();
                _ExamServices = new List<ServiceCLS>();
                _TTServices = new List<ServiceCLS>();
                _RemedyCountADOs = new List<RemedyCountADO>();
                _ExpMestMatyReqADOs = new List<ExpMestMatyReqADO>();
                _ServiceReqMetyADOs = new List<ServiceReqMetyADO>();
                _ServiceReqMatyADOs = new List<ServiceReqMatyADO>();
                _ImpMestMedicineADOs = new List<ImpMestMedicineADO>();
                _ImpMestMaterialADOs = new List<ImpMestMaterialADO>();
                _ImpMestBloodADOs = new List<ImpMestBloodADO>();
                _MedicalInstructions = new List<MedicalInstruction>();

                _ExpMestMetyReqADOsV2 = new List<ExpMestMetyReqADO>();
                _SereServRationADO = new List<SereServRationADO>();

                lstServiceReqs = new List<HIS_SERVICE_REQ>();

                ServiceReq = new List<HIS_SERVICE_REQ>();
                _ServiceReqDuTrus = new List<HIS_SERVICE_REQ>();
                _ServiceReqTHDT = new List<HIS_SERVICE_REQ>();

                _ExpMestMetyReqADOCommons_Merge = new List<ExpMestMetyReqADO>();
                _ExpMestMetyReqADOCommonsDuTru_Merge = new List<ExpMestMetyReqADO>();

                if (rdo._Trackings != null && rdo._Trackings.Count > 0)
                {
                    foreach (var itemTracking in rdo._Trackings)
                    {
                        this.IS_SHOW = "X";
                        this.sum_medi_mate = 0;
                        _tracking = new V_HIS_TRACKING();
                        _tracking = itemTracking;
                        keyTamThan = new SingleKeyTracking();

                        #region Thông tin chẩn đoán tờ điều trị
                        string icd_Name = "", icd_text = "";
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

                        icd_text = itemTracking.ICD_TEXT;


                        #endregion

                        #region TrackingTime
                        Mps000062ADO _service = new Mps000062ADO();

                        Mapper.CreateMap<V_HIS_TRACKING, Mps000062ADO>();
                        _service = Mapper.Map<V_HIS_TRACKING, Mps000062ADO>(itemTracking);
                        _service.TRACKING_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(itemTracking.TRACKING_TIME);
                        var CheckTrackingTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(itemTracking.TRACKING_TIME);
                        _service.IS_T7_OR_CN = (CheckTrackingTime.Value.DayOfWeek == DayOfWeek.Sunday || CheckTrackingTime.Value.DayOfWeek == DayOfWeek.Saturday) ? "X" : "";
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

                        if (rdo._DicServiceReqs != null)
                        {
                            ServiceReq = rdo._DicServiceReqs.Values.Distinct().ToList();
                            var ser = rdo._DicServiceReqs.Values.Where(p => p.TRACKING_ID == itemTracking.ID || p.USED_FOR_TRACKING_ID == itemTracking.ID).ToList();
                            if (ser != null && ser.Count > 0)
                            {
                                _ServiceReqs = ser.ToList();
                                lstServiceReqs.AddRange(ser.ToList());
                            }

                            var serDuTru = rdo._DicServiceReqs.Values.Where(p => p.TRACKING_ID == itemTracking.ID && (p.USE_TIME ?? 0) > p.INTRUCTION_DATE && (p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)).ToList();
                            if (serDuTru != null && serDuTru.Count > 0)
                            {
                                _ServiceReqDuTrus.AddRange(serDuTru);
                                _ServiceReqDuTrus = _ServiceReqDuTrus.OrderBy(o => o.INTRUCTION_TIME).ThenBy(p => p.USE_TIME).ThenBy(n => n.ID).ToList();
                            }

                            var serTHDT = rdo._DicServiceReqs.Values.Where(p => p.USED_FOR_TRACKING_ID == itemTracking.ID && (p.USE_TIME ?? 0) > p.INTRUCTION_DATE && (p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                        || p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)).ToList();
                            if (serTHDT != null && serTHDT.Count > 0)
                            {
                                _ServiceReqTHDT.AddRange(serTHDT);
                                _ServiceReqTHDT = _ServiceReqTHDT.OrderBy(o => o.INTRUCTION_TIME).ThenBy(p => p.USE_TIME).ThenBy(n => n.ID).ToList();
                            }
                        }
                        else
                        {
                            return;
                        }
                        lstServiceReqs.Distinct().ToList();
                        Inventec.Common.Logging.LogSystem.Warn("ServiceReqIds: " + itemTracking.ID);
                        Inventec.Common.Logging.LogSystem.Info("ProcessorDataPrint: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ServiceReqTHDT), _ServiceReqTHDT));
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
                        _service.ICD_TEXT_TRACKING = icd_text;

                        if (itemTracking.TRACKING_TIME != null && rdo._Treatment != null && rdo._Treatment.CLINICAL_IN_TIME != null)
                        {
                            System.DateTime TrackingTime = System.DateTime.ParseExact(itemTracking.TRACKING_TIME.ToString(), "yyyyMMddHHmmss",
                                         System.Globalization.CultureInfo.InvariantCulture);
                            System.DateTime ClinicalInTime = System.DateTime.ParseExact(rdo._Treatment.CLINICAL_IN_TIME.ToString(), "yyyyMMddHHmmss",
                                         System.Globalization.CultureInfo.InvariantCulture);

                            _service.NUMBER_DAYS_TREATMENT = (long)((TimeSpan)(TrackingTime.Date - ClinicalInTime.Date)).TotalDays + 1;
                        }

                        _Mps000062ADOs.Add(_service);

                        this.ProcessMedicineLine();
                    }
                }

                if (rdo._SereServRations != null && rdo._SereServRations.Count > 0)
                {
                    var Ration = rdo._SereServRations.GroupBy(o => o.TRACKING_ID).Select(p => p.ToList()).ToList();

                    foreach (var itemRation in Ration)
                    {
                        List<string> RationInfos = new List<string>();
                        SereServRationADO ado = new SereServRationADO(itemRation[0]);
                        foreach (var item in itemRation)
                        {
                            RationInfos.Add(item.RATION_TIME_NAME + " : " + item.SERVICE_CODE);
                        }

                        if (RationInfos != null && RationInfos.Count > 0)
                        {
                            ado.RATION_INFO = String.Join("; ", RationInfos);
                        }
                        _SereServRationADO.Add(ado);
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
            Thread threadServicesOrder = new Thread(ServiceOrders);
            // Thread threadServiceReqMetys = new Thread(ServiceReqMetys);
            //  Thread threadServiceReqMatys = new Thread(ServiceReqMatys);
            Thread threadBloos = new Thread(Bloods);

            try
            {
                threadMedicines.Start();
                threadMaterials.Start();
                threadServices.Start();
                threadServicesOrder.Start();
                //threadServiceReqMetys.Start();
                // threadServiceReqMatys.Start();
                threadBloos.Start();

                threadMedicines.Join();
                threadMaterials.Join();
                threadServices.Join();
                threadServicesOrder.Join();
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
                threadServicesOrder.Abort();
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


                        var _expMestMetyReqGroups = _expMestMedicines.GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.TUTORIAL }).Select(p => p.ToList()).ToList();
                        this.sum_medi_mate += _expMestMetyReqGroups.Count();

                        int d = 0;


                        foreach (var itemExpMestMetyReq in _expMestMetyReqGroups)
                        {
                            ExpMestMetyReqADO group = new ExpMestMetyReqADO(itemExpMestMetyReq[0]);
                            group.TRACKING_ID = _tracking.ID;

                            DateTime? dtIntructionTime = null;

                            if (itemByServiceReq.USE_TIME.HasValue)
                                dtIntructionTime = System.DateTime.ParseExact(itemByServiceReq.USE_TIME.ToString(), "yyyyMMddHHmmss",
                                      System.Globalization.CultureInfo.InvariantCulture);
                            else if (itemExpMest.TDL_INTRUCTION_TIME > 0)
                                dtIntructionTime = System.DateTime.ParseExact(itemExpMest.TDL_INTRUCTION_TIME.ToString(), "yyyyMMddHHmmss",
                                     System.Globalization.CultureInfo.InvariantCulture);
                            if (group.USE_TIME_TO > 0 && dtIntructionTime != null)
                            {

                                DateTime dtUserTimeTo = System.DateTime.ParseExact(group.USE_TIME_TO.ToString(), "yyyyMMddHHmmss",
                                                       System.Globalization.CultureInfo.InvariantCulture);
                                TimeSpan ts = new TimeSpan();
                                ts = (TimeSpan)(dtUserTimeTo - dtIntructionTime);
                                if (ts != null && ts.Days >= 0)
                                {
                                    group.NUMBER_USE_DAY = ts.Days + 1;
                                }
                            }

                            //group.NUMBER_USE_DAY = group.USE_TIME_TO - itemExpMest.TDL_INTRUCTION_TIME + 1;

                            var expMestMedicinesDutru = itemExpMestMetyReq.Where(o => _ServiceReqDuTrus.Select(p => p.ID).Contains(o.TDL_SERVICE_REQ_ID ?? 0)).ToList();

                            if (expMestMedicinesDutru != null && expMestMedicinesDutru.Count > 0)
                            {
                                group.AMOUNTDuTru = expMestMedicinesDutru.Sum(p => p.AMOUNT);
                            }

                            var expMestMedicinesTHDT = itemExpMestMetyReq.Where(o => _ServiceReqTHDT.Select(p => p.ID).Contains(o.TDL_SERVICE_REQ_ID ?? 0)).ToList();

                            if (expMestMedicinesTHDT != null && expMestMedicinesTHDT.Count > 0)
                            {
                                group.AMOUNTTHDT = expMestMedicinesTHDT.Sum(p => p.AMOUNT);
                            }

                            group.AMOUNT = itemExpMestMetyReq.Sum(p => p.AMOUNT);
                            var medicineTypeName = rdo._MedicineTypes.FirstOrDefault(p => p.ID == group.TDL_MEDICINE_TYPE_ID);
                            if (medicineTypeName != null)
                            {
                                group.MEDICINE_USE_FORM_ID = medicineTypeName.MEDICINE_USE_FORM_ID;
                                group.DOSAGE_FORM_NAME = medicineTypeName.DOSAGE_FORM;
                                group.MEDICINE_GROUP_ID = medicineTypeName.MEDICINE_GROUP_ID;

                                group.MEDICINE_TYPE_NAME = medicineTypeName.MEDICINE_TYPE_NAME;
                                group.MEDICINE_TYPE_CODE = medicineTypeName.MEDICINE_TYPE_CODE;
                                //if (rdo._WorkPlaceSDO != null && rdo._WorkPlaceSDO.IsShowMedicineLine)
                                //{
                                group.MEDICINE_LINE_ID = medicineTypeName.MEDICINE_LINE_ID != null ? medicineTypeName.MEDICINE_LINE_ID : 0;
                                group.MEDICINE_LINE_CODE = medicineTypeName.MEDICINE_LINE_CODE;
                                group.MEDICINE_LINE_NAME = string.IsNullOrEmpty(medicineTypeName.MEDICINE_LINE_NAME) ? "Không xác định" : medicineTypeName.MEDICINE_LINE_NAME;
                                group.MEDICINE_LINE_NUM_ORDER = medicineTypeName.MEDICINE_LINE_NUM_ORDER;
                                //}

                                group.SERVICE_UNIT_CODE = medicineTypeName.SERVICE_UNIT_CODE;
                                group.SERVICE_UNIT_NAME = medicineTypeName.SERVICE_UNIT_NAME;
                                group.MEDICINE_USE_FORM_NAME = medicineTypeName.MEDICINE_USE_FORM_NAME;
                                group.CONCENTRA = medicineTypeName.CONCENTRA;

                                group.IS_NOT_TREATMENT_DAY_COUNT = medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT;

                                group.INTRUCTION_DATE = itemServiceReq.INTRUCTION_DATE;
                                group.INTRUCTION_TIME = itemServiceReq.INTRUCTION_TIME;
                                group.ASSIGN_TIME_TO = itemServiceReq.ASSIGN_TIME_TO;
                                group.USED_FOR_TRACKING_ID = itemServiceReq.USED_FOR_TRACKING_ID;

                                group.USE_TIME = itemServiceReq.USE_TIME;
                                group.SERVICE_REQ_TYPE_ID = itemServiceReq.SERVICE_REQ_TYPE_ID;
                                group.USE_TIME_AND_INTRUCTION_TIME = itemServiceReq.USE_TIME.HasValue ? (itemServiceReq.USE_TIME / 1000000) : (itemServiceReq.INTRUCTION_TIME / 1000000);

                                //group.INTRUCTION_TIME_STR = itemServiceReq.INTRUCTION_TIME;

                                var userForm = rdo._MedicineUseForms.FirstOrDefault(p => p.ID == medicineTypeName.MEDICINE_USE_FORM_ID && p.IS_ACTIVE == 1);
                                group.MEDICINE_USE_FORM_ID = medicineTypeName.MEDICINE_USE_FORM_ID;
                                group.ACTIVE_INGR_BHYT_CODE = medicineTypeName.ACTIVE_INGR_BHYT_CODE;
                                group.ACTIVE_INGR_BHYT_NAME = medicineTypeName.ACTIVE_INGR_BHYT_NAME;
                                group.NUM_ORDER_BY_USE_FORM = userForm != null ? userForm.NUM_ORDER ?? 0 : 0;

                                group.CONVERT_RATIO = medicineTypeName.CONVERT_RATIO;
                                group.CONVERT_UNIT_CODE = medicineTypeName.CONVERT_UNIT_CODE;
                                group.CONVERT_UNIT_NAME = medicineTypeName.CONVERT_UNIT_NAME;
                                group.CONVERT_AMOUNT = group.AMOUNT * medicineTypeName.CONVERT_RATIO;

                                #region ---- MEDICINE_GROUP_ID------

                                if (medicineTypeName.IS_TREATMENT_DAY_COUNT == 1 && _DicCountNumberByTypes != null && _DicCountNumberByTypes.Count > 0
                                   && _DicCountNumberByTypes.ContainsKey(medicineTypeName.ID) && medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                                {
                                    var _DataNumberByType = _DicCountNumberByTypes[medicineTypeName.ID].FirstOrDefault(p => p.INTRUCTION_DATE == (itemServiceReq.USE_TIME ?? itemServiceReq.INTRUCTION_DATE) && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                    Inventec.Common.Logging.LogSystem.Debug("medicineTypeName.IS_TREATMENT_DAY_COUNT: " + medicineTypeName.IS_TREATMENT_DAY_COUNT);
                                    if (_DataNumberByType != null)
                                    {
                                        group.NUMBER_BY_TYPE = (long)_DataNumberByType.num;
                                    }
                                }

                                if (medicineTypeName.IS_TREATMENT_DAY_COUNT == 1 && _DicCountNumberByTypes_InOut != null && _DicCountNumberByTypes_InOut.Count > 0
                               && _DicCountNumberByTypes_InOut.ContainsKey(medicineTypeName.ID) && medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                                {
                                    var _DataNumberByType_InOut = _DicCountNumberByTypes_InOut[medicineTypeName.ID].FirstOrDefault(p => p.INTRUCTION_DATE == (itemServiceReq.USE_TIME ?? itemServiceReq.INTRUCTION_DATE) && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);

                                    if (_DataNumberByType_InOut != null)
                                    {
                                        group.NUMBER_BY_TYPE_IN_OUT = (long)_DataNumberByType_InOut.num;
                                    }
                                }

                                if (medicineTypeName.IS_TREATMENT_DAY_COUNT == 1 && _DicCountNumberActives != null && _DicCountNumberActives.Count > 0
                                       && !String.IsNullOrEmpty(medicineTypeName.ACTIVE_INGR_BHYT_CODE) && medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                                {
                                    if (_DicCountNumberActives.ContainsKey(medicineTypeName.ACTIVE_INGR_BHYT_CODE))
                                    {
                                        var _DataNumber = _DicCountNumberActives[medicineTypeName.ACTIVE_INGR_BHYT_CODE].FirstOrDefault(p => p.INTRUCTION_DATE == group.USE_TIME_AND_INTRUCTION_TIME && p.ACTIVE_INGR_BHYT_CODE == medicineTypeName.ACTIVE_INGR_BHYT_CODE);
                                        if (_DataNumber != null)
                                        {
                                            group.NUMBER_ACTIVE_INGR = (long)_DataNumber.num;
                                        }
                                    }
                                }
                                else
                                    group.NUMBER_ACTIVE_INGR = null;

                                if (medicineTypeName.MEDICINE_GROUP_ID > 0)
                                {
                                    group.MEDICINE_GROUP_ID = medicineTypeName.MEDICINE_GROUP_ID;

                                    group.GROUP_BHYT = "X";

                                    var MedicineGroup = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == medicineTypeName.MEDICINE_GROUP_ID);

                                    group.MEDICINE_GROUP_NUM_ORDER = (MedicineGroup != null) ? MedicineGroup.NUM_ORDER : null;

                                    group.IS_NUMBERED_TRACKING = (MedicineGroup != null) ? MedicineGroup.IS_NUMBERED_TRACKING : null;

                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _DicCountNumbers), _DicCountNumbers)
                                        + "____medicineTypeName.MEDICINE_GROUP_ID=" + medicineTypeName.MEDICINE_GROUP_ID);
                                    if (_DicCountNumbers != null && _DicCountNumbers.Count > 0
                                        && _DicCountNumbers.ContainsKey(medicineTypeName.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        var _DataNumber = _DicCountNumbers[medicineTypeName.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == (itemServiceReq.USE_TIME ?? itemServiceReq.INTRUCTION_DATE) && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                        if (_DataNumber != null)
                                        {
                                            //group.Num_Order_by_group = _DataNumber.Num_Order;
                                            //group.Num_Order_by_group dùng để săp xếp thuốc Theo loại thuốc: Thuốc phóng xạ >> Thuốc độc >> Thuốc hướng thần, gây nghiện, corticoid, kháng sinh (sắp xếp theo đường dùng quy định ở TT23)>> Thuốc thường (sắp xếp theo đường dùng quy định ở TT23)
                                            if (medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                                            {
                                                group.NUMBER_H_N = (long)_DataNumber.num;
                                            }

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

                                    if (_DicCountNumberByGroup != null && _DicCountNumberByGroup.Count > 0
                                        && _DicCountNumberByGroup.ContainsKey(medicineTypeName.MEDICINE_GROUP_ID ?? 0))
                                    {
                                        var _DataNumberByGroup = _DicCountNumberByGroup[medicineTypeName.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == (itemServiceReq.USE_TIME ?? itemServiceReq.INTRUCTION_DATE) && p.MEDICINE_TYPE_ID == itemExpMestMetyReq[0].TDL_MEDICINE_TYPE_ID);
                                        if (_DataNumberByGroup != null)
                                        {
                                            if (medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                                            {
                                                group.NUMBER_BY_GROUP = (long)_DataNumberByGroup.num;
                                            }
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
                                if (_amount >= 1 && _amount < 10)
                                {
                                    amount_str = "0" + _amount;
                                }
                            }
                            group.AMOUNT_STR = amount_str;

                            group.TUTORIAL_WITH_ENTER = group.TUTORIAL;

                            if (group.AMOUNT - group.TH_AMOUNT == 0)
                            {
                                group.AMOUNT_TH = "Đã thu hồi";
                            }
                            else
                            {
                                group.AMOUNT_TEXT = "x " + group.AMOUNT + " " + group.SERVICE_UNIT_NAME;
                            }

                            if (group.TH_AMOUNT > 0)
                            {
                                group.TH_AMOUNT_STR = "(Thu hồi " + group.TH_AMOUNT + " " + group.SERVICE_UNIT_NAME + ")";
                            }
                            group.TDL_SERVICE_REQ_ID = itemServiceReq.ID;
                            _ExpMestMetyReqADOs.Add(group);
                            __ExpMestMetyReqADO_V2s.Add(group);

                            keyTamThan.Y_LENH += " - " + group.MEDICINE_TYPE_NAME + " x" + amount_str + " " + group.SERVICE_UNIT_NAME + ", " + group.MEDICINE_USE_FORM_NAME + "\r\n" + "     " + group.TUTORIAL + "\r\n";
                        }
                        _ExpMestMetyReqADOs = ProcessSortListExpMestMetyReq(_ExpMestMetyReqADOs);
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
                            _ExpMestMetyReqADOsV2 = ProcessSortListExpMestMetyReq(_ExpMestMetyReqADOsV2);
                        }


                        #region ---- new -----
                        var medicineGroupsV2 = __ExpMestMetyReqADO_V2s.GroupBy(o => new { o.MEDICINE_LINE_ID, o.EXP_MEST_ID, o.TDL_SERVICE_REQ_ID });
                        foreach (var g in medicineGroupsV2)
                        {
                            RemedyCountADO remedyCount = new RemedyCountADO();
                            remedyCount.MEDICINE_LINE_ID = g.First().MEDICINE_LINE_ID;
                            remedyCount.TRACKING_ID = _tracking.ID;

                            if (_RemedyCountADOs.Where(o => o.EXP_MEST_ID == g.FirstOrDefault().EXP_MEST_ID).Count() >= 1)
                            {
                                remedyCount.EXP_MEST_ID = 0;
                            }
                            else
                            {
                                remedyCount.EXP_MEST_ID = g.FirstOrDefault().EXP_MEST_ID ?? 0;
                            }
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
                        if (itemByServiceReq.IS_NOT_SHOW_OUT_MEDI_TRACKING == 1)
                            continue;
                        long item = itemByServiceReq.ID;
                        if (rdo._DicServiceReqMetys.ContainsKey(item))
                        {
                            dataMety.AddRange(rdo._DicServiceReqMetys[item]);
                        }
                    }
                    if (dataMety != null && dataMety.Count > 0)
                    {
                        _ServiceReqMetyADOs.AddRange((from r in dataMety where r.IS_SUB_PRES != 1 select new ServiceReqMetyADO(r, _tracking.ID, _ServiceReqs)).ToList());
                    }

                    if (_ServiceReqMetyADOs != null && _ServiceReqMetyADOs.Count > 0)
                    {
                        foreach (var item in _ServiceReqMetyADOs)
                        {
                            var check = rdo._MedicineTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            item.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                            item.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                            item.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID;
                            item.CONCENTRA = check.CONCENTRA;
                            var medicineTypeName = rdo._MedicineTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (_DicCountNumberByGroup != null && _DicCountNumberByGroup.Count > 0
                                && _DicCountNumberByGroup.ContainsKey(medicineTypeName.MEDICINE_GROUP_ID ?? 0))
                            {
                                var _DataNumberByGroup = _DicCountNumberByGroup[medicineTypeName.MEDICINE_GROUP_ID ?? 0].FirstOrDefault(p => p.INTRUCTION_DATE == (item.USE_TIME ?? item.INTRUCTION_TIME_STR) && p.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);
                                if (_DataNumberByGroup != null)
                                {
                                    if (medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                                    {
                                        item.NUMBER_BY_GROUP = (long)_DataNumberByGroup.num;
                                    }
                                }
                            }

                            if (medicineTypeName.IS_TREATMENT_DAY_COUNT == 1 && _DicCountNumberByTypes_InOut != null && _DicCountNumberByTypes_InOut.Count > 0
                              && _DicCountNumberByTypes_InOut.ContainsKey(medicineTypeName.ID) && medicineTypeName.IS_NOT_TREATMENT_DAY_COUNT != 1)
                            {
                                var _DataNumberByType_InOut = _DicCountNumberByTypes_InOut[medicineTypeName.ID].FirstOrDefault(p => p.INTRUCTION_DATE == (item.USE_TIME ?? item.INTRUCTION_TIME_STR) && p.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID);

                                if (_DataNumberByType_InOut != null)
                                {
                                    item.NUMBER_BY_TYPE_IN_OUT = (long)_DataNumberByType_InOut.num;
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
                        foreach (var r in dataMaty)
                        {
                            if (r.IS_SUB_PRES != 1)
                            {
                                var materialTypeName = rdo._MaterialTypes.FirstOrDefault(p => p.ID == r.MATERIAL_TYPE_ID);
                                if (materialTypeName != null)
                                {
                                    if (materialTypeName.IS_NOT_SHOW_TRACKING == 1)
                                        continue;
                                }
                                _ServiceReqMatyADOs.Add(new ServiceReqMatyADO(r, this._tracking.ID, _ServiceReqs));
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
                                    if (materialTypeName.IS_NOT_SHOW_TRACKING == 1)
                                        continue;
                                    ado.MATERIAL_TYPE_NAME = materialTypeName.MATERIAL_TYPE_NAME;
                                    ado.MATERIAL_TYPE_CODE = materialTypeName.MATERIAL_TYPE_CODE;
                                    ado.SERVICE_UNIT_CODE = materialTypeName.SERVICE_UNIT_CODE;
                                    ado.SERVICE_UNIT_NAME = materialTypeName.SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = materialTypeName.CONCENTRA;
                                    ado.CONVERT_RATIO = materialTypeName.CONVERT_RATIO;
                                    ado.CONVERT_UNIT_CODE = materialTypeName.CONVERT_UNIT_CODE;
                                    ado.CONVERT_UNIT_NAME = materialTypeName.CONVERT_UNIT_NAME;
                                    ado.CONVERT_AMOUNT = ado.AMOUNT * materialTypeName.CONVERT_RATIO;
                                }
                                decimal totalNewAmount = ado.AMOUNT - ado.AMOUNT;
                                string amount_str = ado.AMOUNT.ToString();
                                if ((Math.Ceiling(totalNewAmount) - totalNewAmount) == 0)
                                {
                                    long _amount = Convert.ToInt64(ado.AMOUNT);
                                    if (_amount >= 1 && _amount < 10)
                                    {
                                        amount_str = "0" + _amount;
                                    }
                                }
                                ado.AMOUNT_STR = amount_str;
                                ado.INTRUCTION_TIME = itemByServiceReq.INTRUCTION_DATE;
                                ado.USED_FOR_TRACKING_ID = itemByServiceReq.USED_FOR_TRACKING_ID;
                                ado.USE_TIME = itemByServiceReq.USE_TIME;

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
                        foreach (var item in sereServCLSByTracking)
                        {
                            ServiceCLS group = new ServiceCLS(item);
                            group.TRACKING_ID = _tracking.ID;
                            var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                            if (sereServExt != null)
                                group.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                            var serviceReq = _ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                            group.USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null;
                            _ServiceCLSSplits.Add(group);
                        }
                    }
                    else
                    {
                        ServiceCLS group = new ServiceCLS();
                        group.TRACKING_ID = _tracking.ID;
                        _ServiceCLSs.Add(group);
                        return;
                    }

                    if (sereServCLSByTracking.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() == 1)
                    {
                        var CLSGroup = sereServCLSByTracking.GroupBy(p => p.TDL_SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();

                        string ServiceTypeNameOld = "";

                        foreach (var itemGroups in CLSGroup)
                        {
                            string serviceName = "";
                            var serviceType = rdo._ServiceTypes.FirstOrDefault(p => p.ID == itemGroups[0].TDL_SERVICE_TYPE_ID);

                            string ServiceTypeName = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                            var itemGroupAmount = itemGroups.GroupBy(o => o.AMOUNT).ToList();
                            foreach (var itemAmount in itemGroupAmount)
                            {
                                ServiceCLS group = new ServiceCLS(itemGroups[0]);

                                if (ServiceTypeName != ServiceTypeNameOld)
                                {
                                    group.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";
                                    ServiceTypeNameOld = group.SERVICE_TYPE_NAME;
                                }
                                bool IsHasSereServ = false;
                                foreach (var item in itemAmount)
                                {
                                    var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                    if (_ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0 && _ServiceReqDuTrus.Exists(o => o.ID == item.SERVICE_REQ_ID))
                                        continue;
                                    if (_ServiceReqTHDT != null && _ServiceReqTHDT.Count > 0 && _ServiceReqTHDT.Exists(o => o.ID == item.SERVICE_REQ_ID))
                                        continue;
                                    if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                    {
                                        group.SERVICE_NAME += item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                        group.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                    }
                                    else
                                    {
                                        group.SERVICE_NAME += item.TDL_SERVICE_NAME + "; ";
                                    }
                                    var serviceReq = _ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                    group.serviceSplits.Add(new ServiceCLS() { SERVICE_NAME = item.TDL_SERVICE_NAME, INSTRUCTION_NOTE = sereServExt != null && !string.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE) ? sereServExt.INSTRUCTION_NOTE : null, USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null, TDL_SERVICE_UNIT_ID = item.TDL_SERVICE_UNIT_ID, SERVICE_ID = item.SERVICE_ID, AMOUNT = item.AMOUNT, TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME });
                                    group.IsGoupService = true;
                                    group.TRACKING_ID = _tracking.ID;
                                    serviceName += item.TDL_SERVICE_NAME + "; ";
                                    group.AMOUNT = item.AMOUNT;
                                    var numOrderServiceType = rdo._ServiceTypes.FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID);
                                    if (numOrderServiceType != null)
                                        group.NUM_ORDER_SERVICE_TYPE = numOrderServiceType.NUM_ORDER;
                                    IsHasSereServ = true;
                                }
                                if (IsHasSereServ)
                                    _ServiceCLSs.Add(group);
                            }
                            keyTamThan.Y_LENH += " -" + (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + " :" + serviceName + "\r\n";
                        }
                    }
                    else
                    {
                        var CLSGroupDate = sereServCLSByTracking.GroupBy(o => o.TDL_INTRUCTION_DATE).Select(x => x.ToList()).ToList();
                        foreach (var ItemCLSDate in CLSGroupDate)
                        {
                            var CLSGroup = ItemCLSDate.GroupBy(p => p.TDL_SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();

                            string ServiceTypeNameOld = "";

                            foreach (var itemGroups in CLSGroup)
                            {
                                string serviceName = "";
                                var serviceType = rdo._ServiceTypes.FirstOrDefault(p => p.ID == itemGroups[0].TDL_SERVICE_TYPE_ID);

                                string ServiceTypeName = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                var itemGroupAmount = itemGroups.GroupBy(o => o.AMOUNT).ToList();
                                foreach (var itemAmount in itemGroupAmount)
                                {
                                    ServiceCLS group = new ServiceCLS(itemGroups[0]);

                                    if (ServiceTypeName != ServiceTypeNameOld)
                                    {
                                        group.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";
                                        ServiceTypeNameOld = group.SERVICE_TYPE_NAME;
                                    }

                                    bool IsHasSereServ = false;
                                    foreach (var item in itemAmount)
                                    {
                                        var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                        if (_ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0 && _ServiceReqDuTrus.Exists(o => o.ID == item.SERVICE_REQ_ID))
                                            continue;
                                        if (_ServiceReqTHDT != null && _ServiceReqTHDT.Count > 0 && _ServiceReqTHDT.Exists(o => o.ID == item.SERVICE_REQ_ID))
                                            continue;
                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            group.SERVICE_NAME += item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                            group.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            group.SERVICE_NAME += item.TDL_SERVICE_NAME + "; ";
                                        }
                                        var serviceReq = _ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                        group.serviceSplits.Add(new ServiceCLS() { SERVICE_NAME = item.TDL_SERVICE_NAME, INSTRUCTION_NOTE = sereServExt != null && !string.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE) ? sereServExt.INSTRUCTION_NOTE : null, USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null, TDL_SERVICE_UNIT_ID = item.TDL_SERVICE_UNIT_ID, SERVICE_ID = item.SERVICE_ID, AMOUNT = item.AMOUNT, TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME });
                                        group.IsGoupService = true;
                                        group.TRACKING_ID = _tracking.ID;
                                        serviceName += item.TDL_SERVICE_NAME + "; ";
                                        group.AMOUNT = item.AMOUNT;
                                        var numOrderServiceType = rdo._ServiceTypes.FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID);
                                        if (numOrderServiceType != null)
                                            group.NUM_ORDER_SERVICE_TYPE = numOrderServiceType.NUM_ORDER;
                                        IsHasSereServ = true;
                                    }
                                    if (IsHasSereServ)
                                        _ServiceCLSs.Add(group);
                                }
                                keyTamThan.Y_LENH += " -" + (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + " :" + serviceName + "\r\n";
                            }
                        }
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

        private void ServiceOrders()
        {
            try
            {
                List<HIS_SERE_SERV> sereServCLSOrderByTracking = new List<HIS_SERE_SERV>();
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    foreach (var item in _ServiceReqs)
                    {
                        if (rdo._DicSereServs.ContainsKey(item.ID))
                        {
                            sereServCLSOrderByTracking.AddRange(
                                rdo._DicSereServs[item.ID].Where(p =>
                                    p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                                    && p.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                                    && (!p.IS_NO_EXECUTE.HasValue || p.IS_NO_EXECUTE != 1)).ToList());
                        }
                    }
                    if (sereServCLSOrderByTracking != null && sereServCLSOrderByTracking.Count > 0)
                    {
                    }
                    else
                    {
                        ServiceCLS group = new ServiceCLS();
                        group.TRACKING_ID = _tracking.ID;
                        _ServiceCLSOrders.Add(group);
                        _ServiceCLSSplitXNs.Add(group);
                        return;
                    }

                    if (sereServCLSOrderByTracking.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() == 1)
                    {
                        var CLSGroup = sereServCLSOrderByTracking.GroupBy(p => p.TDL_SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();

                        string ServiceTypeNameOld = "";

                        foreach (var itemGroups in CLSGroup)
                        {
                            string serviceName = "";
                            var serviceType = rdo._ServiceTypes.FirstOrDefault(p => p.ID == itemGroups[0].TDL_SERVICE_TYPE_ID);

                            string ServiceTypeName = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                            var itemGroupAmount = itemGroups.GroupBy(o => o.AMOUNT).ToList();
                            foreach (var itemAmount in itemGroupAmount)
                            {
                                ServiceCLS group = new ServiceCLS(itemGroups[0]);

                                if (ServiceTypeName != ServiceTypeNameOld)
                                {
                                    group.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";
                                    ServiceTypeNameOld = group.SERVICE_TYPE_NAME;
                                }

                                if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                                {
                                    group.TYPE_ID = 1;
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                {
                                    foreach (var item in itemAmount)
                                    {
                                        ServiceCLS groupTT = new ServiceCLS(item);
                                        groupTT.TYPE_ID = 2;

                                        groupTT.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                        var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            groupTT.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                            groupTT.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            groupTT.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + "; ";
                                        }
                                        groupTT.TRACKING_ID = _tracking.ID;
                                        serviceName += item.TDL_SERVICE_NAME + "; ";
                                        groupTT.AMOUNT = item.AMOUNT;
                                        _ServiceCLSOrders.Add(groupTT);

                                        ServiceCLS groupTT_Split = new ServiceCLS(groupTT);
                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            groupTT_Split.SERVICE_NAME = item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE;
                                            groupTT_Split.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            groupTT_Split.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                        }
                                        _ServiceCLSSplitXNs.Add(groupTT_Split);
                                    }
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                {
                                    foreach (var item in itemAmount)
                                    {
                                        var service = rdo._HisServiceViews.FirstOrDefault(o => o.ID == item.SERVICE_ID);

                                        ServiceCLS groupXN = new ServiceCLS(item);

                                        if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH)
                                        {
                                            groupXN.TYPE_ID = 3;
                                        }
                                        else if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH)
                                        {
                                            groupXN.TYPE_ID = 4;
                                        }
                                        else if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__NT)
                                        {
                                            groupXN.TYPE_ID = 5;
                                        }
                                        else if (service != null && service.TEST_TYPE_ID == 9) // xét nghiệm phân
                                        {
                                            groupXN.TYPE_ID = 6;
                                        }
                                        else if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS)
                                        {
                                            groupXN.TYPE_ID = 7;
                                        }
                                        else
                                        {
                                            groupXN.TYPE_ID = 8;
                                        }
                                        groupXN.TEST_TYPE_ID = service != null ? service.TEST_TYPE_ID : null;
                                        groupXN.TEST_TYPE_CODE = service != null ? service.TEST_TYPE_CODE : null;
                                        groupXN.TEST_TYPE_NAME = service != null ? service.TEST_TYPE_NAME : null;

                                        groupXN.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                        var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            groupXN.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                            groupXN.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            groupXN.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + "; ";
                                        }
                                        groupXN.TRACKING_ID = _tracking.ID;
                                        serviceName += item.TDL_SERVICE_NAME + "; ";
                                        groupXN.AMOUNT = item.AMOUNT;
                                        _ServiceCLSOrders.Add(groupXN);

                                        ServiceCLS groupXN_Split = new ServiceCLS(groupXN);

                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            groupXN_Split.SERVICE_NAME = item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE;
                                            groupXN_Split.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            groupXN_Split.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                        }
                                        _ServiceCLSSplitXNs.Add(groupXN_Split);
                                    }
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                                {
                                    group.TYPE_ID = 9;
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                                {
                                    group.TYPE_ID = 10;
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS)
                                {
                                    group.TYPE_ID = 11;
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                                {
                                    group.TYPE_ID = 12;
                                }
                                else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                                {
                                    foreach (var item in itemAmount)
                                    {
                                        ServiceCLS groupG = new ServiceCLS(item);
                                        groupG.TYPE_ID = 13;

                                        groupG.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                        var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            groupG.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                            groupG.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            groupG.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + "; ";
                                        }
                                        groupG.TRACKING_ID = _tracking.ID;
                                        serviceName += item.TDL_SERVICE_NAME + "; ";
                                        groupG.AMOUNT = item.AMOUNT;
                                        _ServiceCLSOrders.Add(groupG);

                                        ServiceCLS groupG_Split = new ServiceCLS(groupG);
                                        if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                        {
                                            groupG_Split.SERVICE_NAME = item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE;
                                        }
                                        else
                                        {
                                            groupG_Split.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                        }
                                        _ServiceCLSSplitXNs.Add(groupG_Split);
                                    }
                                }

                                if (itemGroups[0].TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && itemGroups[0].TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && itemGroups[0].TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                {
                                    foreach (var item in itemAmount)
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
                                        var serviceReq = _ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                        group.serviceSplits.Add(new ServiceCLS() { SERVICE_NAME = item.TDL_SERVICE_NAME, INSTRUCTION_NOTE = sereServExt != null && !string.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE) ? sereServExt.INSTRUCTION_NOTE : null, USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null, TDL_SERVICE_UNIT_ID = item.TDL_SERVICE_UNIT_ID, SERVICE_ID = item.SERVICE_ID, AMOUNT = item.AMOUNT, TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME });
                                        group.IsGoupService = true;
                                        group.TRACKING_ID = _tracking.ID;
                                        serviceName += item.TDL_SERVICE_NAME + "; ";
                                        group.AMOUNT = item.AMOUNT;
                                    }

                                    _ServiceCLSOrders.Add(group);
                                    _ServiceCLSSplitXNs.Add(group);
                                }
                            }
                            keyTamThan.Y_LENH += " -" + (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + " :" + serviceName + "\r\n";
                        }
                    }
                    else
                    {
                        var CLSGroupDate = sereServCLSOrderByTracking.GroupBy(o => o.TDL_INTRUCTION_DATE).Select(x => x.ToList()).ToList();
                        foreach (var ItemCLSDate in CLSGroupDate)
                        {
                            var CLSGroup = ItemCLSDate.GroupBy(p => p.TDL_SERVICE_TYPE_ID).Select(x => x.ToList()).ToList();

                            string ServiceTypeNameOld = "";

                            foreach (var itemGroups in CLSGroup)
                            {
                                string serviceName = "";
                                var serviceType = rdo._ServiceTypes.FirstOrDefault(p => p.ID == itemGroups[0].TDL_SERVICE_TYPE_ID);

                                string ServiceTypeName = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                var itemGroupAmount = itemGroups.GroupBy(o => o.AMOUNT).ToList();
                                foreach (var itemAmount in itemGroupAmount)
                                {
                                    ServiceCLS group = new ServiceCLS(itemGroups[0]);

                                    if (ServiceTypeName != ServiceTypeNameOld)
                                    {
                                        group.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";
                                        ServiceTypeNameOld = group.SERVICE_TYPE_NAME;
                                    }

                                    if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                                    {
                                        group.TYPE_ID = 1;
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                    {
                                        foreach (var item in itemAmount)
                                        {
                                            ServiceCLS groupTT = new ServiceCLS(item);
                                            groupTT.TYPE_ID = 2;

                                            groupTT.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                            var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                            {
                                                groupTT.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                                groupTT.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                            }
                                            else
                                            {
                                                groupTT.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + "; ";
                                            }
                                            groupTT.TRACKING_ID = _tracking.ID;
                                            serviceName += item.TDL_SERVICE_NAME + "; ";
                                            groupTT.AMOUNT = item.AMOUNT;
                                            _ServiceCLSOrders.Add(groupTT);

                                            ServiceCLS groupTT_Split = new ServiceCLS(groupTT);
                                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                            {
                                                groupTT_Split.SERVICE_NAME = item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE;
                                            }
                                            else
                                            {
                                                groupTT_Split.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                            }
                                            _ServiceCLSSplitXNs.Add(groupTT_Split);
                                        }
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                    {
                                        foreach (var item in itemAmount)
                                        {
                                            var service = rdo._HisServiceViews.FirstOrDefault(o => o.ID == item.SERVICE_ID) ?? null;

                                            ServiceCLS groupXN = new ServiceCLS(item);
                                            if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH)
                                            {
                                                groupXN.TYPE_ID = 3;
                                            }
                                            else if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH)
                                            {
                                                groupXN.TYPE_ID = 4;
                                            }
                                            else if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__NT)
                                            {
                                                groupXN.TYPE_ID = 5;
                                            }
                                            else if (service != null && service.TEST_TYPE_ID == 9) // xét nghiệm phân
                                            {
                                                groupXN.TYPE_ID = 6;
                                            }
                                            else if (service != null && service.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS)
                                            {
                                                groupXN.TYPE_ID = 7;
                                            }
                                            else
                                            {
                                                groupXN.TYPE_ID = 8;
                                            }

                                            groupXN.TEST_TYPE_ID = service != null ? service.TEST_TYPE_ID : null;
                                            groupXN.TEST_TYPE_CODE = service != null ? service.TEST_TYPE_CODE : null;
                                            groupXN.TEST_TYPE_NAME = service != null ? service.TEST_TYPE_NAME : null;

                                            groupXN.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";


                                            var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                            {
                                                groupXN.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                                groupXN.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                            }
                                            else
                                            {
                                                groupXN.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + "; ";
                                            }
                                            groupXN.TRACKING_ID = _tracking.ID;
                                            serviceName += item.TDL_SERVICE_NAME + "; ";
                                            groupXN.AMOUNT = item.AMOUNT;
                                            _ServiceCLSOrders.Add(groupXN);

                                            ServiceCLS groupXN_Split = new ServiceCLS(groupXN);
                                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                            {
                                                groupXN_Split.SERVICE_NAME = item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE;
                                            }
                                            else
                                            {
                                                groupXN_Split.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                            }
                                            _ServiceCLSSplitXNs.Add(groupXN_Split);
                                        }
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                                    {
                                        group.TYPE_ID = 9;
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                                    {
                                        group.TYPE_ID = 10;
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS)
                                    {
                                        group.TYPE_ID = 11;
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                                    {
                                        group.TYPE_ID = 12;
                                    }
                                    else if (itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                                    {
                                        foreach (var item in itemAmount)
                                        {
                                            ServiceCLS groupG = new ServiceCLS(item);
                                            groupG.TYPE_ID = 13;

                                            groupG.SERVICE_TYPE_NAME = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + ": ";

                                            var sereServExt = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                            {
                                                groupG.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE + "; ";
                                                groupG.INSTRUCTION_NOTE = sereServExt.INSTRUCTION_NOTE;
                                            }
                                            else
                                            {
                                                groupG.SERVICE_NAME = "- " + item.TDL_SERVICE_NAME + "; ";
                                            }
                                            groupG.TRACKING_ID = _tracking.ID;
                                            serviceName += item.TDL_SERVICE_NAME + "; ";
                                            groupG.AMOUNT = item.AMOUNT;
                                            _ServiceCLSOrders.Add(groupG);

                                            ServiceCLS groupG_Split = new ServiceCLS(item);
                                            if (sereServExt != null && !String.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE))
                                            {
                                                groupG_Split.SERVICE_NAME = item.TDL_SERVICE_NAME + ": " + sereServExt.INSTRUCTION_NOTE;
                                            }
                                            else
                                            {
                                                groupG_Split.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                            }

                                            _ServiceCLSSplitXNs.Add(groupG_Split);
                                        }
                                    }

                                    if (itemGroups[0].TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT && itemGroups[0].TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G && itemGroups[0].TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                                    {
                                        foreach (var item in itemAmount)
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
                                            var serviceReq = _ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                            group.serviceSplits.Add(new ServiceCLS() { SERVICE_NAME = item.TDL_SERVICE_NAME, INSTRUCTION_NOTE = sereServExt != null && !string.IsNullOrEmpty(sereServExt.INSTRUCTION_NOTE) ? sereServExt.INSTRUCTION_NOTE : null, USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null, TDL_SERVICE_UNIT_ID = item.TDL_SERVICE_UNIT_ID, SERVICE_ID = item.SERVICE_ID, AMOUNT = item.AMOUNT, TDL_INTRUCTION_TIME = item.TDL_INTRUCTION_TIME });
                                            group.IsGoupService = true;
                                            group.TRACKING_ID = _tracking.ID;
                                            serviceName += item.TDL_SERVICE_NAME + "; ";
                                            group.AMOUNT = item.AMOUNT;
                                        }

                                        _ServiceCLSOrders.Add(group);
                                        _ServiceCLSSplitXNs.Add(group);
                                    }
                                }
                                keyTamThan.Y_LENH += " -" + (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "Không xác định") + " :" + serviceName + "\r\n";
                            }
                        }
                    }
                    _ServiceCLSOrders = _ServiceCLSOrders.OrderBy(o => o.TYPE_ID).ToList();
                }
                else
                {
                    ServiceCLS group = new ServiceCLS();
                    group.TRACKING_ID = _tracking.ID;
                    _ServiceCLSOrders.Add(group);
                    _ServiceCLSSplitXNs.Add(group);
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
                    if (rdo._ExpMestBltyReq2 != null && rdo._ExpMestBltyReq2.Count > 0)
                    {
                        foreach (var item in rdo._ExpMestBltyReq2)
                        {
                            if (_Bloods == null || _Bloods.Count <= 0 || !_Bloods.Select(o => o.ID).Contains(item.ID))
                            {
                                ServiceCLS BltyReq2 = new ServiceCLS(item);
                                _Bloods.Add(BltyReq2);
                            }
                        }
                    }
                    else
                    {
                        if (bloodByTracking != null && bloodByTracking.Count > 0)
                        {
                            if (bloodByTracking.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() == 1)
                            {
                                var bloodGroups = bloodByTracking.GroupBy(p => p.BLOOD_ID).Select(x => x.ToList()).ToList();
                                foreach (var itemGroups in bloodGroups)
                                {
                                    if (_Bloods == null || _Bloods.Count <= 0 || !_Bloods.Select(o => o.ID).Contains(itemGroups[0].ID))
                                    {
                                        ServiceCLS group = new ServiceCLS(itemGroups[0]);
                                        group.AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                                        group.TRACKING_ID = _tracking.ID;
                                        _Bloods.Add(group);
                                    }
                                }
                            }
                            else
                            {
                                var bloodGroupDate = bloodByTracking.GroupBy(p => p.TDL_INTRUCTION_DATE).Select(x => x.ToList()).ToList();

                                foreach (var itemBloodDate in bloodGroupDate)
                                {
                                    //var bloodGroups = bloodByTracking.GroupBy(p => p.BLOOD_ID).Select(x => x.ToList()).ToList();
                                    var bloodGroups = itemBloodDate.GroupBy(p => p.BLOOD_ID).Select(x => x.ToList()).ToList();

                                    foreach (var itemGroups in bloodGroups)
                                    {
                                        if (_Bloods == null || _Bloods.Count <= 0 || !_Bloods.Select(o => o.ID).Contains(itemGroups[0].ID))
                                        {
                                            ServiceCLS group = new ServiceCLS(itemGroups[0]);
                                            group.AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                                            group.TRACKING_ID = _tracking.ID;
                                            _Bloods.Add(group);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (examByTracking != null && examByTracking.Count > 0)
                    {
                        if (examByTracking.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() == 1)
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
                        else
                        {
                            var examGroupsDate = examByTracking.GroupBy(p => p.TDL_INTRUCTION_DATE).Select(x => x.ToList()).ToList();
                            foreach (var itemExamDate in examGroupsDate)
                            {
                                //var examGroups = examByTracking.GroupBy(p => p.SERVICE_ID).Select(x => x.ToList()).ToList();
                                var examGroups = itemExamDate.GroupBy(p => p.SERVICE_ID).Select(x => x.ToList()).ToList();
                                foreach (var itemGroups in examGroups)
                                {
                                    ServiceCLS group = new ServiceCLS(itemGroups[0]);
                                    group.AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                                    group.TRACKING_ID = _tracking.ID;
                                    _ExamServices.Add(group);
                                }
                            }
                        }
                    }
                    if (TTByTracking != null && TTByTracking.Count > 0)
                    {
                        var TTByTrackingGroup = TTByTracking.GroupBy(p => p.TDL_INTRUCTION_DATE).Select(x => x.ToList()).ToList();

                        foreach (var itemGroup in TTByTrackingGroup)
                        {
                            foreach (var item in itemGroup)
                            {
                                ServiceCLS group = new ServiceCLS(item);
                                group.AMOUNT = item.AMOUNT;
                                group.TRACKING_ID = _tracking.ID;
                                var intruc = rdo._SereServExts.FirstOrDefault(p => p.SERE_SERV_ID == item.ID);
                                if (intruc != null)
                                {
                                    group.INSTRUCTION_NOTE = intruc.INSTRUCTION_NOTE;
                                }
                                var numOrderServiceType = rdo._ServiceTypes.FirstOrDefault(o => o.ID == item.TDL_SERVICE_TYPE_ID);
                                if (numOrderServiceType != null)
                                {
                                    group.NUM_ORDER_SERVICE_TYPE = numOrderServiceType.NUM_ORDER;
                                    group.SERVICE_TYPE_NAME = numOrderServiceType.SERVICE_TYPE_NAME;
                                }
                                var serviceReq = _ServiceReqs.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                group.USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null;
                                _TTServices.Add(group);
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

        private void MediMateTH()
        {
            try
            {
                if (rdo._MobaImpMests != null && rdo._MobaImpMests.Count > 0)
                {
                    List<V_HIS_IMP_MEST_2> lstImpMest2 = new List<V_HIS_IMP_MEST_2>();
                    List<HIS_SERVICE_REQ> LstserviceReqAll = new List<HIS_SERVICE_REQ>();

                    var MobaImpMestByTrackings = rdo._MobaImpMests.Where(o => o.TRACKING_ID == this._tracking.ID).ToList();

                    List<string> lstServiceReqCode = new List<string>();
                    List<string> serviceReqCodes = new List<string>();

                    if (MobaImpMestByTrackings != null && MobaImpMestByTrackings.Count > 0)
                    {
                        lstImpMest2.AddRange(MobaImpMestByTrackings);
                        var MobaImpMestReqCode = MobaImpMestByTrackings.Select(o => o.TDL_SERVICE_REQ_CODE).ToList();
                        var ServiceReqs = rdo._DicServiceReqs.Values.Where(p => MobaImpMestReqCode.Contains(p.SERVICE_REQ_CODE)).ToList();

                        if (ServiceReqs != null && ServiceReqs.Count > 0)
                        {
                            lstServiceReqCode = ServiceReqs.Select(o => o.SERVICE_REQ_CODE).ToList();
                            LstserviceReqAll.AddRange(ServiceReqs);
                        }
                    }

                    var LstserviceReq = rdo._DicServiceReqs.Values.Where(p => p.TRACKING_ID == this._tracking.ID).ToList();

                    if ((lstServiceReqCode != null && lstServiceReqCode.Count > 0) || (LstserviceReq != null && LstserviceReq.Count > 0))
                    {
                        LstserviceReq = LstserviceReq.Where(p => !lstServiceReqCode.Contains(p.SERVICE_REQ_CODE)).ToList();
                        LstserviceReqAll.AddRange(LstserviceReq);
                        serviceReqCodes = LstserviceReq.Select(o => o.SERVICE_REQ_CODE).ToList();
                    }

                    if (serviceReqCodes != null && serviceReqCodes.Count > 0)
                    {
                        var _ImpMestMedicineADOByTrackings = rdo._MobaImpMests.Where(o => serviceReqCodes.Contains(o.TDL_SERVICE_REQ_CODE) && o.TRACKING_ID == null).ToList();

                        if (_ImpMestMedicineADOByTrackings != null && _ImpMestMedicineADOByTrackings.Count > 0)
                        {
                            lstImpMest2.AddRange(_ImpMestMedicineADOByTrackings);
                        }
                    }

                    if (lstImpMest2 != null && lstImpMest2.Count > 0)
                    {
                        foreach (var medi_TL in lstImpMest2)
                        {
                            var serviceReq = LstserviceReqAll.FirstOrDefault(o => o.SERVICE_REQ_CODE == medi_TL.TDL_SERVICE_REQ_CODE);
                            //THUOC
                            if (rdo._ImpMestMedicines_TL != null && rdo._ImpMestMedicines_TL.Count > 0)
                            {
                                var Medicines_TL = rdo._ImpMestMedicines_TL.Where(o => o.IMP_MEST_ID == medi_TL.ID).ToList();
                                if (Medicines_TL != null && Medicines_TL.Count > 0)
                                {
                                    var dataGroups = Medicines_TL.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                                    foreach (var MediTL in dataGroups)
                                    {
                                        ImpMestMedicineADO ado = new ImpMestMedicineADO(MediTL[0], serviceReq);
                                        ado.AMOUNT = MediTL.Sum(p => p.AMOUNT);
                                        ado.TRACKING_ID = this._tracking.ID; ;
                                        this._ImpMestMedicineADOs.Add(ado);
                                    }
                                }
                            }
                            //VT
                            if (rdo._ImpMestMaterial_TL != null && rdo._ImpMestMaterial_TL.Count > 0)
                            {
                                var Material_TL = rdo._ImpMestMaterial_TL.Where(o => o.IMP_MEST_ID == medi_TL.ID).ToList();
                                if (Material_TL != null && Material_TL.Count > 0)
                                {
                                    var dataGroups = Material_TL.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                                    foreach (var mateTL in dataGroups)
                                    {
                                        ImpMestMaterialADO ado = new ImpMestMaterialADO(mateTL[0], serviceReq);
                                        ado.AMOUNT = mateTL.Sum(p => p.AMOUNT);
                                        ado.TRACKING_ID = this._tracking.ID; ;
                                        this._ImpMestMaterialADOs.Add(ado);
                                    }
                                }
                            }
                            //MAU
                            if (rdo._ImpMestBlood_TL != null && rdo._ImpMestBlood_TL.Count > 0)
                            {
                                var Blood_TL = rdo._ImpMestBlood_TL.Where(o => o.IMP_MEST_ID == medi_TL.ID).ToList();
                                if (Blood_TL != null && Blood_TL.Count > 0)
                                {
                                    var dataGroups = Blood_TL.GroupBy(p => p.BLOOD_TYPE_ID).Select(p => p.ToList()).ToList();
                                    foreach (var bloodTL in dataGroups)
                                    {
                                        ImpMestBloodADO ado = new ImpMestBloodADO(bloodTL[0], serviceReq);
                                        ado.VOLUME = bloodTL.Sum(p => p.VOLUME);
                                        ado.TRACKING_ID = this._tracking.ID; ;
                                        this._ImpMestBloodADOs.Add(ado);
                                    }
                                }
                            }
                        }
                    }
                }

                if (this._ImpMestMedicineADOs != null && _ImpMestMedicineADOs.Count > 0)
                {
                    var ImpMestMedicineADO_G = this._ImpMestMedicineADOs.GroupBy(p => p.TRACKING_ID).ToList();

                    foreach (var iGroup in ImpMestMedicineADO_G)
                    {
                        var iGroup_S = iGroup.OrderBy(p => p.USE_TIME ?? 99999999999999).ThenBy(o => o.INTRUCTION_TIME).ToList();
                        long? IntructionTime = 0;
                        long? UseTime = null;

                        foreach (var item in iGroup_S)
                        {
                            if (UseTime != item.USE_TIME)
                            {
                                UseTime = item.USE_TIME;
                            }
                            else
                            {
                                item.USE_TIME = null;
                            }

                            if (IntructionTime != item.INTRUCTION_TIME)
                            {
                                IntructionTime = item.INTRUCTION_TIME;
                            }
                            else
                            {
                                item.INTRUCTION_TIME = null;
                            }
                        }
                    }
                }

                if (this._ImpMestMaterialADOs != null && _ImpMestMaterialADOs.Count > 0)
                {
                    var ImpMestMaterialADOs_G = this._ImpMestMaterialADOs.GroupBy(p => p.TRACKING_ID);

                    foreach (var iGroup in ImpMestMaterialADOs_G)
                    {
                        var iGroup_S = iGroup.OrderBy(p => p.USE_TIME ?? 99999999999999).ThenBy(o => o.INTRUCTION_TIME).ToList();
                        long? IntructionTime = 0;
                        long? UseTime = null;
                        foreach (var item in iGroup_S)
                        {
                            if (UseTime != item.USE_TIME)
                            {
                                UseTime = item.USE_TIME;
                            }
                            else
                            {
                                item.USE_TIME = null;
                            }

                            if (IntructionTime != item.INTRUCTION_TIME)
                            {
                                IntructionTime = item.INTRUCTION_TIME;
                            }
                            else
                            {
                                item.INTRUCTION_TIME = null;
                            }
                        }
                    }
                }

                if (this._ImpMestBloodADOs != null && _ImpMestBloodADOs.Count > 0)
                {
                    var ImpMestBloodADOs_G = this._ImpMestBloodADOs.GroupBy(p => p.TRACKING_ID);

                    foreach (var iGroup in ImpMestBloodADOs_G)
                    {
                        var iGroup_S = iGroup.OrderBy(o => o.INTRUCTION_TIME ?? 99999999999999).ToList();
                        long? IntructionTime = 0;
                        foreach (var item in iGroup_S)
                        {
                            if (IntructionTime != item.INTRUCTION_TIME)
                            {
                                IntructionTime = item.INTRUCTION_TIME;
                            }
                            else
                            {
                                item.INTRUCTION_TIME = null;
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

                dicImage.Add(Mps000062ExtendSingleKey.BARCODE_PATIENT_CODE_STR, barcodePatientCode);

                Inventec.Common.BarcodeLib.Barcode barcodeTreatment = new Inventec.Common.BarcodeLib.Barcode(rdo._Treatment.TREATMENT_CODE);
                barcodeTreatment.Alignment = Inventec.Common.BarcodeLib.AlignmentPositions.CENTER;
                barcodeTreatment.Width = 120;
                barcodeTreatment.Height = 40;
                barcodeTreatment.RotateFlipType = RotateFlipType.Rotate180FlipXY;
                barcodeTreatment.LabelPosition = Inventec.Common.BarcodeLib.LabelPositions.BOTTOMCENTER;
                barcodeTreatment.EncodedType = Inventec.Common.BarcodeLib.TYPE.CODE128;
                barcodeTreatment.IncludeLabel = true;

                dicImage.Add(Mps000062ExtendSingleKey.BARCODE_TREATMENT_CODE_STR, barcodeTreatment);
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
                Inventec.Common.Logging.LogSystem.Debug("Mps000062 ------ ProcessData----1");

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                SetDataExpMestADO();
                CheckSTTByMedicineGroup();
                CheckSTTMedicineOutStock();
                CheckSTTByActive();

                ProcessorDataPrint();

                SetBarcodeKey();

                SetSingleKey();

                SetImageKey();

                if (_RemedyCountADOs != null && _RemedyCountADOs.Count > 0)
                {
                    _RemedyCountADOs = _RemedyCountADOs.OrderBy(o => o.PRESCRIPTION_TYPE_ID).ToList();
                }

                long? PreviousUseDay = null;

                long count = 0;

                if (_ExpMestMetyReqADOsV2 != null
                    && _ExpMestMetyReqADOsV2.Count > 0
                    && rdo._WorkPlaceSDO.IsShowMedicineLine)
                {
                    this._ExpMestMetyReqADOCommons = new List<ExpMestMetyReqADO>();
                    this._ExpMestMetyReqADOCommonsMix = new List<ExpMestMetyReqADO>();
                    this._ExpMestMetyReqADOCommons_Merge = new List<ExpMestMetyReqADO>();
                    List<ExpMestMetyReqADO> orderADO = new List<ExpMestMetyReqADO>();
                    orderADO.AddRange(this._ExpMestMetyReqADOsV2);

                    var orderADO1 = orderADO.GroupBy(o => o.TDL_MEDICINE_TYPE_ID);
                    foreach (var item in orderADO1)
                    {
                        var item1 = item.OrderByDescending(o => o.PREVIOUS_USING_COUNT).ToList();
                        PreviousUseDay = item1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? item1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;

                        foreach (var item2 in item)
                        {
                            count = 0;
                            PreviousUseDay = null;
                            List<HIS_EXP_MEST_MEDICINE> lstCheck = new List<HIS_EXP_MEST_MEDICINE>();

                            if (item2.PREVIOUS_USING_COUNT != null)
                            {
                                PreviousUseDay = item2.PREVIOUS_USING_COUNT;
                            }

                            if (ServiceReq != null)
                            {
                                var CountServideReq = ServiceReq.Where(o => o.INTRUCTION_TIME <= item2.INTRUCTION_TIME).Select(o => o.ID).ToList();
                                if (CountServideReq != null && CountServideReq.Count > 0)
                                {
                                    foreach (var itemDem in CountServideReq)
                                    {
                                        var check = _ExpMestMedicinesAll.Where(o => o.TDL_MEDICINE_TYPE_ID == item2.TDL_MEDICINE_TYPE_ID && o.TDL_TREATMENT_ID == rdo._Treatment.ID && o.TDL_SERVICE_REQ_ID == itemDem).ToList();

                                        count += check.Count();

                                        if (PreviousUseDay == null)
                                        {
                                            if (check != null && check.Count > 0)
                                            {
                                                lstCheck.AddRange(check);
                                            }
                                        }
                                    }
                                }
                            }

                            if (PreviousUseDay == null && lstCheck != null && lstCheck.Count > 0)
                            {
                                var Check1 = lstCheck.Where(o => o.PREVIOUS_USING_COUNT != null).OrderByDescending(o => o.PREVIOUS_USING_COUNT).ToList();
                                if (Check1 != null && Check1.Count > 0)
                                {
                                    PreviousUseDay = Check1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? Check1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;
                                }
                            }

                            PreviousUseDay = PreviousUseDay != null ? PreviousUseDay : 0;

                            Inventec.Common.Logging.LogSystem.Warn("PreviousUseDay: " + PreviousUseDay + "__ count: " + count + " item2.TDL_MEDICINE_TYPE_ID: " + item2.TDL_MEDICINE_TYPE_ID + " item2.INTRUCTION_TIME: " + item2.INTRUCTION_TIME);
                            item2.USING_COUNT_NUMBER = PreviousUseDay + count;
                        }

                        #region đếm số lần sử dụng thuốc "trong ngày"

                        var OrderNumByType = item.GroupBy(o => o.NUMBER_BY_TYPE).Select(p => p.ToList()).ToList();

                        foreach (var iGroup in OrderNumByType)
                        {
                            for (int i = 0; i < iGroup.Count; i++)
                            {
                                if (iGroup[i].NUMBER_BY_TYPE > 0)
                                {
                                    string NumberOfUseInDay = "";
                                    if (i == 0)
                                    {
                                        NumberOfUseInDay = iGroup[i].NUMBER_BY_TYPE.ToString();
                                    }
                                    else
                                    {
                                        NumberOfUseInDay = iGroup[i].NUMBER_BY_TYPE.ToString() + "." + i.ToString();
                                    }

                                    iGroup[i].NUMBER_OF_USE_IN_DAY = NumberOfUseInDay;

                                    this._ExpMestMetyReqADOsV2.FirstOrDefault(o => o.ID == iGroup[i].ID).NUMBER_OF_USE_IN_DAY = iGroup[i].NUMBER_OF_USE_IN_DAY;
                                }
                            }
                        }

                        #endregion
                    }

                    List<long> IDS = new List<long>();
                    var groupADO1 = orderADO.OrderBy(p => p.INTRUCTION_TIME).ThenBy(o => o.USE_TIME).ThenBy(n => n.ID).GroupBy(o => o.MEDICINE_USE_FORM_ID).ToList();

                    foreach (var itemADO2 in groupADO1)
                    {
                        var groupADO2 = itemADO2.OrderBy(p => p.INTRUCTION_TIME).ThenBy(o => o.USE_TIME).ThenBy(n => n.ID).GroupBy(o => o.ACTIVE_INGR_BHYT_CODE);

                        foreach (var itemADO3 in groupADO2)
                        {
                            long dem = 0;
                            foreach (var itemADO4 in itemADO3)
                            {
                                if (itemADO4.IS_NUMBERED_TRACKING == 1 && !IDS.Contains(itemADO4.ID))
                                {
                                    dem++;
                                    itemADO4.NUMBER_USE_AND_ACTIVE = dem;
                                }
                                IDS.Add(itemADO4.ID);
                            }
                        }
                    }

                    foreach (var item in this._ExpMestMetyReqADOsV2)
                    {
                        var lstcheck = orderADO.FirstOrDefault(o => o.ID == item.ID);

                        if (lstcheck != null)
                        {
                            item.USING_COUNT_NUMBER = lstcheck.USING_COUNT_NUMBER;

                            if (item.IS_NUMBERED_TRACKING == 1 && item.IS_NOT_TREATMENT_DAY_COUNT != 1)
                            {
                                item.NUMBER_USE_AND_ACTIVE = lstcheck.NUMBER_USE_AND_ACTIVE;
                            }
                            else
                            {
                                item.NUMBER_USE_AND_ACTIVE = null;
                            }
                        }
                    }

                    var dataMedicine = this._ExpMestMetyReqADOsV2.Where(p => (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();

                    this.DataMedicine_Merge(dataMedicine, this._ExpMestMetyReqADOCommons_Merge);

                    //this._ExpMestMetyReqADOsV2 = this._ExpMestMetyReqADOsV2.OrderBy(o => o.INTRUCTION_DATE).ToList();
                    var _ExpMestMetyReqADOsV2NoMix = this._ExpMestMetyReqADOsV2.Where(p => p.MIXED_INFUSION == null && (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    var _ExpMestMetyReqADOsV2Mix = this._ExpMestMetyReqADOsV2.Where(p => p.MIXED_INFUSION != null && (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();

                    var dataGroupsIntructionNoMix = _ExpMestMetyReqADOsV2NoMix.GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();
                    var dataGroupsIntructionMix = _ExpMestMetyReqADOsV2Mix.GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();

                    foreach (var itemIn in dataGroupsIntructionNoMix)
                    {
                        if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                        {
                            List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                            var itemOrder = itemIn.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                            var dataGroups = itemOrder.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                            foreach (var itemGr in dataGroups)
                            {
                                var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();
                                _dataNews1.AddRange(dtGroups);
                            }

                            _ExpMestMetyReqADOCommons.AddRange(_dataNews1);
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList());

                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());

                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(ProcessSortListExpMestMetyReq(itemIn));
                        }
                    }

                    //Thuốc pha truyền
                    foreach (var itemIn in dataGroupsIntructionMix)
                    {
                        if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }

                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }

                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                        {
                            foreach (var itemIsMixed in itemIn)
                            {
                                this._ExpMestMetyReqADOCommonsMix.AddRange(ProcessSortListExpMestMetyReq(new List<ExpMestMetyReqADO>() { itemIsMixed }));

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(ProcessSortListExpMestMetyReq(childMixed));
                            }
                        }
                    }
                }
                else if (this._ExpMestMetyReqADOs != null
                    && this._ExpMestMetyReqADOs.Count > 0
                    && rdo._WorkPlaceSDO != null)
                {
                    this._ExpMestMetyReqADOCommons = new List<ExpMestMetyReqADO>();
                    this._ExpMestMetyReqADOCommonsMix = new List<ExpMestMetyReqADO>();
                    this._ExpMestMetyReqADOCommons_Merge = new List<ExpMestMetyReqADO>();
                    List<ExpMestMetyReqADO> orderADO = new List<ExpMestMetyReqADO>();
                    orderADO.AddRange(this._ExpMestMetyReqADOs);

                    var orderADO1 = orderADO.GroupBy(o => o.TDL_MEDICINE_TYPE_ID);

                    Inventec.Common.Logging.LogSystem.Info("_ExpMestMedicinesAll: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMedicinesAll), _ExpMestMedicinesAll));

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ServiceReq), ServiceReq));

                    foreach (var item in orderADO1)
                    {
                        //var item1 = item.OrderByDescending(o => o.PREVIOUS_USING_COUNT).ToList();
                        //PreviousUseDay = item1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? item1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;

                        foreach (var item2 in item)
                        {
                            count = 0;
                            PreviousUseDay = null;
                            List<HIS_EXP_MEST_MEDICINE> lstCheck = new List<HIS_EXP_MEST_MEDICINE>();

                            if (item2.PREVIOUS_USING_COUNT.HasValue && item2.PREVIOUS_USING_COUNT.Value > 0)
                            {
                                PreviousUseDay = item2.PREVIOUS_USING_COUNT;
                            }

                            if (ServiceReq != null)
                            {
                                var CountServideReq = ServiceReq.Where(o => o.INTRUCTION_TIME <= item2.INTRUCTION_TIME).Select(o => o.ID).ToList();
                                if (CountServideReq != null && CountServideReq.Count > 0)
                                {
                                    foreach (var itemDem in CountServideReq)
                                    {
                                        var check = _ExpMestMedicinesAll.Where(o => o.TDL_MEDICINE_TYPE_ID == item2.TDL_MEDICINE_TYPE_ID && o.TDL_TREATMENT_ID == rdo._Treatment.ID && o.TDL_SERVICE_REQ_ID == itemDem).ToList();

                                        count += check.Count();

                                        if (PreviousUseDay == null)
                                        {
                                            if (check != null && check.Count > 0)
                                            {
                                                lstCheck.AddRange(check);
                                            }
                                        }
                                    }
                                }
                            }

                            if (lstCheck != null && lstCheck.Count > 0)
                            {
                                var Check1 = lstCheck.Where(o => o.PREVIOUS_USING_COUNT != null).OrderByDescending(o => o.PREVIOUS_USING_COUNT).ToList();
                                if (Check1 != null && Check1.Count > 0)
                                {
                                    PreviousUseDay = Check1.FirstOrDefault().PREVIOUS_USING_COUNT;
                                }
                            }

                            PreviousUseDay = PreviousUseDay != null ? PreviousUseDay : 0;

                            Inventec.Common.Logging.LogSystem.Warn("PreviousUseDay: " + PreviousUseDay + "__ count: " + count + Inventec.Common.Logging.LogUtil.TraceData(" item2", item2));
                            item2.USING_COUNT_NUMBER = PreviousUseDay + count;
                        }

                        #region đếm số lần sử dụng thuốc "trong ngày"

                        var OrderNumByType = item.GroupBy(o => o.NUMBER_BY_TYPE).Select(p => p.ToList()).ToList();

                        foreach (var iGroup in OrderNumByType)
                        {
                            for (int i = 0; i < iGroup.Count; i++)
                            {
                                if (iGroup[i].NUMBER_BY_TYPE > 0)
                                {
                                    string NumberOfUseInDay = "";
                                    if (i == 0)
                                    {
                                        NumberOfUseInDay = iGroup[i].NUMBER_BY_TYPE.ToString();
                                    }
                                    else
                                    {
                                        NumberOfUseInDay = iGroup[i].NUMBER_BY_TYPE.ToString() + "." + i.ToString();
                                    }

                                    iGroup[i].NUMBER_OF_USE_IN_DAY = NumberOfUseInDay;

                                    this._ExpMestMetyReqADOs.FirstOrDefault(o => o.ID == iGroup[i].ID).NUMBER_OF_USE_IN_DAY = iGroup[i].NUMBER_OF_USE_IN_DAY;
                                }
                            }
                        }

                        #endregion
                    }

                    List<long> IDS = new List<long>();
                    var groupADO1 = orderADO.OrderBy(p => p.INTRUCTION_TIME).ThenBy(o => o.USE_TIME).ThenBy(n => n.ID).GroupBy(o => o.MEDICINE_USE_FORM_ID).ToList();

                    foreach (var itemADO2 in groupADO1)
                    {
                        var groupADO2 = itemADO2.OrderBy(p => p.INTRUCTION_TIME).ThenBy(o => o.USE_TIME).ThenBy(n => n.ID).GroupBy(o => o.ACTIVE_INGR_BHYT_CODE);

                        foreach (var itemADO3 in groupADO2)
                        {
                            long dem = 0;
                            foreach (var itemADO4 in itemADO3)
                            {
                                if (itemADO4.IS_NUMBERED_TRACKING == 1 && !IDS.Contains(itemADO4.ID))
                                {
                                    dem++;
                                    itemADO4.NUMBER_USE_AND_ACTIVE = dem;
                                }
                                IDS.Add(itemADO4.ID);
                            }
                        }
                    }

                    foreach (var item in this._ExpMestMetyReqADOs)
                    {
                        var lstcheck = orderADO.FirstOrDefault(o => o.ID == item.ID);

                        if (lstcheck != null)
                        {
                            item.USING_COUNT_NUMBER = lstcheck.USING_COUNT_NUMBER;

                            if (item.IS_NUMBERED_TRACKING == 1 && item.IS_NOT_TREATMENT_DAY_COUNT != 1)
                            {
                                item.NUMBER_USE_AND_ACTIVE = lstcheck.NUMBER_USE_AND_ACTIVE;
                            }
                            else
                            {
                                item.NUMBER_USE_AND_ACTIVE = null;
                            }
                        }
                    }
                    var dataMedicine = this._ExpMestMetyReqADOs.Where(p => (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    this.DataMedicine_Merge(dataMedicine, this._ExpMestMetyReqADOCommons_Merge);

                    //this._ExpMestMetyReqADOs = this._ExpMestMetyReqADOs.OrderBy(o => o.INTRUCTION_DATE).ToList();
                    var _ExpMestMetyReqADOsNoMix = this._ExpMestMetyReqADOs.Where(p => p.MIXED_INFUSION == null && (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    var _ExpMestMetyReqADOsMix = this._ExpMestMetyReqADOs.Where(p => p.MIXED_INFUSION != null && (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    var dataGroupsIntructionNoMix = _ExpMestMetyReqADOsNoMix.GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();
                    var dataGroupsIntructionMix = _ExpMestMetyReqADOsMix.GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();

                    foreach (var itemIn in dataGroupsIntructionNoMix)
                    {
                        if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                        {
                            List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                            var itemOrder = itemIn.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                            var dataGroups = itemOrder.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                            foreach (var itemGr in dataGroups)
                            {
                                var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();
                                _dataNews1.AddRange(dtGroups);
                            }

                            this._ExpMestMetyReqADOCommons.AddRange(_dataNews1);
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());

                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());

                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                        {
                            this._ExpMestMetyReqADOCommons.AddRange(ProcessSortListExpMestMetyReq(itemIn));
                        }
                    }

                    //thuốc pha truyền
                    foreach (var itemIn in dataGroupsIntructionMix)
                    {
                        if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                        {

                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                        {
                            var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                            foreach (var itemIsMixed in itemIsMixedMains)
                            {
                                this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                            }
                        }
                        else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                        {
                            foreach (var itemIsMixed in itemIn)
                            {
                                this._ExpMestMetyReqADOCommonsMix.AddRange(ProcessSortListExpMestMetyReq(new List<ExpMestMetyReqADO>() { itemIsMixed }));

                                var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).ToList();

                                this._ExpMestMetyReqADOCommonsMix.AddRange(ProcessSortListExpMestMetyReq(childMixed));
                            }
                        }
                    }

                }
                else if (this._ExpMestMetyReqADOs != null && this._ExpMestMetyReqADOs.Count > 0)
                {

                    this._ExpMestMetyReqADOCommons = new List<ExpMestMetyReqADO>();
                    this._ExpMestMetyReqADOCommonsMix = new List<ExpMestMetyReqADO>();
                    this._ExpMestMetyReqADOCommons_Merge = new List<ExpMestMetyReqADO>();
                    List<ExpMestMetyReqADO> orderADO = new List<ExpMestMetyReqADO>();
                    orderADO.AddRange(this._ExpMestMetyReqADOs);

                    var orderADO1 = orderADO.GroupBy(o => o.TDL_MEDICINE_TYPE_ID);
                    foreach (var item in orderADO1)
                    {
                        var item1 = item.OrderByDescending(o => o.PREVIOUS_USING_COUNT).ToList();
                        PreviousUseDay = item1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? item1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;

                        foreach (var item2 in item)
                        {
                            PreviousUseDay = null;
                            List<HIS_EXP_MEST_MEDICINE> lstCheck = new List<HIS_EXP_MEST_MEDICINE>();

                            if (item2.PREVIOUS_USING_COUNT != null)
                            {
                                PreviousUseDay = item2.PREVIOUS_USING_COUNT;
                            }

                            if (ServiceReq != null)
                            {
                                var CountServideReq = ServiceReq.Where(o => o.INTRUCTION_TIME <= item2.INTRUCTION_TIME).Select(o => o.ID).ToList();
                                if (CountServideReq != null && CountServideReq.Count > 0)
                                {
                                    foreach (var itemDem in CountServideReq)
                                    {
                                        var check = _ExpMestMedicinesAll.Where(o => o.TDL_MEDICINE_TYPE_ID == item2.TDL_MEDICINE_TYPE_ID && o.TDL_TREATMENT_ID == rdo._Treatment.ID && o.TDL_SERVICE_REQ_ID == itemDem).ToList();

                                        count += check.Count();

                                        if (PreviousUseDay == null)
                                        {
                                            if (check != null && check.Count > 0)
                                            {
                                                lstCheck.AddRange(check);
                                            }
                                        }
                                    }
                                }
                            }

                            if (PreviousUseDay == null && lstCheck != null && lstCheck.Count > 0)
                            {
                                var Check1 = lstCheck.Where(o => o.PREVIOUS_USING_COUNT != null).OrderByDescending(o => o.PREVIOUS_USING_COUNT).ToList();
                                if (Check1 != null && Check1.Count > 0)
                                {
                                    PreviousUseDay = Check1.FirstOrDefault().PREVIOUS_USING_COUNT != null ? Check1.FirstOrDefault().PREVIOUS_USING_COUNT : 0;
                                }
                            }

                            PreviousUseDay = PreviousUseDay != null ? PreviousUseDay : 0;

                            Inventec.Common.Logging.LogSystem.Warn("PreviousUseDay: " + PreviousUseDay + "__ count: " + count + " item2.TDL_MEDICINE_TYPE_ID: " + item2.TDL_MEDICINE_TYPE_ID + " item2.INTRUCTION_TIME: " + item2.INTRUCTION_TIME);
                            item2.USING_COUNT_NUMBER = PreviousUseDay + count;
                        }

                        #region đếm số lần sử dụng thuốc "trong ngày"

                        var OrderNumByType = item.GroupBy(o => o.NUMBER_BY_TYPE).Select(p => p.ToList()).ToList();

                        foreach (var iGroup in OrderNumByType)
                        {
                            for (int i = 0; i < iGroup.Count; i++)
                            {
                                if (iGroup[i].NUMBER_BY_TYPE > 0)
                                {
                                    string NumberOfUseInDay = "";
                                    if (i == 0)
                                    {
                                        NumberOfUseInDay = iGroup[i].NUMBER_BY_TYPE.ToString();
                                    }
                                    else
                                    {
                                        NumberOfUseInDay = iGroup[i].NUMBER_BY_TYPE.ToString() + "." + i.ToString();
                                    }

                                    iGroup[i].NUMBER_OF_USE_IN_DAY = NumberOfUseInDay;

                                    this._ExpMestMetyReqADOs.FirstOrDefault(o => o.ID == iGroup[i].ID).NUMBER_OF_USE_IN_DAY = iGroup[i].NUMBER_OF_USE_IN_DAY;
                                }
                            }
                        }

                        #endregion
                    }

                    List<long> IDS = new List<long>();

                    var groupADO1 = orderADO.OrderBy(p => p.INTRUCTION_TIME).ThenBy(o => o.USE_TIME).ThenBy(n => n.ID).GroupBy(o => o.MEDICINE_USE_FORM_ID).ToList();

                    foreach (var itemADO2 in groupADO1)
                    {
                        var groupADO2 = itemADO2.OrderBy(p => p.INTRUCTION_TIME).ThenBy(o => o.USE_TIME).ThenBy(n => n.ID).GroupBy(o => o.ACTIVE_INGR_BHYT_CODE);

                        foreach (var itemADO3 in groupADO2)
                        {
                            long dem = 0;
                            foreach (var itemADO4 in itemADO3)
                            {
                                if (itemADO4.IS_NUMBERED_TRACKING == 1 && !IDS.Contains(itemADO4.ID))
                                {
                                    dem++;
                                    itemADO4.NUMBER_USE_AND_ACTIVE = dem;
                                }
                                IDS.Add(itemADO4.ID);
                            }
                        }
                    }

                    foreach (var item in this._ExpMestMetyReqADOs)
                    {
                        var lstcheck = orderADO.FirstOrDefault(o => o.ID == item.ID);

                        if (lstcheck != null)
                        {
                            item.USING_COUNT_NUMBER = lstcheck.USING_COUNT_NUMBER;

                            if (item.IS_NUMBERED_TRACKING == 1 && item.IS_NOT_TREATMENT_DAY_COUNT != 1)
                            {
                                item.NUMBER_USE_AND_ACTIVE = lstcheck.NUMBER_USE_AND_ACTIVE;
                            }
                            else
                            {
                                item.NUMBER_USE_AND_ACTIVE = null;
                            }
                        }
                    }

                    var dataMedicine = this._ExpMestMetyReqADOs.Where(p => (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    this.DataMedicine_Merge(dataMedicine, this._ExpMestMetyReqADOCommons_Merge);

                    var _ExpMestMetyReqADOsNoMix = this._ExpMestMetyReqADOs.Where(p => p.MIXED_INFUSION == null && (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    var _ExpMestMetyReqADOsMix = this._ExpMestMetyReqADOs.Where(p => p.MIXED_INFUSION != null && (p.USE_TIME <= p.INTRUCTION_DATE || p.USE_TIME == null) && p.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_DATE).ToList();

                    var dataGroupsIntructionNoMix = _ExpMestMetyReqADOsNoMix.GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();
                    var dataGroupsIntructionMix = _ExpMestMetyReqADOsMix.GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();

                    foreach (var itemIn in dataGroupsIntructionNoMix)
                    {
                        this._ExpMestMetyReqADOCommons.AddRange(itemIn.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList());
                    }

                    //thuốc pha truyền
                    foreach (var itemIn in dataGroupsIntructionMix)
                    {
                        var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList();

                        foreach (var itemIsMixed in itemIsMixedMains)
                        {
                            this._ExpMestMetyReqADOCommonsMix.Add(itemIsMixed);

                            var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList();

                            this._ExpMestMetyReqADOCommonsMix.AddRange(childMixed);
                        }
                    }

                }

                if (_ExpMestMetyReqADOCommonsMix != null && _ExpMestMetyReqADOCommonsMix.Count > 0)
                {
                    _ExpMestMetyReqADOCommonsMix = _ExpMestMetyReqADOCommonsMix.Distinct().ToList();
                }

                this._ExpMestMetyReqADOCommonsDuTru = new List<ExpMestMetyReqADO>();
                this._ExpMestMetyReqADOCommonsTHDT = new List<ExpMestMetyReqADO>();
                this._MediInfusionDutru = new List<ExpMestMetyReqADO>();
                this._MediInfusionTHDT = new List<ExpMestMetyReqADO>();
                this._ServiceClsDuTru = new List<ServiceCLS>();
                this._ServiceClsTHDT = new List<ServiceCLS>();
                this._ServiceTtDuTru = new List<ServiceCLS>();
                this._ServiceTtTHDT = new List<ServiceCLS>();
                this.MedicineDuTruAndThucHien();

                long IntructionTimeT = 0;

                int Number_VT_YHCT = (this._ExpMestMetyReqADOCommons != null && this._ExpMestMetyReqADOCommons.Count > 0) ? this._ExpMestMetyReqADOCommons.Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT).Count() : 0;

                int dem_VT_YHCT = 1;
                foreach (var item in this._ExpMestMetyReqADOCommons)
                {
                    if (item.INTRUCTION_DATE != IntructionTimeT)
                    {
                        item.INTRUCTION_TIME_STR = item.INTRUCTION_DATE;
                        IntructionTimeT = item.INTRUCTION_DATE;
                    }
                    else
                    {
                        item.INTRUCTION_TIME_STR = 0;
                    }

                    if (item.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT)
                    {
                        string HDSD = item.TUTORIAL_WITH_ENTER;
                        if (dem_VT_YHCT != Number_VT_YHCT)
                        {
                            item.TUTORIAL_WITH_ENTER = null;
                        }
                        else
                        {
                            item.TUTORIAL_WITH_ENTER = "\n" + HDSD;
                        }
                        dem_VT_YHCT++;
                    }
                    else
                    {
                        string HDSD = item.TUTORIAL_WITH_ENTER;
                        item.TUTORIAL_WITH_ENTER = "\n" + HDSD;
                    }
                }

                //Vật tư dự trù
                this._ExpMestMatyReqADOsDuTru = new List<ExpMestMatyReqADO>();

                if (_ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0)
                {
                    this._ExpMestMatyReqADOsDuTru = _ExpMestMatyReqADOs.Where(o => _ServiceReqDuTrus.Select(p => p.ID).Contains(o.TDL_SERVICE_REQ_ID ?? 0)).ToList() ?? null;
                }

                //Vật tư thực hiện dự trù
                this._ExpMestMatyReqADOsTHDT = new List<ExpMestMatyReqADO>();

                if (_ServiceReqTHDT != null && _ServiceReqTHDT.Count > 0)
                {
                    this._ExpMestMatyReqADOsTHDT = _ExpMestMatyReqADOs.Where(o => _ServiceReqTHDT.Select(p => p.ID).Contains(o.TDL_SERVICE_REQ_ID ?? 0)).ToList() ?? null;
                }

                var servicereqIdDTs = _ExpMestMetyReqADOCommonsDuTru.Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList() ?? null;
                var servicereqIdTHDTs = _ExpMestMetyReqADOCommonsTHDT.Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList() ?? null;

                var serviceIdClsDTs = _ServiceClsDuTru.Select(o => o.SERVICE_REQ_ID).Distinct().ToList() ?? null;
                var serviceIdClsTHDTs = _ServiceClsTHDT.Select(o => o.SERVICE_REQ_ID).Distinct().ToList() ?? null;

                var serviceIdTtDTs = _ServiceTtDuTru.Select(o => o.SERVICE_REQ_ID).Distinct().ToList() ?? null;
                var serviceIdTtTHDTs = _ServiceTtTHDT.Select(o => o.SERVICE_REQ_ID).Distinct().ToList() ?? null;

                var VTservicereqIdDTs = _ExpMestMatyReqADOsDuTru.Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList() ?? null;
                var VTservicereqIdTHDTs = _ExpMestMatyReqADOsTHDT.Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList() ?? null;

                var _ServiceReqDuTrus_T = _ServiceReqDuTrus.Where(o => servicereqIdDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;
                var _ServiceReqTHDT_T = _ServiceReqTHDT.Where(o => servicereqIdTHDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;

                _ServiceReqDuTrus_T.ForEach(o => o.IS_NOT_SHOW_MATERIAL_TRACKING = 1);
                _ServiceReqTHDT_T.ForEach(o => o.IS_NOT_SHOW_MATERIAL_TRACKING = 1);

                #region y lệnh thuốc pha truyền dự trù
                var servicereqIdDTs_Mix = _MediInfusionDutru.Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList() ?? null;
                var servicereqIdTHDTs_Mix = _MediInfusionTHDT.Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList() ?? null;


                var _ServiceReqDuTrus_T_Mix = _ServiceReqDuTrus.Where(o => servicereqIdDTs_Mix.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;
                var _ServiceReqTHDT_T_Mix = _ServiceReqTHDT.Where(o => servicereqIdTHDTs_Mix.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;

                _ServiceReqDuTrus_T_Mix.ForEach(o => o.IS_NOT_SHOW_MATERIAL_TRACKING = 1);
                _ServiceReqTHDT_T_Mix.ForEach(o => o.IS_NOT_SHOW_MATERIAL_TRACKING = 1);
                #endregion

                var _ServiceReqDuTrus_VT = _ServiceReqDuTrus.Where(o => VTservicereqIdDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;
                var _ServiceReqTHDT_VT = _ServiceReqTHDT.Where(o => VTservicereqIdTHDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;

                var _ServiceReqDuTrus_Cls = _ServiceReqDuTrus.Where(o => serviceIdClsDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;
                var _ServiceReqTHDT_Cls = _ServiceReqTHDT.Where(o => serviceIdClsTHDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;
                var _ServiceReqDuTrus_Tt = _ServiceReqDuTrus.Where(o => serviceIdTtDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;
                var _ServiceReqTHDT_Tt = _ServiceReqTHDT.Where(o => serviceIdTtTHDTs.Contains(o.ID)).OrderBy(o => o.USE_TIME).ToList() ?? null;

                _ServiceReqDuTrus_VT.ForEach(o => o.IS_NOT_SHOW_MEDICINE_TRACKING = 1);
                _ServiceReqTHDT_VT.ForEach(o => o.IS_NOT_SHOW_MEDICINE_TRACKING = 1);

                _ServiceReqDuTrus_Cls.ForEach(o => { o.IS_NOT_SHOW_MEDICINE_TRACKING = null; o.IS_NOT_SHOW_MATERIAL_TRACKING = null; });
                _ServiceReqTHDT_Cls.ForEach(o => { o.IS_NOT_SHOW_MEDICINE_TRACKING = null; o.IS_NOT_SHOW_MATERIAL_TRACKING = null; });
                _ServiceReqDuTrus_Tt.ForEach(o => { o.IS_NOT_SHOW_MEDICINE_TRACKING = null; o.IS_NOT_SHOW_MATERIAL_TRACKING = null; });
                _ServiceReqTHDT_Tt.ForEach(o => { o.IS_NOT_SHOW_MEDICINE_TRACKING = null; o.IS_NOT_SHOW_MATERIAL_TRACKING = null; });

                _ServiceReqDuTrus = new List<HIS_SERVICE_REQ>();

                _ServiceReqDuTrus.AddRange(_ServiceReqDuTrus_T);
                _ServiceReqDuTrus.AddRange(_ServiceReqDuTrus_VT);
                _ServiceReqDuTrus.AddRange(_ServiceReqDuTrus_T_Mix);
                _ServiceReqDuTrus.AddRange(_ServiceReqDuTrus_Cls);
                _ServiceReqDuTrus.AddRange(_ServiceReqDuTrus_Tt);

                _ServiceReqTHDT = new List<HIS_SERVICE_REQ>();
                _ServiceReqTHDT.AddRange(_ServiceReqTHDT_T);
                _ServiceReqTHDT.AddRange(_ServiceReqTHDT_VT);
                _ServiceReqTHDT.AddRange(_ServiceReqTHDT_T_Mix);
                _ServiceReqTHDT.AddRange(_ServiceReqTHDT_Cls);
                _ServiceReqTHDT.AddRange(_ServiceReqTHDT_Tt);

                _ServiceReqDuTrus = _ServiceReqDuTrus.Distinct().ToList();
                _ServiceReqTHDT = _ServiceReqTHDT.Distinct().ToList();

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ServiceReqDuTrus), _ServiceReqDuTrus));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ServiceReqTHDT), _ServiceReqTHDT));

                this._ExpMestMatyReqADOs = this._ExpMestMatyReqADOs.Where(o => (o.USE_TIME <= o.INTRUCTION_TIME || o.USE_TIME == null) && o.USED_FOR_TRACKING_ID == null).OrderBy(o => o.INTRUCTION_TIME).ToList();

                long IntructionTimeVT = 0;
                foreach (var item in this._ExpMestMatyReqADOs)
                {
                    if (item.INTRUCTION_TIME != IntructionTimeVT)
                    {
                        item.INTRUCTION_TIME_STR = item.INTRUCTION_TIME;
                        IntructionTimeVT = item.INTRUCTION_TIME;
                    }
                    else
                    {
                        item.INTRUCTION_TIME_STR = 0;
                    }
                }

                List<RemedyCountADO> RemedyCountADONew = new List<RemedyCountADO>();

                if (this._RemedyCountADOs != null && this._RemedyCountADOs.Count > 0)
                {
                    RemedyCountADONew.AddRange(this._RemedyCountADOs.Where(o => o.EXP_MEST_ID == 0));
                }

                var listExpMestID = this._ExpMestMetyReqADOCommons.Select(o => o.EXP_MEST_ID).Distinct().ToList();

                foreach (var itemId in listExpMestID)
                {
                    if (this._RemedyCountADOs != null && this._RemedyCountADOs.Count > 0)
                    {
                        RemedyCountADONew.AddRange(this._RemedyCountADOs.Where(o => o.EXP_MEST_ID == itemId));
                    }
                }

                this._RemedyCountADOs = RemedyCountADONew;

                this._ServiceReqMetyADOs = this._ServiceReqMetyADOs.OrderBy(o => o.INTRUCTION_TIME_STR).ToList();
                long MetyADOTime = 0;
                _ServiceReqMetyADOs = ProcessSortListServiceReqMety(_ServiceReqMetyADOs);
                foreach (var item in _ServiceReqMetyADOs)
                {
                    if (MetyADOTime == item.INTRUCTION_TIME_STR)
                    {
                        item.INTRUCTION_TIME_STR = 0;
                    }
                    else
                    {
                        MetyADOTime = item.INTRUCTION_TIME_STR;
                    }
                }

                this._ServiceReqMatyADOs = this._ServiceReqMatyADOs.OrderBy(o => o.INTRUCTION_TIME_STR).ToList();

                long MatyADOTime = 0;
                foreach (var item in _ServiceReqMatyADOs)
                {
                    if (MatyADOTime == item.INTRUCTION_TIME_STR)
                    {
                        item.INTRUCTION_TIME_STR = 0;
                    }
                    else
                    {
                        MatyADOTime = item.INTRUCTION_TIME_STR;
                    }
                }

                //xếp ngày dịch vụ
                if (this._ServiceCLSs != null && this._ServiceCLSs.Count > 0 && (this._ServiceCLSs.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() > 1))
                {

                    this._ServiceCLSs = this._ServiceCLSs.OrderBy(o => o.TDL_INTRUCTION_DATE).ToList();
                    long IntructionTimeDV = 0;
                    foreach (var item in this._ServiceCLSs)
                    {
                        if (item.TDL_INTRUCTION_DATE != IntructionTimeDV)
                        {
                            item.INTRUCTION_DATE_STR = item.TDL_INTRUCTION_DATE;
                            IntructionTimeDV = item.TDL_INTRUCTION_DATE;
                        }
                        else
                        {
                            item.INTRUCTION_DATE_STR = 0;
                        }
                    }
                }
                //xếp ngày dịch vụ
                if (this._ServiceCLSSplits != null && this._ServiceCLSSplits.Count > 0 && (this._ServiceCLSSplits.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() > 1))
                {

                    this._ServiceCLSSplits = this._ServiceCLSSplits.OrderBy(o => o.TDL_INTRUCTION_DATE).ToList();
                    long IntructionTimeDV = 0;
                    foreach (var item in this._ServiceCLSSplits)
                    {
                        if (item.TDL_INTRUCTION_DATE != IntructionTimeDV)
                        {
                            item.INTRUCTION_DATE_STR = item.TDL_INTRUCTION_DATE;
                            IntructionTimeDV = item.TDL_INTRUCTION_DATE;
                        }
                        else
                        {
                            item.INTRUCTION_DATE_STR = 0;
                        }
                    }
                }

                //xếp ngày dịch vụ
                if (this._ServiceCLSOrders != null && this._ServiceCLSOrders.Count > 0 && (this._ServiceCLSOrders.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() > 0))
                {
                    this._ServiceCLSOrders = this._ServiceCLSOrders.OrderBy(o => o.TDL_INTRUCTION_DATE).ThenBy(o => o.TYPE_ID).ToList();
                    long IntructionTimeDV = 0;
                    long? TestTypeIdOld = null;
                    string ServiceTypeNameOld = "";
                    foreach (var item in this._ServiceCLSOrders)
                    {
                        if (item.TDL_INTRUCTION_DATE != IntructionTimeDV)
                        {
                            item.INTRUCTION_DATE_STR = item.TDL_INTRUCTION_DATE;
                            IntructionTimeDV = item.TDL_INTRUCTION_DATE;
                        }
                        else
                        {
                            item.INTRUCTION_DATE_STR = 0;
                        }

                        if (item.TEST_TYPE_ID != TestTypeIdOld)
                        {
                            TestTypeIdOld = item.TEST_TYPE_ID;
                        }
                        else
                        {
                            item.TEST_TYPE_ID = null;
                            item.TEST_TYPE_CODE = null;
                            item.TEST_TYPE_NAME = null;
                        }

                        if (item.SERVICE_TYPE_NAME != ServiceTypeNameOld)
                        {
                            ServiceTypeNameOld = item.SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            item.SERVICE_TYPE_NAME = null;
                        }
                    }
                }

                if (this._ServiceCLSSplitXNs != null && this._ServiceCLSSplitXNs.Count > 0 && (this._ServiceCLSSplitXNs.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() > 0))
                {
                    long ServiceTypeId = 0;
                    List<ServiceCLS> _ServiceCLSSplitXNs_1 = new List<ServiceCLS>();
                    _ServiceCLSSplitXNs_1.AddRange(this._ServiceCLSSplitXNs);
                    _ServiceCLSSplitXNs_1 = _ServiceCLSSplitXNs_1.OrderBy(o => o.TDL_INTRUCTION_DATE).ThenBy(o => o.TYPE_ID).ThenBy(o => o.TEST_TYPE_ID).ToList();
                    var _SplitXN_Group = _ServiceCLSSplitXNs_1.GroupBy(o => new { o.TYPE_ID, o.TEST_TYPE_ID });
                    this._ServiceCLSSplitXNs = new List<ServiceCLS>();
                    foreach (var Group in _SplitXN_Group)
                    {
                        ServiceCLS cls = Group.FirstOrDefault();
                        cls.SERVICE_NAME = String.Join("; ", Group.Select(o => o.SERVICE_NAME).ToList());
                        if (ServiceTypeId == cls.TDL_SERVICE_TYPE_ID)
                        {
                            cls.SERVICE_TYPE_NAME = "";
                        }
                        else
                            ServiceTypeId = cls.TDL_SERVICE_TYPE_ID;
                        this._ServiceCLSSplitXNs.Add(cls);
                    }

                    this._ServiceCLSSplitXNs = this._ServiceCLSSplitXNs.OrderBy(o => o.TYPE_ID).ToList();
                }

                //xếp ngày dịch vụ máu
                if (this._Bloods != null && this._Bloods.Count > 0 && (this._Bloods.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() > 1))
                {

                    this._Bloods = this._Bloods.OrderBy(o => o.TDL_INTRUCTION_DATE).ToList();
                    long IntructionTimeBlood = 0;
                    foreach (var item in this._Bloods)
                    {
                        if (item.TDL_INTRUCTION_DATE != IntructionTimeBlood)
                        {
                            item.INTRUCTION_DATE_STR = item.TDL_INTRUCTION_DATE;
                            IntructionTimeBlood = item.TDL_INTRUCTION_DATE;
                        }
                        else
                        {
                            item.INTRUCTION_DATE_STR = 0;
                        }
                    }
                }

                //xếp ngày dịch vụ khám
                if (this._ExamServices != null && this._ExamServices.Count > 0 && (this._ExamServices.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() > 1))
                {

                    this._ExamServices = this._ExamServices.OrderBy(o => o.TDL_INTRUCTION_DATE).ToList();
                    long IntructionTimeExam = 0;
                    foreach (var item in this._ExamServices)
                    {
                        if (item.TDL_INTRUCTION_DATE != IntructionTimeExam)
                        {
                            item.INTRUCTION_DATE_STR = item.TDL_INTRUCTION_DATE;
                            IntructionTimeExam = item.TDL_INTRUCTION_DATE;
                        }
                        else
                        {
                            item.INTRUCTION_DATE_STR = 0;
                        }
                    }
                }

                long IntructionDateS = 0;

                lstServiceReqs = lstServiceReqs.OrderBy(o => o.INTRUCTION_DATE).ToList();
                var ServiceReqGroup = lstServiceReqs.GroupBy(o => o.TRACKING_ID).Select(p => p.ToList()).ToList();
                foreach (var itemG in ServiceReqGroup)
                {
                    foreach (var item in itemG)
                    {
                        if (item.INTRUCTION_DATE != IntructionDateS)
                        {
                            IntructionDateS = item.INTRUCTION_DATE;
                        }
                        else
                        {
                            item.INTRUCTION_DATE = 0;
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("_ExpMestMetyReqADOs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._ExpMestMetyReqADOs), this._ExpMestMetyReqADOs));


                result = (this.templateType == ProcessorBase.PrintConfig.TemplateType.Excel) ? ProcessDataExcel() : ((this.templateType == ProcessorBase.PrintConfig.TemplateType.Word) ? ProcessDataWord() : ProcessDataXtraReport());

                Inventec.Common.Logging.LogSystem.Debug("Mps000062 ------ ProcessData----2");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// Lấy thuốc dự trù
        /// </summary>
        /// <returns></returns>
        public void MedicineDuTruAndThucHien()
        {
            try
            {
                if (this._ExpMestMetyReqADOs != null && this._ExpMestMetyReqADOs.Count > 0
                   )
                {
                    List<ExpMestMetyReqADO> _ExpMestMetyReqADOsDuTru = new List<ExpMestMetyReqADO>();
                    List<ExpMestMetyReqADO> _ExpMestMetyReqADOsTHDT = new List<ExpMestMetyReqADO>();


                    if (_ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0)
                    {
                        _ExpMestMetyReqADOsDuTru = this._ExpMestMetyReqADOs.Where(o => _ServiceReqDuTrus.Select(p => p.ID).Contains(o.TDL_SERVICE_REQ_ID ?? 0)).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    }

                    if (_ServiceReqTHDT != null && _ServiceReqTHDT.Count > 0)
                    {
                        _ExpMestMetyReqADOsTHDT = this._ExpMestMetyReqADOs.Where(o => _ServiceReqTHDT.Select(p => p.ID).Contains(o.TDL_SERVICE_REQ_ID ?? 0)).OrderBy(o => o.INTRUCTION_DATE).ToList();
                    }

                    this.DataMedicine_Merge(_ExpMestMetyReqADOsDuTru, this._ExpMestMetyReqADOCommonsDuTru_Merge);


                    var dataGroupsIntructionDuTru = (_ExpMestMetyReqADOsDuTru != null && _ExpMestMetyReqADOsDuTru.Count > 0) ? _ExpMestMetyReqADOsDuTru.Where(p => p.MIXED_INFUSION == null).GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList() : null;
                    var dataGroupsIntructionDuTruMix = (_ExpMestMetyReqADOsDuTru != null && _ExpMestMetyReqADOsDuTru.Count > 0) ? _ExpMestMetyReqADOsDuTru.Where(p => p.MIXED_INFUSION != null).GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList() : null;

                    var dataGroupsIntructionTHDT = (_ExpMestMetyReqADOsTHDT != null && _ExpMestMetyReqADOsTHDT.Count > 0) ? _ExpMestMetyReqADOsTHDT.Where(p => p.MIXED_INFUSION == null).GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList() : null;
                    var dataGroupsIntructionTHDTMix = (_ExpMestMetyReqADOsTHDT != null && _ExpMestMetyReqADOsTHDT.Count > 0) ? _ExpMestMetyReqADOsTHDT.Where(p => p.MIXED_INFUSION != null).GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList() : null;

                    #region thuốc dự trù
                    if (dataGroupsIntructionDuTru != null && dataGroupsIntructionDuTru.Count() > 0)
                    {
                        foreach (var itemIn in dataGroupsIntructionDuTru)
                        {
                            if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                            {
                                List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                                var itemOrder = itemIn.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                                var dataGroups = itemOrder.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                                foreach (var itemGr in dataGroups)
                                {
                                    var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();
                                    _dataNews1.AddRange(dtGroups);
                                }

                                this._ExpMestMetyReqADOCommonsDuTru.AddRange(_dataNews1);
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                            {
                                this._ExpMestMetyReqADOCommonsDuTru.AddRange(itemIn.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());

                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                            {
                                this._ExpMestMetyReqADOCommonsDuTru.AddRange(itemIn.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());

                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                            {

                                this._ExpMestMetyReqADOCommonsDuTru.AddRange(itemIn.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                            {
                                this._ExpMestMetyReqADOCommonsDuTru.AddRange(ProcessSortListExpMestMetyReq(itemIn));
                            }
                        }
                    }
                    #endregion

                    #region thuốc pha truyền dự trù
                    if (dataGroupsIntructionDuTruMix != null && dataGroupsIntructionDuTruMix.Count() > 0)
                    {
                        foreach (var itemIn in dataGroupsIntructionDuTruMix)
                        {
                            if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                            {
                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionDutru.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionDutru.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                            {
                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionDutru.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionDutru.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                            {

                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionDutru.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionDutru.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                            {
                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionDutru.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionDutru.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                            {
                                foreach (var itemIsMixed in itemIn)
                                {
                                    this._MediInfusionDutru.AddRange(ProcessSortListExpMestMetyReq(new List<ExpMestMetyReqADO>() { itemIsMixed }));
                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).ToList();

                                    this._MediInfusionDutru.AddRange(ProcessSortListExpMestMetyReq(childMixed));
                                }
                            }
                        }
                    }
                    _MediInfusionDutru = _MediInfusionDutru.Distinct().ToList();
                    #endregion

                    #region thuốc thực hiện dự trù
                    if (dataGroupsIntructionTHDT != null && dataGroupsIntructionTHDT.Count() > 0)
                    {
                        foreach (var itemIn in dataGroupsIntructionTHDT)
                        {
                            if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                            {
                                List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                                var itemOrder = itemIn.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                                var dataGroups = itemOrder.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                                foreach (var itemGr in dataGroups)
                                {
                                    var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();
                                    _dataNews1.AddRange(dtGroups);
                                }

                                this._ExpMestMetyReqADOCommonsTHDT.AddRange(_dataNews1);
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                            {
                                this._ExpMestMetyReqADOCommonsTHDT.AddRange(itemIn.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());

                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                            {
                                this._ExpMestMetyReqADOCommonsTHDT.AddRange(itemIn.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());

                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                            {

                                this._ExpMestMetyReqADOCommonsTHDT.AddRange(itemIn.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                            {
                                this._ExpMestMetyReqADOCommonsTHDT.AddRange(ProcessSortListExpMestMetyReq(itemIn));
                            }
                        }
                    }
                    #endregion

                    #region thuốc pha truyền thực hiện dự trù
                    if (dataGroupsIntructionTHDTMix != null && dataGroupsIntructionTHDTMix.Count() > 0)
                    {
                        foreach (var itemIn in dataGroupsIntructionTHDTMix)
                        {
                            if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                            {
                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionTHDT.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ThenByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionTHDT.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                            {
                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionTHDT.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionTHDT.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                            {

                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionTHDT.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionTHDT.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                            {
                                var itemIsMixedMains = itemIn.OrderBy(i => i.EXP_MEST_ID).OrderBy(n => n.IS_MIXED_MAIN ?? 999999).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                foreach (var itemIsMixed in itemIsMixedMains)
                                {
                                    this._MediInfusionTHDT.Add(itemIsMixed);

                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();

                                    this._MediInfusionTHDT.AddRange(childMixed);
                                }
                            }
                            else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                            {
                                foreach (var itemIsMixed in itemIn)
                                {
                                    this._MediInfusionTHDT.AddRange(ProcessSortListExpMestMetyReq(new List<ExpMestMetyReqADO>() { itemIsMixed }));
                                    var childMixed = itemIn.Where(o => o.EXP_MEST_ID == itemIsMixed.EXP_MEST_ID && o.IS_MIXED_MAIN != 1 && o.MIXED_INFUSION == itemIsMixed.MIXED_INFUSION).ToList();

                                    this._MediInfusionTHDT.AddRange(ProcessSortListExpMestMetyReq(childMixed));
                                }
                            }
                        }
                    }
                    _MediInfusionTHDT = _MediInfusionTHDT.Distinct().ToList();
                    #endregion
                }


                #region dv cls dự trù
                if (_ServiceCLSSplits != null && _ServiceCLSSplits.Count > 0 && _ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0)
                {
                    _ServiceClsDuTru = _ServiceCLSSplits.Where(o => _ServiceReqDuTrus.Exists(p => p.ID == o.SERVICE_REQ_ID && p.USE_TIME > p.INTRUCTION_TIME)).ToList();
                    if (_ServiceClsDuTru != null && _ServiceClsDuTru.Count > 0)
                    {
                        _ServiceClsDuTru.ForEach(o => o.USE_TIME = _ServiceReqDuTrus.FirstOrDefault(p => p.ID == o.SERVICE_REQ_ID).USE_TIME);
                    }
                }
                #endregion

                #region dv tt dự trù
                if (_TTServices != null && _TTServices.Count > 0 && _ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0)
                {
                    _ServiceTtDuTru = _TTServices.Where(o => _ServiceReqDuTrus.Exists(p => p.ID == o.SERVICE_REQ_ID && p.USE_TIME > p.INTRUCTION_TIME)).ToList();
                    if (_ServiceTtDuTru != null && _ServiceTtDuTru.Count > 0)
                    {
                        _ServiceTtDuTru.ForEach(o => o.USE_TIME = _ServiceReqDuTrus.FirstOrDefault(p => p.ID == o.SERVICE_REQ_ID).USE_TIME);
                        _TTServices = _TTServices.Where(o => !_ServiceTtDuTru.Exists(p => p.SERVICE_ID == o.SERVICE_ID && p.SERVICE_REQ_ID == o.SERVICE_REQ_ID)).ToList();
                    }
                }
                #endregion



                #region thực hiện dv cls dự trù

                var serviceReqTracking = _ServiceReqTHDT != null && _ServiceReqTHDT.Count > 0 ? _ServiceReqTHDT.Where(o => rdo._Trackings.Exists(p => o.USED_FOR_TRACKING_ID == p.ID)).ToList() : null;
                if (_ServiceCLSSplits != null && _ServiceCLSSplits.Count > 0 && serviceReqTracking != null && serviceReqTracking.Count > 0)
                {
                    _ServiceClsTHDT = _ServiceCLSSplits.Where(o => serviceReqTracking.Exists(p => p.ID == o.SERVICE_REQ_ID)).ToList();
                    if (_ServiceClsTHDT != null && _ServiceClsTHDT.Count > 0)
                    {
                        _ServiceClsTHDT.ForEach(o => o.USE_TIME = serviceReqTracking.FirstOrDefault(p => p.ID == o.SERVICE_REQ_ID).USE_TIME);
                    }
                }
                #endregion

                #region thực hiện tt dự trù
                if (_TTServices != null && _TTServices.Count > 0 && serviceReqTracking != null && serviceReqTracking.Count > 0)
                {
                    _ServiceTtTHDT = _TTServices.Where(o => serviceReqTracking.Exists(p => p.ID == o.SERVICE_REQ_ID)).ToList();
                    if (_ServiceTtTHDT != null && _ServiceTtTHDT.Count > 0)
                    {
                        _ServiceTtTHDT.ForEach(o => o.USE_TIME = serviceReqTracking.FirstOrDefault(p => p.ID == o.SERVICE_REQ_ID).USE_TIME);
                        _TTServices = _TTServices.Where(o => !_ServiceTtTHDT.Exists(p => p.SERVICE_ID == o.SERVICE_ID && p.SERVICE_REQ_ID == o.SERVICE_REQ_ID)).ToList();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

                Inventec.Common.Logging.LogSystem.Info("thuốc pha truyền: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOCommonsMix), _ExpMestMetyReqADOCommonsMix));
                Inventec.Common.Logging.LogSystem.Info("thuốc: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOCommons), _ExpMestMetyReqADOCommons));

                Inventec.Common.Logging.LogSystem.Info("vật tư: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMatyReqADOs), _ExpMestMatyReqADOs));

                Inventec.Common.Logging.LogSystem.Info("_RemedyCountADOs: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _RemedyCountADOs), _RemedyCountADOs));
                Inventec.Common.Logging.LogSystem.Info("thuốc dự trù: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOCommonsDuTru), _ExpMestMetyReqADOCommonsDuTru));
                Inventec.Common.Logging.LogSystem.Info("thuốc THDT: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMetyReqADOCommonsTHDT), _ExpMestMetyReqADOCommonsTHDT));
                Inventec.Common.Logging.LogSystem.Info("vật tư dự trù: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMatyReqADOsDuTru), _ExpMestMatyReqADOsDuTru));
                Inventec.Common.Logging.LogSystem.Info("vật tư THDT: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMatyReqADOsTHDT), _ExpMestMetyReqADOCommonsTHDT));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("thuốc pha truyền dự trù: ", _MediInfusionDutru));
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("thuốc pha truyền THDT: ", _MediInfusionTHDT));


                objectTag.AddObjectData(store, "Medicines", ProcessSortListExpMestMetyReq(_ExpMestMetyReqADOCommons) ?? new List<ExpMestMetyReqADO>());
                objectTag.AddObjectData(store, "MedicinesInfusion", ProcessSortListExpMestMetyReq(_ExpMestMetyReqADOCommonsMix) ?? new List<ExpMestMetyReqADO>());
                objectTag.AddObjectData(store, "MedicinesDuTru", ProcessSortListExpMestMetyReq(_ExpMestMetyReqADOCommonsDuTru) ?? new List<ExpMestMetyReqADO>());
                objectTag.AddObjectData(store, "MedicinesTHDT", ProcessSortListExpMestMetyReq(_ExpMestMetyReqADOCommonsTHDT) ?? new List<ExpMestMetyReqADO>());
                objectTag.AddObjectData(store, "MediInfusionDutru", ProcessSortListExpMestMetyReq(_MediInfusionDutru) ?? new List<ExpMestMetyReqADO>());
                objectTag.AddObjectData(store, "MediInfusionTHDT", ProcessSortListExpMestMetyReq(_MediInfusionTHDT) ?? new List<ExpMestMetyReqADO>());

                objectTag.AddObjectData(store, "ServiceReq", lstServiceReqs ?? new List<HIS_SERVICE_REQ>());
                objectTag.AddObjectData(store, "ServiceReqDuTru", _ServiceReqDuTrus ?? new List<HIS_SERVICE_REQ>());
                objectTag.AddObjectData(store, "ServiceReqTHDT", _ServiceReqTHDT ?? new List<HIS_SERVICE_REQ>());

                objectTag.AddObjectData(store, "TrackingADOs", this._Mps000062ADOs);
                objectTag.AddObjectData(store, "RemedyCount", this._RemedyCountADOs);

                objectTag.AddObjectData(store, "Materials", this._ExpMestMatyReqADOs);
                objectTag.AddObjectData(store, "MaterialsDuTru", this._ExpMestMatyReqADOsDuTru);
                objectTag.AddObjectData(store, "MaterialsTHDT", this._ExpMestMatyReqADOsTHDT);
                //V+ 175853 
                objectTag.AddObjectData(store, "ServiceReqMety", ProcessSortListServiceReqMety(this._ServiceReqMetyADOs));
                objectTag.AddObjectData(store, "ServiceReqMaty", this._ServiceReqMatyADOs);
                objectTag.AddObjectData(store, "ServiceCLS", this._ServiceCLSs);
                objectTag.AddObjectData(store, "ServiceCLSOrder", this._ServiceCLSOrders);
                objectTag.AddObjectData(store, "ServiceCLSSplitXN", this._ServiceCLSSplitXNs);
                objectTag.AddObjectData(store, "CARE", rdo._Cares);
                objectTag.AddObjectData(store, "CareDetail", rdo._CareDetails);
                objectTag.AddObjectData(store, "MedicalInstruction", this._MedicalInstructions);
                objectTag.AddObjectData(store, "Bloods", this._Bloods);
                objectTag.AddObjectData(store, "ExamServices", this._ExamServices);
                objectTag.AddObjectData(store, "TTServices", this._TTServices);

                objectTag.AddObjectData(store, "ServiceCLSDuTru", this._ServiceClsDuTru);
                objectTag.AddObjectData(store, "ServiceCLSTHDT", this._ServiceClsTHDT);
                objectTag.AddObjectData(store, "TTServicesDuTru", this._ServiceTtDuTru);
                objectTag.AddObjectData(store, "TTServicesTHDT", this._ServiceTtTHDT);
                //TH
                objectTag.AddObjectData(store, "ImpMestMedicine", this._ImpMestMedicineADOs);
                objectTag.AddObjectData(store, "ImpMestMaterial", this._ImpMestMaterialADOs);
                objectTag.AddObjectData(store, "ImpMestBlood", this._ImpMestBloodADOs);
                objectTag.AddObjectData(store, "MedicineLines", this.MedicineLineADOs);

                objectTag.SetUserFunction(store, "FuncMergeData", new CalculateMergerData() { _Mps000062ADOs = this._Mps000062ADOs });

                //suat an
                objectTag.AddObjectData(store, "Ration", this._SereServRationADO);

                objectTag.AddRelationship(store, "TrackingADOs", "MedicineLines", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "MedicineLines", "Medicines", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLines", "RemedyCount", "ID", "MEDICINE_LINE_ID");
                objectTag.AddRelationship(store, "MedicineLines", "MedicinesInfusion", "ID", "MEDICINE_LINE_ID");
                //objectTag.AddRelationship(store, "MedicineLines", "MedicinesDuTru", "ID", "MEDICINE_LINE_ID");
                //objectTag.AddRelationship(store, "MedicineLines", "MedicinesTHDT", "ID", "MEDICINE_LINE_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "RemedyCount", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "RemedyCount", "Medicines", "EXP_MEST_ID", "EXP_MEST_ID");
                objectTag.AddRelationship(store, "RemedyCount", "MedicinesInfusion", "EXP_MEST_ID", "EXP_MEST_ID");
                //objectTag.AddRelationship(store, "RemedyCount", "MedicinesDuTru", "EXP_MEST_ID", "EXP_MEST_ID");
                //objectTag.AddRelationship(store, "RemedyCount", "MedicinesTHDT", "EXP_MEST_ID", "EXP_MEST_ID");

                //objectTag.AddRelationship(store, "RemedyCount", "Medicines", "MEDICINE_LINE_ID", "MEDICINE_LINE_ID");

                objectTag.AddRelationship(store, "ServiceReq", "Medicines", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReq", "MedicinesInfusion", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReqDuTru", "MedicinesDuTru", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReqDuTru", "MaterialsDuTru", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReqDuTru", "MediInfusionDutru", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReqTHDT", "MedicinesTHDT", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReqTHDT", "MaterialsTHDT", "ID", "TDL_SERVICE_REQ_ID");
                objectTag.AddRelationship(store, "ServiceReqTHDT", "MediInfusionTHDT", "ID", "TDL_SERVICE_REQ_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReqMety", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReqMaty", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLS", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLSOrder", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLSSplitXN", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "Bloods", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ExamServices", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "TTServices", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "CARE", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "CareDetail", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLSDuTru", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceCLSTHDT", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "TTServicesDuTru", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "TTServicesTHDT", "ID", "TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "Medicines", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "Materials", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MedicinesInfusion", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MaterialsDuTru", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MedicinesDuTru", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MediInfusionDutru", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MedicinesTHDT", "ID", "USED_FOR_TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MaterialsTHDT", "ID", "USED_FOR_TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "MediInfusionTHDT", "ID", "USED_FOR_TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "MedicalInstruction", "ID", "TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "ImpMestMedicine", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ImpMestMaterial", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ImpMestBlood", "ID", "TRACKING_ID");

                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReq", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReqDuTru", "ID", "TRACKING_ID");
                objectTag.AddRelationship(store, "TrackingADOs", "ServiceReqTHDT", "ID", "USED_FOR_TRACKING_ID");

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        private List<ExpMestMedicineADO> ProcessSortListExpMestMedicine(List<ExpMestMedicineADO> expMestMedicine)
        {
            List<ExpMestMedicineADO> data = new List<ExpMestMedicineADO>();
            if (expMestMedicine == null)
                return data;
            try
            {
                if (rdo._WorkPlaceSDO.IsOrderByType == 4 && expMestMedicine != null && expMestMedicine.Count > 0)
                {
                    var medicinegroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();
                    foreach (var current in expMestMedicine)
                    {
                        ExpMestMedicineADO ado = new ExpMestMedicineADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(ado, current);
                        var mediType = rdo._MedicineTypes.FirstOrDefault(o => o.ID == current.TDL_MEDICINE_TYPE_ID);
                        if (rdo.MedicineLine != null && rdo.MedicineLine.Count > 0)
                        {
                            var mediLine = rdo.MedicineLine.FirstOrDefault(o => o.ID == mediType.MEDICINE_LINE_ID);
                            if (mediLine != null)
                            {
                                current.NUM_ORDER_MEDICINE_LINE = mediLine.NUM_ORDER ?? 0;
                                current.MEDICINE_LINE_NAME = mediLine.MEDICINE_LINE_NAME;
                            }
                            var mediUf = rdo._MedicineUseForms.FirstOrDefault(o => o.ID == mediType.MEDICINE_USE_FORM_ID);
                            if (mediUf != null)
                            {
                                current.NUM_ORDER_MEDICINE_USE_FORM = mediUf.NUM_ORDER ?? 0;
                                current.MEDICINE_USE_FORM_NAME = mediUf.MEDICINE_USE_FORM_NAME;
                            }
                            var mediG = medicinegroups.FirstOrDefault(o => o.ID == mediType.MEDICINE_GROUP_ID);
                            if (mediG != null)
                            {
                                current.NUM_ORDER_MEDICINE_GROUP = mediG.NUM_ORDER ?? 0;
                                current.MEDICINE_GROUP_NAME = mediG.MEDICINE_GROUP_NAME;
                            }
                            var dosaForm = rdo.DosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME == mediType.DOSAGE_FORM);
                            if (dosaForm != null)
                            {
                                current.NUM_ORDER_DOSAGE_FORM = dosaForm.NUM_ORDER ?? 0;
                                current.DOSAGE_FORM_NAME = dosaForm.DOSAGE_FORM_NAME;
                            }
                        }
                        data.Add(ado);
                    }
                    data = data.OrderByDescending(o => o.NUM_ORDER_MEDICINE_LINE ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_USE_FORM ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_GROUP ?? -1).ThenByDescending(o => o.NUM_ORDER_DOSAGE_FORM ?? -1).ThenBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = expMestMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }
        private List<ImpMestMedicineADO> ProcessSortListImpMestMedicine(List<ImpMestMedicineADO> impMestMedicine)
        {
            List<ImpMestMedicineADO> data = new List<ImpMestMedicineADO>();
            if (impMestMedicine == null)
                return data;
            try
            {
                if (impMestMedicine != null && impMestMedicine.Count > 0)
                {
                    var medicinegroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();
                    foreach (var current in impMestMedicine)
                    {
                        ImpMestMedicineADO ado = new ImpMestMedicineADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMedicineADO>(ado, current);
                        var mediType = rdo._MedicineTypes.FirstOrDefault(o => o.ID == current.MEDICINE_TYPE_ID);
                        if (rdo.MedicineLine != null && rdo.MedicineLine.Count > 0)
                        {
                            var mediLine = rdo.MedicineLine.FirstOrDefault(o => o.ID == mediType.MEDICINE_LINE_ID);
                            if (mediLine != null)
                            {
                                ado.NUM_ORDER_MEDICINE_LINE = mediLine.NUM_ORDER ?? 0;
                                ado.MEDICINE_LINE_NAME = mediLine.MEDICINE_LINE_NAME;
                            }
                            var mediUf = rdo._MedicineUseForms.FirstOrDefault(o => o.ID == mediType.MEDICINE_USE_FORM_ID);
                            if (mediUf != null)
                            {
                                ado.NUM_ORDER_MEDICINE_USE_FORM = mediUf.NUM_ORDER ?? 0;
                                ado.MEDICINE_USE_FORM_NAME = mediUf.MEDICINE_USE_FORM_NAME;
                            }
                            var mediG = medicinegroups.FirstOrDefault(o => o.ID == mediType.MEDICINE_GROUP_ID);
                            if (mediG != null)
                            {
                                ado.NUM_ORDER_MEDICINE_GROUP = mediG.NUM_ORDER ?? 0;
                                ado.MEDICINE_GROUP_NAME = mediG.MEDICINE_GROUP_NAME;
                            }
                            var dosaForm = rdo.DosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME == mediType.DOSAGE_FORM);
                            if (dosaForm != null)
                            {
                                ado.NUM_ORDER_DOSAGE_FORM = dosaForm.NUM_ORDER ?? 0;
                                ado.DOSAGE_FORM_NAME = dosaForm.DOSAGE_FORM_NAME;
                            }
                        }
                        data.Add(ado);
                    }
                    switch (rdo._WorkPlaceSDO.IsOrderByType)
                    {
                        case 1:
                            data = data.OrderByDescending(o => o.NUM_ORDER_MEDICINE_USE_FORM ?? -1).ThenBy(o => o.INTRUCTION_TIME_OLD ?? Int64.MaxValue).ToList();
                            break;
                        case 2:
                            data = data.OrderByDescending(o => o.NUM_ORDER_MEDICINE_GROUP ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_USE_FORM).ThenBy(o => o.INTRUCTION_TIME_OLD ?? Int64.MaxValue).ToList();
                            break;
                        case 3:
                            data = data.OrderBy(o => o.NUM_ORDER_MEDICINE_USE_FORM ?? -1).ThenBy(o => o.INTRUCTION_TIME_OLD ?? Int64.MaxValue).ToList();
                            break;
                        case 4:
                            data = data.OrderByDescending(o => o.NUM_ORDER_MEDICINE_LINE ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_USE_FORM ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_GROUP ?? -1).ThenByDescending(o => o.NUM_ORDER_DOSAGE_FORM ?? -1).ThenBy(o => o.NUM_ORDER).ToList();
                            break;
                        default:
                            break;
                    }
                }
                else
                    data = impMestMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }

        private List<ServiceReqMetyADO> ProcessSortListServiceReqMety(List<ServiceReqMetyADO> serviceReqMeties)
        {
            List<ServiceReqMetyADO> data = new List<ServiceReqMetyADO>();
            if (serviceReqMeties == null)
                return data;
            try
            {
                if (rdo._WorkPlaceSDO.IsOrderByType == 4 && serviceReqMeties != null && serviceReqMeties.Count > 0)
                {
                    var medicinegroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();
                    foreach (var current in serviceReqMeties)
                    {
                        ServiceReqMetyADO ado = new ServiceReqMetyADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqMetyADO>(ado, current);
                        var mediType = rdo._MedicineTypes.FirstOrDefault(o => o.ID == ado.MEDICINE_TYPE_ID);
                        if (rdo.MedicineLine != null && rdo.MedicineLine.Count > 0)
                        {
                            var mediLine = rdo.MedicineLine.FirstOrDefault(o => o.ID == mediType.MEDICINE_LINE_ID);
                            if (mediLine != null)
                            {
                                ado.NUM_ORDER_MEDICINE_LINE = mediLine.NUM_ORDER ?? 0;
                                ado.MEDICINE_LINE_NAME = mediLine.MEDICINE_LINE_NAME;
                            }
                        }
                        if (rdo._MedicineUseForms != null && rdo._MedicineUseForms.Count > 0)
                        {
                            var mediUf = rdo._MedicineUseForms.FirstOrDefault(o => o.ID == mediType.MEDICINE_USE_FORM_ID);
                            if (mediUf != null)
                            {
                                ado.NUM_ORDER_MEDICINE_USE_FORM = mediUf.NUM_ORDER ?? 0;
                                ado.MEDICINE_USE_FORM_NAME = mediUf.MEDICINE_USE_FORM_NAME;
                            }
                        }
                        if (medicinegroups != null && medicinegroups.Count > 0)
                        {
                            var mediG = medicinegroups.FirstOrDefault(o => o.ID == mediType.MEDICINE_GROUP_ID);
                            if (mediG != null)
                            {
                                ado.NUM_ORDER_MEDICINE_GROUP = mediG.NUM_ORDER ?? 0;
                                ado.MEDICINE_GROUP_NAME = mediG.MEDICINE_GROUP_NAME;
                            }
                        }

                        if (rdo.DosageForm != null && rdo.DosageForm.Count > 0)
                        {
                            var dosaForm = rdo.DosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME == mediType.DOSAGE_FORM);
                            if (dosaForm != null)
                            {
                                ado.NUM_ORDER_DOSAGE_FORM = dosaForm.NUM_ORDER ?? 0;
                                ado.DOSAGE_FORM_NAME = dosaForm.DOSAGE_FORM_NAME;
                            }
                        }
                        data.Add(ado);
                    }
                    data = data.OrderByDescending(o => o.NUM_ORDER_MEDICINE_LINE ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_USE_FORM ?? -1).ThenByDescending(o => o.NUMBER_BY_GROUP ?? -1).ThenByDescending(o => o.NUM_ORDER_DOSAGE_FORM ?? -1).ThenBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = serviceReqMeties;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }

        private List<ExpMestMetyReqADO> ProcessSortListExpMestMetyReq(List<ExpMestMetyReqADO> lstExpMest)
        {
            List<ExpMestMetyReqADO> data = new List<ExpMestMetyReqADO>();
            if (lstExpMest == null)
                return data;
            try
            {
                if (rdo._WorkPlaceSDO.IsOrderByType == 4 && lstExpMest != null && lstExpMest.Count > 0)
                {
                    var medicinegroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();
                    foreach (var current in lstExpMest)
                    {
                        ExpMestMetyReqADO ado = new ExpMestMetyReqADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMetyReqADO>(ado, current);
                        var mediType = rdo._MedicineTypes.FirstOrDefault(o => o.ID == ado.TDL_MEDICINE_TYPE_ID);
                        if (rdo.MedicineLine != null && rdo.MedicineLine.Count > 0)
                        {
                            var mediLine = rdo.MedicineLine.FirstOrDefault(o => o.ID == mediType.MEDICINE_LINE_ID);
                            if (mediLine != null)
                            {
                                ado.NUM_ORDER_MEDICINE_LINE = mediLine.NUM_ORDER ?? 0;
                                ado.MEDICINE_LINE_NAME = mediLine.MEDICINE_LINE_NAME;
                            }
                        }
                        if (rdo._MedicineUseForms != null && rdo._MedicineUseForms.Count > 0)
                        {
                            var mediUf = rdo._MedicineUseForms.FirstOrDefault(o => o.ID == mediType.MEDICINE_USE_FORM_ID);
                            if (mediUf != null)
                            {
                                ado.NUM_ORDER_MEDICINE_USE_FORM = mediUf.NUM_ORDER ?? 0;
                                ado.MEDICINE_USE_FORM_NAME = mediUf.MEDICINE_USE_FORM_NAME;
                            }
                        }
                        if (medicinegroups != null && medicinegroups.Count > 0)
                        {
                            var mediG = medicinegroups.FirstOrDefault(o => o.ID == mediType.MEDICINE_GROUP_ID);
                            if (mediG != null)
                            {
                                ado.NUM_ORDER_MEDICINE_GROUP = mediG.NUM_ORDER ?? 0;
                                ado.MEDICINE_GROUP_NAME = mediG.MEDICINE_GROUP_NAME;
                            }
                        }

                        if (rdo.DosageForm != null && rdo.DosageForm.Count > 0)
                        {
                            var dosaForm = rdo.DosageForm.FirstOrDefault(o => o.DOSAGE_FORM_NAME == mediType.DOSAGE_FORM);
                            if (dosaForm != null)
                            {
                                ado.NUM_ORDER_DOSAGE_FORM = dosaForm.NUM_ORDER ?? 0;
                                ado.DOSAGE_FORM_NAME = dosaForm.DOSAGE_FORM_NAME;
                            }
                        }
                        data.Add(ado);
                    }
                    data = data.OrderByDescending(o => o.NUM_ORDER_MEDICINE_LINE ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_USE_FORM ?? -1).ThenByDescending(o => o.NUM_ORDER_MEDICINE_GROUP ?? -1).ThenByDescending(o => o.NUM_ORDER_DOSAGE_FORM ?? -1).ThenBy(o => o.NUM_ORDER).ToList();
                }
                else
                    data = lstExpMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return data;
        }

        private bool ProcessDataXtraReport()
        {
            bool success = false;
            try
            {
                Inventec.Common.XtraReportExport.ProcessSingleTag singleTag = new Inventec.Common.XtraReportExport.ProcessSingleTag();
                Inventec.Common.XtraReportExport.ProcessObjectTag objectTag = new Inventec.Common.XtraReportExport.ProcessObjectTag();

                mps000062ADOExt.Mps000062ADOs = (from m in this._Mps000062ADOs select new Mps000062ExtADO(m)).ToList();

                int itemIndex = 0;

                var users = (mps000062ADOExt.Mps000062ADOs != null && mps000062ADOExt.Mps000062ADOs.Count > 0) ? BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>() : null;

                //Inventec.Common.Logging.LogSystem.Warn("dữ liệu mobaImpMests-1: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mobaImpMests), mobaImpMests));

                foreach (var item in mps000062ADOExt.Mps000062ADOs)
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

                    var medicine_Merges = this._ExpMestMetyReqADOCommons_Merge != null && _ExpMestMetyReqADOCommons_Merge.Count > 0 ? _ExpMestMetyReqADOCommons_Merge.Where(o => o.TRACKING_ID == item.ID).ToList() : null;

                    if (medicine_Merges == null)
                    {
                        medicine_Merges = new List<ExpMestMetyReqADO>();
                    }

                    //ServiceReqMety
                    var listServiceReqMetyADOs = this._ServiceReqMetyADOs != null && this._ServiceReqMetyADOs.Count > 0 ? this._ServiceReqMetyADOs.Where(o => o.TRACKING_ID == item.ID && o.USE_TIME == null).ToList() : null;

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
                                INTRUCTION_TIME_STR = item1.INTRUCTION_TIME_STR,
                                NUMBER_H_N = rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1 ? item1.NUMBER_BY_GROUP : null,
                                NUMBER_BY_TYPE = rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1 ? item1.NUMBER_BY_TYPE_IN_OUT : null,
                                TDL_MEDICINE_TYPE_ID = item1.MEDICINE_TYPE_ID,
                                HTU_TEXT = item1.HTU_TEXT,
                                MEDICINE_GROUP_ID = item1.MEDICINE_GROUP_ID,
                                ACTIVE_INGR_BHYT_CODE = item1.ACTIVE_INGR_BHYT_CODE,
                                ACTIVE_INGR_BHYT_NAME = item1.ACTIVE_INGR_BHYT_NAME
                            });

                            medicine_Merges.Insert(medicine_Merges.Count, new ExpMestMetyReqADO()
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
                                INTRUCTION_TIME_STR = item1.INTRUCTION_TIME_STR,
                                NUMBER_H_N = rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1 ? item1.NUMBER_BY_GROUP : null,
                                NUMBER_BY_TYPE = rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1 ? item1.NUMBER_BY_TYPE_IN_OUT : null,
                                TDL_MEDICINE_TYPE_ID = item1.MEDICINE_TYPE_ID,
                                HTU_TEXT = item1.HTU_TEXT
                            });
                        }
                        medicines = ProcessSortListExpMestMetyReq(medicines);
                        medicine_Merges = ProcessSortListExpMestMetyReq(medicine_Merges);
                    }

                    if (medicine_Merges != null && medicine_Merges.Count > 0)
                    {
                        item.MEDICINES_MERGE___DATA = "";
                        item.MEDICINES_MERGE_HTU___DATA = "";
                        int dem = 0;
                        long IntructionTime = 0;

                        foreach (var medi in medicine_Merges)
                        {
                            string s1 = "";

                            if (!String.IsNullOrEmpty(medi.MEDICINE_TYPE_CODE) && rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1)
                            {
                                medi.NUMBER_H_N = medi.NUMBER_BY_GROUP;
                                medi.NUMBER_BY_TYPE = medi.NUMBER_BY_TYPE_IN_OUT;
                            }

                            s1 += GetUsedDayCounting(medi);
                            if ((medi.REMEDY_COUNT ?? 0) == 0)
                            {
                                s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                            }
                            else
                            {
                                s1 += " " + medi.MEDICINE_TYPE_NAME;
                            }

                            if (medi.IS_MIXED_MAIN != 1 && medi.MIXED_INFUSION > 0)
                            {
                                var MediMixMain = medicine_Merges.FirstOrDefault(o => o.EXP_MEST_ID == medi.EXP_MEST_ID && o.MIXED_INFUSION == medi.MIXED_INFUSION && o.IS_MIXED_MAIN == 1);
                                if (MediMixMain != null)
                                {
                                    s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + "(" + MediMixMain.MEDICINE_TYPE_NAME + ")";
                                }
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

                            if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//GayNghien..2.4 //HuongThan..2.5
                            {
                                strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "") + " (" + Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0)) + ")";
                            }
                            else //if (medi.TYPE_ID == 5)
                            {
                                strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                            }
                            //else
                            //    strAmount = Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0);

                            string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                            if (IntructionTime != medi.INTRUCTION_TIME)
                            {
                                IntructionTime = medi.INTRUCTION_TIME;
                                item.MEDICINES_MERGE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME), FontStyle.Bold);
                                item.MEDICINES_MERGE_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME), FontStyle.Bold);
                            }
                            else
                            {
                                item.MEDICINES_MERGE___DATA += "";
                                item.MEDICINES_MERGE_HTU___DATA += "";
                            }

                            item.MEDICINES_MERGE___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0} {1}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{2}</td></tr></table>", s1, medi.CONCENTRA, s2);
                            item.MEDICINES_MERGE_HTU___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0} {1}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{2}</td></tr></table>", s1, medi.CONCENTRA, s2);

                            if ((medi.REMEDY_COUNT ?? 0) <= 0)
                            {
                                item.MEDICINES_MERGE___DATA += medi.TUTORIAL;
                                item.MEDICINES_MERGE_HTU___DATA += medi.TUTORIAL;
                                if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                {
                                    item.MEDICINES_MERGE_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_MERGE_HTU___DATA += medi.HTU_TEXT;
                                }
                                if (dem < medicines.Count - 1)
                                {
                                    item.MEDICINES_MERGE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_MERGE_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }

                            dem++;
                        }
                    }

                    //Danh sách để tổng hợp chi tiết của MEDICINES___DATA, MEDICINES_DuTru___DATA,  MEDICINES_THDT___DATA, sắp xếp theo key HIS.Desktop.Plugins.TrackingPrint.OderOption
                    List<ExpMestMetyReqADO> lstExpMestMety = new List<ExpMestMetyReqADO>();

                    //Danh sách gồm dịch vụ của các key [SERVICE_CLS_X01___DATA], [SERVICE_CLS_DuTru_X01___DATA], [SERVICE_CLS_THDT_X01___DATA], [TT_SERVICE___DATA], [TT_SERVICE_DuTru___DATA], [TT_SERVICE_THDT___DATA]
                    List<ServiceCLS> lstServiceCls = new List<ServiceCLS>();
                    if (medicines != null && medicines.Count > 0)
                    {
                        item.MEDICINES___DATA = "";
                        item.MEDICINES___DATA1 = "";
                        item.MEDICINES___DATA2 = "";
                        item.MEDICINES___DATA3 = "";
                        item.MEDICINES_TAY___DATA = "";
                        item.MEDICINES_NO_CONCENTRA__DATA = "";
                        item.MEDICINES_HTU___DATA = "";
                        int dem = 0;

                        Inventec.Common.Logging.LogSystem.Warn("dữ liệu medicines: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicines), medicines));

                        Inventec.Common.Logging.LogSystem.Warn("_UsedDayCountingOption: " + rdo._WorkPlaceSDO.UsedDayCountingOption);

                        foreach (var medi in medicines)
                        {
                            string s1 = "", sTay = "", ActiveIngrBhytName = "", S1_NoConcentra = "";

                            if (!String.IsNullOrEmpty(medi.MEDICINE_TYPE_CODE) && rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1)
                            {
                                medi.NUMBER_H_N = medi.NUMBER_BY_GROUP;
                                medi.NUMBER_BY_TYPE = medi.NUMBER_BY_TYPE_IN_OUT;
                            }

                            s1 += GetUsedDayCounting(medi);
                            if ((medi.REMEDY_COUNT ?? 0) == 0)
                            {
                                s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                sTay = s1;
                            }
                            else
                            {
                                s1 += " " + medi.MEDICINE_TYPE_NAME;
                            }

                            S1_NoConcentra = s1;
                            s1 += " ";

                            if (medi.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT)
                            {
                                s1 += medi.CONCENTRA;
                                if ((medi.REMEDY_COUNT ?? 0) == 0)
                                {
                                    sTay += " " + medi.CONCENTRA;
                                }
                            }

                            if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && medi.NUMBER_BY_TYPE == 1 && !String.IsNullOrEmpty(medi.ACTIVE_INGR_BHYT_NAME))
                            {
                                ActiveIngrBhytName += String.Format("&lt;{0}&gt;", medi.ACTIVE_INGR_BHYT_NAME);
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

                            if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//GayNghien..2.4 //HuongThan..2.5
                            {
                                strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "") + " (" + Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0)) + ")";
                            }
                            else //if (medi.TYPE_ID == 5)
                            {
                                strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                            }
                            //else
                            //    strAmount = Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0);

                            string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                            if (medi.INTRUCTION_TIME_STR > 0)
                            {
                                item.MEDICINES___DATA3 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);
                                item.MEDICINES_NO_CONCENTRA__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);

                                if ((medi.REMEDY_COUNT ?? 0) == 0)
                                {
                                    item.MEDICINES_TAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);
                                }
                            }
                            else
                            {
                                item.MEDICINES___DATA3 += "";
                                item.MEDICINES_TAY___DATA += "";
                                item.MEDICINES_NO_CONCENTRA__DATA += "";
                            }

                            item.MEDICINES___DATA3 += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0} {1}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{2}</td></tr></table>", s1, ActiveIngrBhytName, s2);
                            item.MEDICINES_NO_CONCENTRA__DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0} {1}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{2}</td></tr></table>", S1_NoConcentra, ActiveIngrBhytName, s2);

                            if ((medi.REMEDY_COUNT ?? 0) == 0)
                            {
                                item.MEDICINES_TAY___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", sTay, s2);
                            }

                            if ((medi.REMEDY_COUNT ?? 0) <= 0)
                            {
                                item.MEDICINES___DATA3 += medi.TUTORIAL;
                                item.MEDICINES_TAY___DATA += medi.TUTORIAL;
                                item.MEDICINES_NO_CONCENTRA__DATA += medi.TUTORIAL;

                                if (dem < medicines.Count - 1)
                                {
                                    item.MEDICINES___DATA3 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_TAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_NO_CONCENTRA__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }

                            if (!String.IsNullOrEmpty(medi.MEDICINE_TYPE_CODE) && rdo._WorkPlaceSDO.UsedDayCountingOutStockOption == 1)
                            {
                                medi.NUMBER_H_N = medi.NUMBER_BY_GROUP;
                                medi.NUMBER_BY_TYPE = medi.NUMBER_BY_TYPE_IN_OUT;
                            }

                            if (medi.INTRUCTION_TIME_STR > 0)
                            {
                                item.MEDICINES___DATA1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);
                            }
                            else
                            {
                                item.MEDICINES___DATA1 += "";
                            }

                            string s3 = "", ActiveIngrBhytName3 = "";

                            s3 += GetUsedDayCounting(medi);
                            string strAmount1 = ((medi.AMOUNT >= 1 && medi.AMOUNT < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(medi.AMOUNT) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(medi.AMOUNT) + "");

                            if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && medi.NUMBER_BY_TYPE == 1 && !String.IsNullOrEmpty(medi.ACTIVE_INGR_BHYT_NAME))
                            {
                                ActiveIngrBhytName3 += " &lt;" + medi.ACTIVE_INGR_BHYT_NAME + "&gt;";
                            }

                            //item.MEDICINES___DATA1 += String.Format("{0}{1} {2} {3}", s1, item1.MEDICINE_TYPE_NAME, item1.CONCENTRA, ((item1.REMEDY_COUNT ?? 0) > 0 ? item1.Amount_By_Remedy_Count + " " + item1.SERVICE_UNIT_NAME : (item1.AMOUNT > 0 ? "   x" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) + " " + item1.SERVICE_UNIT_NAME + "   " + item1.MEDICINE_USE_FORM_NAME : "")));

                            item.MEDICINES___DATA1 += String.Format("{0}{1}{2} {3} {4}", s3, medi.MEDICINE_TYPE_NAME, medi.CONCENTRA, ActiveIngrBhytName3, ((medi.REMEDY_COUNT ?? 0) > 0 ? medi.Amount_By_Remedy_Count + " " + medi.SERVICE_UNIT_NAME : (medi.AMOUNT > 0 ? "   x" + strAmount1 + " " + medi.SERVICE_UNIT_NAME + "   " + medi.MEDICINE_USE_FORM_NAME : "")));

                            if (medi.PRESCRIPTION_TYPE_ID != 2)
                            {
                                item.MEDICINES___DATA1 += medi.TUTORIAL;
                            }
                            if (dem < medicines.Count - 1)
                            {
                                item.MEDICINES___DATA1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }

                            if (medi.INTRUCTION_TIME_STR > 0)
                            {
                                item.MEDICINES___DATA2 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);
                            }
                            else
                            {
                                item.MEDICINES___DATA2 += "";
                            }

                            item.MEDICINES___DATA2 += String.Format("{0}--{1}{2} {3}", s3, medi.MEDICINE_TYPE_NAME, ActiveIngrBhytName3, ((medi.REMEDY_COUNT ?? 0) > 0 ? medi.Amount_By_Remedy_Count + " (" + medi.REMEDY_COUNT + " thang) " + medi.SERVICE_UNIT_NAME : (medi.AMOUNT > 0 ? " x" + strAmount1 + " " + medi.SERVICE_UNIT_NAME + "   " + medi.MEDICINE_USE_FORM_NAME : "")));

                            item.MEDICINES___DATA2 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            item.MEDICINES___DATA2 += "    " + medi.TUTORIAL;
                            if (dem < medicines.Count - 1)
                            {
                                item.MEDICINES___DATA2 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }

                            if ((medi.REMEDY_COUNT ?? 0) == 0)
                            {
                                s3 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                            }
                            else
                            {
                                s3 += " " + medi.MEDICINE_TYPE_NAME;
                            }
                            s3 += " ";

                            if (medi.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT)
                            {
                                s3 += medi.CONCENTRA;
                            }

                            if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && medi.NUMBER_BY_TYPE == 1 && !String.IsNullOrEmpty(medi.ACTIVE_INGR_BHYT_NAME))
                            {
                                s3 += " &lt;" + medi.ACTIVE_INGR_BHYT_NAME + "&gt;";
                            }
                            if (medi.INTRUCTION_TIME_STR > 0)
                            {
                                item.MEDICINES___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);
                                item.MEDICINES_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày sử dụng: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(medi.INTRUCTION_TIME_STR), FontStyle.Bold);
                            }
                            else
                            {
                                item.MEDICINES___DATA += "";
                                item.MEDICINES_HTU___DATA += "";
                            }

                            string fmrData = String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s3, s2);
                            item.MEDICINES___DATA += fmrData;
                            item.MEDICINES_HTU___DATA += fmrData;
                            s3 = string.Concat("- ") + s3;
                            string dataRepx = fmrData;
                            string dataHtuRepx = fmrData;
                            if ((medi.REMEDY_COUNT ?? 0) <= 0)
                            {
                                dataRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.TUTORIAL, FontStyle.Italic);
                                dataRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                dataHtuRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.HTU_TEXT, FontStyle.Italic);
                                dataHtuRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                item.MEDICINES___DATA += medi.TUTORIAL;
                                item.MEDICINES_HTU___DATA += medi.TUTORIAL;
                                if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                {
                                    item.MEDICINES_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_HTU___DATA += medi.HTU_TEXT;
                                }
                                if (dem < medicines.Count - 1)
                                {
                                    item.MEDICINES___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }
                            item.MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID;
                            dem++;

                            lstExpMestMety.Add(new ExpMestMetyReqADO() { INTRUCTION_DATE = medi.INTRUCTION_DATE, INTRUCTION_TIME = medi.INTRUCTION_TIME, INTRUCTION_TIME_STR = medi.INTRUCTION_TIME_STR, MEDICINE_GROUP_NUM_ORDER = medi.MEDICINE_GROUP_NUM_ORDER, NUM_ORDER_BY_USE_FORM = medi.MEDICINE_GROUP_NUM_ORDER ?? -1, TDL_SERVICE_REQ_ID = medi.TDL_SERVICE_REQ_ID, NUM_ORDER = medi.NUM_ORDER, NUMBER_H_N = medi.NUMBER_H_N, TDL_MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID, REMEDY_COUNT = medi.REMEDY_COUNT, USE_TIME = medi.USE_TIME, USE_TIME_TO = medi.USE_TIME_TO, ADVISE = medi.ADVISE, HTU_TEXT = medi.HTU_TEXT, ASSIGN_TIME_TO = medi.ASSIGN_TIME_TO, DAY_COUNT = medi.DAY_COUNT, DATA_REPX = dataRepx, DATA_DAY_REPX = dataRepx, DATA_DAY_HTU_REPX = dataHtuRepx });
                        }
                    }

                    //Thuốc Pha truyền
                    var medicineInfusions = _ExpMestMetyReqADOCommonsMix != null && _ExpMestMetyReqADOCommonsMix.Count > 0 ? _ExpMestMetyReqADOCommonsMix.Where(o => o.TRACKING_ID == item.ID).ToList() : null;

                    if (medicineInfusions == null)
                    {
                        medicineInfusions = new List<ExpMestMetyReqADO>();
                    }

                    if (medicineInfusions != null && medicineInfusions.Count > 0)
                    {
                        medicineInfusions = ProcessSortListExpMestMetyReq(medicineInfusions);
                        var medicineInfusionsGroup = medicineInfusions.Where(p => p.TRACKING_ID == item.ID).GroupBy(o => new { o.EXP_MEST_ID, o.MIXED_INFUSION });

                        item.MEDICINES_INFUSION___DATA = "";
                        item.MEDICINES_INFUSION_DATA_WITH_BOLD_NAME = "";

                        foreach (var InfusionsGroup in medicineInfusionsGroup)
                        {
                            string s1 = "", s2 = "";
                            InfusionsGroup.OrderBy(o => o.IS_MIXED_MAIN ?? 99999).ToList();
                            foreach (var MediIns in InfusionsGroup)
                            {
                                string strAmount = "", s3 = "", s4 = "";

                                strAmount = ((MediIns.AMOUNT >= 1 && MediIns.AMOUNT < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(MediIns.AMOUNT) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(MediIns.AMOUNT) + "");
                                s1 = GetUsedDayCounting(MediIns);
                                s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + MediIns.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                s1 += " " + MediIns.CONCENTRA;
                                if (MediIns.IS_MIXED_MAIN != 1)
                                {
                                    var MixedMain = InfusionsGroup.FirstOrDefault(o => o.IS_MIXED_MAIN == 1);
                                    s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + (MixedMain != null ? " (Thuốc đi kèm thuốc " + MixedMain.MEDICINE_TYPE_NAME + " " + MixedMain.CONCENTRA + ")" : "");
                                }
                                s2 = strAmount + " " + MediIns.SERVICE_UNIT_NAME + "/ngày";

                                item.MEDICINES_INFUSION___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                item.MEDICINES_INFUSION_HTU___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                item.MEDICINES_INFUSION___DATA += MediIns.TUTORIAL;
                                item.MEDICINES_INFUSION_HTU___DATA += MediIns.TUTORIAL;
                                if (!string.IsNullOrEmpty(MediIns.HTU_TEXT))
                                {
                                    item.MEDICINES_INFUSION_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_INFUSION_HTU___DATA += MediIns.HTU_TEXT;
                                }
                                s3 += GetUsedDayCounting(MediIns);
                                s3 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + MediIns.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                if (MediIns.MEDICINE_LINE_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__CP_YHCT)
                                {
                                    s3 += " " + MediIns.CONCENTRA;
                                }

                                s4 = strAmount + " " + MediIns.SERVICE_UNIT_NAME;

                                item.MEDICINES_INFUSION_DATA_WITH_BOLD_NAME += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s3, s4);

                                item.MEDICINE_TYPE_ID = MediIns.TDL_MEDICINE_TYPE_ID;
                            }

                            var MixedMain1 = InfusionsGroup.FirstOrDefault(o => o.IS_MIXED_MAIN == 1) ?? null;
                            item.MEDICINES_INFUSION_DATA_WITH_BOLD_NAME += MixedMain1 != null ? MixedMain1.TUTORIAL_INFUSION : "";
                        }
                    }

                    #region Thuốc dự trù

                    item.MEDICINES_DuTru___DATA = "";
                    item.MEDI_DUTRU_NO_CONCENTRA = "";
                    item.MEDICINES_INFUSION_DuTru___DATA = "";
                    item.MEDICINES_OutStock_DuTru__DATA = "";
                    item.MEDICINES_MERGE_DUTRU___DATA = "";

                    item.SERVICE_CLS_DuTru___DATA = "";
                    item.TT_SERVICE_DuTru___DATA = "";

                    foreach (var ReqDT in _ServiceReqDuTrus)
                    {
                        if (ReqDT.TRACKING_ID == item.ID && ReqDT.IS_NOT_SHOW_MATERIAL_TRACKING == 1)
                        {
                            #region thuốc dự trù gộp
                            var medicineDuTru_Merges = _ExpMestMetyReqADOCommonsDuTru_Merge != null && _ExpMestMetyReqADOCommonsDuTru_Merge.Count > 0 ? _ExpMestMetyReqADOCommonsDuTru_Merge.Where(o => o.TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqDT.ID).ToList() : null;

                            if (medicineDuTru_Merges == null)
                            {
                                medicineDuTru_Merges = new List<ExpMestMetyReqADO>();
                            }

                            if (medicineDuTru_Merges != null && medicineDuTru_Merges.Count > 0)
                            {
                                int dem = 0;
                                medicineDuTru_Merges = ProcessSortListExpMestMetyReq(medicineDuTru_Merges);
                                foreach (var medi in medicineDuTru_Merges)
                                {
                                    string s1 = "";

                                    s1 += GetUsedDayCounting(medi);

                                    if ((medi.REMEDY_COUNT ?? 0) == 0)
                                    {
                                        s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                    }
                                    else
                                    {
                                        s1 += " " + medi.MEDICINE_TYPE_NAME;
                                    }
                                    s1 += " " + medi.CONCENTRA;
                                    if (medi.IS_MIXED_MAIN != 1 && medi.MIXED_INFUSION > 0)
                                    {
                                        var MediMixMain = medicineDuTru_Merges.FirstOrDefault(o => o.EXP_MEST_ID == medi.EXP_MEST_ID && o.MIXED_INFUSION == medi.MIXED_INFUSION && o.IS_MIXED_MAIN == 1);
                                        if (MediMixMain != null)
                                        {
                                            s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + "(" + MediMixMain.MEDICINE_TYPE_NAME + " " + MediMixMain.CONCENTRA + ")";
                                        }
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

                                    if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//GayNghien..2.4 //HuongThan..2.5
                                    {
                                        strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "") + " (" + Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0)) + ")";
                                    }
                                    else
                                    {
                                        strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                                    }

                                    string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                                    if (dem == 0 && ReqDT.USE_TIME != null)
                                    {
                                        item.MEDICINES_MERGE_DUTRU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDICINES_MERGE_DUTRU_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                    }
                                    else
                                    {
                                        item.MEDICINES_MERGE_DUTRU___DATA += "";
                                        item.MEDICINES_MERGE_DUTRU_HTU___DATA += "";
                                    }

                                    item.MEDICINES_MERGE_DUTRU___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                    item.MEDICINES_MERGE_DUTRU_HTU___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                    if ((medi.REMEDY_COUNT ?? 0) <= 0)
                                    {
                                        item.MEDICINES_MERGE_DUTRU___DATA += medi.TUTORIAL;
                                        item.MEDICINES_MERGE_DUTRU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                        item.MEDICINES_MERGE_DUTRU_HTU___DATA += medi.TUTORIAL;
                                        if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                        {
                                            item.MEDICINES_MERGE_DUTRU_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            item.MEDICINES_MERGE_DUTRU_HTU___DATA += medi.HTU_TEXT;
                                        }
                                    }

                                    item.MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID;
                                    dem++;
                                }
                            }
                            #endregion

                            #region thuốc trong kho dự trù
                            var medicineDuTrus = _ExpMestMetyReqADOCommonsDuTru != null && _ExpMestMetyReqADOCommonsDuTru.Count > 0 ? _ExpMestMetyReqADOCommonsDuTru.Where(o => o.TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqDT.ID).ToList() : null;

                            if (medicineDuTrus == null)
                            {
                                medicineDuTrus = new List<ExpMestMetyReqADO>();
                            }

                            if (medicineDuTrus != null && medicineDuTrus.Count > 0)
                            {
                                int dem = 0;

                                medicineDuTrus = ProcessSortListExpMestMetyReq(medicineDuTrus);
                                foreach (var medi in medicineDuTrus)
                                {
                                    string s1 = "", S1_NoConcentra = "";

                                    s1 += GetUsedDayCounting(medi);

                                    if ((medi.REMEDY_COUNT ?? 0) == 0)
                                    {
                                        s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                    }
                                    else
                                    {
                                        s1 += " " + medi.MEDICINE_TYPE_NAME;
                                    }
                                    S1_NoConcentra = s1;
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

                                    if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//GayNghien..2.4 //HuongThan..2.5
                                    {
                                        strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "") + " (" + Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0)) + ")";
                                    }
                                    else
                                    {
                                        strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                                    }

                                    string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                                    if (dem == 0 && ReqDT.USE_TIME != null)
                                    {
                                        item.MEDICINES_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDI_DUTRU_NO_CONCENTRA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDICINES_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                    }
                                    else
                                    {
                                        item.MEDICINES_DuTru___DATA += "";
                                        item.MEDI_DUTRU_NO_CONCENTRA += "";
                                        item.MEDICINES_DuTru_HTU___DATA += "";
                                    }
                                    string frmData = String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                    item.MEDICINES_DuTru___DATA += frmData;
                                    item.MEDICINES_DuTru_HTU___DATA += frmData;
                                    s1 = string.Concat("- ") + s1;
                                    string dataRepx = frmData;
                                    string dataHtuRepx = frmData;
                                    item.MEDI_DUTRU_NO_CONCENTRA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", S1_NoConcentra, s2);
                                    if ((medi.REMEDY_COUNT ?? 0) <= 0)
                                    {
                                        dataRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.TUTORIAL, FontStyle.Italic);
                                        dataRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        dataHtuRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.HTU_TEXT, FontStyle.Italic);
                                        dataHtuRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_DuTru___DATA += medi.TUTORIAL;
                                        item.MEDICINES_DuTru_HTU___DATA += medi.TUTORIAL;
                                        item.MEDI_DUTRU_NO_CONCENTRA += medi.TUTORIAL;
                                        //if (dem < medicineDuTrus.Count - 1)
                                        //{
                                        item.MEDICINES_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDI_DUTRU_NO_CONCENTRA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        //}
                                        if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                        {
                                            item.MEDICINES_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            item.MEDICINES_DuTru_HTU___DATA += medi.HTU_TEXT;
                                        }
                                    }

                                    item.MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID;
                                    dem++;
                                    lstExpMestMety.Add(new ExpMestMetyReqADO() { INTRUCTION_DATE = medi.INTRUCTION_DATE, INTRUCTION_TIME = medi.INTRUCTION_TIME, INTRUCTION_TIME_STR = medi.INTRUCTION_TIME_STR, MEDICINE_GROUP_NUM_ORDER = medi.MEDICINE_GROUP_NUM_ORDER, NUM_ORDER_BY_USE_FORM = medi.MEDICINE_GROUP_NUM_ORDER ?? -1, TDL_SERVICE_REQ_ID = medi.TDL_SERVICE_REQ_ID, NUM_ORDER = medi.NUM_ORDER, NUMBER_H_N = medi.NUMBER_H_N, TDL_MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID, REMEDY_COUNT = medi.REMEDY_COUNT, ASSIGN_TIME_TO = medi.ASSIGN_TIME_TO, USE_TIME = ReqDT.USE_TIME, USE_TIME_TO = medi.USE_TIME_TO, ADVISE = medi.ADVISE, HTU_TEXT = medi.HTU_TEXT, DAY_COUNT = medi.DAY_COUNT, DATA_REPX = dataRepx, DATA_DAY_REPX = dataRepx, DATA_DAY_HTU_REPX = dataHtuRepx });
                                }
                            }
                            #endregion

                            #region Thuốc Pha truyền dự trù
                            var medicineInfusion_DuTrus = _MediInfusionDutru != null && _MediInfusionDutru.Count > 0 ? _MediInfusionDutru.Where(o => o.TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqDT.ID).ToList() : null;

                            if (medicineInfusion_DuTrus == null)
                            {
                                medicineInfusion_DuTrus = new List<ExpMestMetyReqADO>();
                            }

                            if (medicineInfusion_DuTrus != null && medicineInfusion_DuTrus.Count > 0)
                            {
                                var medicineInfusion_DuTruGroup = medicineInfusion_DuTrus.GroupBy(o => new { o.EXP_MEST_ID, o.MIXED_INFUSION });

                                int checkdem = 0;
                                foreach (var InfusionsGroup in medicineInfusion_DuTruGroup)
                                {
                                    string s1 = "", s2 = "";
                                    InfusionsGroup.OrderBy(o => o.IS_MIXED_MAIN ?? 99999).ToList();

                                    var InfusionsList = ProcessSortListExpMestMetyReq(InfusionsGroup.ToList());
                                    foreach (var MediIns in InfusionsList)
                                    {
                                        if (checkdem == 0 && ReqDT.USE_TIME != null)
                                        {
                                            item.MEDICINES_INFUSION_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc pha truyền dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                            item.MEDICINES_INFUSION_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc pha truyền dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                        }

                                        string strAmount = "";

                                        strAmount = ((MediIns.AMOUNT >= 1 && MediIns.AMOUNT < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(MediIns.AMOUNT) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(MediIns.AMOUNT) + "");
                                        s1 = GetUsedDayCounting(MediIns);
                                        s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + MediIns.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                        s1 += " " + MediIns.CONCENTRA;
                                        if (MediIns.IS_MIXED_MAIN != 1)
                                        {
                                            var MixedMain = InfusionsGroup.FirstOrDefault(o => o.IS_MIXED_MAIN == 1);
                                            s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + (MixedMain != null ? " (Thuốc đi kèm thuốc " + MixedMain.MEDICINE_TYPE_NAME + " " + MixedMain.CONCENTRA + ")" : "");
                                        }
                                        s2 = strAmount + " " + MediIns.SERVICE_UNIT_NAME + "/ngày";

                                        item.MEDICINES_INFUSION_DuTru___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                        item.MEDICINES_INFUSION_DuTru___DATA += MediIns.TUTORIAL;

                                        item.MEDICINES_INFUSION_DuTru_HTU___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                        item.MEDICINES_INFUSION_DuTru_HTU___DATA += MediIns.TUTORIAL;
                                        if (!string.IsNullOrEmpty(MediIns.HTU_TEXT))
                                        {
                                            item.MEDICINES_INFUSION_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            item.MEDICINES_INFUSION_DuTru_HTU___DATA += MediIns.HTU_TEXT;
                                        }
                                        item.MEDICINE_TYPE_ID = MediIns.TDL_MEDICINE_TYPE_ID;
                                        checkdem++;
                                    }
                                    item.MEDICINES_INFUSION_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.MEDICINES_INFUSION_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }
                            #endregion

                            #region thuốc mua ngoài dụ trù
                            var OutStockDuTrus = _ServiceReqMetyADOs != null && _ServiceReqMetyADOs.Count > 0 ? _ServiceReqMetyADOs.Where(o => o.TRACKING_ID == item.ID && o.SERVICE_REQ_ID == ReqDT.ID).ToList() : null;

                            if (OutStockDuTrus == null)
                            {
                                OutStockDuTrus = new List<ServiceReqMetyADO>();
                            }

                            if (OutStockDuTrus != null && OutStockDuTrus.Count > 0)
                            {
                                int dem = 0;

                                OutStockDuTrus = ProcessSortListServiceReqMety(OutStockDuTrus);
                                foreach (var medi in OutStockDuTrus)
                                {
                                    string s1 = "";

                                    s1 += medi.MEDICINE_TYPE_NAME + " ";
                                    decimal? amount = 0;
                                    string strAmount = "";
                                    if (medi.AMOUNT > 0)
                                    {
                                        amount = medi.AMOUNT;
                                    }

                                    strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");

                                    string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                                    if (dem == 0 && ReqDT.USE_TIME != null)
                                    {
                                        item.MEDICINES_OutStock_DuTru__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDICINES_OutStock_DuTru_HTU__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDT.USE_TIME ?? 0), FontStyle.Bold);
                                    }
                                    else
                                    {
                                        item.MEDICINES_OutStock_DuTru__DATA += "";
                                        item.MEDICINES_OutStock_DuTru_HTU__DATA += "";
                                    }

                                    item.MEDICINES_OutStock_DuTru__DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                    item.MEDICINES_OutStock_DuTru__DATA += medi.TUTORIAL;
                                    item.MEDICINES_OutStock_DuTru__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);


                                    item.MEDICINES_OutStock_DuTru_HTU__DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                    item.MEDICINES_OutStock_DuTru_HTU__DATA += medi.TUTORIAL;
                                    item.MEDICINES_OutStock_DuTru_HTU__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                    {
                                        item.MEDICINES_OutStock_DuTru_HTU__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_OutStock_DuTru_HTU__DATA += medi.HTU_TEXT;
                                    }
                                    item.MEDICINE_TYPE_ID = medi.MEDICINE_TYPE_ID;
                                    dem++;
                                }
                            }
                            #endregion
                        }

                    }
                    #endregion

                    #region dịch vụ dự trù

                    if (_ServiceReqDuTrus != null && _ServiceReqDuTrus.Count > 0)
                    {
                        #region dvu cls dự trù
                        List<ServiceCLS> ServiceAll = new List<ServiceCLS>();
                        if (_ServiceClsDuTru != null && _ServiceClsDuTru.Count > 0 && _ServiceClsDuTru.Exists(o => o.TRACKING_ID == item.ID))
                        {
                            //_ServiceClsDuTru.ForEach(o => o.USE_TIME = _ServiceReqDuTrus.FirstOrDefault(ReqDT => o.SERVICE_REQ_ID == ReqDT.ID).USE_TIME);
                            //ServiceAll.AddRange(ClsDuTrus);
                            var _ServiceClsDuTruGroupUseTime = _ServiceClsDuTru.Where(o => o.TRACKING_ID == item.ID).OrderBy(o => o.USE_TIME).GroupBy(o => o.USE_TIME).ToList();
                            {
                                foreach (var lstService in _ServiceClsDuTruGroupUseTime)
                                {
                                    string title = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstService.Key ?? 0), FontStyle.Bold);
                                    item.SERVICE_CLS_DuTru___DATA += title;
                                    item.SERVICE_CLS_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_DuTru_X01___DATA += title;
                                    item.SERVICE_CLS_DuTru_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    var groupByServiceType = lstService.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                                    foreach (var serviceType in groupByServiceType)
                                    {
                                        var namesvt = rdo._ServiceTypes.FirstOrDefault(p => p.ID == serviceType.Key);
                                        item.SERVICE_CLS_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(namesvt.SERVICE_TYPE_NAME + ": ", FontStyle.Bold);
                                        item.SERVICE_CLS_DuTru_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(namesvt.SERVICE_TYPE_NAME + ": ", FontStyle.Bold);
                                        List<string> serviceClss = new List<string>();
                                        List<string> serviceClss01 = new List<string>();
                                        foreach (var service in serviceType.ToList())
                                        {
                                            string s1 = (service.SERVICE_NAME ?? service.TDL_SERVICE_NAME) + " ";
                                            decimal? amount = 0;
                                            string strAmount = "";
                                            if (service.AMOUNT > 0)
                                            {
                                                amount = service.AMOUNT;
                                            }

                                            strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");

                                            serviceClss.Add(string.Format("{0}x{1}", s1, amount));
                                            serviceClss01.Add(string.Format("{0}x{1}", s1, strAmount));

                                            lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = namesvt.SERVICE_TYPE_NAME, SERVICE_NAME = s1, AMOUNT = service.AMOUNT, INSTRUCTION_NOTE = service.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = namesvt.NUM_ORDER ?? Int64.MinValue, IsGoupService = service.IsGoupService, USE_TIME = service.USE_TIME, TDL_INTRUCTION_TIME = service.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = service.TDL_SERVICE_UNIT_ID, SERVICE_ID = service.SERVICE_ID, serviceSplits = service.serviceSplits, TDL_SERVICE_TYPE_ID = service.TDL_SERVICE_TYPE_ID });

                                        }
                                        if (serviceClss != null && serviceClss.Count > 0)
                                        {
                                            item.SERVICE_CLS_DuTru___DATA += string.Join("; ", serviceClss);
                                            item.SERVICE_CLS_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                            item.SERVICE_CLS_DuTru_X01___DATA += string.Join("; ", serviceClss01);
                                            item.SERVICE_CLS_DuTru_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region dvu tt dự trù
                        //var TtDutrus = this._TTServices != null && this._TTServices.Count > 0 ? this._TTServices.Where(o => _ServiceReqDuTrus.Exists(ReqDT => o.SERVICE_REQ_ID == ReqDT.ID && (ReqDT.USE_TIME ?? 0) > ReqDT.INTRUCTION_TIME)).ToList() : new List<ServiceCLS>();
                        if (_ServiceTtDuTru != null && _ServiceTtDuTru.Count > 0 && _ServiceTtDuTru.Exists(o => o.TRACKING_ID == item.ID))
                        {
                            //ServiceAll.AddRange(TtDutrus);
                            var _ServiceTtDuTruGroupUseTime = _ServiceTtDuTru.Where(o => o.TRACKING_ID == item.ID).OrderBy(o => o.USE_TIME).GroupBy(o => o.USE_TIME).ToList();
                            {
                                foreach (var lstService in _ServiceTtDuTruGroupUseTime)
                                {
                                    string title = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstService.Key ?? 0), FontStyle.Bold);
                                    item.TT_SERVICE_DuTru___DATA += title;
                                    item.TT_SERVICE_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    var groupByServiceType = lstService.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                                    foreach (var serviceType in groupByServiceType)
                                    {
                                        var namesvt = rdo._ServiceTypes.FirstOrDefault(p => p.ID == serviceType.Key);
                                        item.TT_SERVICE_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(namesvt.SERVICE_TYPE_NAME + ": ", FontStyle.Bold);
                                        List<string> serviceTts = new List<string>();
                                        foreach (var service in serviceType.ToList())
                                        {
                                            var s1 = (service.SERVICE_NAME ?? service.TDL_SERVICE_NAME) + " ";
                                            serviceTts.Add(string.Format("{0}", s1));

                                            lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = namesvt.SERVICE_TYPE_NAME, SERVICE_NAME = s1, AMOUNT = service.AMOUNT, INSTRUCTION_NOTE = service.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = namesvt.NUM_ORDER ?? Int64.MinValue, IsGoupService = service.IsGoupService, USE_TIME = service.USE_TIME, TDL_INTRUCTION_TIME = service.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = service.TDL_SERVICE_UNIT_ID, SERVICE_ID = service.SERVICE_ID, serviceSplits = service.serviceSplits, TDL_SERVICE_TYPE_ID = service.TDL_SERVICE_TYPE_ID });

                                        }
                                        if (serviceTts != null && serviceTts.Count > 0)
                                        {
                                            item.TT_SERVICE_DuTru___DATA += string.Join("; ", serviceTts);
                                            item.TT_SERVICE_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Group Dự trù về theo ngày
                        //if (ServiceAll != null && ServiceAll.Count > 0)
                        //{
                        //    var groupServiceByUseTime = ServiceAll.GroupBy(o => o.USE_TIME).ToList();
                        //    foreach (var svs in groupServiceByUseTime)
                        //    {
                        //        int demService = 0;
                        //        foreach (var service in svs.ToList())
                        //        {
                        //            bool IsCls = ClsDuTrus.Exists(o => o.SERVICE_ID == service.SERVICE_ID);
                        //            string s1 = "";

                        //            s1 += (service.SERVICE_NAME ?? service.TDL_SERVICE_NAME) + " ";
                        //            decimal? amount = 0;
                        //            string strAmount = "";
                        //            if (service.AMOUNT > 0)
                        //            {
                        //                amount = service.AMOUNT;
                        //            }

                        //            strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");

                        //            string s2 = strAmount;


                        //            if (demService == 0)
                        //            {
                        //                string title = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(svs.Key ?? 0), FontStyle.Bold);
                        //                if (IsCls)
                        //                    item.SERVICE_CLS_DuTru___DATA += title;
                        //                else
                        //                    item.TT_SERVICE_DuTru___DATA += title;
                        //            }
                        //            var content = String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                        //            if (IsCls)
                        //            {
                        //                item.SERVICE_CLS_DuTru___DATA += content;
                        //                item.SERVICE_CLS_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                        //            }
                        //            else
                        //            {
                        //                item.TT_SERVICE_DuTru___DATA += content;
                        //                item.TT_SERVICE_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                        //            }
                        //            demService++;
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                    #endregion

                    #region Thuốc thực hiện dự trù

                    item.MEDICINES_THDT___DATA = "";
                    item.MEDI_THDT_NO_CONCENTRA = "";
                    item.MEDICINES_INFUSION_THDT___DATA = "";
                    item.MEDICINES_OutStock_THDT__DATA = "";

                    item.SERVICE_CLS_THDT___DATA = "";
                    item.TT_SERVICE_THDT___DATA = "";
                    foreach (var ReqTHDT in _ServiceReqTHDT)
                    {
                        if (ReqTHDT.USED_FOR_TRACKING_ID == item.ID && ReqTHDT.IS_NOT_SHOW_MATERIAL_TRACKING == 1)
                        {
                            #region trong kho thực hiện dự trù
                            var medicineTHDTs = _ExpMestMetyReqADOCommonsTHDT != null && _ExpMestMetyReqADOCommonsTHDT.Count > 0 ? _ExpMestMetyReqADOCommonsTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqTHDT.ID).ToList() : null;

                            if (medicineTHDTs == null)
                            {
                                medicineTHDTs = new List<ExpMestMetyReqADO>();
                            }

                            if (medicineTHDTs != null && medicineTHDTs.Count > 0)
                            {
                                int dem = 0;

                                medicineTHDTs = ProcessSortListExpMestMetyReq(medicineTHDTs);
                                foreach (var medi in medicineTHDTs)
                                {
                                    string s1 = "", S1_NoConcentra = "";

                                    s1 += GetUsedDayCounting(medi);

                                    if ((medi.REMEDY_COUNT ?? 0) == 0)
                                    {
                                        s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                    }
                                    else
                                    {
                                        s1 += " " + medi.MEDICINE_TYPE_NAME;
                                    }
                                    S1_NoConcentra = s1;
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

                                    if (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//GayNghien..2.4 //HuongThan..2.5
                                    {
                                        strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "") + " (" + Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0)) + ")";
                                    }
                                    else
                                    {
                                        strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                                    }

                                    string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                                    if (dem == 0 && ReqTHDT.USE_TIME != null)
                                    {
                                        item.MEDICINES_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDI_THDT_NO_CONCENTRA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDICINES_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                    }
                                    else
                                    {
                                        item.MEDICINES_THDT___DATA += "";
                                        item.MEDI_THDT_NO_CONCENTRA += "";
                                        item.MEDICINES_THDT_HTU___DATA += "";
                                    }
                                    string fmrData = String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                    item.MEDICINES_THDT___DATA += fmrData;
                                    item.MEDICINES_THDT_HTU___DATA += fmrData;
                                    s1 = string.Concat("- ") + s1;
                                    string dataRepx = fmrData;
                                    string dataHtuRepx = fmrData;
                                    item.MEDI_THDT_NO_CONCENTRA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", S1_NoConcentra, s2);

                                    if ((medi.REMEDY_COUNT ?? 0) <= 0)
                                    {
                                        dataRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.TUTORIAL, FontStyle.Italic);
                                        dataRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        dataHtuRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + medi.HTU_TEXT, FontStyle.Italic);
                                        dataHtuRepx += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_THDT___DATA += medi.TUTORIAL;
                                        item.MEDICINES_THDT_HTU___DATA += medi.TUTORIAL;
                                        item.MEDI_THDT_NO_CONCENTRA += medi.TUTORIAL;
                                        //if (dem < medicineTHDTs.Count - 1)
                                        //{
                                        item.MEDICINES_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDI_THDT_NO_CONCENTRA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        //}
                                        if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                        {
                                            item.MEDICINES_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            item.MEDICINES_THDT_HTU___DATA += medi.HTU_TEXT;
                                        }
                                    }

                                    item.MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID;
                                    dem++;

                                    lstExpMestMety.Add(new ExpMestMetyReqADO() { INTRUCTION_DATE = medi.INTRUCTION_DATE, INTRUCTION_TIME = medi.INTRUCTION_TIME, INTRUCTION_TIME_STR = medi.INTRUCTION_TIME_STR, MEDICINE_GROUP_NUM_ORDER = medi.MEDICINE_GROUP_NUM_ORDER, NUM_ORDER_BY_USE_FORM = medi.MEDICINE_GROUP_NUM_ORDER ?? -1, TDL_SERVICE_REQ_ID = medi.TDL_SERVICE_REQ_ID, NUM_ORDER = medi.NUM_ORDER, NUMBER_H_N = medi.NUMBER_H_N, TDL_MEDICINE_TYPE_ID = medi.TDL_MEDICINE_TYPE_ID, REMEDY_COUNT = medi.REMEDY_COUNT, ASSIGN_TIME_TO = medi.ASSIGN_TIME_TO, USE_TIME = ReqTHDT.USE_TIME, USE_TIME_TO = medi.USE_TIME_TO, ADVISE = medi.ADVISE, HTU_TEXT = medi.HTU_TEXT, DAY_COUNT = medi.DAY_COUNT, DATA_REPX = dataRepx, DATA_DAY_REPX = dataRepx, DATA_DAY_HTU_REPX = dataHtuRepx });
                                }

                            }
                            #endregion

                            #region Thuốc Pha truyền thực hiện dự trù
                            var medicineInfusion_THDTs = _MediInfusionTHDT != null && _MediInfusionTHDT.Count > 0 ? _MediInfusionTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqTHDT.ID).ToList() : null;

                            if (medicineInfusion_THDTs == null)
                            {
                                medicineInfusion_THDTs = new List<ExpMestMetyReqADO>();
                            }

                            if (medicineInfusion_THDTs != null && medicineInfusion_THDTs.Count > 0)
                            {
                                var medicineInfusion_THDTGroup = medicineInfusion_THDTs.Where(p => p.TRACKING_ID == item.ID).GroupBy(o => new { o.EXP_MEST_ID, o.MIXED_INFUSION });

                                int checkdem = 0;
                                foreach (var InfusionsGroup in medicineInfusion_THDTGroup)
                                {
                                    string s1 = "", s2 = "";
                                    InfusionsGroup.OrderBy(o => o.IS_MIXED_MAIN ?? 99999).ToList();

                                    var InfusionsList = ProcessSortListExpMestMetyReq(InfusionsGroup.ToList());
                                    foreach (var MediIns in InfusionsList)
                                    {
                                        if (checkdem == 0 && ReqTHDT.USE_TIME != null)
                                        {
                                            item.MEDICINES_INFUSION_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc pha truyền thực hiện dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                            item.MEDICINES_INFUSION_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn thuốc pha truyền thực hiện dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                        }

                                        string strAmount = "";

                                        strAmount = ((MediIns.AMOUNT >= 1 && MediIns.AMOUNT < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(MediIns.AMOUNT) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(MediIns.AMOUNT) + "");
                                        s1 = GetUsedDayCounting(MediIns);
                                        s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + MediIns.MEDICINE_TYPE_NAME, FontStyle.Bold);
                                        s1 += " " + MediIns.CONCENTRA;
                                        if (MediIns.IS_MIXED_MAIN != 1)
                                        {
                                            var MixedMain = InfusionsGroup.FirstOrDefault(o => o.IS_MIXED_MAIN == 1);
                                            s1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br) + (MixedMain != null ? " (Thuốc đi kèm thuốc " + MixedMain.MEDICINE_TYPE_NAME + " " + MediIns.CONCENTRA + ")" : "");
                                        }
                                        s2 = strAmount + " " + MediIns.SERVICE_UNIT_NAME + "/ngày";

                                        item.MEDICINES_INFUSION_THDT___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                        item.MEDICINES_INFUSION_THDT___DATA += MediIns.TUTORIAL;

                                        item.MEDICINES_INFUSION_THDT_HTU___DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                        item.MEDICINES_INFUSION_THDT_HTU___DATA += MediIns.TUTORIAL;
                                        if (!string.IsNullOrEmpty(MediIns.HTU_TEXT))
                                        {
                                            item.MEDICINES_INFUSION_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            item.MEDICINES_INFUSION_THDT_HTU___DATA += MediIns.HTU_TEXT;
                                        }

                                        item.MEDICINE_TYPE_ID = MediIns.TDL_MEDICINE_TYPE_ID;
                                        checkdem++;
                                    }
                                    item.MEDICINES_INFUSION_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                }
                            }
                            #endregion

                            #region ngoài kho thực hiện dự trù
                            var OutStockTHDTs = _ServiceReqMetyADOs != null && _ServiceReqMetyADOs.Count > 0 ? _ServiceReqMetyADOs.Where(o => o.USED_FOR_TRACKING_ID == item.ID && o.SERVICE_REQ_ID == ReqTHDT.ID).ToList() : null;

                            if (OutStockTHDTs == null)
                            {
                                OutStockTHDTs = new List<ServiceReqMetyADO>();
                            }

                            if (OutStockTHDTs != null && OutStockTHDTs.Count > 0)
                            {
                                int dem = 0;
                                OutStockTHDTs = ProcessSortListServiceReqMety(OutStockTHDTs);
                                foreach (var medi in OutStockTHDTs)
                                {
                                    string s1 = "";

                                    s1 += medi.MEDICINE_TYPE_NAME + " ";

                                    decimal? amount = 0;
                                    string strAmount = "";
                                    if (medi.AMOUNT > 0)
                                    {
                                        amount = medi.AMOUNT;
                                    }
                                    strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");

                                    string s2 = strAmount + " " + medi.SERVICE_UNIT_NAME;

                                    if (dem == 0 && ReqTHDT.USE_TIME != null)
                                    {
                                        item.MEDICINES_OutStock_THDT__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn thuốc mua ngoài dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MEDICINES_OutStock_THDT_HTU__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn thuốc mua ngoài dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                    }
                                    else
                                    {
                                        item.MEDICINES_OutStock_THDT__DATA += "";
                                        item.MEDICINES_OutStock_THDT_HTU__DATA += "";
                                    }

                                    item.MEDICINES_OutStock_THDT__DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                    item.MEDICINES_OutStock_THDT__DATA += medi.TUTORIAL;
                                    item.MEDICINES_OutStock_THDT__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                    item.MEDICINES_OutStock_THDT_HTU__DATA += String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                    item.MEDICINES_OutStock_THDT_HTU__DATA += medi.TUTORIAL;
                                    item.MEDICINES_OutStock_THDT_HTU__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    if (!string.IsNullOrEmpty(medi.HTU_TEXT))
                                    {
                                        item.MEDICINES_OutStock_THDT_HTU__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_OutStock_THDT_HTU__DATA += medi.HTU_TEXT;
                                    }

                                    item.MEDICINE_TYPE_ID = medi.MEDICINE_TYPE_ID;
                                    dem++;
                                }

                            }
                            #endregion
                        }

                    }
                    #endregion



                    #region thực hiện dịch vụ dự trù

                    //if (_ServiceReqTHDT != null && _ServiceReqTHDT.Count > 0 && _ServiceReqTHDT.Exists(o => o.TRACKING_ID == item.ID))
                    //{
                    #region thực hiện dvu cls dự trù
                    //List<ServiceCLS> ServiceAll = new List<ServiceCLS>();
                    //var ClsDuTrus = this._ServiceCLSSplits != null && this._ServiceCLSSplits.Count > 0 ? this._ServiceCLSSplits.Where(o => _ServiceReqTHDT.Exists(ReqTHDT => o.SERVICE_REQ_ID == ReqTHDT.ID && ReqTHDT.USED_FOR_TRACKING_ID == item.ID)).ToList() : null;
                    if (_ServiceClsTHDT != null && _ServiceClsTHDT.Count > 0 && _ServiceClsTHDT.Exists(o => o.TRACKING_ID == item.ID))
                    {
                        //ServiceAll.AddRange(ClsDuTrus);
                        var _ServiceClsTHDTGroupUseTime = _ServiceClsTHDT.Where(o => o.TRACKING_ID == item.ID).OrderBy(o => o.USE_TIME).GroupBy(o => o.USE_TIME).ToList();
                        foreach (var lstService in _ServiceClsTHDTGroupUseTime)
                        {
                            string title = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstService.Key ?? 0), FontStyle.Bold);
                            item.SERVICE_CLS_THDT___DATA += title;
                            item.SERVICE_CLS_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            item.SERVICE_CLS_THDT_X01___DATA += title;
                            item.SERVICE_CLS_THDT_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            var groupByServiceType = lstService.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                            foreach (var serviceType in groupByServiceType)
                            {
                                var namesvt = rdo._ServiceTypes.FirstOrDefault(p => p.ID == serviceType.Key);
                                item.SERVICE_CLS_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(namesvt.SERVICE_TYPE_NAME + ": ", FontStyle.Bold);
                                item.SERVICE_CLS_THDT_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(namesvt.SERVICE_TYPE_NAME + ": ", FontStyle.Bold);
                                List<string> serviceClss = new List<string>();
                                List<string> serviceClss01 = new List<string>();
                                foreach (var service in serviceType.ToList())
                                {
                                    var s1 = (service.SERVICE_NAME ?? service.TDL_SERVICE_NAME) + " ";
                                    decimal? amount = 0;
                                    string strAmount = "";
                                    if (service.AMOUNT > 0)
                                    {
                                        amount = service.AMOUNT;
                                    }

                                    strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");

                                    serviceClss.Add(string.Format("{0}x{1}", s1, amount));
                                    serviceClss01.Add(string.Format("{0}x{1}", s1, strAmount));
                                    lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = namesvt.SERVICE_TYPE_NAME, SERVICE_NAME = s1, AMOUNT = service.AMOUNT, INSTRUCTION_NOTE = service.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = namesvt.NUM_ORDER ?? Int64.MinValue, IsGoupService = service.IsGoupService, USE_TIME = service.USE_TIME, TDL_INTRUCTION_TIME = service.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = service.TDL_SERVICE_UNIT_ID, SERVICE_ID = service.SERVICE_ID, serviceSplits = service.serviceSplits, TDL_SERVICE_TYPE_ID = service.TDL_SERVICE_TYPE_ID });

                                }
                                if (serviceClss != null && serviceClss.Count > 0)
                                {
                                    item.SERVICE_CLS_THDT___DATA += string.Join("; ", serviceClss);
                                    item.SERVICE_CLS_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                    item.SERVICE_CLS_THDT_X01___DATA += string.Join("; ", serviceClss01);
                                    item.SERVICE_CLS_THDT_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }
                        }
                    }
                    #endregion

                    #region thực hiện dvu tt dự trù
                    //var TtDutrus = this._TTServices != null && this._TTServices.Count > 0 ? this._TTServices.Where(o => _ServiceReqTHDT.Exists(ReqTHDT => o.SERVICE_REQ_ID == ReqTHDT.ID && ReqTHDT.USED_FOR_TRACKING_ID == item.ID)).ToList() : null;
                    if (_ServiceTtTHDT != null && _ServiceTtTHDT.Count > 0 && _ServiceTtTHDT.Exists(o => o.TRACKING_ID == item.ID))
                    {
                        //TtDutrus.ForEach(o => o.USE_TIME = _ServiceReqDuTrus.FirstOrDefault(ReqDT => o.SERVICE_REQ_ID == ReqDT.ID).USE_TIME);
                        //_TTServices = _TTServices.Where(o => !TtDutrus.Exists(p => p.SERVICE_ID == o.SERVICE_ID && p.SERVICE_REQ_ID == o.SERVICE_REQ_ID)).ToList();
                        //ServiceAll.AddRange(TtDutrus);
                        var _ServiceTtTHDTGroupUseTime = _ServiceTtTHDT.Where(o => o.TRACKING_ID == item.ID).OrderBy(o => o.USE_TIME).GroupBy(o => o.USE_TIME).ToList();

                        foreach (var lstService in _ServiceTtTHDTGroupUseTime)
                        {
                            string title = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(lstService.Key ?? 0), FontStyle.Bold);
                            item.TT_SERVICE_THDT___DATA += title;
                            item.TT_SERVICE_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            var groupByServiceType = lstService.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                            foreach (var serviceType in groupByServiceType)
                            {
                                var namesvt = rdo._ServiceTypes.FirstOrDefault(p => p.ID == serviceType.Key);
                                item.TT_SERVICE_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(namesvt.SERVICE_TYPE_NAME + ": ", FontStyle.Bold);
                                List<string> serviceTts = new List<string>();
                                foreach (var service in serviceType.ToList())
                                {
                                    var s1 = (service.SERVICE_NAME ?? service.TDL_SERVICE_NAME) + " ";

                                    serviceTts.Add(string.Format("{0}", s1));
                                    lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = namesvt.SERVICE_TYPE_NAME, SERVICE_NAME = s1, AMOUNT = service.AMOUNT, INSTRUCTION_NOTE = service.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = namesvt.NUM_ORDER ?? Int64.MinValue, IsGoupService = service.IsGoupService, USE_TIME = service.USE_TIME, TDL_INTRUCTION_TIME = service.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = service.TDL_SERVICE_UNIT_ID, SERVICE_ID = service.SERVICE_ID, serviceSplits = service.serviceSplits, TDL_SERVICE_TYPE_ID = service.TDL_SERVICE_TYPE_ID });

                                }
                                if (serviceTts != null && serviceTts.Count > 0)
                                {
                                    item.TT_SERVICE_THDT___DATA += string.Join("; ", serviceTts);
                                    item.TT_SERVICE_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Group Dự trù về theo ngày
                    //if (ServiceAll != null && ServiceAll.Count > 0)
                    //{
                    //    var groupServiceByUseTime = ServiceAll.GroupBy(o => o.USE_TIME).ToList();
                    //    foreach (var svs in groupServiceByUseTime)
                    //    {
                    //        int demService = 0;
                    //        foreach (var service in svs)
                    //        {
                    //            bool IsCls = ClsDuTrus.Exists(o => o.SERVICE_ID == service.SERVICE_ID);
                    //            string s1 = "";

                    //            s1 += (service.SERVICE_NAME ?? service.TDL_SERVICE_NAME) + " ";
                    //            decimal? amount = 0;
                    //            string strAmount = "";
                    //            if (service.AMOUNT > 0)
                    //            {
                    //                amount = service.AMOUNT;
                    //            }

                    //            strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");

                    //            string s2 = strAmount;
                    //            if (demService == 0)
                    //            {
                    //                string title = Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(svs.Key ?? 0), FontStyle.Bold);
                    //                if (IsCls)
                    //                    item.SERVICE_CLS_THDT___DATA += title;
                    //                else
                    //                    item.TT_SERVICE_THDT___DATA += title;
                    //            }
                    //            string content = String.Format("<table><tr><td style =\"vertical-align: top\" width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td style =\"vertical-align: top\" text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                    //            if (IsCls)
                    //            {
                    //                item.SERVICE_CLS_THDT___DATA += content;
                    //                item.SERVICE_CLS_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                    //            }
                    //            else
                    //            {
                    //                item.TT_SERVICE_THDT___DATA += content;
                    //                item.TT_SERVICE_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                    //            }
                    //            demService++;
                    //        }
                    //    }
                    //}
                    #endregion
                    //}
                    #endregion


                    item.MOBA_IMP_MEST_MEDICINE__DATA = "";
                    item.MOBA_IMP_MEST_MATERIAL__DATA = "";
                    item.MOBA_IMP_MEST_BLOOD__DATA = "";
                    #region thuoc hoan tra

                    if (this._ImpMestMedicineADOs != null && this._ImpMestMedicineADOs.Count > 0)
                    {
                        var impMestMedicineAPIs_G = this._ImpMestMedicineADOs.Where(o => o.TRACKING_ID == item.ID).ToList().GroupBy(o => o.TRACKING_ID);

                        foreach (var impMestMedicine in impMestMedicineAPIs_G)
                        {
                            long? IntructionTime = 0;
                            long? UseTime = null;
                            var impMestMedicine_S = impMestMedicine.OrderBy(p => p.USE_TIME_OLD ?? 99999999999999).ThenBy(o => o.INTRUCTION_TIME_OLD ?? 99999999999999).ToList();
                            impMestMedicine_S = ProcessSortListImpMestMedicine(impMestMedicine_S);
                            foreach (var IMedi in impMestMedicine_S)
                            {
                                string s1 = "", s2 = "";

                                if (IMedi.USE_TIME_OLD != null && UseTime != IMedi.USE_TIME_OLD)
                                {
                                    UseTime = IMedi.USE_TIME_OLD;
                                    item.MOBA_IMP_MEST_MEDICINE__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Hoàn trả thuốc dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(UseTime ?? 0), FontStyle.Bold);
                                }
                                else if (IMedi.INTRUCTION_TIME_OLD != null && IntructionTime != IMedi.INTRUCTION_TIME_OLD)
                                {
                                    IntructionTime = IMedi.INTRUCTION_TIME_OLD;
                                    item.MOBA_IMP_MEST_MEDICINE__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Hoàn trả thuốc sử dụng ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(IntructionTime ?? 0), FontStyle.Bold);
                                }

                                s1 = IMedi.MEDICINE_TYPE_NAME;
                                s1 += " " + IMedi.CONCENTRA;
                                s2 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(IMedi.AMOUNT) + " " + IMedi.SERVICE_UNIT_NAME;
                                item.MOBA_IMP_MEST_MEDICINE__DATA += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);

                                item.MEDICINE_TYPE_ID = IMedi.MEDICINE_TYPE_ID;
                            }
                        }
                    }

                    if (this._ImpMestMaterialADOs != null && this._ImpMestMaterialADOs.Count > 0)
                    {
                        var impMestMaterialAPIs_G = this._ImpMestMaterialADOs.Where(o => o.TRACKING_ID == item.ID).ToList().GroupBy(o => o.TRACKING_ID);

                        foreach (var impMestMaterial in impMestMaterialAPIs_G)
                        {
                            long? IntructionTime = 0;
                            long? UseTime = null;
                            var impMestMaterial_S = impMestMaterial.OrderBy(p => p.USE_TIME_OLD ?? 99999999999999).ThenBy(o => o.INTRUCTION_TIME_OLD ?? 99999999999999).ToList();
                            foreach (var IMate in impMestMaterial_S)
                            {
                                string s1 = "", s2 = "";

                                if (IMate.USE_TIME_OLD != null && UseTime != IMate.USE_TIME_OLD)
                                {
                                    UseTime = IMate.USE_TIME_OLD;
                                    item.MOBA_IMP_MEST_MATERIAL__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Hoàn trả vật tư dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(UseTime ?? 0), FontStyle.Bold);
                                }
                                else if (IMate.INTRUCTION_TIME_OLD != null && IntructionTime != IMate.INTRUCTION_TIME_OLD)
                                {
                                    IntructionTime = IMate.INTRUCTION_TIME_OLD;
                                    item.MOBA_IMP_MEST_MATERIAL__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Hoàn trả vật tư sử dụng ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(IntructionTime ?? 0), FontStyle.Bold);
                                }

                                s1 = IMate.MATERIAL_TYPE_NAME;
                                s2 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(IMate.AMOUNT) + " " + IMate.SERVICE_UNIT_NAME;
                                item.MOBA_IMP_MEST_MATERIAL__DATA += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                            }
                        }
                    }

                    if (this._ImpMestBloodADOs != null && this._ImpMestBloodADOs.Count > 0)
                    {
                        var ImpMestBloodAPIs_G = this._ImpMestBloodADOs.Where(o => o.TRACKING_ID == item.ID).ToList().GroupBy(o => o.TRACKING_ID);

                        foreach (var impMestBlood in ImpMestBloodAPIs_G)
                        {
                            long? IntructionTime = 0;
                            var impMestBlood_S = impMestBlood.OrderBy(o => o.INTRUCTION_TIME ?? 99999999999999).ToList();
                            foreach (var IBlood in impMestBlood_S)
                            {
                                string s1 = "", s2 = "";

                                if (IBlood.INTRUCTION_TIME_OLD != null && IntructionTime != IBlood.INTRUCTION_TIME_OLD)
                                {
                                    IntructionTime = IBlood.INTRUCTION_TIME_OLD;
                                    item.MOBA_IMP_MEST_BLOOD__DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Hoàn trả máu sử dụng ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(IntructionTime ?? 0), FontStyle.Bold);
                                }
                                s1 = IBlood.BLOOD_TYPE_NAME;
                                s2 = Inventec.Common.Number.Convert.NumberToStringRoundMax4(IBlood.VOLUME) + " " + IBlood.SERVICE_UNIT_NAME;
                                item.MOBA_IMP_MEST_BLOOD__DATA += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                            }
                        }
                    }
                    #endregion

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

                    if (serviceReqDDTs != null && serviceReqDDTs.Count > 0)
                    {
                        //1.Lay tu danh sach yeu cau
                        foreach (var itemByServiceReq in serviceReqDDTs)
                        {
                            var itemExpMest = rdo._DicHisExpMests.ContainsKey(itemByServiceReq.ID) ? rdo._DicHisExpMests[itemByServiceReq.ID] : null;

                            if (itemExpMest == null)
                                continue;

                            List<ExpMestMedicineADO> _expMestMedicines = new List<ExpMestMedicineADO>();
                            if (rdo._DicExpMestMedicines.ContainsKey(itemExpMest.ID))
                            {
                                foreach (var iem in rdo._DicExpMestMedicines[itemExpMest.ID].ToList())
                                {
                                    ExpMestMedicineADO Ado = new ExpMestMedicineADO(iem, rdo._MedicineTypes);
                                    _expMestMedicines.Add(Ado);
                                }
                            }

                            if (_expMestMedicines != null && _expMestMedicines.Count > 0)
                            {
                                _expMestMedicines = _expMestMedicines.Where(o => o.USE_TIME_TO >= item.TRACKING_TIME).OrderBy(p => p.NUM_ORDER).ToList();
                                _expMestMedicines = ProcessSortListExpMestMedicine(_expMestMedicines);

                                foreach (var emmedi in _expMestMedicines)
                                {
                                    var medicineTypeName = rdo._MedicineTypes.FirstOrDefault(p => p.ID == emmedi.TDL_MEDICINE_TYPE_ID);

                                    if (medicineTypeName != null)
                                    {
                                        string s1 = "", s2 = "";
                                        s1 = medicineTypeName.MEDICINE_TYPE_NAME;
                                        s1 += " " + medicineTypeName.CONCENTRA;
                                        s2 = emmedi.TUTORIAL;
                                        item.PRE_MEDICINE += String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">- {0}</td></span></tr><tr><td text-align=\"right\" align=\"left\" width=\"650\">{1}</td></tr></table>", s1, s2);

                                    }
                                    item.MEDICINE_TYPE_ID = emmedi.TDL_MEDICINE_TYPE_ID;
                                }
                            }
                        }
                    }

                    //RemedyCount

                    var medicineDY = medicines != null && medicines.Count > 0 ? medicines.Where(o => (o.REMEDY_COUNT ?? 0) > 0).ToList() : null;
                    var listRemedyCountADOs = this._RemedyCountADOs != null && this._RemedyCountADOs.Count > 0 ? this._RemedyCountADOs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listRemedyCountADOs != null && listRemedyCountADOs.Count > 0)
                    {
                        item.REMEDY_COUNT___DATA = "";
                        item.REMEDY_COUNT___DATA1 = "";
                        item.MEDICINES_DONG___DATA = "";
                        item.MEDICINES_DONG_DETAIL___DATA = "";
                        item.MEDICINES_DONG_HTU___DATA = "";
                        int dem = 0;
                        foreach (var item1 in listRemedyCountADOs)
                        {
                            if (medicineDY != null && medicineDY.Count > 0)
                            {
                                var DongYExp_Mest = medicineDY.Where(o => o.EXP_MEST_ID == item1.EXP_MEST_ID).ToList();
                                if (DongYExp_Mest != null && DongYExp_Mest.Count > 0)
                                {
                                    int dem1 = 0;
                                    DongYExp_Mest = ProcessSortListExpMestMetyReq(DongYExp_Mest);
                                    List<MedicineDongY> meys = new List<MedicineDongY>();
                                    foreach (var DY in DongYExp_Mest)
                                    {
                                        string s1 = "";

                                        s1 += "- " + DY.MEDICINE_TYPE_NAME;
                                        s1 += " " + DY.CONCENTRA;
                                        decimal? amount = 0;
                                        string strAmount = "";
                                        amount = DY.Amount_By_Remedy_Count;

                                        if (DY.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || DY.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)//GayNghien..2.4 //HuongThan..2.5
                                        {
                                            strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "") + " (" + Inventec.Common.String.Convert.CurrencyToVneseString(Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0)) + ")";
                                        }
                                        else
                                        {
                                            strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount ?? 0) + "");
                                        }

                                        string s2 = strAmount + " " + DY.SERVICE_UNIT_NAME;

                                        var stringFormat = String.Format("<table><tr><td width=\"650\" text-align=\"left\" align=\"left\">{0}</td></span><td text-align=\"right\" align=\"right\" width=\"150\">{1}</td></tr></table>", s1, s2);
                                        meys.Add(new MedicineDongY() { ServiceReqId = DY.TDL_SERVICE_REQ_ID ?? 0, Content = stringFormat });
                                        item.MEDICINES_DONG___DATA += stringFormat;
                                        item.MEDICINES_DONG_HTU___DATA += stringFormat;
                                        if (dem1 == DongYExp_Mest.Count - 1)
                                        {
                                            if ((item1.REMEDY_COUNT ?? 0) > 0)
                                            {
                                                item.MEDICINES_DONG___DATA += "Liều 1 thang x " + item1.DAY_COUNT * item1.REMEDY_COUNT + " thang";
                                                item.MEDICINES_DONG___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                                item.MEDICINES_DONG___DATA += item1.TUTORIAL_REMEDY;
                                                item.MEDICINES_DONG___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);


                                                item.MEDICINES_DONG_HTU___DATA += "Liều 1 thang x " + item1.DAY_COUNT * item1.REMEDY_COUNT + " thang";
                                                item.MEDICINES_DONG_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                                item.MEDICINES_DONG_HTU___DATA += item1.TUTORIAL_REMEDY;
                                                item.MEDICINES_DONG_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            }
                                        }
                                        item.MEDICINES_DONG_HTU___DATA += DY.HTU_TEXT;
                                        item.MEDICINES_DONG_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINE_TYPE_ID = DY.TDL_MEDICINE_TYPE_ID;
                                        dem1++;
                                    }
                                    string PreDetail = null;
                                    string AfterDetail = null;
                                    if (rdo._DicServiceReqs != null && rdo._DicServiceReqs.Count > 0)
                                    {
                                        var serviceReqList = rdo._DicServiceReqs.Values.Where(p => DongYExp_Mest.Select(o => o.TDL_SERVICE_REQ_ID).ToList().Exists(o => o == p.ID)).ToList();
                                        foreach (var serviceReq in serviceReqList)
                                        {
                                            if (serviceReq != null && serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && (item1.REMEDY_COUNT ?? 0) > 0 && serviceReq.ASSIGN_TIME_TO > serviceReq.INTRUCTION_TIME)
                                            {
                                                var title = string.Format("Bài thuốc sử dụng từ ngày {0} đến {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.INTRUCTION_TIME), Inventec.Common.DateTime.Convert.TimeNumberToDateString(serviceReq.ASSIGN_TIME_TO ?? 0));
                                                //if (item.MEDICINES_DONG_DETAIL___DATA.IndexOf(title) < 0)
                                                //{
                                                PreDetail += title;
                                                PreDetail += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                                //}

                                                long assignTimeTo = serviceReq.ASSIGN_TIME_TO ?? 0;
                                                long intructionTime = serviceReq.INTRUCTION_TIME;
                                                if (assignTimeTo > 0 && intructionTime > 0)
                                                {
                                                    DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(intructionTime.ToString().Substring(0, 8) + "000000")) ?? DateTime.Now;
                                                    DateTime dtassignTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(assignTimeTo.ToString().Substring(0, 8) + "000000")) ?? DateTime.Now;
                                                    TimeSpan ts = new TimeSpan();
                                                    ts = (TimeSpan)(dtassignTimeTo - dtIntructionTime);
                                                    if (ts != null && ts.Days >= 0)
                                                    {
                                                        AfterDetail = string.Format("x {0} thang", item1.DAY_COUNT * item1.REMEDY_COUNT * (ts.Days + 1));
                                                    }
                                                }
                                                AfterDetail += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                            }

                                            if (meys.Exists(o => o.ServiceReqId == serviceReq.ID))
                                            {
                                                item.MEDICINES_DONG_DETAIL___DATA += string.Format("{0}{1}{2}", PreDetail, string.Join("", meys.Where(o => o.ServiceReqId == serviceReq.ID).Select(o => o.Content)), AfterDetail);
                                                if (!string.IsNullOrEmpty(serviceReq.ADVISE))
                                                {
                                                    item.MEDICINES_DONG_DETAIL___DATA += serviceReq.ADVISE;
                                                    item.MEDICINES_DONG_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

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
                                if (!String.IsNullOrEmpty(item.REMEDY_COUNT___DATA))
                                {
                                    item.REMEDY_COUNT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                                if (!String.IsNullOrEmpty(item.REMEDY_COUNT___DATA1))
                                {
                                    item.REMEDY_COUNT___DATA1 += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
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
                    var listBloodss = this._Bloods != null && this._Bloods.Count > 0 ? this._Bloods.Where(o => o.TRACKING_ID == item.ID).GroupBy(o => new { o.BLOOD_TYPE_ID, o.BLOOD_ABO_ID, o.BLOOD_RH_ID }).ToList() : null;
                    item.BLOOD___DATA = "";
                    if (listBloodss != null && listBloodss.Count > 0)
                    {
                        int dem = 0;
                        foreach (var listBloods in listBloodss)
                        {
                            if (listBloods.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() == 1)
                            {
                                foreach (var item1 in listBloods)
                                {
                                    item.BLOOD___DATA += item1.TDL_SERVICE_NAME;

                                    if (!String.IsNullOrEmpty(item1.BLOOD_ABO_CODE))
                                    {
                                        item.BLOOD___DATA += " (" + item1.BLOOD_ABO_CODE;

                                        if (!String.IsNullOrEmpty(item1.BLOOD_RH_CODE))
                                        {
                                            item.BLOOD___DATA += item1.BLOOD_RH_CODE + ")";
                                        }
                                    }

                                    item.BLOOD___DATA += " * " + item1.AMOUNT + " Đơn vị";
                                    item.BLOOD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);


                                    if (dem < listBloods.ToList().Count - 1)
                                    {
                                        item.BLOOD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }
                                    dem++;
                                }
                            }
                            else
                            {
                                foreach (var item1 in listBloods)
                                {
                                    if (item1.INTRUCTION_DATE_STR > 0)
                                    {
                                        item.BLOOD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày y lệnh: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(item1.INTRUCTION_DATE_STR), FontStyle.Bold) + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }

                                    item.BLOOD___DATA += item1.TDL_SERVICE_NAME;

                                    if (!String.IsNullOrEmpty(item1.BLOOD_ABO_CODE))
                                    {
                                        item.BLOOD___DATA += " (" + item1.BLOOD_ABO_CODE;

                                        if (!String.IsNullOrEmpty(item1.BLOOD_RH_CODE))
                                        {
                                            item.BLOOD___DATA += item1.BLOOD_RH_CODE + ")";
                                        }
                                    }

                                    item.BLOOD___DATA += " * " + item1.AMOUNT + " Đơn vị";

                                    if (dem < listBloods.ToList().Count - 1)
                                    {
                                        item.BLOOD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }
                                    dem++;
                                }
                            }
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
                    //item.ExpMestMatyReqADOs = new List<ExpMestMatyReqADO>();
                    //item.ExpMestMatyReqADOs = materials;
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
                                HTU_TEXT = item1.HTU_TEXT
                            });
                        }
                    }
                    if (materials != null && materials.Count > 0)
                    {
                        int dem = 0;
                        foreach (var item1 in materials)
                        {
                            item.MATERIAL___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                            item.MATERIAL_HTU___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                            if (!string.IsNullOrEmpty(item1.HTU_TEXT))
                            {
                                item.MATERIAL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                item.MATERIAL_HTU___DATA += item1.HTU_TEXT;
                            }
                            if (dem < materials.Count - 1)
                            {
                                item.MATERIAL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                item.MATERIAL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            dem++;
                        }
                    }
                    #region vật tư dự trù
                    item.MATERIAL_DuTru___DATA = "";
                    foreach (var ReqDuTru in _ServiceReqDuTrus)
                    {
                        if (ReqDuTru.TRACKING_ID == item.ID && ReqDuTru.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                        {
                            var materialDuTrus = this._ExpMestMatyReqADOsDuTru != null && this._ExpMestMatyReqADOsDuTru.Count > 0 ? this._ExpMestMatyReqADOsDuTru.Where(o => o.TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqDuTru.ID).ToList() : null;


                            if (materialDuTrus != null && materialDuTrus.Count > 0)
                            {
                                int dem = 0;
                                foreach (var item1 in materialDuTrus)
                                {
                                    if (dem == 0 && ReqDuTru.USE_TIME != null)
                                    {
                                        item.MATERIAL_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn vật tư dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDuTru.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MATERIAL_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                        item.MATERIAL_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Đơn vật tư dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqDuTru.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MATERIAL_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }

                                    item.MATERIAL_DuTru___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                                    //if (dem < materialDuTrus.Count - 1)
                                    //{
                                    item.MATERIAL_DuTru___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    //}

                                    item.MATERIAL_DuTru_HTU___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                                    if (!string.IsNullOrEmpty(item1.HTU_TEXT))
                                    {
                                        item.MEDICINES_INFUSION_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MEDICINES_INFUSION_THDT_HTU___DATA += item1.HTU_TEXT;
                                    }
                                    //if (dem < materialDuTrus.Count - 1)
                                    //{
                                    item.MATERIAL_DuTru_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    //}
                                    dem++;
                                }
                            }
                        }
                    }
                    #endregion

                    #region vật tư thự hiện dự trù
                    item.MATERIAL_THDT___DATA = "";
                    foreach (var ReqTHDT in _ServiceReqTHDT)
                    {
                        if (ReqTHDT.USED_FOR_TRACKING_ID == item.ID && ReqTHDT.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                        {
                            var materialTHDTs = this._ExpMestMatyReqADOsTHDT != null && this._ExpMestMatyReqADOsTHDT.Count > 0 ? this._ExpMestMatyReqADOsTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID && o.TRACKING_ID == item.ID && o.TDL_SERVICE_REQ_ID == ReqTHDT.ID).ToList() : null;


                            if (materialTHDTs != null && materialTHDTs.Count > 0)
                            {
                                int dem = 0;
                                foreach (var item1 in materialTHDTs)
                                {
                                    if (dem == 0 && ReqTHDT.USE_TIME != null)
                                    {
                                        item.MATERIAL_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn vật tư dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MATERIAL_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                        item.MATERIAL_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Thực hiện đơn vật tư dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ReqTHDT.USE_TIME ?? 0), FontStyle.Bold);
                                        item.MATERIAL_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    }

                                    item.MATERIAL_THDT___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                                    //if (dem < materialTHDTs.Count - 1)
                                    //{
                                    item.MATERIAL_THDT___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    //}



                                    item.MATERIAL_THDT_HTU___DATA += item1.MATERIAL_TYPE_NAME + "  x " + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT);
                                    if (!string.IsNullOrEmpty(item1.HTU_TEXT))
                                    {
                                        item.MATERIAL_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        item.MATERIAL_THDT_HTU___DATA += item1.HTU_TEXT;
                                    }
                                    //if (dem < materialTHDTs.Count - 1)
                                    //{
                                    item.MATERIAL_THDT_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    //}
                                    dem++;
                                }
                            }
                        }
                    }
                    #endregion

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
                            item.TT_SERVICE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.TDL_SERVICE_NAME, FontStyle.Bold) + "    -     " + item1.INSTRUCTION_NOTE;
                            if (dem < ttServices.Count - 1)
                            {
                                item.TT_SERVICE___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            }
                            lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = item1.SERVICE_TYPE_NAME, SERVICE_NAME = item1.TDL_SERVICE_NAME, AMOUNT = item1.AMOUNT, INSTRUCTION_NOTE = item1.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = item1.NUM_ORDER_SERVICE_TYPE ?? Int64.MinValue, IsGoupService = item1.IsGoupService, USE_TIME = item1.USE_TIME, TDL_INTRUCTION_TIME = item1.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = item1.TDL_SERVICE_UNIT_ID, SERVICE_ID = item1.SERVICE_ID, serviceSplits = item1.serviceSplits, TDL_SERVICE_TYPE_ID = item1.TDL_SERVICE_TYPE_ID });
                            dem++;
                        }
                    }

                    item.SERVICE_CLS___DATA = "";
                    item.SERVICE_CLS_BOLD___DATA = "";
                    item.SERVICE_CLS_X01___DATA = "";
                    item.SERVICE_CLS_BOLD_X01___DATA = "";
                    var listServiceCLSs = this._ServiceCLSs != null && this._ServiceCLSs.Count > 0 ? this._ServiceCLSs.Where(o => o.TRACKING_ID == item.ID).ToList() : null;
                    if (listServiceCLSs != null && listServiceCLSs.Count > 0 && !string.IsNullOrEmpty(listServiceCLSs[0].SERVICE_NAME))
                    {
                        int dem = 0;
                        if (listServiceCLSs.Select(o => o.TDL_INTRUCTION_DATE).Distinct().Count() == 1)
                        {
                            foreach (var item1 in listServiceCLSs)
                            {
                                item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + item1.SERVICE_NAME;
                                item.SERVICE_CLS_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + item1.SERVICE_NAME;
                                item.SERVICE_CLS_BOLD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_NAME, FontStyle.Bold);
                                item.SERVICE_CLS_BOLD_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_NAME, FontStyle.Bold);
                                if (item1.AMOUNT > 0)
                                {
                                    item.SERVICE_CLS___DATA += "           x" + item1.AMOUNT;
                                    item.SERVICE_CLS_BOLD___DATA += "           x" + item1.AMOUNT;

                                    var amount = ((item1.AMOUNT >= 1 && item1.AMOUNT < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) + "");
                                    item.SERVICE_CLS_X01___DATA += "           x" + amount;
                                    item.SERVICE_CLS_BOLD_X01___DATA += "           x" + amount;
                                }

                                if (dem < listServiceCLSs.Count - 1)
                                {
                                    item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_BOLD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_BOLD_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                                lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = item1.SERVICE_TYPE_NAME, SERVICE_NAME = item1.SERVICE_NAME, AMOUNT = item1.AMOUNT, INSTRUCTION_NOTE = item1.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = item1.NUM_ORDER_SERVICE_TYPE ?? Int64.MinValue, IsGoupService = item1.IsGoupService, USE_TIME = item1.USE_TIME, TDL_INTRUCTION_TIME = item1.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = item1.TDL_SERVICE_UNIT_ID, SERVICE_ID = item1.SERVICE_ID, serviceSplits = item1.serviceSplits, TDL_SERVICE_TYPE_ID = item1.TDL_SERVICE_TYPE_ID });
                                dem++;
                            }
                        }
                        else
                        {
                            foreach (var item1 in listServiceCLSs)
                            {
                                if (item1.INTRUCTION_DATE_STR > 0)
                                {
                                    item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày y lệnh: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(item1.INTRUCTION_DATE_STR), FontStyle.Bold) + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày y lệnh: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(item1.INTRUCTION_DATE_STR), FontStyle.Bold) + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_BOLD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày y lệnh: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(item1.INTRUCTION_DATE_STR), FontStyle.Bold) + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_BOLD_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("Ngày y lệnh: " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(item1.INTRUCTION_DATE_STR), FontStyle.Bold) + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }

                                item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + item1.SERVICE_NAME;
                                item.SERVICE_CLS_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + item1.SERVICE_NAME;
                                item.SERVICE_CLS_BOLD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_NAME, FontStyle.Bold);
                                item.SERVICE_CLS_BOLD_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_TYPE_NAME, FontStyle.Bold) + "  " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(item1.SERVICE_NAME, FontStyle.Bold);

                                if (item1.AMOUNT > 0)
                                {
                                    item.SERVICE_CLS___DATA += "           x" + item1.AMOUNT;
                                    item.SERVICE_CLS_BOLD___DATA += "           x" + item1.AMOUNT;

                                    var amount = ((item1.AMOUNT >= 1 && item1.AMOUNT < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(item1.AMOUNT) + "");
                                    item.SERVICE_CLS_X01___DATA += "           x" + amount;
                                    item.SERVICE_CLS_BOLD_X01___DATA += "           x" + amount;
                                }

                                if (dem < listServiceCLSs.Count - 1)
                                {
                                    item.SERVICE_CLS___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_BOLD___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                    item.SERVICE_CLS_BOLD_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                                lstServiceCls.Add(new ServiceCLS() { SERVICE_TYPE_NAME = item1.SERVICE_TYPE_NAME, SERVICE_NAME = item1.SERVICE_NAME, AMOUNT = item1.AMOUNT, INSTRUCTION_NOTE = item1.INSTRUCTION_NOTE, NUM_ORDER_SERVICE_TYPE = item1.NUM_ORDER_SERVICE_TYPE ?? Int64.MinValue, IsGoupService = item1.IsGoupService, USE_TIME = item1.USE_TIME, TDL_INTRUCTION_TIME = item1.TDL_INTRUCTION_TIME, TDL_SERVICE_UNIT_ID = item1.TDL_SERVICE_UNIT_ID, SERVICE_ID = item1.SERVICE_ID, serviceSplits = item1.serviceSplits, TDL_SERVICE_TYPE_ID = item1.TDL_SERVICE_TYPE_ID });
                                dem++;
                            }
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

                    ///Xử lý danh sách tổng hợp
                    if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                    {
                        List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                        var itemOrder = lstExpMestMety.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                        var dataGroups = itemOrder.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();
                            _dataNews1.AddRange(dtGroups);
                        }
                        lstExpMestMety = _dataNews1;
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        lstExpMestMety = (lstExpMestMety.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                    {
                        lstExpMestMety = (lstExpMestMety.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());

                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                    {
                        lstExpMestMety = (lstExpMestMety.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList());
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                    {
                        lstExpMestMety = ProcessSortListExpMestMetyReq(lstExpMestMety);
                        var groupByMedicineLine = lstExpMestMety.GroupBy(o => new { o.MEDICINE_LINE_ID, o.MEDICINE_LINE_NAME }).ToList();
                        item.MEDICINES_MERGE_DETAIL___DATA = "";
                        item.MEDICINES_MERGE_DETAIL_HTU___DATA = "";
                        foreach (var ml in groupByMedicineLine)
                        {
                            item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("- " + ml.Key.MEDICINE_LINE_NAME, FontStyle.Bold);
                            item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            item.MEDICINES_MERGE_DAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("- " + ml.Key.MEDICINE_LINE_NAME, FontStyle.Bold);
                            item.MEDICINES_MERGE_DAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            item.MEDICINES_MERGE_DETAIL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("- " + ml.Key.MEDICINE_LINE_NAME, FontStyle.Bold);
                            item.MEDICINES_MERGE_DETAIL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            item.MEDICINES_MERGE_DAY_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("- " + ml.Key.MEDICINE_LINE_NAME, FontStyle.Bold);
                            item.MEDICINES_MERGE_DAY_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                            var groupByRemedy = ml.GroupBy(o => o.TDL_SERVICE_REQ_ID).ToList();
                            foreach (var re in groupByRemedy)
                            {
                                var uiTime = re.ToList()[0].USE_TIME ?? re.ToList()[0].INTRUCTION_TIME;
                                if (re.ToList()[0].REMEDY_COUNT != null)
                                {
                                    re.ToList()[0].ASSIGN_TIME_TO = (re.ToList()[0].ASSIGN_TIME_TO ?? 0) < uiTime ? uiTime : re.ToList()[0].ASSIGN_TIME_TO;
                                    item.MEDICINES_MERGE_DETAIL___DATA += string.Format("Bài thuốc sử dụng từ ngày {0} đến ngày {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(uiTime), Inventec.Common.DateTime.Convert.TimeNumberToDateString(re.ToList()[0].ASSIGN_TIME_TO ?? uiTime));
                                    item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                    item.MEDICINES_MERGE_DETAIL_HTU___DATA += string.Format("Bài thuốc sử dụng từ ngày {0} đến ngày {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(uiTime), Inventec.Common.DateTime.Convert.TimeNumberToDateString(re.ToList()[0].ASSIGN_TIME_TO ?? uiTime));
                                    item.MEDICINES_MERGE_DETAIL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                                item.MEDICINES_MERGE_DETAIL___DATA += string.Join("", re.ToList().Select(o => o.DATA_REPX));
                                //item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                item.MEDICINES_MERGE_DETAIL_HTU___DATA += string.Join("", re.ToList().Select(o => o.DATA_REPX));

                                if (re.ToList()[0].REMEDY_COUNT != null)
                                {
                                    long userTimeTo = re.ToList().Max(p => p.USE_TIME_TO ?? 0);
                                    long assignTimeTo = re.ToList()[0].ASSIGN_TIME_TO ?? 0;
                                    long intructionTime = uiTime;

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
                                            re.ToList()[0].DAY_COUNT = ts.Days + 1;
                                        }
                                    }

                                    if (assignTimeTo > 0 && intructionTime > 0)
                                    {
                                        DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(intructionTime.ToString().Substring(0, 8) + "000000")) ?? DateTime.Now;
                                        DateTime dtassignTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(assignTimeTo.ToString().Substring(0, 8) + "000000")) ?? DateTime.Now;
                                        TimeSpan ts = new TimeSpan();
                                        ts = (TimeSpan)(dtassignTimeTo - dtIntructionTime);
                                        if (ts != null && ts.Days >= 0)
                                        {
                                            item.MEDICINES_MERGE_DETAIL___DATA += string.Format("x {0} thang", re.ToList()[0].DAY_COUNT * re.ToList()[0].REMEDY_COUNT * (ts.Days + 1));
                                            item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                            item.MEDICINES_MERGE_DETAIL_HTU___DATA += string.Format("x {0} thang", re.ToList()[0].DAY_COUNT * re.ToList()[0].REMEDY_COUNT * (ts.Days + 1));
                                            item.MEDICINES_MERGE_DETAIL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                        }
                                    }
                                }
                                if (rdo._DicServiceReqs != null && rdo._DicServiceReqs.Count > 0 && rdo._DicServiceReqs.ContainsKey(re.Key ?? 0) && !string.IsNullOrEmpty(rdo._DicServiceReqs[re.Key ?? 0].ADVISE))
                                {
                                    item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + rdo._DicServiceReqs[re.Key ?? 0].ADVISE, FontStyle.Italic);
                                    item.MEDICINES_MERGE_DETAIL___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }

                                if (!string.IsNullOrEmpty(re.ToList()[0].HTU_TEXT))
                                {
                                    item.MEDICINES_MERGE_DETAIL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + re.ToList()[0].HTU_TEXT, FontStyle.Italic);
                                    item.MEDICINES_MERGE_DETAIL_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }


                            }


                            var SortIntructionTime = ml.ToList().OrderBy(o => o.INTRUCTION_TIME);
                            var groupIntructionTime = SortIntructionTime.GroupBy(o => o.INTRUCTION_TIME);
                            foreach (var it in groupIntructionTime)
                            {
                                List<string> dtTimes = it.ToList().OrderBy(o => o.USE_TIME ?? o.INTRUCTION_TIME).Select(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.USE_TIME ?? o.INTRUCTION_TIME)).ToList();
                                item.MEDICINES_MERGE_DAY___DATA += BuildDateString(dtTimes);
                                item.MEDICINES_MERGE_DAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                item.MEDICINES_MERGE_DAY_HTU___DATA += BuildDateString(dtTimes);
                                item.MEDICINES_MERGE_DAY_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                var itServiceReq = it.ToList().OrderBy(o => o.TDL_SERVICE_REQ_ID).Select(o => o.TDL_SERVICE_REQ_ID).Distinct().ToList()[0];

                                item.MEDICINES_MERGE_DAY___DATA += string.Join("", it.Where(o => o.TDL_SERVICE_REQ_ID == itServiceReq).Select(o => o.DATA_DAY_REPX));
                                item.MEDICINES_MERGE_DAY_HTU___DATA += string.Join("", it.Where(o => o.TDL_SERVICE_REQ_ID == itServiceReq).Select(o => o.DATA_DAY_HTU_REPX));

                                long? sthang = it.GroupBy(o => new{o.TDL_SERVICE_REQ_ID, o.REMEDY_COUNT }).Sum(o=>o.Key.REMEDY_COUNT ?? 0);
                                if (sthang > 0)
                                {
                                    item.MEDICINES_MERGE_DAY___DATA += string.Format("x {0} thang", sthang < 10 ? "0" + sthang : sthang.ToString());
                                    item.MEDICINES_MERGE_DAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                    item.MEDICINES_MERGE_DAY_HTU___DATA += string.Format("x {0} thang", sthang < 10 ? "0" + sthang : sthang.ToString());
                                    item.MEDICINES_MERGE_DAY_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }

                                if (rdo._DicServiceReqs != null && rdo._DicServiceReqs.Count > 0 && rdo._DicServiceReqs.ContainsKey(it.ToList().OrderBy(o => o.TDL_SERVICE_REQ_ID ?? 0).FirstOrDefault().TDL_SERVICE_REQ_ID ?? 0) && !string.IsNullOrEmpty(rdo._DicServiceReqs[it.ToList().OrderBy(o => o.TDL_SERVICE_REQ_ID ?? 0).FirstOrDefault().TDL_SERVICE_REQ_ID ?? 0].ADVISE))
                                {
                                    item.MEDICINES_MERGE_DAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + rdo._DicServiceReqs[it.ToList().OrderBy(o => o.TDL_SERVICE_REQ_ID ?? 0).FirstOrDefault().TDL_SERVICE_REQ_ID ?? 0].ADVISE, FontStyle.Italic);
                                    item.MEDICINES_MERGE_DAY___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                                    item.MEDICINES_MERGE_DAY_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(" " + rdo._DicServiceReqs[it.ToList().OrderBy(o => o.TDL_SERVICE_REQ_ID ?? 0).FirstOrDefault().TDL_SERVICE_REQ_ID ?? 0].ADVISE, FontStyle.Italic);
                                    item.MEDICINES_MERGE_DAY_HTU___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }


                            }

                        }
                    }

                    if (rdo._WorkPlaceSDO.IsOrderByType < 4)
                    {
                        item.MEDICINES_MERGE_DETAIL___DATA = string.Join(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br), lstExpMestMety.Select(o => o.DATA_REPX));
                        item.MEDICINES_MERGE_DETAIL_HTU___DATA = string.Join(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br), lstExpMestMety.Select(o => o.DATA_REPX));
                        item.MEDICINES_MERGE_DAY___DATA = string.Join(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br), lstExpMestMety.Select(o => o.DATA_DAY_REPX));
                        item.MEDICINES_MERGE_DAY_HTU___DATA = string.Join(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br), lstExpMestMety.Select(o => o.DATA_DAY_HTU_REPX));
                    }

                    item.SERVICE_MERGE_X01___DATA = "";
                    if (lstServiceCls != null && lstServiceCls.Count > 0)
                    {
                        lstServiceCls.ForEach(o =>
                        {
                            o.SERVICE_TYPE_NAME = (o.SERVICE_TYPE_NAME ?? "").Replace(":", "").Trim();//Xóa bỏ dấu : sau loại dịch vụ nếu có
                        });
                        var groupServiceType = lstServiceCls.Where(o => o.NUM_ORDER_SERVICE_TYPE > 0).GroupBy(o => new { o.NUM_ORDER_SERVICE_TYPE, o.TDL_SERVICE_TYPE_ID }).ToList().OrderByDescending(o => o.Key.NUM_ORDER_SERVICE_TYPE).ToList();
                        var ListNumOrderNull = lstServiceCls.Where(o => o.NUM_ORDER_SERVICE_TYPE < 0).ToList();
                        if (ListNumOrderNull != null && ListNumOrderNull.Count > 0)
                        {
                            var groupNomOrderNull = ListNumOrderNull.OrderBy(o => o.USE_TIME ?? Int64.MinValue).OrderBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => new { o.NUM_ORDER_SERVICE_TYPE, o.TDL_SERVICE_TYPE_ID }).ToList();
                            if (groupServiceType == null)
                                groupServiceType = groupNomOrderNull;
                            else
                                groupServiceType.AddRange(groupNomOrderNull);
                        }
                        foreach (var st in groupServiceType)
                        {
                            item.SERVICE_MERGE_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle("- " + (!string.IsNullOrEmpty(st.ToList()[0].SERVICE_TYPE_NAME) ? st.ToList()[0].SERVICE_TYPE_NAME : rdo._ServiceTypes.FirstOrDefault(o => o.ID == st.Key.TDL_SERVICE_TYPE_ID).SERVICE_TYPE_NAME) + ":", FontStyle.Bold);
                            item.SERVICE_MERGE_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);

                            List<ServiceCLS> serCls = new List<ServiceCLS>();
                            foreach (var sv in st)
                            {
                                if (sv.IsGoupService)
                                {
                                    serCls.AddRange(sv.serviceSplits);
                                }
                                else
                                {
                                    serCls.Add(sv);
                                }
                            }
                            serCls = serCls.OrderBy(o => o.USE_TIME ?? Int64.MinValue).ThenBy(o => o.TDL_INTRUCTION_TIME).ToList();
                            foreach (var slc in serCls)
                            {
                                if (slc.SERVICE_ID > 0)
                                {
                                    slc.SERVICE_PARENT_ID = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == slc.SERVICE_ID).PARENT_ID;
                                    if (slc.SERVICE_PARENT_ID != null)
                                    {
                                        var parentS = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == slc.SERVICE_PARENT_ID);
                                        slc.SERVICE_NUM_ORDER = parentS != null && parentS.NUM_ORDER != null ? (long?)parentS.NUM_ORDER : null;
                                    }
                                }
                            }
                            var groupByServiceParent = serCls.GroupBy(o => new { o.SERVICE_PARENT_ID, o.SERVICE_NUM_ORDER }).OrderBy(o => o.Key.SERVICE_NUM_ORDER ?? -1).ThenBy(o => o.Key.SERVICE_PARENT_ID ?? -1).ToList();
                            foreach (var serClsByParent in groupByServiceParent)
                            {
                                var groupService = serClsByParent.OrderBy(o => o.USE_TIME ?? Int64.MinValue).ThenBy(o => o.TDL_INTRUCTION_TIME).GroupBy(o => o.SERVICE_ID).ToList();

                                foreach (var gs in groupService)
                                {
                                    var sv = gs.ToList()[0];
                                    var amount = gs.ToList().Sum(o => o.AMOUNT);
                                    var instructionNotes = gs.ToList().Where(o => !string.IsNullOrEmpty(o.INSTRUCTION_NOTE)).ToList();
                                    var strInstructionNote = "";
                                    if (instructionNotes != null && instructionNotes.Count > 0)
                                    {
                                        strInstructionNote = string.Join(", ", instructionNotes.Select(o => o.INSTRUCTION_NOTE));
                                    }
                                    if (!string.IsNullOrEmpty(strInstructionNote))
                                        item.SERVICE_MERGE_X01___DATA += sv.SERVICE_NAME.Replace(":", "").Trim() + ": " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(strInstructionNote, FontStyle.Italic);
                                    else item.SERVICE_MERGE_X01___DATA += sv.SERVICE_NAME.Replace(";", "").Trim();
                                    if (rdo._ServiceTypes.FirstOrDefault(o => o.ID == st.Key.TDL_SERVICE_TYPE_ID).ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                    {
                                        var strAmount = ((amount >= 1 && amount < 10) ? "0" + Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount) : Inventec.Common.Number.Convert.NumberToStringRoundMax4(amount) + "");
                                        item.SERVICE_MERGE_X01___DATA += " x" + strAmount;
                                        var serviceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sv.TDL_SERVICE_UNIT_ID);
                                        if (serviceUnit != null)
                                            item.SERVICE_MERGE_X01___DATA += " " + serviceUnit.SERVICE_UNIT_NAME;
                                    }
                                    item.SERVICE_MERGE_X01___DATA += Inventec.Desktop.Common.HtmlString.ProcessorString.InsertSpacialTag("", Inventec.Desktop.Common.HtmlString.SpacialTag.Tag.Br);
                                }
                            }
                        }
                    }

                    var user = users != null ? users.FirstOrDefault(o => o.LOGINNAME == item.CREATOR) : null;
                    item.TRACKING_USERNAME = user != null ? user.USERNAME : "";

                    item.ICD_DIFF_CODE = GetStringValueInSingleValueDictionaryByKey("ICD_DIFF_CODE");
                    item.ICD_DIFF_TEXT = GetStringValueInSingleValueDictionaryByKey("ICD_DIFF_TEXT");
                    item.PARENT_ORGANIZATION_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.PARENT_ORGANIZATION_NAME);
                    item.ORGANIZATION_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.ORGANIZATION_NAME);
                    item.DEPARTMENT_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.DEPARTMENT_NAME);

                    item.EXECUTE_TIME_DHST = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.EXECUTE_TIME_DHST);
                    item.NGAY_1 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.NGAY_1);
                    item.NGAY_2 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.NGAY_2);
                    item.NGAY_3 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.NGAY_3);
                    item.NGAY_4 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.NGAY_4);
                    item.Y_LENH_1 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.Y_LENH_1);
                    item.Y_LENH_2 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.Y_LENH_2);
                    item.Y_LENH_3 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.Y_LENH_3);
                    item.Y_LENH_4 = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.Y_LENH_4);
                    item.TDL_PATIENT_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_NAME");
                    item.AGE = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.AGE);
                    item.AGE_TRACKING = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.AGE_TRACKING);
                    item.TDL_PATIENT_GENDER_NAME = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_GENDER_NAME");
                    item.TDL_PATIENT_ADDRESS = GetStringValueInSingleValueDictionaryByKey("TDL_PATIENT_ADDRESS");
                    item.IN_CODE = GetStringValueInSingleValueDictionaryByKey("IN_CODE");
                    item.PHONE = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.PHONE);
                    item.ROOM_NAME = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.ROOM_NAME);
                    item.BED_NAME = GetStringValueInSingleValueDictionaryByKey("BED_NAME");
                    item.ICD_NAME_FULL = GetStringValueInSingleValueDictionaryByKey("ICD_NAME");
                    item.TREATMENT_CODE = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.TREATMENT_CODE);
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
                    item.ORDER_SHEET = GetStringValueInSingleValueDictionaryByKey(Mps000062ExtendSingleKey.ORDER_SHEET);
                    if (itemIndex > 0)
                    {
                        item.PRIVIOUS_TRACKING_DATE_STR = GetPriviousTrackingDate(itemIndex - 1);
                    }
                    if (mps000062ADOExt.Mps000062ADOs.Count > itemIndex + 1)
                        item.NEXT_TRACKING_DATE_STR = GetPriviousTrackingDate(itemIndex + 1);
                    itemIndex++;
                }
                success = xtraReportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                success = success && singleTag.ProcessData(xtraReportStore, singleValueDictionary);
                success = success && objectTag.AddObjectData<Mps000062ExtADO>(xtraReportStore, mps000062ADOExt.Mps000062ADOs);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mps000062ADOExt.Mps000062ADOs), mps000062ADOExt.Mps000062ADOs));
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        static string BuildDateString(List<string> dateList)
        {
            List<string> lst = new List<string>();
            var groupYear = dateList.GroupBy(o => o.Substring(6, 4));
            foreach (var item in groupYear)
            {
                if (item.Distinct().Count() == 1)
                    lst.Add(item.ToList()[0]);
                else
                {
                    var groupMonth = item.GroupBy(o => o.Substring(3, 2));
                    int count = 0;
                    foreach (var dt in groupMonth)
                    {
                        count++;
                        string data = "";
                        if (dt.Distinct().Count() == 1)
                            data = dt.ToList()[0].Substring(0, 5);
                        else
                        {
                            data = string.Join(",", dt.ToList().Select(o => o.Substring(0, 2)).Distinct()) + "/" + dt.Key;
                        }
                        if (count == groupMonth.Count())
                            data += "/" + item.Key;
                        lst.Add(data);
                    }
                }
            }
            return string.Format("({0})", string.Join("+", lst));
        }
        string GetUsedDayCounting(ExpMestMetyReqADO medi)
        {
            string value = "";
            try
            {
                if (rdo._WorkPlaceSDO.UsedDayCountingOption == 1)
                {
                    if (rdo._WorkPlaceSDO.UsedDayCountingFormatOption == 1 && (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT))//GayNghien..2.4 //HuongThan..2.5
                    {
                        value += (medi.NUMBER_H_N ?? 0) > 0 ? "- " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_H_N.ToString()).Trim()) + "", FontStyle.Bold) + " -" : "";
                    }
                    else
                    {
                        value += (medi.NUMBER_H_N ?? 0) > 0 ? "- " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", medi.NUMBER_H_N) + "", FontStyle.Bold) + " -" : "";
                    }
                }
                else if (rdo._WorkPlaceSDO.UsedDayCountingOption == 2)
                {
                    if (rdo._WorkPlaceSDO.UsedDayCountingFormatOption == 1 && (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT))//GayNghien..2.4 //HuongThan..2.5
                    {
                        value += (medi.NUMBER_BY_TYPE ?? 0) > 0 ? "- " + String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_BY_TYPE.ToString())) + " -" : "";
                    }
                    else
                    {
                        value += (medi.NUMBER_BY_TYPE ?? 0) > 0 ? "- " + String.Format("({0})", medi.NUMBER_BY_TYPE) + " -" : "";
                    }
                }
                else if (rdo._WorkPlaceSDO.UsedDayCountingOption == 3)
                {
                    if (rdo._WorkPlaceSDO.UsedDayCountingFormatOption == 1 && (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT))//GayNghien..2.4 //HuongThan..2.5
                    {
                        value += (medi.NUMBER_USE_AND_ACTIVE ?? 0) > 0 ? "- " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_USE_AND_ACTIVE.ToString()).Trim()) + "", FontStyle.Bold) : "";
                    }
                    else
                    {
                        value += (medi.NUMBER_USE_AND_ACTIVE ?? 0) > 0 ? "- " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", medi.NUMBER_USE_AND_ACTIVE) + "", FontStyle.Bold) : "";
                    }
                }
                else if (rdo._WorkPlaceSDO.UsedDayCountingOption == 4)
                {
                    if (rdo._WorkPlaceSDO.UsedDayCountingFormatOption == 1 && (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT))//GayNghien..2.4 //HuongThan..2.5
                    {
                        if (medi.NUMBER_BY_TYPE != null && medi.NUMBER_BY_TYPE > 0)
                        {
                            value += "- " + String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_BY_TYPE.ToString()).Trim()) + " -";
                        }
                        else
                        {
                            value += (medi.NUMBER_H_N ?? 0) > 0 ? "- " + String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_H_N.ToString()).Trim()) + " -" : "";
                        }
                    }
                    else
                    {
                        if (medi.NUMBER_BY_TYPE != null && medi.NUMBER_BY_TYPE > 0)
                        {
                            value += "- " + String.Format("({0})", medi.NUMBER_BY_TYPE) + " -";
                        }
                        else
                        {
                            value += (medi.NUMBER_H_N ?? 0) > 0 ? "- " + String.Format("({0})", medi.NUMBER_H_N) + " -" : "";
                        }
                    }
                }
                else if (rdo._WorkPlaceSDO.UsedDayCountingOption == 5)
                {
                    if (rdo._WorkPlaceSDO.UsedDayCountingFormatOption == 1 && (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT))//GayNghien..2.4 //HuongThan..2.5
                    {
                        value += !String.IsNullOrEmpty(medi.NUMBER_OF_USE_IN_DAY) ? "- " + String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_OF_USE_IN_DAY).Trim()) + " -" : "";
                    }
                    else
                    {
                        value += !String.IsNullOrEmpty(medi.NUMBER_OF_USE_IN_DAY) ? "- " + String.Format("({0})", medi.NUMBER_OF_USE_IN_DAY) + " -" : "";
                    }
                }
                else if (rdo._WorkPlaceSDO.UsedDayCountingOption == 6)
                {
                    if (rdo._WorkPlaceSDO.UsedDayCountingFormatOption == 1 && (medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN || medi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT))//GayNghien..2.4 //HuongThan..2.5
                    {
                        value += (medi.NUMBER_ACTIVE_INGR ?? 0) > 0 ? "- " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", Inventec.Common.String.Convert.CurrencyToVneseString(medi.NUMBER_ACTIVE_INGR.ToString()).Trim()) + "", FontStyle.Bold) : "";
                    }
                    else
                    {
                        value += (medi.NUMBER_ACTIVE_INGR ?? 0) > 0 ? "- " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(String.Format("({0})", medi.NUMBER_ACTIVE_INGR) + "", FontStyle.Bold) : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);

                return "";
            }
            return value;
        }

        string GetPriviousTrackingDate(int index)
        {
            string value = "";
            try
            {
                if (mps000062ADOExt != null && mps000062ADOExt.Mps000062ADOs != null && mps000062ADOExt.Mps000062ADOs.Count > index)
                    value = mps000062ADOExt.Mps000062ADOs[index].TRACKING_DATE_STR;
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

                mps000062ADOExt.Mps000062ADOs = (from m in this._Mps000062ADOs select new Mps000062ExtADO(m)).ToList();
                mps000062ADOExt.WorkPlaceSDO = rdo._WorkPlaceSDO;
                mps000062ADOExt.SingleValueDictionary = singleValueDictionary;

                foreach (var item in mps000062ADOExt.Mps000062ADOs)
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
                    item.ExpMestMetyReqADOs = this._ExpMestMetyReqADOCommons.Where(o => o.TRACKING_ID == item.ID).ToList();

                    item.MedicinesInfusion = this._ExpMestMetyReqADOCommonsMix.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.MedicinesDuTru = this._ExpMestMetyReqADOCommonsDuTru.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.MedicinesTHDT = this._ExpMestMetyReqADOCommonsTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID).ToList();
                    item.MediInfusionDutru = this._MediInfusionDutru.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.MediInfusionTHDT = this._MediInfusionTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID).ToList();
                    item.ServiceReq = this.lstServiceReqs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ServiceReqDuTru = this._ServiceReqDuTrus.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ServiceReqTHDT = this._ServiceReqTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID).ToList();
                    item.MaterialsDuTru = this._ExpMestMatyReqADOsDuTru.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.MaterialsTHDT = this._ExpMestMatyReqADOsTHDT.Where(o => o.USED_FOR_TRACKING_ID == item.ID).ToList();
                    item.ServiceCLSOrder = this._ServiceCLSOrders.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ServiceCLSOrder = this._ServiceCLSSplitXNs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.ImpMestBlood = this._ImpMestBloodADOs.Where(o => o.TRACKING_ID == item.ID).ToList();
                    item.Ration = this._SereServRationADO.Where(o => o.TRACKING_ID == item.ID).ToList();
                }
                success = templaterExportStore.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                success = success && barCodeTag.ProcessData(templaterExportStore, dicImage);
                success = success && singleTag.ProcessData(templaterExportStore, singleValueDictionary);
                //List<Mps000062ADOExt> mps000062ADOExts = new List<Mps000062ADOExt>();
                //mps000062ADOExts.Add(mps000062ADOExt);
                success = success && objectTag.AddObjectData(templaterExportStore, mps000062ADOExt.Mps000062ADOs);
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
                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.EXECUTE_TIME_DHST, this._EXECUE_TIME_DHST));
                if (_SingleKeyTamThans != null && _SingleKeyTamThans.Count > 0)
                {
                    for (int i = 0; i < _SingleKeyTamThans.Count; i++)
                    {
                        if (i == 4)
                            break;
                        switch (i)
                        {
                            case 0:
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.NGAY_1, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.Y_LENH_1, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 1:
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.NGAY_2, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.Y_LENH_2, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 2:
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.NGAY_3, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.Y_LENH_3, _SingleKeyTamThans[i].Y_LENH));
                                break;
                            case 3:
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.NGAY_4, _SingleKeyTamThans[i].TRACKING_DATE_STR));
                                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.Y_LENH_4, _SingleKeyTamThans[i].Y_LENH));
                                break;
                        }
                    }
                }
                if (rdo._Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo._Treatment.TDL_PATIENT_DOB)));

                    if (this._Mps000062ADOs != null && this._Mps000062ADOs.Count > 0)
                    {
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.AGE_TRACKING, AgeUtil.CalculateFullAge(rdo._Treatment.TDL_PATIENT_DOB, this._Mps000062ADOs.FirstOrDefault().TRACKING_TIME)));
                    }
                }
                if (rdo._Trackings != null && rdo._Trackings.Count > 0)
                {
                    var dataTracking = rdo._Trackings.FirstOrDefault();
                    var TrackingLast = rdo._Trackings.OrderByDescending(o => o.TRACKING_TIME).FirstOrDefault();

                    if (rdo._Trackings.Count == 1)
                    {
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_CODE_BY_TRACKING, dataTracking.ICD_CODE));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_NAME_BY_TRACKING, dataTracking.ICD_NAME));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_SUB_CODE_BY_TRACKING, dataTracking.ICD_SUB_CODE));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_TEXT_BY_TRACKING, dataTracking.ICD_TEXT));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_DIFF_CODE, dataTracking.ICD_DIFF_CODE));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_DIFF_TEXT, dataTracking.ICD_DIFF_TEXT));
                    }
                    else
                    {
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_CODE_BY_TRACKING, TrackingLast.ICD_CODE));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_NAME_BY_TRACKING, TrackingLast.ICD_NAME));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_SUB_CODE_BY_TRACKING, TrackingLast.ICD_SUB_CODE));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_TEXT_BY_TRACKING, TrackingLast.ICD_TEXT));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_DIFF_CODE, dataTracking.ICD_DIFF_CODE));
                        SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ICD_DIFF_TEXT, dataTracking.ICD_DIFF_TEXT));
                    }
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.DEPARTMENT_CODE, TrackingLast.DEPARTMENT_CODE));
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.DEPARTMENT_NAME, TrackingLast.DEPARTMENT_NAME));
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ROOM_CODE, TrackingLast.ROOM_CODE));
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ROOM_NAME, TrackingLast.ROOM_NAME));

                    long? OrderSheet = dataTracking.SHEET_ORDER;
                    for (int i = 0; i < rdo._Trackings.Count; i++)
                    {
                        //long OrderSheet = rdo._Trackings.Min(o => o.SHEET_ORDER.Value) > 0 ? rdo._Trackings.Min(o => o.SHEET_ORDER.Value): 1;

                        if (rdo._Trackings[i].SHEET_ORDER < dataTracking.SHEET_ORDER)
                        {
                            OrderSheet = rdo._Trackings[i].SHEET_ORDER;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Warn("OrderSheet: " + OrderSheet);
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.ORDER_SHEET, OrderSheet == null ? 1 : OrderSheet.Value));
                }
                if (rdo._TreatmentBedRooms != null && rdo._TreatmentBedRooms.Count() > 0)
                {
                    string bedCodeStr = "";
                    string bedNameStr = "";
                    string bedRoomCodeStr = "";
                    string bedRoomNameStr = "";
                    if (rdo._TreatmentBedRooms.Count() == 1)
                    {
                        var treatmentBedRoom = rdo._TreatmentBedRooms.First();
                        bedCodeStr = treatmentBedRoom.BED_CODE;
                        bedNameStr = treatmentBedRoom.BED_NAME;
                        bedRoomCodeStr = treatmentBedRoom.BED_ROOM_CODE;
                        bedRoomNameStr = treatmentBedRoom.BED_ROOM_NAME;
                    }
                    else
                    {
                        var _TreatmentBedRoomsDistinct = rdo._TreatmentBedRooms.Distinct().ToList();
                        foreach (var item in _TreatmentBedRoomsDistinct)
                        {
                            bedCodeStr += !String.IsNullOrEmpty(item.BED_CODE) ? (item.BED_CODE + ";") : "";
                            bedNameStr += !String.IsNullOrEmpty(item.BED_NAME) ? (item.BED_NAME + ";") : "";
                            bedRoomCodeStr += !String.IsNullOrEmpty(item.BED_ROOM_CODE) ? (item.BED_ROOM_CODE + ";") : "";
                            bedRoomNameStr += !String.IsNullOrEmpty(item.BED_ROOM_NAME) ? (item.BED_ROOM_NAME + ";") : "";
                        }
                        if (bedCodeStr.Length > 0)
                            bedCodeStr = bedCodeStr.Remove(bedCodeStr.Length - 1, 1);
                        if (bedNameStr.Length > 0)
                            bedNameStr = bedNameStr.Remove(bedNameStr.Length - 1, 1);
                        if (bedRoomCodeStr.Length > 0)
                            bedRoomCodeStr = bedRoomCodeStr.Remove(bedRoomCodeStr.Length - 1, 1);
                        if (bedRoomNameStr.Length > 0)
                            bedRoomNameStr = bedRoomNameStr.Remove(bedRoomNameStr.Length - 1, 1);
                    }
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.BED_CODE, bedCodeStr));
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.BED_NAME, bedNameStr));
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.BED_ROOM_CODE, bedRoomCodeStr));
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.BED_ROOM_NAME, bedRoomNameStr));
                }
                if (rdo.PatientTypeAlter != null)
                    SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.HEIN_CARD_ADDRESS, rdo.PatientTypeAlter.ADDRESS));
                SetSingleKey(new KeyValue(Mps000062ExtendSingleKey.PHONE, ""));
                AddObjectKeyIntoListkey<Mps000062SingleKey>(rdo._WorkPlaceSDO, false);
                AddObjectKeyIntoListkey<HIS_TREATMENT>(rdo._Treatment);
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
                log = "Mã điều trị: " + rdo._Treatment.TREATMENT_CODE;
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
                if (rdo != null && rdo._Trackings != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo._Treatment.TREATMENT_CODE;
                    List<string> trackingIds = new List<string>();
                    foreach (var item in rdo._Trackings.Select(s => s.ID).Distinct().ToList())
                    {
                        trackingIds.Add("HIS_TRACKING:" + item);
                    }

                    string trackingId = string.Join(",", trackingIds);
                    result = String.Format("{0} {1} {2}", printTypeCode, treatmentCode, trackingId);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }


        internal void SetImageKey()
        {
            try
            {
                bool isBhytAndAvtNull = true;
                if (rdo._Treatment != null && !String.IsNullOrEmpty(rdo._Treatment.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000062ExtendSingleKey.IMG_AVATAR, rdo._Treatment.TDL_PATIENT_AVATAR_URL);
                    isBhytAndAvtNull = false;
                }

                if (isBhytAndAvtNull)
                {
                    SetSingleKey(Mps000062ExtendSingleKey.AVT_AND_BHYT_NULL, "1");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void DataMedicine_Merge(List<ExpMestMetyReqADO> Data, List<ExpMestMetyReqADO> Result)
        {
            if (Data != null && Data.Count > 0)
            {
                var dataGroupsIntruction_Merge = Data.OrderBy(o => o.INTRUCTION_DATE).GroupBy(o => o.INTRUCTION_DATE).Select(o => o.ToList()).ToList();

                foreach (var itemIn in dataGroupsIntruction_Merge)
                {
                    if (rdo._WorkPlaceSDO.IsOrderByType == 2)
                    {
                        List<ExpMestMetyReqADO> _dataNews1 = new List<ExpMestMetyReqADO>();
                        var itemOrder = itemIn.OrderByDescending(p => p.MEDICINE_GROUP_NUM_ORDER).ToList();
                        var dataGroups = itemOrder.GroupBy(p => p.MEDICINE_GROUP_NUM_ORDER).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in dataGroups)
                        {
                            var dtGroups = itemGr.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(n => n.NUMBER_H_N).ThenBy(t => t.USING_COUNT_NUMBER).ToList();
                            _dataNews1.AddRange(dtGroups);
                        }

                        Result.AddRange(_dataNews1);
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 1)
                    {
                        Result.AddRange(itemIn.OrderByDescending(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(p => p.USING_COUNT_NUMBER).ToList());

                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 0)
                    {
                        Result.AddRange(itemIn.OrderBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(q => q.ID).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 3)
                    {
                        Result.AddRange(itemIn.OrderBy(p => p.NUM_ORDER_BY_USE_FORM).ThenBy(m => m.TDL_SERVICE_REQ_ID).ThenBy(o => o.NUM_ORDER).ThenBy(p => p.NUMBER_H_N).ThenBy(n => n.USING_COUNT_NUMBER).ToList());
                    }
                    else if (rdo._WorkPlaceSDO.IsOrderByType == 4)
                    {
                        Result.AddRange(ProcessSortListExpMestMetyReq(itemIn));
                    }
                }
            }
        }
    }
    class MedicineDongY
    {
        public MedicineDongY() { }
        public long ServiceReqId { get; set; }
        public string Content { get; set; }
    }

    class CalculateMergerData : TFlexCelUserFunction
    {
        internal List<Mps000062ADO> _Mps000062ADOs;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                int index = int.Parse(parameters[0].ToString());

                if (_Mps000062ADOs != null && _Mps000062ADOs.Count > 0 && index < _Mps000062ADOs.Count - 1)
                {
                    if (_Mps000062ADOs[index].TRACKING_DATE_STR == _Mps000062ADOs[index + 1].TRACKING_DATE_STR)
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
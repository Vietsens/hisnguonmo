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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintPublicMedicines.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HIS.Desktop.Plugins.Library.PrintPublicMedicines
{
    class Mps000303
    {
        int dayCountData = 0;
        internal Mps000303(long? roomId)
        {
            this.roomId = roomId;
        }
        long _treatmentId;
        long? roomId;
        public Mps000303(string printTypeCode, string fileName, long _treatmentId, bool printNow, ref bool result, long? roomId)
        {
            try
            {
                //TODO GetData
                this._treatmentId = _treatmentId;
                this.roomId = roomId;
                WaitingManager.Show();

                GetAllSereServV2();

                AddDataToDatetime();

                #region ----- Groups -----
                List<MPS.Processor.Mps000303.PDO.Mps000303BySereServ> _Mps000303ByServiceGroups = new List<MPS.Processor.Mps000303.PDO.Mps000303BySereServ>();
                if (this._Mps000303BySereServ != null && this._Mps000303BySereServ.Count > 0)
                {
                    var rsGroup = this._Mps000303BySereServ.GroupBy(p => new { p.SERVICE_ID, p.PRICE, p.Service_Type_Id, p.CONCENTRA, p.TYPE_ID, p.TDL_REQUEST_DEPARTMENT_ID }).ToList();
                    foreach (var itemGroup in rsGroup)
                    {
                        MPS.Processor.Mps000303.PDO.Mps000303BySereServ ado = new MPS.Processor.Mps000303.PDO.Mps000303BySereServ();
                        ado.Service_Type_Id = itemGroup.FirstOrDefault().Service_Type_Id;
                        ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                        ado.TDL_SERVICE_NAME = itemGroup.FirstOrDefault().TDL_SERVICE_NAME;
                        ado.PRICE = itemGroup.FirstOrDefault().PRICE;
                        ado.SERVICE_UNIT_NAME = itemGroup.FirstOrDefault().SERVICE_UNIT_NAME;
                        ado.CONCENTRA = itemGroup.FirstOrDefault().CONCENTRA;
                        ado.TYPE_ID = itemGroup.FirstOrDefault().TYPE_ID;
                        ado.TDL_REQUEST_DEPARTMENT_ID = itemGroup.FirstOrDefault().TDL_REQUEST_DEPARTMENT_ID;
                        ado.REQUEST_DEPARTMENT_NAME = itemGroup.FirstOrDefault().REQUEST_DEPARTMENT_NAME;
                        decimal day1 = 0;
                        decimal day2 = 0;
                        decimal day3 = 0;
                        decimal day4 = 0;
                        decimal day5 = 0;
                        decimal day6 = 0;
                        decimal day7 = 0;
                        decimal day8 = 0;
                        decimal day9 = 0;
                        decimal day10 = 0;
                        decimal day11 = 0;
                        decimal day12 = 0;
                        decimal day13 = 0;
                        decimal day14 = 0;
                        decimal day15 = 0;
                        decimal day16 = 0;
                        decimal day17 = 0;
                        decimal day18 = 0;
                        decimal day19 = 0;
                        decimal day20 = 0;
                        decimal day21 = 0;
                        decimal day22 = 0;
                        decimal day23 = 0;
                        decimal day24 = 0;
                        decimal day25 = 0;
                        decimal day26 = 0;
                        decimal day27 = 0;
                        decimal day28 = 0;
                        decimal day29 = 0;
                        decimal day30 = 0;
                        decimal day31 = 0;
                        decimal day32 = 0;
                        decimal day33 = 0;
                        decimal day34 = 0;
                        decimal day35 = 0;
                        decimal day36 = 0;
                        decimal day37 = 0;
                        decimal day38 = 0;
                        decimal day39 = 0;
                        decimal day40 = 0;
                        decimal day41 = 0;
                        decimal day42 = 0;
                        decimal day43 = 0;
                        decimal day44 = 0;
                        decimal day45 = 0;
                        decimal day46 = 0;
                        decimal day47 = 0;
                        decimal day48 = 0;
                        decimal day49 = 0;
                        decimal day50 = 0;
                        decimal day51 = 0;
                        decimal day52 = 0;
                        decimal day53 = 0;
                        decimal day54 = 0;
                        decimal day55 = 0;
                        decimal day56 = 0;
                        decimal day57 = 0;
                        decimal day58 = 0;
                        decimal day59 = 0;
                        decimal day60 = 0;

                        foreach (var item in itemGroup)
                        {
                            day1 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day1);
                            day2 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day2);
                            day3 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day3);
                            day4 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day4);
                            day5 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day5);
                            day6 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day6);
                            day7 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day7);
                            day8 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day8);
                            day9 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day9);
                            day10 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day10);
                            day11 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day11);
                            day12 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day12);
                            day13 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day13);
                            day14 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day14);
                            day15 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day15);
                            day16 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day16);
                            day17 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day17);
                            day18 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day18);
                            day19 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day19);
                            day20 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day20);
                            day21 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day21);
                            day22 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day22);
                            day23 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day23);
                            day24 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day24);
                            day25 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day25);
                            day26 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day26);
                            day27 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day27);
                            day28 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day28);
                            day29 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day29);
                            day30 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day30);
                            day31 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day31);
                            day32 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day32);
                            day33 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day33);
                            day34 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day34);
                            day35 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day35);
                            day36 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day36);
                            day37 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day37);
                            day38 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day38);
                            day39 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day39);
                            day40 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day40);
                            day41 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day41);
                            day42 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day42);
                            day43 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day43);
                            day44 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day44);
                            day45 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day45);
                            day46 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day46);
                            day47 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day47);
                            day48 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day48);
                            day49 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day49);
                            day50 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day50);
                            day51 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day51);
                            day52 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day52);
                            day53 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day53);
                            day54 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day54);
                            day55 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day55);
                            day56 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day56);
                            day57 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day57);
                            day58 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day58);
                            day59 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day59);
                            day60 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day60);
                        }
                        if (day1 > 0)
                            ado.Day1 = day1 + "";
                        if (day2 > 0)
                            ado.Day2 = day2 + "";
                        if (day3 > 0)
                            ado.Day3 = day3 + "";
                        if (day4 > 0)
                            ado.Day4 = day4 + "";
                        if (day5 > 0)
                            ado.Day5 = day5 + "";
                        if (day6 > 0)
                            ado.Day6 = day6 + "";
                        if (day7 > 0)
                            ado.Day7 = day7 + "";
                        if (day8 > 0)
                            ado.Day8 = day8 + "";
                        if (day9 > 0)
                            ado.Day9 = day9 + "";
                        if (day10 > 0)
                            ado.Day10 = day10 + "";
                        if (day11 > 0)
                            ado.Day11 = day11 + "";
                        if (day12 > 0)
                            ado.Day12 = day12 + "";
                        if (day13 > 0)
                            ado.Day13 = day13 + "";
                        if (day14 > 0)
                            ado.Day14 = day14 + "";
                        if (day15 > 0)
                            ado.Day15 = day15 + "";
                        if (day16 > 0)
                            ado.Day16 = day16 + "";
                        if (day17 > 0)
                            ado.Day17 = day17 + "";
                        if (day18 > 0)
                            ado.Day18 = day18 + "";
                        if (day19 > 0)
                            ado.Day19 = day19 + "";
                        if (day20 > 0)
                            ado.Day20 = day20 + "";
                        if (day21 > 0)
                            ado.Day21 = day21 + "";
                        if (day22 > 0)
                            ado.Day22 = day22 + "";
                        if (day23 > 0)
                            ado.Day23 = day23 + "";
                        if (day24 > 0)
                            ado.Day24 = day24 + "";
                        if (day25 > 0)
                            ado.Day25 = day25 + "";
                        if (day26 > 0)
                            ado.Day26 = day26 + "";
                        if (day27 > 0)
                            ado.Day27 = day27 + "";
                        if (day28 > 0)
                            ado.Day28 = day28 + "";
                        if (day29 > 0)
                            ado.Day29 = day29 + "";
                        if (day30 > 0)
                            ado.Day30 = day30 + "";
                        if (day31 > 0)
                            ado.Day31 = day31 + "";
                        if (day32 > 0)
                            ado.Day32 = day32 + "";
                        if (day33 > 0)
                            ado.Day33 = day33 + "";
                        if (day34 > 0)
                            ado.Day34 = day34 + "";
                        if (day35 > 0)
                            ado.Day35 = day35 + "";
                        if (day36 > 0)
                            ado.Day36 = day36 + "";
                        if (day37 > 0)
                            ado.Day37 = day37 + "";
                        if (day38 > 0)
                            ado.Day38 = day38 + "";
                        if (day39 > 0)
                            ado.Day39 = day39 + "";
                        if (day40 > 0)
                            ado.Day40 = day40 + "";
                        if (day51 > 0)
                            ado.Day51 = day51 + "";
                        if (day52 > 0)
                            ado.Day52 = day52 + "";
                        if (day53 > 0)
                            ado.Day53 = day53 + "";
                        if (day54 > 0)
                            ado.Day54 = day54 + "";
                        if (day55 > 0)
                            ado.Day55 = day55 + "";
                        if (day56 > 0)
                            ado.Day56 = day56 + "";
                        if (day57 > 0)
                            ado.Day57 = day57 + "";
                        if (day58 > 0)
                            ado.Day58 = day58 + "";
                        if (day59 > 0)
                            ado.Day59 = day59 + "";
                        if (day60 > 0)
                            ado.Day60 = day60 + "";
                        _Mps000303ByServiceGroups.Add(ado);
                    }
                }
                #endregion

                if (_Mps000303ByServiceGroups != null && _Mps000303ByServiceGroups.Count > 0)
                    _Mps000303ByServiceGroups = _Mps000303ByServiceGroups.OrderBy(p => p.TDL_SERVICE_NAME).ToList();

                // Lấy thông tin bệnh nhân
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = _treatmentId;
                var _Treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, null).FirstOrDefault();

                MPS.Processor.Mps000303.PDO.SingleKeys __SingleKeys = new MPS.Processor.Mps000303.PDO.SingleKeys();

                //__SingleKeys.REQUEST_DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId) != null ? WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName : null;
                __SingleKeys.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                __SingleKeys.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();


                EmrDataStore.treatmentCode = _Treatment != null ? _Treatment.TREATMENT_CODE : "";
                MPS.Processor.Mps000303.PDO.Mps000303PDO Mps000303RDO = new MPS.Processor.Mps000303.PDO.Mps000303PDO(
                    _Treatment,
                    this._Mps000303ADOs,
                    _Mps000303ByServiceGroups,
                    __SingleKeys
                                );
                WaitingManager.Hide();

                Print.PrintData(printTypeCode, fileName, Mps000303RDO, printNow, ref result, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool GetAllSereServV2()
        {
            bool result = true;
            try
            {
                this._Datas = new List<Service_NT_ADO>();
                dicExpMest = new Dictionary<long, HIS_EXP_MEST>();
                dicServiceReq = new Dictionary<long, HIS_SERVICE_REQ>();
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                //1.Get ServiceReq là đơn nt và tt, theo khoa yc
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this._treatmentId;


                var _currentServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                    HisRequestUriStore.HIS_SERVICE_REQ_GET,
                    ApiConsumers.MosConsumer,
                    serviceReqFilter,
                    param);

                List<long> _expMestIds = new List<long>();
                List<long> _serviceReqId_T_VTs = new List<long>();
                List<long> _serviceReqId_SVs = new List<long>();
                if (_currentServiceReqs != null && _currentServiceReqs.Count > 0)
                {
                    foreach (var itemSer in _currentServiceReqs)
                    {
                        if (!dicServiceReq.ContainsKey(itemSer.ID))
                        {
                            dicServiceReq[itemSer.ID] = new HIS_SERVICE_REQ();
                            dicServiceReq[itemSer.ID] = itemSer;
                        }
                    }
                    _serviceReqId_T_VTs = _currentServiceReqs.Select(p => p.ID).ToList();
                    _serviceReqId_SVs = _currentServiceReqs.Where(p =>
                        p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                        && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                        && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                        && p.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                        ).Select(p => p.ID).ToList();

                    if (_serviceReqId_SVs != null && _serviceReqId_SVs.Count > 0)
                    {
                        MOS.Filter.HisSereServViewFilter _ssFiler = new HisSereServViewFilter();
                        _ssFiler.TREATMENT_ID = this._treatmentId;
                        _ssFiler.SERVICE_REQ_IDs = _serviceReqId_SVs;

                        var _SereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, _ssFiler, param);
                        if (_SereServs != null && _SereServs.Count > 0)
                        {
                            foreach (var item in _SereServs)
                            {
                                //Review
                                Service_NT_ADO ado = new Service_NT_ADO();
                                ado.SERVICE_NAME = item.TDL_SERVICE_NAME;
                                ado.INTRUCTION_DATE = item.TDL_INTRUCTION_DATE;
                                ado.SERVICE_TYPE_ID = item.TDL_SERVICE_TYPE_ID;
                                ado.SERVICE_ID = item.SERVICE_ID;
                                ado.PRICE = item.PRICE;
                                ado.AMOUNT = item.AMOUNT;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.REQUEST_DEPARTMENT_ID = item.TDL_REQUEST_DEPARTMENT_ID;
                                ado.REQUEST_DEPARTMENT_NAME = item.REQUEST_DEPARTMENT_NAME;

                                this._Datas.Add(ado);
                            }
                        }
                    }

                    MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.SERVICE_REQ_IDs = _serviceReqId_T_VTs;
                    var _currentExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                    if (_currentExpMests != null && _currentExpMests.Count > 0)
                    {
                        _expMestIds = _currentExpMests.Select(p => p.ID).ToList();
                        foreach (var item in _currentExpMests)
                        {
                            if (!dicExpMest.ContainsKey(item.ID))
                            {
                                dicExpMest[item.ID] = new HIS_EXP_MEST();
                                dicExpMest[item.ID] = item;
                            }
                        }
                    }
                }
                else
                {
                    WaitingManager.Hide();

                    result = false;
                    return result;
                }

                CreateThreadLoadData(_expMestIds);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
                result = false;
            }
            return result;
        }

        Dictionary<long, HIS_EXP_MEST> dicExpMest = new Dictionary<long, HIS_EXP_MEST>();
        Dictionary<long, HIS_SERVICE_REQ> dicServiceReq = new Dictionary<long, HIS_SERVICE_REQ>();

        internal List<Service_NT_ADO> _Datas { get; set; }
        /// <summary>
        /// Trong Kho
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadData(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));
            Thread threadBlood = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataBloodNewThread));

            //threadMedicine.Priority = ThreadPriority.Highest;
            //threadMaterial.Priority = ThreadPriority.Normal;
            //threadBlood.Priority = ThreadPriority.Normal;
            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);
                threadBlood.Start(param);



                threadMedicine.Join();
                threadMaterial.Join();
                threadBlood.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
                threadBlood.Abort();
            }
        }

        private void LoadDataMedicineNewThread(object param)
        {
            try
            {
                LoadDataMedicine((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter expMestMediFilter = new HisExpMestMedicineViewFilter();
                expMestMediFilter.EXP_MEST_IDs = _expMestIds;


                var _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMediFilter, param);


                if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var expMestMedicineGroups = _ExpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.MEDICINE_ID, p.EXP_MEST_ID, p.PRICE, p.CONCENTRA }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in expMestMedicineGroups)
                    {
                        if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                        {
                            var _expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0];
                            if (_expMest != null)
                            {
                                decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                Service_NT_ADO expMedi = new Service_NT_ADO();
                                expMedi.DESCRIPTION = itemGroups[0].DESCRIPTION;
                                expMedi.PRICE = itemGroups[0].PRICE;
                                expMedi.SERVICE_CODE = itemGroups[0].MEDICINE_TYPE_CODE;
                                expMedi.SERVICE_ID = itemGroups[0].MEDICINE_TYPE_ID;
                                expMedi.SERVICE_NAME = itemGroups[0].MEDICINE_TYPE_NAME;
                                expMedi.SERVICE_UNIT_CODE = itemGroups[0].SERVICE_UNIT_CODE;
                                expMedi.SERVICE_UNIT_ID = itemGroups[0].SERVICE_UNIT_ID;
                                expMedi.SERVICE_UNIT_NAME = itemGroups[0].SERVICE_UNIT_NAME;
                                expMedi.INTRUCTION_DATE = _expMest.TDL_INTRUCTION_DATE ?? 0;
                                expMedi.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                expMedi.AMOUNT = _AMOUNT;
                                expMedi.CONCENTRA = itemGroups[0].CONCENTRA;
                                expMedi.REQUEST_DEPARTMENT_ID = _expMest.REQ_DEPARTMENT_ID;
                                var depar = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _expMest.REQ_DEPARTMENT_ID);
                                expMedi.REQUEST_DEPARTMENT_NAME = depar != null ? depar.DEPARTMENT_NAME : "";
                                _Datas.Add(expMedi);
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

        private void LoadDataMaterialNewThread(object param)
        {
            try
            {
                LoadDataMaterial((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialViewFilter expMestMateFilter = new HisExpMestMaterialViewFilter();
                expMestMateFilter.EXP_MEST_IDs = _expMestIds;

                var _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMateFilter, param);


                if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var expMestMaterialGroups = _ExpMestMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.MATERIAL_ID, p.EXP_MEST_ID, p.PRICE }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in expMestMaterialGroups)
                    {
                        if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                        {
                            var _expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0];
                            if (_expMest != null)
                            {
                                decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                Service_NT_ADO expMate = new Service_NT_ADO();
                                expMate.DESCRIPTION = itemGroups[0].DESCRIPTION;
                                expMate.PRICE = itemGroups[0].PRICE;
                                expMate.SERVICE_CODE = itemGroups[0].MATERIAL_TYPE_CODE;
                                expMate.SERVICE_ID = itemGroups[0].MATERIAL_TYPE_ID;
                                expMate.SERVICE_NAME = itemGroups[0].MATERIAL_TYPE_NAME;
                                expMate.SERVICE_UNIT_CODE = itemGroups[0].SERVICE_UNIT_CODE;
                                expMate.SERVICE_UNIT_ID = itemGroups[0].SERVICE_UNIT_ID;
                                expMate.SERVICE_UNIT_NAME = itemGroups[0].SERVICE_UNIT_NAME;
                                expMate.INTRUCTION_DATE = _expMest.TDL_INTRUCTION_DATE ?? 0;
                                expMate.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                                expMate.AMOUNT = _AMOUNT;
                                var concentra = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == itemGroups[0].MATERIAL_TYPE_ID);
                                if (concentra != null)
                                {
                                    expMate.CONCENTRA = concentra.CONCENTRA;
                                }
                                expMate.REQUEST_DEPARTMENT_ID = _expMest.REQ_DEPARTMENT_ID;
                                var depar = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _expMest.REQ_DEPARTMENT_ID);
                                expMate.REQUEST_DEPARTMENT_NAME = depar != null ? depar.DEPARTMENT_NAME : "";
                                _Datas.Add(expMate);
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

        private void LoadDataBloodNewThread(object param)
        {
            try
            {
                LoadDataBlood((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBlood(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_IDs = _expMestIds;

                var _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);

                if (_ExpMestBloods != null && _ExpMestBloods.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var expMestBllodGroups = _ExpMestBloods.GroupBy(p => new { p.BLOOD_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in expMestBllodGroups)
                    {
                        if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID))
                        {
                            var _expMest = this.dicExpMest[itemGroups[0].EXP_MEST_ID];
                            if (_expMest != null)
                            {
                                decimal _AMOUNT = itemGroups.Count;
                                Service_NT_ADO expMate = new Service_NT_ADO();
                                expMate.DESCRIPTION = itemGroups[0].DESCRIPTION;
                                expMate.PRICE = itemGroups[0].PRICE;
                                expMate.SERVICE_CODE = itemGroups[0].BLOOD_TYPE_CODE;
                                expMate.SERVICE_ID = itemGroups[0].BLOOD_TYPE_ID;
                                expMate.SERVICE_NAME = itemGroups[0].BLOOD_TYPE_NAME;
                                expMate.SERVICE_UNIT_CODE = itemGroups[0].SERVICE_UNIT_CODE;
                                expMate.SERVICE_UNIT_ID = itemGroups[0].SERVICE_UNIT_ID;
                                expMate.SERVICE_UNIT_NAME = itemGroups[0].SERVICE_UNIT_NAME;
                                expMate.INTRUCTION_DATE = _expMest.TDL_INTRUCTION_DATE ?? 0;
                                expMate.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                                expMate.AMOUNT = _AMOUNT;
                                expMate.REQUEST_DEPARTMENT_ID = _expMest.REQ_DEPARTMENT_ID;
                                var depar = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == _expMest.REQ_DEPARTMENT_ID);
                                expMate.REQUEST_DEPARTMENT_NAME = depar != null ? depar.DEPARTMENT_NAME : "";
                                _Datas.Add(expMate);
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

        internal List<MPS.Processor.Mps000303.PDO.Mps000303ADO> _Mps000303ADOs { get; set; }
        internal List<MPS.Processor.Mps000303.PDO.Mps000303BySereServ> _Mps000303BySereServ;

        private void AddDataToDatetime()
        {
            try
            {
                this._Mps000303ADOs = new List<MPS.Processor.Mps000303.PDO.Mps000303ADO>();
                this._Mps000303BySereServ = new List<MPS.Processor.Mps000303.PDO.Mps000303BySereServ>();
                if (this._Datas != null && this._Datas.Count > 0)
                {
                    List<long> distinctDates = this._Datas
                        .Select(o => o.INTRUCTION_DATE)
                        .Distinct().OrderBy(t => t).ToList();
                    var sereServGroups = this._Datas.GroupBy(p => new { p.SERVICE_ID, p.PRICE, p.SERVICE_TYPE_ID, p.CONCENTRA, p.INTRUCTION_DATE }).ToList();

                    this.dayCountData = distinctDates.Count;
                    int dayPageSize = (int)(Inventec.Common.Number.Convert.RoundUpValue(((double)this.dayCountData / (double)Config.congKhaiDichVu_DaySize), 0));

                    int start = 0;
                    int count = distinctDates.Count;
                    //int d = 0;
                    //while (count > 0)
                    //{
                    //    int limit = (count <= 10) ? count : 10;
                    //    var listSubs = distinctDates.Skip(start).Take(limit).ToList();

                    //    d++;

                    //    int index1 = 0;
                    //    #region ThuatToan
                    //    while (index1 < listSubs.Count)
                    //    {
                    //        MPS.Processor.Mps000303.PDO.Mps000303ADO sdo = new MPS.Processor.Mps000303.PDO.Mps000303ADO();
                    //        sdo.Day1 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day2 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day3 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day4 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day5 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day6 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day7 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day8 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day9 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";
                    //        sdo.Day10 = index1 < listSubs.Count ? TimeNumberToDateString(listSubs[index1++]) : "";

                    //        sdo.TYPE_ID = d;

                    //        foreach (var group in sereServGroups)
                    //        {
                    //            if (group.AMOUNT <= 0)
                    //                continue;
                    //            MPS.Processor.Mps000303.PDO.Mps000303BySereServ sereServPrint = new MPS.Processor.Mps000303.PDO.Mps000303BySereServ();
                    //            List<Service_NT_ADO> sereServs = new List<Service_NT_ADO>();
                    //            sereServs.Add(group);
                    //            //Review gán lại từng dữ liệu
                    //            sereServPrint.TDL_SERVICE_NAME = group.SERVICE_NAME;//Check
                    //            sereServPrint.Service_Type_Id = group.SERVICE_TYPE_ID;
                    //            sereServPrint.SERVICE_ID = group.SERVICE_ID;
                    //            sereServPrint.SERVICE_UNIT_NAME = group.SERVICE_UNIT_NAME;
                    //            sereServPrint.AMOUNT = group.AMOUNT;
                    //            sereServPrint.CONCENTRA = group.CONCENTRA;
                    //            sereServPrint.TDL_REQUEST_DEPARTMENT_ID = group.REQUEST_DEPARTMENT_ID;
                    //            sereServPrint.REQUEST_DEPARTMENT_NAME = group.REQUEST_DEPARTMENT_NAME;
                    //            sereServPrint.TYPE_ID = d;

                    //            var amount = sereServs[0].AMOUNT.ToString();
                    //            sereServPrint.Day1 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day1).Any() ? amount : "";
                    //            sereServPrint.Day2 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day2).Any() ? amount : "";
                    //            sereServPrint.Day3 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day3).Any() ? amount : "";
                    //            sereServPrint.Day4 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day4).Any() ? amount : "";
                    //            sereServPrint.Day5 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day5).Any() ? amount : "";
                    //            sereServPrint.Day6 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day6).Any() ? amount : "";
                    //            sereServPrint.Day7 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day7).Any() ? amount : "";
                    //            sereServPrint.Day8 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day8).Any() ? amount : "";
                    //            sereServPrint.Day9 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day9).Any() ? amount : "";
                    //            sereServPrint.Day10 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day10).Any() ? amount : "";

                    //            this._Mps000303BySereServ.Add(sereServPrint);
                    //        }
                    //    #endregion
                    //        // sdo.Mps000303BySereServADOs = this._Mps000303BySereServ;

                    //        this._Mps000303ADOs.Add(sdo);
                    //    }


                    //    start += 10;
                    //    count -= 10;
                    //}

                    int indexYear = 0;
                    int index = 0;
                    int d = 0;
                    for (int i = 1; i <= dayPageSize; i++)
                    {
                        index = 0;
                        indexYear = 0;
                        List<long> distinctDatesInPage = distinctDates.Skip((i - 1) * Config.congKhaiDichVu_DaySize).Take(Config.congKhaiDichVu_DaySize).ToList();
                        d++;
                        #region ThuatToan
                        //while (index < distinctDatesInPage.Count)
                        //{
                        MPS.Processor.Mps000303.PDO.Mps000303ADO sdo = new MPS.Processor.Mps000303.PDO.Mps000303ADO();
                        #region ---Day---
                        sdo.Day1 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day2 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day3 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day4 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day5 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day6 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day7 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day8 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day9 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day10 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day11 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day12 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day13 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day14 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day15 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day16 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day17 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day18 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day19 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day20 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day21 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day22 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day23 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day24 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day25 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day26 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day27 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day28 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day29 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day30 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day31 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day32 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day33 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day34 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day35 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day36 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day37 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day38 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day39 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day40 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day41 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day42 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day43 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day44 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day45 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day46 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day47 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day48 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day49 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day50 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day51 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day52 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day53 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day54 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day55 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day56 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day57 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day58 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day59 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.Day60 = index < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[index++]) : "";
                        sdo.TYPE_ID = d;
                        #endregion

                        #region ---Day and year---
                        sdo.DayAndYear1 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear2 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear3 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear4 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear5 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear6 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear7 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear8 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear9 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear10 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear11 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear12 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear13 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear14 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear15 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear16 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear17 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear18 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear19 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear20 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear21 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear22 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear23 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear24 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear25 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear26 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear27 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear28 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear29 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear30 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear31 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear32 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear33 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear34 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear35 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear36 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear37 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear38 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear39 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear40 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear41 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear42 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear43 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear44 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear45 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear46 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear47 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear48 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear49 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear50 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear51 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear52 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear53 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear54 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear55 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear56 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear57 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear58 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear59 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        sdo.DayAndYear60 = indexYear < distinctDatesInPage.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDatesInPage[indexYear++]) : "";
                        #endregion
                        sdo.Mps000303BySereServADOs = new List<MPS.Processor.Mps000303.PDO.Mps000303BySereServ>();
                        foreach (var group in sereServGroups)
                        {
                            if (group.ToList().Where(o => distinctDatesInPage.Contains(o.INTRUCTION_DATE)).Sum(o => o.AMOUNT) <= 0)
                                continue;

                            MPS.Processor.Mps000303.PDO.Mps000303BySereServ sereServPrint = new MPS.Processor.Mps000303.PDO.Mps000303BySereServ();
                            List<Service_NT_ADO> sereServs = new List<Service_NT_ADO>();
                            sereServs.AddRange(group.ToList());
                            //Review gán lại từng dữ liệu
                            sereServPrint.TDL_SERVICE_NAME = group.First().SERVICE_NAME;//Check
                            sereServPrint.Service_Type_Id = group.First().SERVICE_TYPE_ID;
                            sereServPrint.SERVICE_ID = group.First().SERVICE_ID;
                            sereServPrint.SERVICE_UNIT_NAME = group.First().SERVICE_UNIT_NAME;
                            sereServPrint.TDL_REQUEST_DEPARTMENT_ID = group.First().REQUEST_DEPARTMENT_ID;
                            sereServPrint.REQUEST_DEPARTMENT_NAME = group.First().REQUEST_DEPARTMENT_NAME;

                            sereServPrint.AMOUNT = group.ToList().Where(o => distinctDatesInPage.Contains(o.INTRUCTION_DATE)).Sum(o => o.AMOUNT);
                            sereServPrint.AMOUNT_STRING = sereServPrint.AMOUNT + "";
                            sereServPrint.CONCENTRA = group.First().CONCENTRA;
                            string amount = sereServPrint.AMOUNT + "";
                            sereServPrint.Day1 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day1).Any() ? amount : "";
                            sereServPrint.Day2 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day2).Any() ? amount : "";
                            sereServPrint.Day3 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day3).Any() ? amount : "";
                            sereServPrint.Day4 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day4).Any() ? amount : "";
                            sereServPrint.Day5 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day5).Any() ? amount : "";
                            sereServPrint.Day6 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day6).Any() ? amount : "";
                            sereServPrint.Day7 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day7).Any() ? amount : "";
                            sereServPrint.Day8 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day8).Any() ? amount : "";
                            sereServPrint.Day9 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day9).Any() ? amount : "";
                            sereServPrint.Day10 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day10).Any() ? amount : "";
                            sereServPrint.Day11 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day11).Any() ? amount : "";
                            sereServPrint.Day12 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day12).Any() ? amount : "";
                            sereServPrint.Day13 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day13).Any() ? amount : "";
                            sereServPrint.Day14 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day14).Any() ? amount : "";
                            sereServPrint.Day15 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day15).Any() ? amount : "";
                            sereServPrint.Day16 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day16).Any() ? amount : "";
                            sereServPrint.Day17 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day17).Any() ? amount : "";
                            sereServPrint.Day18 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day18).Any() ? amount : "";
                            sereServPrint.Day19 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day19).Any() ? amount : "";
                            sereServPrint.Day20 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day20).Any() ? amount : "";
                            sereServPrint.Day21 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day21).Any() ? amount : "";
                            sereServPrint.Day22 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day22).Any() ? amount : "";
                            sereServPrint.Day23 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day23).Any() ? amount : "";
                            sereServPrint.Day24 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day24).Any() ? amount : "";
                            sereServPrint.Day25 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day25).Any() ? amount : "";
                            sereServPrint.Day26 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day26).Any() ? amount : "";
                            sereServPrint.Day27 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day27).Any() ? amount : "";
                            sereServPrint.Day28 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day28).Any() ? amount : "";
                            sereServPrint.Day29 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day29).Any() ? amount : "";
                            sereServPrint.Day30 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day30).Any() ? amount : "";
                            sereServPrint.Day31 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day31).Any() ? amount : "";
                            sereServPrint.Day32 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day32).Any() ? amount : "";
                            sereServPrint.Day33 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day33).Any() ? amount : "";
                            sereServPrint.Day34 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day34).Any() ? amount : "";
                            sereServPrint.Day35 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day35).Any() ? amount : "";
                            sereServPrint.Day36 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day36).Any() ? amount : "";
                            sereServPrint.Day37 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day37).Any() ? amount : "";
                            sereServPrint.Day38 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day38).Any() ? amount : "";
                            sereServPrint.Day39 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day39).Any() ? amount : "";
                            sereServPrint.Day40 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day40).Any() ? amount : "";
                            sereServPrint.Day41 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day41).Any() ? amount : "";
                            sereServPrint.Day42 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day32).Any() ? amount : "";
                            sereServPrint.Day43 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day43).Any() ? amount : "";
                            sereServPrint.Day44 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day44).Any() ? amount : "";
                            sereServPrint.Day45 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day45).Any() ? amount : "";
                            sereServPrint.Day46 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day46).Any() ? amount : "";
                            sereServPrint.Day47 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day47).Any() ? amount : "";
                            sereServPrint.Day48 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day48).Any() ? amount : "";
                            sereServPrint.Day49 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day49).Any() ? amount : "";
                            sereServPrint.Day50 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day50).Any() ? amount : "";
                            sereServPrint.Day51 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day51).Any() ? amount : "";
                            sereServPrint.Day52 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day52).Any() ? amount : "";
                            sereServPrint.Day53 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day53).Any() ? amount : "";
                            sereServPrint.Day54 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day54).Any() ? amount : "";
                            sereServPrint.Day55 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day55).Any() ? amount : "";
                            sereServPrint.Day56 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day56).Any() ? amount : "";
                            sereServPrint.Day57 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day57).Any() ? amount : "";
                            sereServPrint.Day58 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day58).Any() ? amount : "";
                            sereServPrint.Day59 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day59).Any() ? amount : "";
                            sereServPrint.Day60 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day60).Any() ? amount : "";
                            sereServPrint.TYPE_ID = d;
                            sdo.Mps000303BySereServADOs.Add(sereServPrint);
                            this._Mps000303BySereServ.Add(sereServPrint);
                        }

                        sdo.Mps000303BySereServADOs = sdo.Mps000303BySereServADOs.OrderBy(p => p.TDL_SERVICE_NAME).ToList();

                        this._Mps000303ADOs.Add(sdo);
                        //}
                        #endregion
                    }


                    //int index = 0;
                    //#region ThuatToan
                    //while (index < distinctDates.Count)
                    //{
                    //    MPS.Processor.Mps000303.PDO.Mps000303ADO sdo = new MPS.Processor.Mps000303.PDO.Mps000303ADO();
                    //    sdo.Day1 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day2 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day3 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day4 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day5 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day6 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day7 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day8 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day9 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";
                    //    sdo.Day10 = index < distinctDates.Count ? TimeNumberToDateString(distinctDates[index++]) : "";

                    //    foreach (var group in sereServGroups)
                    //    {
                    //        if (group.AMOUNT <= 0)
                    //            continue;
                    //        MPS.Processor.Mps000303.PDO.Mps000303BySereServ sereServPrint = new MPS.Processor.Mps000303.PDO.Mps000303BySereServ();
                    //        List<Service_NT_ADO> sereServs = new List<Service_NT_ADO>();
                    //        sereServs.Add(group);
                    //        //Review gán lại từng dữ liệu
                    //        sereServPrint.TDL_SERVICE_NAME = group.SERVICE_NAME;//Check
                    //        sereServPrint.Service_Type_Id = group.SERVICE_TYPE_ID;
                    //        sereServPrint.SERVICE_ID = group.SERVICE_ID;
                    //        sereServPrint.SERVICE_UNIT_NAME = group.SERVICE_UNIT_NAME;
                    //        sereServPrint.AMOUNT = group.AMOUNT;
                    //        sereServPrint.CONCENTRA = group.CONCENTRA;
                    //        sereServPrint.TDL_REQUEST_DEPARTMENT_ID = group.REQUEST_DEPARTMENT_ID;

                    //        var amount = sereServs[0].AMOUNT.ToString();
                    //        sereServPrint.Day1 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day1).Any() ? amount : "";
                    //        sereServPrint.Day2 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day2).Any() ? amount : "";
                    //        sereServPrint.Day3 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day3).Any() ? amount : "";
                    //        sereServPrint.Day4 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day4).Any() ? amount : "";
                    //        sereServPrint.Day5 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day5).Any() ? amount : "";
                    //        sereServPrint.Day6 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day6).Any() ? amount : "";
                    //        sereServPrint.Day7 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day7).Any() ? amount : "";
                    //        sereServPrint.Day8 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day8).Any() ? amount : "";
                    //        sereServPrint.Day9 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day9).Any() ? amount : "";
                    //        sereServPrint.Day10 = sereServs.Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == sdo.Day10).Any() ? amount : "";

                    //        this._Mps000303BySereServ.Add(sereServPrint);
                    //    }
                    //#endregion
                    //    sdo.Mps000303BySereServADOs = this._Mps000303BySereServ;

                    //    this._Mps000303ADOs.Add(sdo);
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string TimeNumberToDateString(long _Time)
        {
            string TimeString = "";
            try
            {
                TimeString = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Time).Substring(0, 5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return TimeString;
        }
    }
}

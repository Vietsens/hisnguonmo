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
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ContentSubclinical.Print
{
    class InKetQuaXetNghiem
    {
        //in gộp xét nghiệp sẽ tăng số lượng dịch vụ
        //cần xử lý để gộp file đủ 
        public InKetQuaXetNghiem(string printTypeCode, string fileName, HIS_SERVICE_REQ currentServiceReqPrint,
            bool printNow, ref bool result, long? roomId, bool isView,
            MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData,
            Action<string> cancelPrint)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InKetQuaXetNghiem().BEGIN!");
                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                long treatmentId = currentServiceReqPrint.TREATMENT_ID;

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilter.ID = currentServiceReqPrint.ID;
                var _ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                //Load Data Treatment
                HIS_TREATMENT _Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new MOS.Filter.HisPatientTypeAlterViewFilter();
                patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "DESC";
                patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param).FirstOrDefault();

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                if (patientTypeAlter != null)
                {
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);
                }

                List<object> obj = new List<object>();
                obj.Add(patientTypeAlter);
                obj.Add(_ServiceReq);
                obj.Add(_Treatment);

                MOS.Filter.HisSereServViewFilter SereServfilter = new MOS.Filter.HisSereServViewFilter();

                SereServfilter.SERVICE_REQ_ID = currentServiceReqPrint.ID;
                SereServfilter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var lstSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, SereServfilter, param);
                
                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                List<V_HIS_SERE_SERV_TEIN> sereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    foreach (var itemss in lstSereServ)
                    {
                        MPS.Processor.Mps000014.PDO.SereServNumOder sereServNumOder = new MPS.Processor.Mps000014.PDO.SereServNumOder(itemss, BackendDataWorker.Get<V_HIS_SERVICE>());
                        _SereServNumOders.Add(sereServNumOder);
                    }
                    _SereServNumOders = _SereServNumOders.OrderByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();

                    MOS.Filter.HisSereServTeinViewFilter sereSerTeinFilter = new MOS.Filter.HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = lstSereServ.Select(o => o.ID).ToList();
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    sereServTeins = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, sereSerTeinFilter, param);

                }

                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders2 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders4 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>> _SereServNumOderss = new Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>>();

                Inventec.Common.Logging.LogSystem.Debug("dữ liệu this._SereServNumOders2" + Inventec.Common.Logging.LogUtil.TraceData("", _SereServNumOders));

                foreach (var item in _SereServNumOders)
                {
                    if (item.ServiceParentId == null)
                    {
                        if (!_SereServNumOderss.ContainsKey(0))
                        {
                            _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[0].Add(item);
                    }
                    else
                    {
                        _SereServNumOders2.Add(item);
                    }
                }
                foreach (var item in _SereServNumOders2)
                {
                    var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);
                    if (parent.PARENT_ID == null)
                    {
                        if (!_SereServNumOderss.ContainsKey(parent.ID))
                        {
                            _SereServNumOderss[parent.ID] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[parent.ID].Add(item);
                    }
                    else
                    {
                        _SereServNumOders4.Add(item);
                    }
                    _SereServNumOders4.GroupBy(o => o.GrandParentID);

                }
                foreach (var item in _SereServNumOders4)
                {
                    if (!_SereServNumOderss.ContainsKey(item.GrandParentID.Value))
                    {
                        _SereServNumOderss[item.GrandParentID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    }
                    _SereServNumOderss[item.GrandParentID.Value].Add(item);
                }


                foreach (var item in _SereServNumOderss.Keys)
                {
                    Inventec.Common.Logging.LogSystem.Debug("dữ liệu _SereServNumOderss[item] " + Inventec.Common.Logging.LogUtil.TraceData("", _SereServNumOderss[item]));

                    MPS.Processor.Mps000014.PDO.Mps000014PDO mps000014RDO = new MPS.Processor.Mps000014.PDO.Mps000014PDO(
                        obj.ToArray(),
                        _SereServNumOderss[item],
                        sereServTeins,
                        ratio_text,
                        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                        _Treatment.TDL_PATIENT_GENDER_ID,
                        BackendDataWorker.Get<V_HIS_SERVICE>()
                        );

                    Print.PrintData(printTypeCode, fileName, mps000014RDO, printNow, ref result, roomId, isView, PreviewType, 0, savedData);

                    WaitingManager.Hide();
                    //MPS.ProcessorBase.Core.PrintData PrintData = null;
                    //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    //}
                    //else
                    //{
                    //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    //}

                    //Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_Treatment != null ? _Treatment.TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                    //Inventec.Common.Logging.LogSystem.Info(_Treatment.TREATMENT_CODE);
                    //PrintData.EmrInputADO = inputADO;

                    //result = MPS.MpsPrinter.Run(PrintData);
                }
                savedData(1, null);
                Inventec.Common.Logging.LogSystem.Debug("InKetQuaXetNghiem().BEGIN!");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

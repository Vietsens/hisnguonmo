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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintAggrExpMest
{
    class PrintMps000262
    {
        string printerName = "";
        List<V_HIS_TREATMENT> listTreatment;
        List<V_HIS_TREATMENT_BED_ROOM> listTreatmentBedRoom;
        List<V_HIS_EXP_MEST> ListExpMestPrint;
        List<V_HIS_EXP_MEST_MATERIAL> ListExpMestMaterial;
        List<V_HIS_EXP_MEST_MEDICINE> ListExpMestMedicine;
        V_HIS_EXP_MEST AggrExpMest;
        List<HIS_SERVICE_REQ> ListServiceReq;//is_home_pres- đơn mang về
        string PrintTypeCode;
        string FileName;
        bool Result;

        public PrintMps000262(string printTypeCode, string fileName, ref bool result, List<V_HIS_EXP_MEST> _listExpMestPrint, List<V_HIS_EXP_MEST_MATERIAL> _listExpMestMaterial, List<V_HIS_EXP_MEST_MEDICINE> _listExpMestMedicine)
        {
            try
            {
                if (_listExpMestPrint != null && _listExpMestPrint.Count > 0)
                {
                    this.ListExpMestPrint = _listExpMestPrint;
                    this.PrintTypeCode = printTypeCode;
                    this.FileName = fileName;

                    if (HisConfigKey.NOT_SHOW_EXPEND == "1")
                    {
                        this.ListExpMestMaterial = _listExpMestMaterial.Where(o => o.IS_EXPEND != 1).ToList();
                        this.ListExpMestMedicine = _listExpMestMedicine.Where(o => o.IS_EXPEND != 1).ToList();
                    }
                    else
                    {
                        this.ListExpMestMaterial = _listExpMestMaterial;
                        this.ListExpMestMedicine = _listExpMestMedicine;
                    }

                    CreateThreadGetData();

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    //hiển thị form chọn máy in
                    if (AppConfigKeys.CHE_DO_IN_CONG_KHAI_THUOC_BENH_NHAN == "2")
                    {
                        WaitingManager.Hide();
                        printerName = Inventec.Common.ChoosePrinter.ChoosePrinterProcessor.GetPrinter();
                    }

                    if (AppConfigKeys.CHE_DO_IN_CONG_KHAI_THUOC_BENH_NHAN == "1" || AppConfigKeys.CHE_DO_IN_CONG_KHAI_THUOC_BENH_NHAN == "2")
                    {
                        CreateTheadPrint();
                        result = true;
                    }
                    else
                    {
                        ProcessPrint();
                        result = Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadGetData()
        {
            System.Threading.Thread treatment = new System.Threading.Thread(ProcessGetTreatment);
            System.Threading.Thread treatmentBedRoom = new System.Threading.Thread(ProcessGetTreatmentBedRoom);
            System.Threading.Thread AggrExpMest = new System.Threading.Thread(ProcessGetAggrExpMest);
            System.Threading.Thread serviceReq = new System.Threading.Thread(ProcessGetServiceReq);
            try
            {
                serviceReq.Start();
                treatment.Start();
                treatmentBedRoom.Start();
                AggrExpMest.Start();

                treatment.Join();
                treatmentBedRoom.Join();
                AggrExpMest.Join();
                serviceReq.Join();
            }
            catch (Exception ex)
            {
                treatment.Abort();
                treatmentBedRoom.Abort();
                AggrExpMest.Abort();
                serviceReq.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetAggrExpMest()
        {
            try
            {
                //lỗi ListExpMestPrint sẽ không in đc
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter expFilter = new HisExpMestViewFilter();
                expFilter.ID = ListExpMestPrint.FirstOrDefault(o => o.AGGR_EXP_MEST_ID.HasValue).AGGR_EXP_MEST_ID;
                var lstexp = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, expFilter, param);
                if (lstexp != null && lstexp.Count > 0)
                {
                    AggrExpMest = lstexp.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetTreatmentBedRoom()
        {
            try
            {
                listTreatmentBedRoom = new List<V_HIS_TREATMENT_BED_ROOM>();
                List<long> treatmentIds = ListExpMestPrint.Select(s => s.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                int skip = 0;
                while (treatmentIds.Count - skip > 0)
                {
                    var listIds = treatmentIds.Skip(skip).Take(Base.ConfigData.MaxReqParam).ToList();
                    skip += Base.ConfigData.MaxReqParam;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentBedRoomViewFilter treatmentbedRoomFilter = new HisTreatmentBedRoomViewFilter();
                    treatmentbedRoomFilter.TREATMENT_IDs = listIds;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentbedRoomFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        listTreatmentBedRoom.AddRange(apiResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetTreatment()
        {
            try
            {
                listTreatment = new List<V_HIS_TREATMENT>();
                List<long> treatmentIds = ListExpMestPrint.Select(s => s.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                int skip = 0;
                while (treatmentIds.Count - skip > 0)
                {
                    var listIds = treatmentIds.Skip(skip).Take(Base.ConfigData.MaxReqParam).ToList();
                    skip += Base.ConfigData.MaxReqParam;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                    hisTreatmentFilter.IDs = listIds;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        listTreatment.AddRange(apiResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetServiceReq()
        {
            try
            {
                ListServiceReq = new List<HIS_SERVICE_REQ>();
                List<long> serviceReqIds = ListExpMestPrint.Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                int skip = 0;
                while (serviceReqIds.Count - skip > 0)
                {
                    var listIds = serviceReqIds.Skip(skip).Take(Base.ConfigData.MaxReqParam).ToList();
                    skip += Base.ConfigData.MaxReqParam;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();
                    serviceReqFilter.IDs = listIds;
                    var apiResult = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        ListServiceReq.AddRange(apiResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateTheadPrint()
        {
            System.Threading.Thread mps262 = new System.Threading.Thread(ProcessPrint);
            try
            {
                mps262.Start();
            }
            catch (Exception ex)
            {
                mps262.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint()
        {
            try
            {
                if (ListExpMestPrint != null && ListExpMestPrint.Count > 0 && AggrExpMest != null)
                {
                    var groupPatient = ListExpMestPrint.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL).ToList();

                    MPS.Processor.Mps000262.PDO.Mps000262PDO mps000262RDO = new MPS.Processor.Mps000262.PDO.Mps000262PDO(
                    ListExpMestMaterial,
                    ListExpMestMedicine,
                    groupPatient,
                    listTreatment,
                    listTreatmentBedRoom,
                    AggrExpMest,
                    ListServiceReq);

                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (AppConfigKeys.CHE_DO_IN_CONG_KHAI_THUOC_BENH_NHAN == "1" || AppConfigKeys.CHE_DO_IN_CONG_KHAI_THUOC_BENH_NHAN == "2")
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(PrintTypeCode, FileName, mps000262RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(PrintTypeCode, FileName, mps000262RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(PrintTypeCode, FileName, mps000262RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                    }

                    Result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

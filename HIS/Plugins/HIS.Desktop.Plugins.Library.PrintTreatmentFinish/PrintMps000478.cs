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

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000478
    {
        MPS.Processor.Mps000478.PDO.Mps000478PDO mps000478RDO { get; set; }

        public PrintMps000478(string printTypeCode, string fileName, ref bool result, bool _printNow, long treatmentId, long? roomId)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000478");
                if (treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    V_HIS_TREATMENT Treatment = GetTreatment_ByID(treatmentId);
                    var listExpMestMedicine_ByTreatment = GetListExpMestMedicine_ByTreatmentID(treatmentId);
                    List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                    if (listExpMestMedicine_ByTreatment != null)
                    {
                        listExpMestMedicine = listExpMestMedicine_ByTreatment.Where(o => o.IS_EXPEND != 1).ToList();
                    }
                    var listSereServ_ByTreatment = GetListSereServ_ByTreatmentID(treatmentId);
                    List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
                    if (listSereServ_ByTreatment != null)
                    {
                        listSereServ = listSereServ_ByTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                                                        || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT).ToList();
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Treatment", Treatment));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listExpMestMedicine", listExpMestMedicine));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstSereServ", listSereServ));
                    WaitingManager.Hide();
                    this.mps000478RDO = new MPS.Processor.Mps000478.PDO.Mps000478PDO
                        (
                        Treatment,
                        listSereServ,
                        listExpMestMedicine
                        );

                    result = Print.RunPrint(printTypeCode, fileName, mps000478RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
                    //if (_printNow
                    //    || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    //}
                    //else
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    //}
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public PrintMps000478(string printTypeCode, string fileName, ref bool result, MPS.ProcessorBase.PrintConfig.PreviewType? _previewType, long treatmentId, long? roomId)
        //{
        //    try
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000478");
        //        if (treatmentId > 0)
        //        {
        //            WaitingManager.Show();
        //            string printerName = "";
        //            if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
        //            {
        //                printerName = GlobalVariables.dicPrinter[printTypeCode];
        //            }

        //            V_HIS_TREATMENT Treatment = GetTreatment_ByID(treatmentId);
        //            var listExpMestMedicine_ByTreatment = GetListExpMestMedicine_ByTreatmentID(treatmentId);
        //            List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
        //            if (listExpMestMedicine_ByTreatment != null)
        //            {
        //                listExpMestMedicine = listExpMestMedicine_ByTreatment.Where(o => o.IS_EXPEND != 1).ToList();
        //            }
        //            var listSereServ_ByTreatment = GetListSereServ_ByTreatmentID(treatmentId);
        //            List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
        //            if (listSereServ_ByTreatment != null)
        //            {
        //                listSereServ = listSereServ_ByTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
        //                                                                || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT).ToList();
        //            }
        //            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Treatment", Treatment));
        //            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listExpMestMedicine", listExpMestMedicine));
        //            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstSereServ", listSereServ));
        //            WaitingManager.Hide();
        //            this.mps000478RDO = new MPS.Processor.Mps000478.PDO.Mps000478PDO
        //                (
        //                Treatment,
        //                listSereServ,
        //                listExpMestMedicine
        //                );

        //            result = Print.RunPrint(printTypeCode, fileName, mps000478RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _previewType, roomId);
        //            //if (_printNow
        //            //    || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
        //            //{
        //            //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
        //            //}
        //            //else
        //            //{
        //            //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void EventLogPrint()
        {
            try
            {
                string message = "In 'Tóm tắt y lệnh phẫu thuật thủ thuật và đơn thuốc'. Mã in : Mps000478" + "  TREATMENT_CODE: " + this.mps000478RDO.Treatment.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private V_HIS_TREATMENT GetTreatment_ByID(long treatmentId)
        {
            V_HIS_TREATMENT result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetListExpMestMedicine_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV> GetListSereServ_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_SERE_SERV> result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LibUsbDotNetF;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    public class PrintKiosk
    {
        V_HIS_PATIENT_TYPE_ALTER patyAlter { get; set; }
        V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        List<V_HIS_SERE_SERV> sereServs { get; set; }
        DelegateReturnSuccess success { get; set; }
        string printTypeCode { get; set; }
        string fileName { get; set; }
        HIS_TREATMENT treatmentPrint { get; set; }
        List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        bool checkPrintAgain { get; set; }
        List<HIS_CARD> listHisCard { get; set; }
        List<HIS_TRANS_REQ> TransReq { get; set; }


        public PrintKiosk(V_HIS_PATIENT_TYPE_ALTER patyAlter, V_HIS_SERVICE_REQ ServiceReqPrint, List<V_HIS_SERE_SERV> sereServs, DelegateReturnSuccess success, string printTypeCode, string fileName, HIS_TREATMENT treatmentPrint, List<HIS_SERE_SERV_DEPOSIT> _ListSereServDeposit, List<HIS_SERE_SERV_BILL> _ListSereServBill, List<V_HIS_TRANSACTION> _ListTransaction, bool _checkPrintAgain)
        {
            this.patyAlter = patyAlter;
            this.ServiceReqPrint = ServiceReqPrint;
            this.sereServs = sereServs;
            this.success = success;
            this.printTypeCode = printTypeCode;
            this.fileName = fileName;
            this.treatmentPrint = treatmentPrint;
            this.ListSereServDeposit = _ListSereServDeposit;
            this.ListSereServBill = _ListSereServBill;
            this.ListTransaction = _ListTransaction;
            this.checkPrintAgain = _checkPrintAgain;
        }
        public PrintKiosk(V_HIS_PATIENT_TYPE_ALTER patyAlter, V_HIS_SERVICE_REQ ServiceReqPrint, List<V_HIS_SERE_SERV> sereServs, DelegateReturnSuccess success, string printTypeCode, string fileName, HIS_TREATMENT treatmentPrint, List<HIS_SERE_SERV_DEPOSIT> _ListSereServDeposit, List<HIS_SERE_SERV_BILL> _ListSereServBill, List<V_HIS_TRANSACTION> _ListTransaction, bool _checkPrintAgain, List<HIS_CARD> _listHisCard)
        {
            this.patyAlter = patyAlter;
            this.ServiceReqPrint = ServiceReqPrint;
            this.sereServs = sereServs;
            this.success = success;
            this.printTypeCode = printTypeCode;
            this.fileName = fileName;
            this.treatmentPrint = treatmentPrint;
            this.ListSereServDeposit = _ListSereServDeposit;
            this.ListSereServBill = _ListSereServBill;
            this.ListTransaction = _ListTransaction;
            this.checkPrintAgain = _checkPrintAgain;
            this.listHisCard = _listHisCard;
        }
        public void RunPrintHasCard()
        {
            try
            {
                List<HIS_TRANS_REQ> transReq = null;
                HIS_TRANS_REQ transReqObj = null;

                if (AppConfigs.HisTranReqQRCodeTreatmentPrint)
                {
                    if (this.ServiceReqPrint != null)
                    {
                        CommonParam param = new CommonParam();
                        HisTransReqFilter filter = new HisTransReqFilter();
                        filter.IDs = new List<long> { this.ServiceReqPrint.TRANS_REQ_ID ?? 0 };
                        transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, filter, param);
                    }
                }
                else
                {
                    if(this.ServiceReqPrint != null)
                    {
                        CommonParam param = new CommonParam();
                        HisTransReqFilter filter = new HisTransReqFilter();
                        filter.TREATMENT_ID = this.ServiceReqPrint.TREATMENT_ID;
                        transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, filter, param);
                        if (transReq != null && transReq.Count > 0)
                        {
                            transReq = transReq.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
                        }
                    }
                }
                var lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();

                if (transReq != null && transReq.Count > 0)
                {
                    transReqObj = AppConfigs.HisTranReqQRCodeTreatmentPrint
                        ? transReq.FirstOrDefault(o => o.ID == ServiceReqPrint.TRANS_REQ_ID)
                        : transReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                }
                MPS.Processor.Mps000025.PDO.Mps000025PDO mps000025PDO = new MPS.Processor.Mps000025.PDO.Mps000025PDO(
                    this.ListSereServDeposit,
                    this.ListSereServBill,
                    this.ListTransaction,
                    this.ServiceReqPrint,
                    this.sereServs,
                    transReqObj,
                    lstConfig,
                    this.patyAlter,
                    this.patyAlter.PATIENT_TYPE_NAME, 
                    treatmentPrint
                );
                LogSystem.Debug(LogUtil.TraceData("Du lieu mps000025PDO khi dang ky kham", mps000025PDO));
                WaitingManager.Hide();

                string templateFile = Application.StartupPath + "\\Mps000025__Temp__" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                if (File.Exists(templateFile))
                {
                    try
                    {
                        File.Delete(templateFile);
                    }
                    catch { }
                }

                LogSystem.Info(LogUtil.TraceData("Du lieu templateFile truyen vao", templateFile));

                PrintData printData = new PrintData(printTypeCode, fileName, mps000025PDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, templateFile);

                bool printSuccess = MPS.MpsPrinter.Run(printData);
                if (printSuccess && File.Exists(templateFile))
                {
                    Inventec.Common.MSOfficePrint.MSOfficePrintProcessor printProcessor = new Inventec.Common.MSOfficePrint.MSOfficePrintProcessor(templateFile, null, null, printData.numCopy, false, "");

                    if (!printProcessor.Print())
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Call printProcessor.Print fail.");
                    }
                    else
                    {
                        if (this.checkPrintAgain)
                        {
                            if (this.sereServs != null && this.sereServs.Count > 0)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV, HIS_SERE_SERV>();
                                var servSereApi = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(this.sereServs);

                                if (servSereApi != null && servSereApi.Count > 0)
                                {
                                    foreach (var item in servSereApi)
                                    {
                                        if (item.IS_NO_EXECUTE == 1)
                                            item.IS_NO_EXECUTE = null;
                                    }
                                }

                                LogSystem.Debug(LogUtil.TraceData("api cap nhat servsere truyen len", servSereApi));

                                CommonParam param = new CommonParam();
                                HisSereServPayslipSDO sdo = new HisSereServPayslipSDO();
                                sdo.SereServs = servSereApi;
                                sdo.Field = MOS.SDO.UpdateField.IS_NO_EXECUTE;
                                sdo.TreatmentId = this.ServiceReqPrint.TREATMENT_ID;
                                LogSystem.Info(LogUtil.TraceData("api cap nhat servsere truyen len", sdo));

                                var updateSs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, sdo, param);
                                LogSystem.Info(LogUtil.TraceData("api cap nhat servsere tra ve", updateSs));
                            }
                        }
                    }

                    if (this.success != null)
                    {
                        this.success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void RunPrint()
        {
            try
            {
                List<HIS_TRANS_REQ> transReq = null;
                HIS_TRANS_REQ transReqMPS37 = null;
                if (AppConfigs.HisTranReqQRCodeTreatmentPrint)
                {
                    if (this.ServiceReqPrint != null)
                    {
                        CommonParam param = new CommonParam();
                        HisTransReqFilter filter = new HisTransReqFilter();
                        filter.IDs = new List<long> { this.ServiceReqPrint.TRANS_REQ_ID ?? 0 };
                        transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, filter, param);
                    }
                }
                else
                {
                    if (this.ServiceReqPrint != null)
                    {
                        CommonParam param = new CommonParam();
                        HisTransReqFilter filter = new HisTransReqFilter();
                        filter.TREATMENT_ID = this.ServiceReqPrint.TREATMENT_ID;
                        transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                            .Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, filter, param);
                        if (transReq != null && transReq.Count > 0)
                        {
                            transReq = transReq.Where(o => o.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE).ToList();
                            transReqMPS37 = transReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                        }
                    }
                }
                HIS_TRANS_REQ transReqObj = null;
                if (transReq != null && transReq.Count > 0)
                {
                    transReqObj = AppConfigs.HisTranReqQRCodeTreatmentPrint
                        ? transReq.FirstOrDefault(o => o.ID == ServiceReqPrint.TRANS_REQ_ID)
                        : transReq.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                }
               var lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();


                MPS.Processor.Mps000025.PDO.Mps000025PDO mps000025PDO = new MPS.Processor.Mps000025.PDO.Mps000025PDO(
                     this.ListSereServDeposit,
                     this.ListSereServBill,
                     this.ListTransaction,
                     this.ServiceReqPrint,
                     this.sereServs,
                     transReqObj,
                     lstConfig,
                     this.patyAlter,
                     this.patyAlter.PATIENT_TYPE_NAME, 
                     treatmentPrint
                );
                LogSystem.Debug(LogUtil.TraceData("Du lieu mps000025PDO khi dang ky kham", mps000025PDO));
                WaitingManager.Hide();
                string templateFile = Application.StartupPath + "\\Mps000025__Temp__" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                if (File.Exists(templateFile))
                {
                    try
                    {
                        File.Delete(templateFile);
                    }
                    catch { }
                }
                LogSystem.Info(LogUtil.TraceData("Du lieu templateFile truyen vao", templateFile));

                PrintData printData = new PrintData(printTypeCode, fileName, mps000025PDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, templateFile);

                bool printSuccess = MPS.MpsPrinter.Run(printData);
                if (printSuccess && File.Exists(templateFile))
                {
                    Inventec.Common.MSOfficePrint.MSOfficePrintProcessor printProcessor = new Inventec.Common.MSOfficePrint.MSOfficePrintProcessor(templateFile, null, null, printData.numCopy, false, "");

                    if (!printProcessor.Print())
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Call printProcessor.Print fail.");
                    }
                    else
                    {
                        if (this.checkPrintAgain)
                        {
                            if (this.sereServs != null && this.sereServs.Count > 0)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV, HIS_SERE_SERV>();
                                var servSereApi = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(this.sereServs);

                                if (servSereApi != null && servSereApi.Count > 0)
                                {
                                    foreach (var item in servSereApi)
                                    {
                                        if (item.IS_NO_EXECUTE == 1)
                                            item.IS_NO_EXECUTE = null;
                                    }
                                }

                                LogSystem.Debug(LogUtil.TraceData("api cap nhat servsere truyen len", servSereApi));

                                CommonParam param = new CommonParam();
                                HisSereServPayslipSDO sdo = new HisSereServPayslipSDO();
                                sdo.SereServs = servSereApi;
                                sdo.Field = MOS.SDO.UpdateField.IS_NO_EXECUTE;
                                sdo.TreatmentId = this.ServiceReqPrint.TREATMENT_ID;
                                LogSystem.Info(LogUtil.TraceData("api cap nhat servsere truyen len", sdo));

                                var updateSs = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, sdo, param);
                                LogSystem.Info(LogUtil.TraceData("api cap nhat servsere tra ve", updateSs));
                            }
                        }
                    }

                    if (this.success != null)
                    {
                        this.success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

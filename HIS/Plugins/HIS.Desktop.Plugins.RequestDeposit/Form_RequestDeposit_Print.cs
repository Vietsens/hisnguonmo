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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Print;
using MOS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class Form_RequestDeposit : HIS.Desktop.Utility.FormBase
    {
        private List<HIS_CONFIG> lstConfig;
        private HIS_TRANS_REQ transReq;

        bool delegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__XAC_NHAN_TAM_UNG__MPS000489:
                        InXacNhanTamUng(printTypeCode, fileName, depositReqPrint);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__YEU_CAU_TAM_UNG__MPS000091:
                        InYeuCauTamUng(printTypeCode, fileName, depositReqPrint);
                        break;
                    default:
                        break;
                }
                //InYeuCauTamUng(printTypeCode, fileName, depositReqPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InXacNhanTamUng(string printTypeCode, string fileName, V_HIS_DEPOSIT_REQ depositReqPrint)
        {
            try
            {
                bool result = false;

                WaitingManager.Show();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((depositReqPrint != null ? depositReqPrint.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000489.PDO.Mps000489PDO mps000489RDO = new MPS.Processor.Mps000489.PDO.Mps000489PDO(depositReqPrint);
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000489RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000489RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InYeuCauTamUng(string printTypeCode, string fileName, V_HIS_DEPOSIT_REQ depositReq)
        {
            try
            {
                bool result = false;

                WaitingManager.Show();

                //Thông tin bệnh nhân
                var patient = PrintGlobalStore.getPatient(treatmentID);
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;

                //BHYT
                //var patyAlterBhyt = PrintGlobalStore.getPatyAlterBhyt(treatmentID, instructionTime);
                CommonParam param = new CommonParam();
                HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                filter.TREATMENT_ID = treatmentID;
                var patyAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                MPS.Processor.Mps000091.PDO.PatyAlterBhytADO patyAlterAdo = new MPS.Processor.Mps000091.PDO.PatyAlterBhytADO();
                if (patyAlter != null && patyAlter.Count > 0)
                {
                    patyAlter = patyAlter.OrderByDescending(o => o.LOG_TIME).ToList();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000091.PDO.PatyAlterBhytADO>(patyAlterAdo, patyAlter.FirstOrDefault());
                }
                GetDataPrintQrCode();
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((depositReq != null ? depositReq.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                MPS.Processor.Mps000091.PDO.Mps000091PDO mps000091RDO = new MPS.Processor.Mps000091.PDO.Mps000091PDO(
                                patient,
                                depositReq,
                                patyAlterAdo,
                                lstConfig,
                                transReq
                                );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000091RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000091RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDataPrintQrCode()
        {
            try
            {
                lstConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (lstConfig != null && lstConfig.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTransReqFilter filter = new HisTransReqFilter();
                    filter.ID = depositReqPrint.TRANS_REQ_ID ?? 0;
                    transReq = new Inventec.Common.Adapter.BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param).First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

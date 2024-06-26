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
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    class Print
    {
        internal static void PrintData(string printTypeCode, string fileName, object data, bool printNow,
            ref bool result, long? roomId, bool isView, MPS.ProcessorBase.PrintConfig.PreviewType? PreviewType,
            int count, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData, string treatmentCode, Action<DocumentSignedUpdateIGSysResultDTO> DlgSendResultSigned)
        {
            try
            {
                treatmentCode = AddZeroToString(treatmentCode);
                Inventec.Common.Logging.LogSystem.Info("________Method PrintData: 1 " + treatmentCode + "_____"+ printTypeCode);
                string printerName = "";
                WaitingManager.Hide();
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.Logging.LogSystem.Debug("PrintData____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PreviewType), PreviewType)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printNow), printNow)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isView), isView)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Config.IsmergePrint), Config.IsmergePrint));

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, roomId);
                inputADO.DlgSendResultSigned = DlgSendResultSigned;
                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo ado = null;
                if (isView)
                {
                    if (Config.IsmergePrint)
                    {
                        MPS.ProcessorBase.Core.PrintData printD = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName) { saveMemoryStream = new System.IO.MemoryStream(), EmrInputADO = inputADO };
                        result = MPS.MpsPrinter.Run(printD);
                        if (result)
                        {
                            ado = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                            ado.data = printD.data;
                            ado.EmrInputADO = printD.EmrInputADO;
                            ado.eventLog = printD.eventLog;
                            ado.eventPrint = printD.eventPrint;
                            ado.fileName = printD.fileName;
                            ado.isAllowExport = printD.isAllowExport;
                            ado.numCopy = printD.numCopy;
                            ado.printerName = printD.printerName;
                            ado.printTypeCode = printD.printTypeCode;
                            ado.saveFilePath = printD.saveFilePath;
                            ado.ShowTutorialModule = printD.ShowTutorialModule;
                            ado.IsAllowEditTemplateFile = true;
                            if (printD.saveMemoryStream != null)
                            {
                                printD.saveMemoryStream.Position = 0;
                                ado.saveMemoryStream = new System.IO.MemoryStream();
                                printD.saveMemoryStream.CopyTo(ado.saveMemoryStream);
                                printD.saveMemoryStream.Dispose();
                            }
                        }
                    }
                    else if (PreviewType != null)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, PreviewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    if (PreviewType == MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow 
                        || PreviewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow
                        || PreviewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, PreviewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    }
                    else if (printNow || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else if (Config.IsmergePrint)
                    {
                        MPS.ProcessorBase.Core.PrintData printD = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName) { saveMemoryStream = new System.IO.MemoryStream(), EmrInputADO = inputADO };
                        result = MPS.MpsPrinter.Run(printD);
                        if (result)
                        {
                            ado = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                            ado.data = printD.data;
                            ado.EmrInputADO = printD.EmrInputADO;
                            ado.eventLog = printD.eventLog;
                            ado.eventPrint = printD.eventPrint;
                            ado.fileName = printD.fileName;
                            ado.isAllowExport = printD.isAllowExport;
                            ado.numCopy = printD.numCopy;
                            ado.printerName = printD.printerName;
                            ado.printTypeCode = printD.printTypeCode;
                            ado.saveFilePath = printD.saveFilePath;
                            ado.ShowTutorialModule = printD.ShowTutorialModule;
                            ado.IsAllowEditTemplateFile = true;
                            if (printD.saveMemoryStream != null)
                            {
                                printD.saveMemoryStream.Position = 0;
                                ado.saveMemoryStream = new System.IO.MemoryStream();
                                printD.saveMemoryStream.CopyTo(ado.saveMemoryStream);
                                printD.saveMemoryStream.Dispose();
                            }
                        }
                    }
                    else if (PreviewType != null)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, PreviewType ?? MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("________Method PrintData: 2 " + treatmentCode + "_____" + printTypeCode);
                savedData(count, ado);
                Inventec.Common.Logging.LogSystem.Info("________Method PrintData: 3 " + treatmentCode + "_____" + printTypeCode);
            }
            catch (Exception ex)
            {
                savedData(count, null);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private static string AddZeroToString(string treatmentCode)
        {
            string result = treatmentCode;
            try
            {
                if (treatmentCode.Length < 12 && checkDigit(treatmentCode))
                {
                    result = string.Format("{0:000000000000}", Convert.ToInt64(treatmentCode));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private static bool checkDigit(string s)
        {
            bool result = true;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == false) return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}

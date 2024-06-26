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
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Debate
{
    class Print
    {
        internal static void PrintData(string printTypeCode, string fileName, object data, long? roomId, Action< Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Method PrintData: " + printTypeCode);
                string printerName = "";
                WaitingManager.Hide();
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(EmrDataStore.treatmentCode, printTypeCode, roomId);
                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo ado = null;

                MPS.ProcessorBase.Core.PrintData printD = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName) { saveMemoryStream = new System.IO.MemoryStream(), EmrInputADO = inputADO };
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printD), printD));
                var result = MPS.MpsPrinter.Run(printD);
                if (result == true)
                {
                    ado = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                    ado.data = printD.data;
                    ado.EmrInputADO = printD.EmrInputADO;
                    ado.eventLog = printD.eventLog;
                    ado.eventPrint = printD.eventPrint;
                    Inventec.Common.Logging.LogSystem.Debug("printD.fileName___" + printD.fileName);
                    ado.fileName = printD.fileName;
                    ado.isAllowExport = printD.isAllowExport;
                    ado.numCopy = printD.numCopy;
                    ado.printerName = printD.printerName;
                    ado.printTypeCode = printD.printTypeCode;
                    ado.saveFilePath = printD.saveFilePath;
                    ado.ShowTutorialModule = printD.ShowTutorialModule;
                    ado.IsAllowEditTemplateFile = true;
                    // printD.IsAllowEditTemplateFile;
                    //ado.TemplateKey = printD.TemplateKey;
                    //ado.ActShowPrintLog = printD.ActShowPrintLog;
                    //ado.PrintLog = printD.PrintLog;
                    //ado.IsSingleCopy = printD.IsSingleCopy;
                    //ado.ShowPrintLog = printD.ShowPrintLog;
                    if (printD.saveMemoryStream != null)
                    {
                        printD.saveMemoryStream.Position = 0;
                        ado.saveMemoryStream = new System.IO.MemoryStream();
                        printD.saveMemoryStream.CopyTo(ado.saveMemoryStream);
                    }
                }
                savedData(ado);
            }
            catch (Exception ex)
            {
                savedData(null);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

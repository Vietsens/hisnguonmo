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

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter.Run
{
    class Print
    {
        internal static MPS.ProcessorBase.Core.PrintData printD;
        internal static Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo ado;
        internal static void PrintData(string printTypeCode, string fileName, object data, bool printNow, Inventec.Common.SignLibrary.ADO.InputADO inputADO,
            ref bool result, long? roomId, bool EmrSign, bool EmrSignAndPrint,
            int count, Action<int, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> savedData)
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

                ado = null;

                if (EmrSignAndPrint)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, printerName) { EmrInputADO = inputADO });
                }
                else if (printNow || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else if (AppConfigKeys.IsmergePrint)
                {
                    printD = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName) { saveMemoryStream = new System.IO.MemoryStream(), EmrInputADO = inputADO };
                    result = MPS.MpsPrinter.Run(printD);
                    if (result && savedData != null)
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
                        }
                    }
                }
                else if (EmrSign)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }

                savedData(count, ado);
            }
            catch (Exception ex)
            {
                savedData(count, null);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal static void DisposePrint()
        {
            try
            {
                printD = null;
                ado = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using MPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau
{
    public class PrintCustomShow<T>
    {
        T rdo { get; set; }
        string printTypeCode { get; set; }
        string fileName { get; set; }
        Inventec.Common.FlexCelPrint.DelegateReturnEventPrint ReturnEventPrint { get; set; }
        MPS.ProcessorBase.PrintConfig.PreviewType previewType;
        bool IsPreview { get; set; }

        public PrintCustomShow(string printTypeCode, string fileName, T rdo, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint, bool isPreview)
        {
            try
            {
               if (GlobalDataStore.CURRENT_PRINT_OPTION == PrintOption.Value.SHOW_DIALOG)
               {
                   this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
               }
               else 
               {
                   this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
               }

                if (GlobalDataStore.CURRENT_PRINT_OPTION == PrintOption.Value.PRINT_NOW ||
                    GlobalDataStore.CURRENT_PRINT_OPTION == PrintOption.Value.PRINT_NOW_AND_INIT_MENU ||
                    ConfigApplicationWorker.Get<long>(AppConfigKey.CHE_DO_IN_PHAN_MEM) == 2)
                {
                    this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                }
                else if (GlobalDataStore.CURRENT_PRINT_OPTION == PrintOption.Value.PRINT_NOW_AND_EMR_SIGN_NOW)
                {
                    this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                }
                else if (GlobalDataStore.CURRENT_PRINT_OPTION == PrintOption.Value.EMR_SIGN_AND_PRINT_PREVIEW)
                {
                    this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;
                }
                else if (GlobalDataStore.CURRENT_PRINT_OPTION == PrintOption.Value.EMR_SIGN_NOW)
                {
                    this.previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                }

                this.rdo = rdo;
                this.printTypeCode = printTypeCode;
                this.fileName = fileName;
                this.ReturnEventPrint = returnEventPrint;
                this.IsPreview = isPreview;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool SignRun(string treatmentCode, long? roomId, string documentName = null)
        {
            bool result = false;
            try
            {
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, roomId);
                inputADO.DlgSendResultSigned = PrintBordereauProcessor.DlgSendResultSigned;
                if (!String.IsNullOrWhiteSpace(documentName))
                {
                    inputADO.DocumentName = documentName;
                }

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, this.previewType, Base.GlobalDataStore.PrinterName, this.ReturnEventPrint) { EmrInputADO = inputADO, isPreview = this.IsPreview });
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public bool Run()
        {
            bool result = false;
            try
            {
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { isPreview = this.IsPreview });
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

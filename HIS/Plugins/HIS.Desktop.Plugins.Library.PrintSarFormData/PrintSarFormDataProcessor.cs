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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintSarFormData
{
    public class PrintSarFormDataProcessor
    {
        private bool printNow;
        List<SAR_FORM_DATA> _SarFormDatas { get; set; }
        object Data { get; set; }
        long? roomId { get; set; }

        public PrintSarFormDataProcessor(object data, List<SAR_FORM_DATA> _sarFormDatas, long? roomId)
        {
            this._SarFormDatas = _sarFormDatas;
            this.Data = data;
            this.roomId = roomId;
        }

        /// <summary>
        /// in ngay
        /// </summary>
        /// <param name="PrintTypeCode">mã in</param>
        public void Print(string PrintTypeCode)
        {
            try
            {
                Print(PrintTypeCode, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrintTypeCode">Mã in (8,10,11,268)</param>
        /// <param name="PrintNow">true/false</param>
        private void Print(string PrintTypeCode, bool PrintNow)
        {
            try
            {
                this.printNow = PrintNow;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);

                switch (PrintTypeCode)
                {
                    case "Mps000288":
                        richEditorMain.RunPrintTemplate("Mps000288", DelegateRunPrinter);
                        break;
                    case "Mps000289":
                        richEditorMain.RunPrintTemplate("Mps000289", DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                if (ProcessDataBeforePrint())
                {
                    switch (printCode)
                    {
                        case "Mps000288":
                            new PrintMps000288(printCode, fileName, ref result, this._SarFormDatas, printNow, roomId);
                            break;
                        case "Mps000289":
                            new PrintMps000289(printCode, fileName, ref result, this._SarFormDatas, printNow, roomId);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool ProcessDataBeforePrint()
        {
            string treatmentCode = "";
            bool result = true;
            if (this.Data != null)
            {
                if (this.Data is HIS_TREATMENT)
                {
                    var treatment = (HIS_TREATMENT)this.Data;
                    treatmentCode = treatment.TREATMENT_CODE;
                }
            }

            //TODO
            //
            EmrDataStore.treatmentCode = treatmentCode;
            return result;
        }
    }
}

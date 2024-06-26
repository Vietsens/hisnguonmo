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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutritionEdit.ADO;
using HIS.Desktop.Plugins.AssignNutritionEdit.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.RichEditor;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionEdit.Run
{
    public partial class frmAssignNutritionEdit : HIS.Desktop.Utility.FormBase
    {
        private void ProcessingPrintV2(string printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                switch (printType)
                {
                    case "Mps000275":
                        richEditorMain.RunPrintTemplate("Mps000275", DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000275":
                        Mps000275(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000275(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.resultSDO != null)
                {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultSDO.ServiceReq.TDL_TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);

                    HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.ID = resultSDO.ServiceReq.ID;
                    var serviceReqList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView",ApiConsumer.ApiConsumers.MosConsumer,filter,null);
                        MPS.Processor.Mps000275.PDO.Mps000275PDO mps000275PDO = new MPS.Processor.Mps000275.PDO.Mps000275PDO
                        (
                            serviceReqList,
                            null,
                            this.resultSDO.SereServRations,
                            null,
                            BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                        );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000275PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000275PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

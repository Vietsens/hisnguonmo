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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate
{
    public class RunUpdateTreatmentBehavior : IRunTemp
    {
        V_HIS_TREATMENT treatment { get; set; }

        public RunUpdateTreatmentBehavior(object data)
            : base()
        {
            this.treatment = data as V_HIS_TREATMENT;
        }

        bool IRunTemp.Run(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate, Dictionary<string, object> dicParamPlus, Dictionary<string, System.Drawing.Image> dicImagePlus, Inventec.Common.RichEditor.RichEditorStore richEditorMain, Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            bool result = false;
            try
            {
                richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Biểu mẫu khác ___", UpdateTreatmentJsonPrint, GetListPrintIdByTreatment, dicParamPlus, dicImagePlus, emrInputADO);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool UpdateTreatmentJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.treatment != null);
                if (valid)
                {
                    HIS_TREATMENT hisTreatment = new HIS_TREATMENT();
                    var listOldPrintIdOfTreatments = GetListPrintIdByTreatment();
                    ProcessTreatmentExecuteForUpdateJsonPrint(hisTreatment, listOldPrintIdOfTreatments, sarPrintCreated);
                    SaveTreatment(hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private void SaveTreatment(HIS_TREATMENT hisTreatment)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                hisTreatment.ID = this.treatment.ID;
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_TREATMENT>(HisRequestUriStore.HIS_TREATMENT_UPDATE_JSON, ApiConsumers.MosConsumer, hisTreatment, param);
                if (hisSereServWithFileResultSDO != null)
                {
                    success = true;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide(); ;
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessTreatmentExecuteForUpdateJsonPrint(HIS_TREATMENT hisTreatment, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.treatment != null)
                {
                    if (jsonPrintId == null)
                    {
                        jsonPrintId = new List<long>();
                    }
                    jsonPrintId.Add(sarPrintCreated.ID);

                    string printIds = "";
                    foreach (var item in jsonPrintId)
                    {
                        printIds += item.ToString() + ",";
                    }
                    hisTreatment.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<long> GetListPrintIdByTreatment()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.treatment != null)
                {
                    if (!String.IsNullOrEmpty(this.treatment.JSON_PRINT_ID))
                    {
                        var arrIds = this.treatment.JSON_PRINT_ID.Split(',', ';');
                        if (arrIds != null && arrIds.Length > 0)
                        {
                            foreach (var id in arrIds)
                            {
                                long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                                if (printId > 0)
                                {
                                    result.Add(printId);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

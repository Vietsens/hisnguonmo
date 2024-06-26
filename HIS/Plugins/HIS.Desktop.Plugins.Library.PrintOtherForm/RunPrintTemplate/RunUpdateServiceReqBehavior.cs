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
    public class RunUpdateServiceReqBehavior : IRunTemp
    {
        V_HIS_SERVICE_REQ serviceReq { get; set; }

        public RunUpdateServiceReqBehavior(object data)
            : base()
        {
            this.serviceReq = data as V_HIS_SERVICE_REQ;
        }

        bool IRunTemp.Run(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate, Dictionary<string, object> dicParamPlus, Dictionary<string, System.Drawing.Image> dicImagePlus, Inventec.Common.RichEditor.RichEditorStore richEditorMain, Inventec.Common.SignLibrary.ADO.InputADO emrInputADO)
        {
            bool result = false;
            try
            {
                richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Biểu mẫu khác ___", UpdateServiceJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus, emrInputADO);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool UpdateServiceJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.serviceReq != null);
                if (valid)
                {
                    HIS_SERVICE_REQ hisServiceReq = new HIS_SERVICE_REQ();
                    var listOldPrintIdOfSereServs = GetListPrintIdByServiceReq();
                    ProcessServiceReqExecuteForUpdateJsonPrint(hisServiceReq, listOldPrintIdOfSereServs, sarPrintCreated);
                    SaveTestServiceReq(hisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private void SaveTestServiceReq(HIS_SERVICE_REQ hisServiceReq)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                hisServiceReq.ID = this.serviceReq.ID;
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UPDATE_JSON, ApiConsumers.MosConsumer, hisServiceReq, param);
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

        private void ProcessServiceReqExecuteForUpdateJsonPrint(HIS_SERVICE_REQ hisServiceReq, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.serviceReq != null)
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
                    hisServiceReq.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<long> GetListPrintIdByServiceReq()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.serviceReq != null)
                {
                    if (!String.IsNullOrEmpty(this.serviceReq.JSON_PRINT_ID))
                    {
                        var arrIds = this.serviceReq.JSON_PRINT_ID.Split(',', ';');
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

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
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        Dictionary<string, object> dicParamPlus = new Dictionary<string, object>();
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
        Dictionary<string, System.Drawing.Image> dicImagePlus = new Dictionary<string, System.Drawing.Image>();
        CommonParam param = new CommonParam();

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);

                        if (String.IsNullOrEmpty(searchKey.Key))
                        {
                            dicParamPlus.Add(pi.Name, pi.GetValue(data));
                        }
                        else
                        {
                            dicParamPlus[pi.Name] = pi.GetValue(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInKetQuaKhacPlus(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                dicParamPlus = new Dictionary<string, object>();
                dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
                dicImagePlus = new Dictionary<string, System.Drawing.Image>();

                string printTypeCode = "Mps000208";// PrintTypeCodeStore.PRINT_TYPE_CODE__TEST___PHIEU_XET_NGHIEM_KHAC;
                dicParamPlus = new Dictionary<string, object>();
                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);// new SarPrintTypeLogic().Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(printTypeCode);
                if (printTemplate != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    //EXE.LOGIC.HisSereServ.HisSereServLogic sereServLogic = new LOGIC.HisSereServ.HisSereServLogic();
                    MOS.Filter.HisSereServView7Filter sereServFilter = new HisSereServView7Filter();
                    sereServFilter.SERVICE_REQ_ID = this.currentServiceReqPrint.ID;
                    var sereSevs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_7>>("api/HisSereServ/GetView7", ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());
                    //var sereSevs = sereServLogic.GetView<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>(sereServFilter);
                    string serviceNames = "";
                    foreach (var sereServ in sereSevs)
                        serviceNames += sereServ.TDL_SERVICE_NAME + "....." + "\r\n";
                    AddKeyIntoDictionaryPrint<ADO.ServiceReqADO>(this.currentServiceReqPrint, dicParamPlus);
                    dicParamPlus.Add("SERVICE_NAME", serviceNames);

                    var currentPatient = PrintGlobalStore.getPatient(currentServiceReqPrint.TREATMENT_ID);
                    AddKeyIntoDictionaryPrint<PatientADO>(currentPatient, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReqPrint.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());
                    dicParamPlus.Add("BED_ROOM_NAME", WorkPlace.GetRoomName());
                    dicParamPlus.Add("BED_NAME", "");
                    dicParamPlus.Add("INSTRUCTION_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReqPrint.INTRUCTION_TIME));
                    dicParamPlus.Add("INSTRUCTION_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentServiceReqPrint.INTRUCTION_TIME));
                    dicParamPlus.Add("INSTRUCTION_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.currentServiceReqPrint.INTRUCTION_TIME));
                    //dicParamPlus.Add("FINISH_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReqPrint.FINISH_TIME ?? 0));
                    // dicParamPlus.Add("FINISH_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentServiceReqPrint.FINISH_TIME ?? 0));
                    //dicParamPlus.Add("FINISH_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.currentServiceReqPrint.FINISH_TIME ?? 0));
                    WaitingManager.Hide();
                    richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Phiếu xét nghiệm", UpdateSereServJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<long> GetListPrintIdByServiceReq()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.currentServiceReqPrint != null)
                {
                    if (!String.IsNullOrEmpty(this.currentServiceReqPrint.JSON_PRINT_ID))
                    {
                        var arrIds = this.currentServiceReqPrint.JSON_PRINT_ID.Split(',', ';');
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool UpdateSereServJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.currentServiceReqPrint != null);
                if (valid)
                {
                    List<FileHolder> listFileHolder = new List<FileHolder>();
                    HIS_SERVICE_REQ hisServiceReq = new HIS_SERVICE_REQ();
                    var listOldPrintIdOfSereServs = GetListPrintIdByServiceReq();
                    ProcessServiceReqExecuteForUpdateJsonPrint(ref hisServiceReq, listOldPrintIdOfSereServs, sarPrintCreated);
                    SaveTestServiceReq(listFileHolder, hisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void SaveTestServiceReq(List<FileHolder> listFileHolder, HIS_SERVICE_REQ hisServiceReq)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                HIS_SERVICE_REQ serviceReqUpdateSDO = new HIS_SERVICE_REQ();
                AutoMapper.Mapper.CreateMap<ADO.ServiceReqADO, HIS_SERVICE_REQ>();
                serviceReqUpdateSDO = AutoMapper.Mapper.Map<ADO.ServiceReqADO, HIS_SERVICE_REQ>(this.currentServiceReqPrint);
                hisServiceReq.ID = this.currentServiceReq.ID;
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UPDATE_JSON, ApiConsumers.MosConsumer, hisServiceReq, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (hisSereServWithFileResultSDO != null)
                {
                    success = true;
                }
                WaitingManager.Hide();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide(); ;
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessServiceReqExecuteForUpdateJsonPrint(ref HIS_SERVICE_REQ hisServiceReq, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.currentServiceReqPrint != null)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}


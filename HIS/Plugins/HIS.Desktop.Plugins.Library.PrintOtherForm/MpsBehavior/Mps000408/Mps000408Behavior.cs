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
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000408
{
    public partial class Mps000408Behavior : MpsDataBase, ILoad
    {
        private V_HIS_SERVICE_REQ serviceReq { get; set; }
        private V_HIS_SERE_SERV_PTTT sereServPTTT { get; set; }

        private PrintOtherInputADO inputAdo { get; set; }

        public Mps000408Behavior(PrintOtherInputADO _InAdo)
            : base()
        {
            this.inputAdo = _InAdo;
        }

        bool ILoad.Load(string printTypeCode, UpdateType.TYPE updateType)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug("Mps000408.InputAdo\n" + Inventec.Common.Logging.LogUtil.TraceData("OatherFormInputADO", this.inputAdo));
                this.LoadData();

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);
                if (printTemplate == null)
                {
                    throw new Exception("Khong tim thay template");
                }

                this.SetDicParamCommon(ref dicParamPlus);

                //Thoi gian vao vien                
                if (this.serviceReq != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.serviceReq, dicParamPlus);

                    dicParamPlus.Add("AGE", this.CalculateFullAge(this.serviceReq.TDL_PATIENT_DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.TDL_PATIENT_DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);
                }
                else
                {
                    this.serviceReq = new V_HIS_SERVICE_REQ();
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.serviceReq, dicParamPlus);

                    dicParamPlus.Add("AGE", "");
                    dicParamPlus.Add("AGE_BY_YEAR", "");
                }

                if (this.sereServPTTT == null) this.sereServPTTT = new V_HIS_SERE_SERV_PTTT();
                AddKeyIntoDictionaryPrint<V_HIS_SERE_SERV_PTTT>(this.sereServPTTT, dicParamPlus);

                List<string> keys = new List<string>();
                foreach (var item in dicParamPlus)
                {
                    if (item.Value == null)
                    {
                        keys.Add(item.Key);
                    }
                }

                if (keys.Count > 0)
                {
                    foreach (var key in keys)
                    {
                        dicParamPlus[key] = "";
                    }
                }
                WaitingManager.Hide();

                if (updateType == UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT)
                {
                    OtherFormAssTreatmentInputADO otherFormAssTreatmentInputADO = new OtherFormAssTreatmentInputADO();
                    otherFormAssTreatmentInputADO.TreatmentId = this.inputAdo.TreatmentId;
                    otherFormAssTreatmentInputADO.PrintTypeCode = printTypeCode;
                    otherFormAssTreatmentInputADO.DicParam = dicParamPlus;
                    List<object> listObj = new List<object>();
                    listObj.Add(otherFormAssTreatmentInputADO);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssTreatment", inputAdo.RoomId ?? 0, inputAdo.RoomTypeId ?? 0, listObj);
                }
                else if (updateType == UpdateType.TYPE.OPEN_OTHER_ASS_SERVICE_REQ)
                {
                    OtherFormAssServiceReqADO otherFormAssServiceReqADO = new OtherFormAssServiceReqADO();
                    otherFormAssServiceReqADO.ServiceReqId = this.inputAdo.ServiceReqId;
                    otherFormAssServiceReqADO.PrintTypeCode = printTypeCode;
                    otherFormAssServiceReqADO.DicParam = dicParamPlus;
                    List<object> listObj = new List<object>();
                    listObj.Add(otherFormAssServiceReqADO);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssServiceReq", inputAdo.RoomId ?? 0, inputAdo.RoomTypeId ?? 0, listObj);
                }
                else
                {
                    IRunTemp behavior = RunUpdateFactory.RunTemplateByUpdateType(this.serviceReq, updateType);
                    result = behavior != null ? (behavior.Run(printTemplate, dicParamPlus, dicImagePlus, richEditorMain, null)) : false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

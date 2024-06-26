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

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000400
{
    public partial class Mps000400Behavior : MpsDataBase, ILoad
    {
        private V_HIS_SERVICE_REQ serviceReq { get; set; }
        private HIS_DHST dhst { get; set; }

        private long departmentId;

        public Mps000400Behavior(long _serviceReqId)
            : base()
        {
            this.ServiceReqId = _serviceReqId;
        }

        bool ILoad.Load(string printTypeCode, UpdateType.TYPE updateType)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                this.LoadData();

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);
                if (printTemplate == null)
                {
                    throw new Exception("Khong tim thay template");
                }

                if (this.serviceReq != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.serviceReq, dicParamPlus);
                }

                this.SetDicParamCommon(ref dicParamPlus);
                this.SetDicParamDhst(ref dicParamPlus);

                dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());

                dicParamPlus.Add("ADVISER_LOGINNAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                dicParamPlus.Add("ADVISER_USERNAME", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());

                if (this.serviceReq != null)
                {
                    dicParamPlus.Add("AGE", this.CalculateFullAge(this.serviceReq.TDL_PATIENT_DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.serviceReq.TDL_PATIENT_DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);
                }

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
                    otherFormAssTreatmentInputADO.TreatmentId = serviceReq.TREATMENT_ID;
                    otherFormAssTreatmentInputADO.PrintTypeCode = printTypeCode;
                    otherFormAssTreatmentInputADO.DicParam = dicParamPlus;
                    List<object> listObj = new List<object>();
                    listObj.Add(otherFormAssTreatmentInputADO);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssTreatment", 0, 0, listObj);
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

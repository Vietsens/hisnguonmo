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

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000407
{
    public partial class Mps000407Behavior : MpsDataBase, ILoad
    {
        private V_HIS_PATIENT patient { get; set; }
        private V_HIS_TREATMENT treatment { get; set; }
        private V_HIS_SERVICE_REQ serviceReq { get; set; }
        private HIS_DHST dhst { get; set; }
        private V_HIS_ROOM room { get; set; }

        private PrintOtherInputADO inputAdo { get; set; }

        public Mps000407Behavior(PrintOtherInputADO _InAdo)
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
                Inventec.Common.Logging.LogSystem.Debug("Mps000407.InputAdo\n" + Inventec.Common.Logging.LogUtil.TraceData("OatherFormInputADO", this.inputAdo));
                this.LoadData();

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);
                if (printTemplate == null)
                {
                    throw new Exception("Khong tim thay template");
                }

                this.SetDicParamCommon(ref dicParamPlus);

                //Thoi gian vao vien                
                if (this.treatment != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(this.treatment, dicParamPlus);
                    if (this.treatment.CLINICAL_IN_TIME.HasValue)
                    {
                        dicParamPlus.Add("CLINICAL_IN_TIME_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetTimeSeparateFromTime(this.treatment.CLINICAL_IN_TIME.Value));
                        dicParamPlus.Add("CLINICAL_IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.treatment.CLINICAL_IN_TIME.Value));
                    }
                    else
                    {
                        dicParamPlus.Add("CLINICAL_IN_TIME_SEPARATE_STR", "");
                        dicParamPlus.Add("CLINICAL_IN_TIME_STR", "");
                    }
                    dicParamPlus.Add("IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.treatment.IN_TIME));
                    dicParamPlus.Add("IN_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.treatment.IN_TIME));
                    dicParamPlus.Add("AGE", this.CalculateFullAge(this.treatment.TDL_PATIENT_DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);

                    dicParamPlus["FULL_NAME"] = this.treatment.TDL_PATIENT_NAME;
                    dicParamPlus["GENDER"] = this.treatment.TDL_PATIENT_GENDER_NAME;
                    dicParamPlus["PATIENT_ADDRESS"] = this.treatment.TDL_PATIENT_ADDRESS;
                    dicParamPlus["PATIENT_CODE"] = this.treatment.TDL_PATIENT_CODE;
                    dicParamPlus["PHONE"] = this.treatment.TDL_PATIENT_PHONE;
                }
                else if (this.patient != null)
                {
                    dicParamPlus.Add("CLINICAL_IN_TIME_SEPARATE_STR", "");
                    dicParamPlus.Add("CLINICAL_IN_TIME_STR", "");
                    dicParamPlus.Add("IN_TIME_STR", "");
                    dicParamPlus.Add("IN_DATE_STR", "");
                    dicParamPlus.Add("AGE", this.CalculateFullAge(this.patient.DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.patient.DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);

                    dicParamPlus["FULL_NAME"] = this.patient.VIR_PATIENT_NAME;
                    dicParamPlus["GENDER"] = this.patient.GENDER_NAME;
                    dicParamPlus["PATIENT_ADDRESS"] = this.patient.VIR_ADDRESS;
                    dicParamPlus["PATIENT_CODE"] = this.patient.PATIENT_CODE;
                    dicParamPlus["PHONE"] = this.patient.PHONE;
                }

                if (this.patient == null) this.patient = new V_HIS_PATIENT();
                AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(this.patient, dicParamPlus);
                if (!String.IsNullOrWhiteSpace(this.patient.CCCD_NUMBER))
                {
                    dicParamPlus["CMND_CCCD"] = this.patient.CCCD_NUMBER;
                    dicParamPlus["CMND_CCCD_DATE"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.patient.CCCD_DATE ?? 0);
                    dicParamPlus["CMND_CCCD_PLACE"] = this.patient.CCCD_PLACE;
                }
                else
                {
                    dicParamPlus["CMND_CCCD"] = this.patient.CMND_NUMBER;
                    dicParamPlus["CMND_CCCD_DATE"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.patient.CMND_DATE ?? 0);
                    dicParamPlus["CMND_CCCD_PLACE"] = this.patient.CMND_PLACE;
                }

                if (this.serviceReq == null) this.serviceReq = new V_HIS_SERVICE_REQ();
                AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.serviceReq, dicParamPlus);

                if (this.dhst == null) this.dhst = new HIS_DHST();
                AddKeyIntoDictionaryPrint<HIS_DHST>(this.dhst, dicParamPlus);


                if (this.room == null) this.room = new V_HIS_ROOM();
                AddKeyIntoDictionaryPrint<V_HIS_ROOM>(this.room, dicParamPlus);


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
                else
                {
                    IRunTemp behavior = RunUpdateFactory.RunTemplateByUpdateType(this.treatment, updateType);
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

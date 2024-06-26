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

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000476
{
    public partial class Mps000476Behavior : MpsDataBase, ILoad
    {
        private V_HIS_DEPARTMENT department { get; set; }
        private V_HIS_TREATMENT treatment { get; set; }

        public Mps000476Behavior(long _treatmentId)
            : base()
        {
            this.TreatmentId = _treatmentId;
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
                    dicParamPlus.Add("AGE", this.CalculateFullAge(this.treatment.TDL_PATIENT_DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);

                    string dobStr = "";
                    if (this.treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        dobStr = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    else
                        dobStr = treatment.TDL_PATIENT_DOB.ToString().Substring(6, 2) + "/" + treatment.TDL_PATIENT_DOB.ToString().Substring(4, 2) + "/" + treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    string deadStr = "";
                    if (this.treatment.DEATH_TIME !=null)
                        deadStr = treatment.DEATH_TIME.ToString().Substring(6, 2) + "/" + treatment.DEATH_TIME.ToString().Substring(4, 2) + "/" + treatment.DEATH_TIME.ToString().Substring(0, 4) + " " +
                            treatment.DEATH_TIME.ToString().Substring(8, 2) + ":" + treatment.DEATH_TIME.ToString().Substring(10, 2)+ ":"+treatment.DEATH_TIME.ToString().Substring(12, 2);

                    dicParamPlus.Add("DOB_STR", dobStr);
                    dicParamPlus.Add("DEATH_STR", deadStr);
                    //dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(this.treatment.TDL_PATIENT_DOB, "", "", "", "", this.treatment.IN_TIME));
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
                    otherFormAssTreatmentInputADO.TreatmentId = this.TreatmentId;
                    otherFormAssTreatmentInputADO.PrintTypeCode = printTypeCode;
                    otherFormAssTreatmentInputADO.DicParam = dicParamPlus;
                    List<object> listObj = new List<object>();
                    listObj.Add(otherFormAssTreatmentInputADO);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.OtherFormAssTreatment", 0, 0, listObj);
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

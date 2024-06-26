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
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Config;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000419
{
    public partial class Mps000419Behavior : MpsDataBase, ILoad
    {
        private V_HIS_PATIENT patient { get; set; }
        private V_HIS_TREATMENT treatment { get; set; }
        private HIS_DHST dhst { get; set; }

        private long patientId;

        public Mps000419Behavior(long _treatmentId, long _patientId)
            : base()
        {
            this.TreatmentId = _treatmentId;
            this.patientId = _patientId;
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
                if (this.patient == null) this.patient = new V_HIS_PATIENT();
                if (patient != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(patient, dicParamPlus);
                    
                }

                //Thoi gian vao vien
                if (this.treatment == null) this.treatment = new V_HIS_TREATMENT();
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

                    //Tuoi
                    var tuoi = this.CalculateFullAge(this.treatment.TDL_PATIENT_DOB);
                    string tuoi1;
                    string tuoi2;

                    if (tuoi.Length < 2)
                    {
                        tuoi1 = null;
                        tuoi2 = tuoi;
                    }
                    else 
                    {
                        tuoi1 = tuoi.Substring(0, 1);
                        tuoi2 = tuoi.Substring(1);
                    }

                    dicParamPlus.Add("AGE1", tuoi1);
                    dicParamPlus.Add("AGE2", tuoi2);


                    dicParamPlus.Add("AGE", this.CalculateFullAge(this.treatment.TDL_PATIENT_DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.TDL_PATIENT_DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);

                    DateTime dob1=(DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.treatment.TDL_PATIENT_DOB);
                    dicParamPlus.Add("AGED", dob1.Day);
                    dicParamPlus.Add("AGEM", dob1.Month);
                    dicParamPlus.Add("AGEY", dob1.Year);

                    //Gioi tinh
                    string NAM;
                    string NU;

                    if (this.treatment.TDL_PATIENT_GENDER_ID == 1)
                    {
                        NU = "X";
                        NAM = "";
                    }
                    else
                    {
                        NAM = "X";
                        NU = "";
                    }

                    dicParamPlus.Add("NAM", NAM);
                    dicParamPlus.Add("NU", NU);

                    //DoiTuong
                    string PATIENT_TYPE1;
                    string PATIENT_TYPE2;
                    string PATIENT_TYPE3;
                    string PATIENT_TYPE4;

                    if (this.treatment.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                    {
                        PATIENT_TYPE1 = "X";
                        PATIENT_TYPE2 = "";
                        PATIENT_TYPE3 = "";
                        PATIENT_TYPE4 = "";
                    }
                    else
                    {
                        if (this.treatment.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__HOSPITAL_FEE)
                        {
                            PATIENT_TYPE1 = "";
                            PATIENT_TYPE2 = "X";
                            PATIENT_TYPE3 = "";
                            PATIENT_TYPE4 = "";
                        }
                        else
                        {
                            if (this.treatment.TDL_PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FREE)
                            {
                                PATIENT_TYPE1 = "";
                                PATIENT_TYPE2 = "";
                                PATIENT_TYPE3 = "X";
                                PATIENT_TYPE4 = "";
                            }
                            else
                            {
                                PATIENT_TYPE1 = "";
                                PATIENT_TYPE2 = "";
                                PATIENT_TYPE3 = "";
                                PATIENT_TYPE4 = "X";
                            }
                        }
                    }
                    dicParamPlus.Add("PATIENT_TYPE1", PATIENT_TYPE1);
                    dicParamPlus.Add("PATIENT_TYPE2", PATIENT_TYPE2);
                    dicParamPlus.Add("PATIENT_TYPE3", PATIENT_TYPE3);
                    dicParamPlus.Add("PATIENT_TYPE4", PATIENT_TYPE4);
                }

                if (this.dhst == null) this.dhst = new HIS_DHST();

                if (this.dhst != null)
                {

                    AddKeyIntoDictionaryPrint<HIS_DHST>(this.dhst, dicParamPlus);
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

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
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor;
using Inventec.Common.WordContent;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.OtherFormAssTreatment
{
    public partial class frmOtherFormAssTreatment : HIS.Desktop.Utility.FormBase
    {
        private void TaoBieuMauMps000379Click(SarPrintTypeAdo sarPrintType)
        {
            try
            {
                WaitingManager.Show();
                dicParamPlus = new Dictionary<string, object>();
                if (dicInputParam != null && dicInputParam.Count > 0)
                    dicParamPlus = dicInputParam;

                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this.TreatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                var serviceReqList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, null);

                this.SetDicParamPatient(ref dicParamPlus);

                this.SetCommonSingleKey();

                if (this.currentModule != null)
                {
                    var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                    if (currentRoom != null)
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "BRANCH_NAME", currentRoom.BRANCH_NAME);
                    else
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "BRANCH_NAME", "");
                }
                //Thoi gian vao vien
                if (this.Treatment != null)
                {
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(Treatment, dicParamPlus);
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "CLINICAL_IN_TIME_STR", this.Treatment.CLINICAL_IN_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.Treatment.CLINICAL_IN_TIME.Value) : "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "IN_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.Treatment.IN_TIME));

                    if (this.Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_GENDER_MALE", "x");
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_GENDER_FEMALE", "");
                    }
                    else if (this.Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_GENDER_FEMALE", "x");
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_GENDER_MALE", "");
                    }
                    else
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_GENDER_UNKNOWN", "x");
                    }

                    if (this.Treatment.OUT_TIME.HasValue)
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "OUT_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.Treatment.OUT_TIME ?? 0));
                    else
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "OUT_TIME_STR", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus, "DOB_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(Treatment.TDL_PATIENT_DOB));
                    if (Treatment.TDL_PATIENT_DOB > 0 && Treatment.TDL_PATIENT_DOB.ToString().Length >= 4)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TDL_PATIENT_DOB_YEAR", Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    }
                    else
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TDL_PATIENT_DOB_YEAR", "");
                    }

                    var treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == Treatment.TDL_TREATMENT_TYPE_ID);
                    if (treatmentType != null)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_TYPE_CODE", treatmentType.TREATMENT_TYPE_CODE);
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_TYPE_NAME", treatmentType.TREATMENT_TYPE_NAME);
                    }
                    else
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_TYPE_CODE", "");
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_TYPE_NAME", "");
                    }

                    var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Treatment.TDL_PATIENT_TYPE_ID);
                    if (patientType != null)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_TYPE_CODE", patientType.PATIENT_TYPE_CODE);
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_TYPE_NAME", patientType.PATIENT_TYPE_NAME);
                    }
                    else
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_TYPE_CODE", "");
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "PATIENT_TYPE_NAME", "");
                    }
                    this.SetDicParamTreatment(ref dicParamPlus);

                    var LastDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.Treatment.LAST_DEPARTMENT_ID);
                    if (LastDepartment != null)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "LAST_DEPARTMENT_NAME", LastDepartment.DEPARTMENT_NAME);
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "LAST_DEPARTMENT_CODE", LastDepartment.DEPARTMENT_CODE);
                    }
                    else
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "LAST_DEPARTMENT_NAME", "");
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "LAST_DEPARTMENT_CODE", "");
                    }
                    if (TreatmentBedRooms != null && TreatmentBedRooms.Count > 0)
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_BED_ROOM_CODE", TreatmentBedRooms.OrderByDescending(o => o.ADD_TIME).First().BED_ROOM_CODE);

                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_BED_ROOM_NAME", TreatmentBedRooms.OrderByDescending(o => o.ADD_TIME).First().BED_ROOM_NAME);
                    }
                    else
                    {
                        TemplateKeyProcessor.SetSingleKey(dicParamPlus, "TREATMENT_BED_ROOM_CODE", "");

                        TemplateKeyProcessor.SetSingleKey(dicParamPlus,"TREATMENT_BED_ROOM_NAME", "");
                    }
                    dicParamPlus["ICD_CODE_TREATMENT"] = Treatment.ICD_CODE;
                    dicParamPlus["ICD_NAME_TREATMENT"] = Treatment.ICD_NAME;
                }
                else
                {
                    V_HIS_TREATMENT temp = new V_HIS_TREATMENT();
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_TREATMENT>(temp, dicParamPlus);
                    //temp.TREATMENT_METHOD
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,"CLINICAL_IN_TIME_STR", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,"IN_TIME_STR", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,"OUT_TIME_STR", "");
                    dicParamPlus["DOB_STR"] = "";
                    dicParamPlus["TDL_PATIENT_DOB_YEAR"] = "";
                    dicParamPlus["ICD_CODE_TREATMENT"] = "";
                    dicParamPlus["ICD_NAME_TREATMENT"] = "";
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,"PATIENT_GENDER_MALE", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,"PATIENT_GENDER_FEMALE", "");
                    TemplateKeyProcessor.SetSingleKey(dicParamPlus,"PATIENT_GENDER_UNKNOWN", "");
                }

                if (serviceReqList != null && serviceReqList.Count > 0)
                {
                    HIS_SERVICE_REQ mainExamServiceReq = new HIS_SERVICE_REQ();
                    var mainExamServiceReqCheck = serviceReqList.FirstOrDefault(o => o.IS_MAIN_EXAM == 1);
                    if (mainExamServiceReqCheck == null)
                    {
                        mainExamServiceReq = serviceReqList.OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault();
                    }
                    else
                    {
                        mainExamServiceReq = mainExamServiceReqCheck;
                    }
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(mainExamServiceReq, dicParamPlus);
                    // get DHST
                    if (mainExamServiceReq.DHST_ID.HasValue && mainExamServiceReq.DHST_ID.Value > 0)
                    {
                        MOS.Filter.HisDhstViewFilter dhstFilter = new HisDhstViewFilter();
                        dhstFilter.ID = mainExamServiceReq.DHST_ID;
                        var dhst = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DHST>>("api/HisDhst/GetView", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, null).FirstOrDefault();
                        TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_DHST>(dhst, dicParamPlus);
                        dicParamPlus["NOTE_DHST"] = dhst.NOTE;
                    }
                    else
                    {
                        V_HIS_DHST tempDhst = new V_HIS_DHST();
                        TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_DHST>(tempDhst, dicParamPlus);
                        dicParamPlus["NOTE_DHST"] = "";
                    }

                    dicParamPlus["ICD_CODE_EXAM"] = mainExamServiceReq.ICD_CODE;
                    dicParamPlus["ICD_NAME_EXAM"] = mainExamServiceReq.ICD_NAME;
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(mainExamServiceReq, dicParamPlus);
                }
                else
                {
                    dicParamPlus["ICD_CODE_EXAM"] = "";
                    dicParamPlus["ICD_NAME_EXAM"] = "";
                    HIS_SERVICE_REQ temp = new HIS_SERVICE_REQ();
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<HIS_SERVICE_REQ>(temp, dicParamPlus);
                    V_HIS_DHST tempDhst = new V_HIS_DHST();
                    TemplateKeyProcessor.AddKeyIntoDictionaryPrint<V_HIS_DHST>(tempDhst, dicParamPlus);
                }

                // #29365
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterViewFilter _alterFilter = new HisPatientTypeAlterViewFilter();
                _alterFilter.TREATMENT_ID = this.TreatmentId;
                _alterFilter.LOG_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, _alterFilter, param).OrderByDescending(p => p.LOG_TIME).FirstOrDefault();
                if (patientTypeAlter != null && patientTypeAlter.ID > 0)
                {
                    var patientBhyt = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.OtherFormAssTreatment.Base.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT));
                    if (patientTypeAlter.PATIENT_TYPE_ID !=
                        (patientBhyt != null ? patientBhyt.ID : 0)
                       )
                    {
                        dicParamPlus["HEIN_CARD_NUMBER"] = "";
                        dicParamPlus["HEIN_CARD_FROM_TIME_STR"] = "";
                        dicParamPlus["HEIN_CARD_TO_TIME_STR"] = "";
                    }
                    else
                    {
                        dicParamPlus["HEIN_CARD_NUMBER"] = patientTypeAlter.HEIN_CARD_NUMBER;
                        dicParamPlus["HEIN_CARD_FROM_TIME_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0);
                        dicParamPlus["HEIN_CARD_TO_TIME_STR"] = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.HEIN_CARD_TO_TIME ?? 0);
                    }
                }
                else
                {
                    dicParamPlus["HEIN_CARD_NUMBER"] = "";
                    dicParamPlus["HEIN_CARD_FROM_TIME_STR"] = "";
                    dicParamPlus["HEIN_CARD_TO_TIME_STR"] = "";
                }

                var sereServs = LoadDataSS();
                var sereServLeaf = sereServs != null && sereServs.Count > 0 ? sereServs.Where(o => o.IsLeaf).OrderBy(p => p.NUM_ORDER).ToList() : null;
                if (sereServLeaf != null && sereServLeaf.Count > 0)
                {
                    List<string> _str = new List<string>();
                    foreach (var item in sereServLeaf)
                    {
                        string dienBien = "";
                        string value = item.VALUE_RANGE;
                        dienBien = item.SERVICE_REQ_CODE + (!string.IsNullOrEmpty(value) ? (": " + value) : "");
                        _str.Add(dienBien);
                    }
                    string _strNews = "";
                    if (_str != null && _str.Count > 0)
                        _strNews = string.Join(";", _str);
                    dicParamPlus["CONTENT_CLINICAL"] = _strNews;
                }
                else
                {
                    dicParamPlus["CONTENT_CLINICAL"] = "";
                }

                string extension = Path.GetExtension(sarPrintType.FILE_NAME);
                WaitingManager.Hide();
                if (extension.Equals(".doc") || extension.Equals(".docx"))
                {
                    ProcessWordAndShowWordContentResult(sarPrintType);

                    //frmPrintEditor printEditor = new frmPrintEditor(sarPrintType.FILE_NAME, "Biểu mẫu khác__", UpdateTreatmentJsonPrint, dicParamPlus, this.dicImagePlus, inputADO);
                    //printEditor.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Sai định dạng file . Chỉ hỗ trợ định dạng file .doc, .docx");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000387.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000387
{
    public class Mps000387Processor : AbstractProcessor
    {
        Mps000387PDO rdo;
        List<ADOs> PathologicalHistorys = new List<ADOs>();
        List<ADOs> HospitalizationStates = new List<ADOs>();
        List<ADOs> TreatmentTrackings = new List<ADOs>();
        List<ADOs> InternalMedicineStates = new List<ADOs>();
        List<ADOs> Prognosiss = new List<ADOs>();
        List<ADOs> SubclinicalProcessess = new List<ADOs>();
        public Mps000387Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000387PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                //Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                SetSingleKey();
                CreateListKey();
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Participants", rdo.LstHisDebateUser);
                objectTag.AddObjectData(store, "ParticipantsEkip", rdo.LstHisDebateEkipUser);
                objectTag.AddObjectData(store, "PathologicalHistorys", PathologicalHistorys);
                objectTag.AddObjectData(store, "HospitalizationStates", HospitalizationStates);
                objectTag.AddObjectData(store, "TreatmentTrackings", TreatmentTrackings);
                objectTag.AddObjectData(store, "InternalMedicineStates", InternalMedicineStates);
                objectTag.AddObjectData(store, "Prognosiss", Prognosiss);
                objectTag.AddObjectData(store, "SubclinicalProcessess", SubclinicalProcessess);

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void CreateListKey()
        {
            try
            {
                if (rdo.CurrentHisDebate != null)
                {
                    if (!String.IsNullOrEmpty(rdo.CurrentHisDebate.PATHOLOGICAL_HISTORY))
                    {
                        rdo.CurrentHisDebate.PATHOLOGICAL_HISTORY.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => PathologicalHistorys.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.CurrentHisDebate.HOSPITALIZATION_STATE))
                    {
                        rdo.CurrentHisDebate.HOSPITALIZATION_STATE.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => HospitalizationStates.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.CurrentHisDebate.TREATMENT_TRACKING))
                    {
                        rdo.CurrentHisDebate.TREATMENT_TRACKING.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => TreatmentTrackings.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.CurrentHisDebate.INTERNAL_MEDICINE_STATE))
                    {
                        rdo.CurrentHisDebate.INTERNAL_MEDICINE_STATE.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => InternalMedicineStates.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.CurrentHisDebate.PROGNOSIS))
                    {
                        rdo.CurrentHisDebate.PROGNOSIS.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => Prognosiss.Add(new ADOs(o.Trim()))); ;
                    }
                    if (!String.IsNullOrEmpty(rdo.CurrentHisDebate.SUBCLINICAL_PROCESSES))
                    {
                        rdo.CurrentHisDebate.SUBCLINICAL_PROCESSES.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => SubclinicalProcessess.Add(new ADOs(o.Trim())));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetSingleKey()
        {
            try
            {
                if (rdo.CurrentHisDebate != null)
                {
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.CurrentHisDebate.DEBATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_DEBATE>(rdo.CurrentHisDebate, false);
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.SERVICE_CODE, rdo.CurrentHisDebate.SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.SERVICE_NAME, rdo.CurrentHisDebate.SERVICE_NAME));
                }

                if (rdo.Treatment != null)
                {
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.IN_CODE, rdo.Treatment.IN_CODE));
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.TREATMENT_CODE, rdo.Treatment.TREATMENT_CODE));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.AGE, CalculateFullAge(rdo.Treatment.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Treatment.TDL_PATIENT_DOB))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.D_O_B, rdo.Treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.GENDER_MALE, rdo.Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "X" : "")));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.GENDER_FEMALE, rdo.Treatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE ? "X" : "")));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.Treatment, false);
                }

                if (rdo.PatyAlterBhyt != null && !string.IsNullOrEmpty(rdo.PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.PatyAlterBhyt, false);
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.IS_HEIN, "X")));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.IS_VIENPHI, "")));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.PatyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_CODE)));
                    SetSingleKey((new KeyValue(Mps000387ExtendSingleKey.HEIN_MEDI_ORG_NAME, rdo.PatyAlterBhyt.HEIN_MEDI_ORG_NAME)));
                }

                if (rdo.DepartmentTran != null)
                {
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTran.DEPARTMENT_IN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.DepartmentTran.DEPARTMENT_IN_TIME ?? 0)));
                }

                if (rdo.SingleKey != null)
                {
                    AddObjectKeyIntoListkey<MPS.Processor.Mps000387.PDO.Mps000387PDO.Mps000387SingleKey>(rdo.SingleKey, true);
                }

                if (this.rdo.LstHisDebateUser != null)
                {
                    //Tìm chủ tọa và thư ký
                    foreach (var item_User in this.rdo.LstHisDebateUser)
                    {
                        if (item_User.IS_PRESIDENT == 1)
                        {
                            SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                            SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.PRESIDENT_DESCRIPTION, item_User.DESCRIPTION));
                        }

                        if (item_User.IS_SECRETARY == 1)
                        {
                            SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                            SetSingleKey(new KeyValue(Mps000387ExtendSingleKey.SECRETARY_DESCRIPTION, item_User.DESCRIPTION));
                        }
                    }

                    rdo.LstHisDebateUser = rdo.LstHisDebateUser.Where(o => o.IS_SECRETARY != 1 && o.IS_PRESIDENT != 1).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}

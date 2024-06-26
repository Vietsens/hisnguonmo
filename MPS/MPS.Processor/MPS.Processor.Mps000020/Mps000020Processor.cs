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
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000020;
using MPS.Processor.Mps000020.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000020
{
    class Mps000020Processor : AbstractProcessor
    {
        Mps000020PDO rdo;
        public Mps000020Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000020PDO)rdoBase;
        }

        internal void SetBarcodeKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Ham xu ly du lieu da qua xu ly
        /// Tao ra cac doi tuong du lieu xu dung trong thu vien xu ly file excel
        /// </summary>
        /// <returns></returns>
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
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Participants", rdo.lstHisDebateUser);
                if (rdo.currentHisDebate != null)
                {
                    List<ObjData> objList = new List<ObjData>();
                    if (!string.IsNullOrEmpty(rdo.currentHisDebate.PATHOLOGICAL_HISTORY))
                        rdo.currentHisDebate.PATHOLOGICAL_HISTORY.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => objList.Add(new ObjData() { Data = o.Trim() }));
                    objectTag.AddObjectData(store, "PathologicalHistorys", objList);

                    objList = new List<ObjData>();
                    if (!string.IsNullOrEmpty(rdo.currentHisDebate.HOSPITALIZATION_STATE))
                        rdo.currentHisDebate.HOSPITALIZATION_STATE.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => objList.Add(new ObjData() { Data = o.Trim() }));
                    objectTag.AddObjectData(store, "HospitalizationStates", objList);

                    objList = new List<ObjData>();
                    if (!string.IsNullOrEmpty(rdo.currentHisDebate.TREATMENT_TRACKING))
                        rdo.currentHisDebate.TREATMENT_TRACKING.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => objList.Add(new ObjData() { Data = o.Trim() }));
                    objectTag.AddObjectData(store, "TreatmentTrackings", objList);

                    objList = new List<ObjData>();
                    if (!string.IsNullOrEmpty(rdo.currentHisDebate.INTERNAL_MEDICINE_STATE))
                        rdo.currentHisDebate.INTERNAL_MEDICINE_STATE.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => objList.Add(new ObjData() { Data = o.Trim() }));
                    objectTag.AddObjectData(store, "InternalMedicineStates", objList);

                    objList = new List<ObjData>();
                    if (!string.IsNullOrEmpty(rdo.currentHisDebate.PROGNOSIS))
                        rdo.currentHisDebate.PROGNOSIS.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => objList.Add(new ObjData() { Data = o.Trim() }));
                    objectTag.AddObjectData(store, "Prognosiss", objList);

                    objList = new List<ObjData>();
                    if (!string.IsNullOrEmpty(rdo.currentHisDebate.SUBCLINICAL_PROCESSES))
                        rdo.currentHisDebate.SUBCLINICAL_PROCESSES.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => objList.Add(new ObjData() { Data = o.Trim() }));
                    objectTag.AddObjectData(store, "SubclinicalProcessess", objList);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo.currentHisDebate != null && rdo._Treatment != null)
                {
                    string treatmentCode = "TREATMENT_CODE:" + rdo._Treatment.TREATMENT_CODE;
                    string debateId = "DEBATE_ID:" + rdo.currentHisDebate.ID;
                    result = String.Format("Mps000020 {0} {1}", treatmentCode, debateId);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("HIS_CODE: " + result);
            return result;
        }
        void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(rdo.patyAlterBhyt, false);
                if (rdo.Patient != null)
                {
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.GENDER_MALE, rdo.Patient.GENDER_CODE == rdo.genderCode__Male ? "X" : "")));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.GENDER_FEMALE, rdo.Patient.GENDER_CODE == rdo.genderCode__FeMale ? "X" : "")));
                }
                if (rdo.patyAlterBhyt != null && !string.IsNullOrEmpty(rdo.patyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.IS_HEIN, "X")));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.IS_VIENPHI, "")));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_CARD_NUMBER_1, rdo.patyAlterBhyt.HEIN_CARD_NUMBER.Substring(0, 2))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_CARD_NUMBER_2, rdo.patyAlterBhyt.HEIN_CARD_NUMBER.Substring(2, 1))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_CARD_NUMBER_3, rdo.patyAlterBhyt.HEIN_CARD_NUMBER.Substring(3, 2))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_CARD_NUMBER_4, rdo.patyAlterBhyt.HEIN_CARD_NUMBER.Substring(5, 2))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_CARD_NUMBER_5, rdo.patyAlterBhyt.HEIN_CARD_NUMBER.Substring(7, 3))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_CARD_NUMBER_6, rdo.patyAlterBhyt.HEIN_CARD_NUMBER.Substring(10, 5))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.STR_HEIN_CARD_FROM_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.patyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.STR_HEIN_CARD_TO_TIME, Inventec.Common.DateTime.Convert.TimeNumberToDateString((rdo.patyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)))));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_MEDI_ORG_CODE, rdo.patyAlterBhyt.HEIN_MEDI_ORG_CODE)));
                    SetSingleKey((new KeyValue(Mps000020ExtendSingleKey.HEIN_MEDI_ORG_NAME, rdo.patyAlterBhyt.HEIN_MEDI_ORG_NAME)));
                }
                if (rdo.departmentTran != null)
                {
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTran.DEPARTMENT_IN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTran.DEPARTMENT_IN_TIME ?? 0)));
                }

                if (this.rdo.currentHisDebate != null)
                {
                    rdo.lstHisDebateUser = new List<MOS.EFMODEL.DataModels.HIS_DEBATE_USER>();
                    if (this.rdo.currentHisDebate.HIS_DEBATE_USER != null && this.rdo.currentHisDebate.HIS_DEBATE_USER.Count > 0)
                    {
                        //Tìm chủ tọa và thư ký
                        foreach (var item_User in this.rdo.currentHisDebate.HIS_DEBATE_USER)
                        {
                            MOS.EFMODEL.DataModels.HIS_DEBATE_USER hisDebateUser = new MOS.EFMODEL.DataModels.HIS_DEBATE_USER();
                            hisDebateUser = item_User;
                            rdo.lstHisDebateUser.Add(hisDebateUser);

                            if (item_User.IS_PRESIDENT == 1)
                            {
                                SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                                SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.PRESIDENT_DESCRIPTION, item_User.DESCRIPTION));
                            }
                            if (item_User.IS_SECRETARY == 1)
                            {
                                SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                                SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.SECRETARY_DESCRIPTION, item_User.DESCRIPTION));
                            }
                        }
                        rdo.lstHisDebateUser = rdo.lstHisDebateUser.Where(o => o.IS_SECRETARY != 1 && o.IS_PRESIDENT != 1).ToList();
                    }
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.BED_ROOM_NAME, rdo.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEPARTMENT_NAME, rdo.departmentName));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentHisDebate.DEBATE_TIME ?? 0)));

                    AddObjectKeyIntoListkey<HIS_DEBATE>(rdo.currentHisDebate, false);
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEBATE_ID, rdo.currentHisDebate.ID + ""));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEBATE_REASON_NAME, rdo.currentHisDebateView.DEBATE_REASON_NAME));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEBATE_REASON_CODE, rdo.currentHisDebateView.DEBATE_REASON_CODE));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEBATE_TYPE_NAME, rdo.currentHisDebateView.DEBATE_TYPE_NAME));
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.DEBATE_TYPE_CODE, rdo.currentHisDebateView.DEBATE_TYPE_CODE));
                }

                if (rdo.currentDateSeparateFullTime != null)
                {
                    SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, rdo.currentDateSeparateFullTime));
                }
                if (rdo._Treatment != null)
                {
                    //SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.IN_CODE, rdo._Treatment.IN_CODE));
                    //SetSingleKey(new KeyValue(Mps000020ExtendSingleKey.TREATMENT_CODE, rdo._Treatment.TREATMENT_CODE));
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo._Treatment, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

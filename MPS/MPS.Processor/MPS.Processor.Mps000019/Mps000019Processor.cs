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
using FlexCel.Report;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000019;
using MPS.Processor.Mps000019.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000019
{
    class Mps000019Processor : AbstractProcessor
    {
        Mps000019PDO rdo;
        public Mps000019Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000019PDO)rdoBase;
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

                objectTag.SetUserFunction(store, "FuncRownumber", new CustomerFuncRownumberData());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                if (rdo.Patient != null)
                {
                    SetSingleKey((new KeyValue(Mps000019ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000019ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000019ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000019ExtendSingleKey.GENDER_MALE, rdo.Patient.GENDER_CODE == rdo.SingleKey.genderCode__Male ? "X" : "")));
                    SetSingleKey((new KeyValue(Mps000019ExtendSingleKey.GENDER_FEMALE, rdo.Patient.GENDER_CODE == rdo.SingleKey.genderCode__FeMale ? "X" : "")));
                }

                if (rdo.departmentTran != null)
                {
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTran.DEPARTMENT_IN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTran.DEPARTMENT_IN_TIME ?? 0)));
                    // AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                }

                if (this.rdo.currentHisDebate != null)
                {
                    if (this.rdo.lstHisDebateUser != null && this.rdo.lstHisDebateUser.Count > 0)
                    {

                        //Tìm chủ tọa và thư ký
                        foreach (var item_User in this.rdo.lstHisDebateUser)
                        {

                            if (item_User.IS_PRESIDENT == 1)
                            {
                                SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                                SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.PRESIDENT_DESCRIPTION, item_User.DESCRIPTION));
                            }
                            if (item_User.IS_SECRETARY == 1)
                            {
                                SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                                SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.SECRETARY_DESCRIPTION, item_User.DESCRIPTION));
                            }
                        }
                        rdo.lstHisDebateUser = rdo.lstHisDebateUser.Where(o => o.IS_SECRETARY != 1 && o.IS_PRESIDENT != 1).ToList();
                    }

                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.BED_ROOM_NAME, rdo.SingleKey.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKey.departmentName));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.BED_CODE, rdo.SingleKey.BED_CODE));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.BED_NAME, rdo.SingleKey.BED_NAME));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.BEFORE_DIAGNOSTIC,
                        rdo.currentHisDebate.BEFORE_DIAGNOSTIC));

                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.IN_CODE, rdo.SingleKey.IN_CODE));

                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentHisDebate.DEBATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.TREATMENT_TRACKING, rdo.currentHisDebate.TREATMENT_TRACKING));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.CONCLUSION, rdo.currentHisDebate.CONCLUSION));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.TREATMENT_METHOD, rdo.currentHisDebate.TREATMENT_METHOD));
                    if (rdo.currentHisDebate.CONTENT_TYPE != null)
                    {
                        string str = "";
                        switch (rdo.currentHisDebate.CONTENT_TYPE)
                        {
                            case 1:
                                str = "Hội chẩn khác";
                                break;
                            case 2:
                                str = "Hội chẩn thuốc";
                                break;
                            case 3:
                                str = "Hội chẩn trước phẫu thuật";
                                break;
                        }
                        SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.CONTENT_TYPE_NAME, str));
                    }
                    AddObjectKeyIntoListkey<V_HIS_DEBATE>(rdo.currentHisDebate);
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEBATE_ID, rdo.currentHisDebate.ID + ""));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEBATE_REASON_NAME, rdo.currentHisDebate.DEBATE_REASON_NAME));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEBATE_REASON_CODE, rdo.currentHisDebate.DEBATE_REASON_CODE));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEBATE_TYPE_NAME, rdo.currentHisDebate.DEBATE_TYPE_NAME));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.DEBATE_TYPE_CODE, rdo.currentHisDebate.DEBATE_TYPE_CODE));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.SERVICE_CODE, rdo.currentHisDebate.SERVICE_CODE));
                    SetSingleKey(new KeyValue(Mps000019ExtendSingleKey.SERVICE_NAME, rdo.currentHisDebate.SERVICE_NAME));
                }

                if (rdo.treatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.treatment, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class CustomerFuncRownumberData : TFlexCelUserFunction
        {
            public CustomerFuncRownumberData()
            {
            }
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");

                long result = 0;
                try
                {
                    long rownumber = Convert.ToInt64(parameters[0]);
                    result = (rownumber + 1);
                }
                catch (Exception ex)
                {
                    LogSystem.Debug(ex);
                }

                return result;
            }
        }
    }
}

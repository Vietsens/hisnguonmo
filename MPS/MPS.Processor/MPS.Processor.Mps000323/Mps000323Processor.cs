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
using MPS.Processor.Mps000323;
using MPS.Processor.Mps000323.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MPS.Processor.Mps000323
{
    class Mps000323Processor : AbstractProcessor
    {
        Mps000323PDO rdo;
        List<ADOs> PathologicalHistorys = new List<ADOs>();
        List<ADOs> HospitalizationStates = new List<ADOs>();
        List<ADOs> TreatmentTrackings = new List<ADOs>();
        List<ADOs> InternalMedicineStates = new List<ADOs>();
        List<ADOs> Prognosiss = new List<ADOs>();
        List<ADOs> SubclinicalProcessess = new List<ADOs>();
        public Mps000323Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000323PDO)rdoBase;
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
                CreateListKey();
                singleTag.ProcessData(store, singleValueDictionary);
                //barCodeTag.ProcessData(store, dicImage);
                objectTag.AddObjectData(store, "Participants", rdo.lstHisDebateUser);
                objectTag.AddObjectData(store, "PathologicalHistorys", PathologicalHistorys);
                objectTag.AddObjectData(store, "HospitalizationStates", HospitalizationStates);
                objectTag.AddObjectData(store, "TreatmentTrackings", TreatmentTrackings);
                objectTag.AddObjectData(store, "InternalMedicineStates", InternalMedicineStates);
                objectTag.AddObjectData(store, "Prognosiss", Prognosiss);
                objectTag.AddObjectData(store, "SubclinicalProcessess", SubclinicalProcessess);
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
        private void CreateListKey()
        {
            try
            {
                if (rdo.currentHisDebate != null)
                {
                    if (!String.IsNullOrEmpty(rdo.currentHisDebate.PATHOLOGICAL_HISTORY))
                    {
                        rdo.currentHisDebate.PATHOLOGICAL_HISTORY.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => PathologicalHistorys.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.currentHisDebate.HOSPITALIZATION_STATE))
                    {
                        rdo.currentHisDebate.HOSPITALIZATION_STATE.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => HospitalizationStates.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.currentHisDebate.TREATMENT_TRACKING))
                    {
                        rdo.currentHisDebate.TREATMENT_TRACKING.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => TreatmentTrackings.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.currentHisDebate.INTERNAL_MEDICINE_STATE))
                    {
                        rdo.currentHisDebate.INTERNAL_MEDICINE_STATE.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => InternalMedicineStates.Add(new ADOs(o.Trim())));
                    }
                    if (!String.IsNullOrEmpty(rdo.currentHisDebate.PROGNOSIS))
                    {
                        rdo.currentHisDebate.PROGNOSIS.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => Prognosiss.Add(new ADOs(o.Trim()))); ;
                    }
                    if (!String.IsNullOrEmpty(rdo.currentHisDebate.SUBCLINICAL_PROCESSES))
                    {
                        rdo.currentHisDebate.SUBCLINICAL_PROCESSES.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(o => SubclinicalProcessess.Add(new ADOs(o.Trim())));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void SetSingleKey()
        {
            try
            {
                AddObjectKeyIntoListkey<V_HIS_PATIENT>(rdo.Patient);
                if (rdo.Patient != null)
                {
                    SetSingleKey((new KeyValue(Mps000323ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000323ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(rdo.Patient.DOB))));
                    SetSingleKey((new KeyValue(Mps000323ExtendSingleKey.D_O_B, rdo.Patient.DOB.ToString().Substring(0, 4))));
                    SetSingleKey((new KeyValue(Mps000323ExtendSingleKey.GENDER_MALE, rdo.SingleKey != null ? rdo.Patient.GENDER_CODE == rdo.SingleKey.genderCode__Male ? "X" : "" : "")));
                    SetSingleKey((new KeyValue(Mps000323ExtendSingleKey.GENDER_FEMALE, rdo.SingleKey != null ? rdo.Patient.GENDER_CODE == rdo.SingleKey.genderCode__FeMale ? "X" : "" : "")));
                }

                if (rdo.SingleKey != null)
                {
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.BED_ROOM_NAME, rdo.SingleKey.bebRoomName));
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.DEPARTMENT_NAME, rdo.SingleKey.departmentName));
                    AddObjectKeyIntoListkey<Mps000323PDO.Mps000323SingleKey>(rdo.SingleKey, false);
                }

                if (this.rdo.currentHisDebate != null)
                {
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.DB_USE_TIME_FROM, this.rdo.currentHisDebate.MEDICINE_USE_TIME));

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
                                SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.USERNAME_PRESIDENT, item_User.USERNAME));
                                SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.PRESIDENT_DESCRIPTION, item_User.DESCRIPTION));
                            }

                            if (item_User.IS_SECRETARY == 1)
                            {
                                SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.USERNAME_SECRETARY, item_User.USERNAME));
                                SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.SECRETARY_DESCRIPTION, item_User.DESCRIPTION));
                            }
                        }

                        rdo.lstHisDebateUser = rdo.lstHisDebateUser.Where(o => o.IS_SECRETARY != 1 && o.IS_PRESIDENT != 1).ToList();
                    }

                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.BEFORE_DIAGNOSTIC,
                        rdo.currentHisDebate.BEFORE_DIAGNOSTIC));
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.DEBATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.currentHisDebate.DEBATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.TREATMENT_TRACKING, rdo.currentHisDebate.TREATMENT_TRACKING));
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.CONCLUSION, rdo.currentHisDebate.CONCLUSION));
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.TREATMENT_METHOD, rdo.currentHisDebate.TREATMENT_METHOD));
                    AddObjectKeyIntoListkey<HIS_DEBATE>(rdo.currentHisDebate, false);
                }

                if (rdo.departmentTran != null)
                {
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTran.DEPARTMENT_IN_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000323ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rdo.departmentTran.DEPARTMENT_IN_TIME ?? 0)));
                    AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(rdo.departmentTran, false);
                }
                if (rdo.HisTreatment != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_TREATMENT>(rdo.HisTreatment, false);
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

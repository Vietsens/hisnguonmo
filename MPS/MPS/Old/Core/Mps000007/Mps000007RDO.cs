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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using Inventec.Common.LocalStorage.SdaConfig;

namespace MPS.Core.Mps000007
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000007RDO : RDOBase
    {
        //internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal PatientADO Patient { get; set; }
        internal V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        internal HIS_DHST DHST { get; set; }
        internal V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter { get; set; }
        internal V_HIS_EXAM_SERVICE_REQ ExamServiceReqs { get; set; }
        internal V_HIS_TRAN_PATI TranPaties { get; set; }
        internal V_HIS_SERE_SERV sereServs { get; set; }
        internal string userName;
        internal string ratio_text = "";
        internal HIS_TREATMENT currentTreatment { get; set; }
        internal V_HIS_EXAM_SERVICE_REQ_1 vHisExamServiceReq1 { get; set; }
        string executeRoomName;
        string executeDepartmentName;

        public Mps000007RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            //PatyAlterBhytADO patyAlterBhyt,
            V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter,
            HIS_DHST dhsts,
            V_HIS_EXAM_SERVICE_REQ examServiceReqs,
            V_HIS_TRAN_PATI tranPaties,
            V_HIS_SERE_SERV sereServs,
            string userName,
            string ratio_text,
            HIS_TREATMENT currentTreatment
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                //this.PatyAlterBhyt = patyAlterBhyt;
                this.currentHispatientTypeAlter = currentHispatientTypeAlter;
                this.DHST = dhsts;
                this.ExamServiceReqs = examServiceReqs;
                this.TranPaties = tranPaties;
                this.sereServs = sereServs;
                this.userName = userName;
                this.ratio_text = ratio_text;
                this.currentTreatment = currentTreatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000007RDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            //PatyAlterBhytADO patyAlterBhyt,
            V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter,
            V_HIS_EXAM_SERVICE_REQ_1 vHisExamServiceReq1,
            V_HIS_TRAN_PATI tranPaties,
            string userName,
            string ratio_text,
            HIS_TREATMENT currentTreatment,
            string executeRoomName,
            string executeDepartmentName
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                //this.PatyAlterBhyt = patyAlterBhyt;
                this.currentHispatientTypeAlter = currentHispatientTypeAlter;
                this.vHisExamServiceReq1 = vHisExamServiceReq1;
                this.TranPaties = tranPaties;
                this.sereServs = sereServs;
                this.userName = userName;
                this.ratio_text = ratio_text;
                this.currentTreatment = currentTreatment;
                this.executeRoomName = executeRoomName;
                this.executeDepartmentName = executeDepartmentName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.RATIO_NG, ratio_text)));

                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.EXECUTE_DEPARTMENT_NAME, executeDepartmentName)));
                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.EXECUTE_ROOM_NAME, executeRoomName)));

                if (DHST != null)
                {
                    DhstADOPrint dhsts = new DhstADOPrint();
                    if (DHST.BELLY != null)
                        dhsts.BELLY = DHST.BELLY.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.BLOOD_PRESSURE_MAX != null)
                        dhsts.BLOOD_PRESSURE_MAX = DHST.BLOOD_PRESSURE_MAX.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.BLOOD_PRESSURE_MIN != null)
                        dhsts.BLOOD_PRESSURE_MIN = DHST.BLOOD_PRESSURE_MIN.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.BREATH_RATE != null)
                        dhsts.BREATH_RATE = DHST.BREATH_RATE.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.CHEST != null)
                        dhsts.CHEST = DHST.CHEST.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.HEIGHT != null)
                        dhsts.HEIGHT = DHST.HEIGHT.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.PULSE != null)
                        dhsts.PULSE = DHST.PULSE.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.TEMPERATURE != null)
                        dhsts.TEMPERATURE = DHST.TEMPERATURE.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.WEIGHT != null)
                        dhsts.WEIGHT = DHST.WEIGHT.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.VIR_BMI != null)
                        dhsts.VIR_BMI = DHST.VIR_BMI.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.VIR_BODY_SURFACE_AREA != null)
                        dhsts.VIR_BODY_SURFACE_AREA = DHST.VIR_BODY_SURFACE_AREA.Value.ToString("G27", CultureInfo.InvariantCulture);
                    if (DHST.TREATMENT_ID != null)
                        dhsts.TREATMENT_ID = DHST.TREATMENT_ID;
                    if (dhsts != null)
                    {
                        GlobalQuery.AddObjectKeyIntoListkey<DhstADOPrint>(dhsts, keyValues, false);
                    }
                }

                if (currentTreatment != null)
                {
                    if (currentTreatment.CLINICAL_IN_TIME != null)
                    {
                        if (departmentTran != null)
                        {
                            if (departmentTran.DEPARTMENT_CODE == "KCC")
                            {
                                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME))));
                            }
                            else
                            {
                                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.CLINICAL_IN_TIME ?? 0))));
                            }
                        }
                    }
                    else
                    {
                        if (departmentTran != null)
                        {
                            if (departmentTran.DEPARTMENT_CODE == "KCC")
                            {
                                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentTreatment.IN_TIME))));
                            }
                            else
                            {
                                if (vHisExamServiceReq1 != null)
                                {
                                    keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.TIME_IN_STR,
                                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vHisExamServiceReq1.START_TIME ?? 0))));
                                }
                            }
                        }
                    }
                }

                if (vHisExamServiceReq1 != null)
                    keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(vHisExamServiceReq1.INTRUCTION_TIME))));

                if (ExamServiceReqs != null)
                    keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.CREATE_TIME_TRAN, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ExamServiceReqs.INTRUCTION_TIME))));

                if (TranPaties != null && TranPaties.TRAN_PATI_TYPE_ID == 1)
                {
                    if (string.IsNullOrEmpty(TranPaties.ICD_NAME))
                    {
                        keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.ICD_NGT_TEXT, TranPaties.ICD_TEXT)));
                    }
                    else
                    {
                        keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.ICD_NGT_TEXT, TranPaties.ICD_NAME)));
                    }
                    // keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.ICD_NGT_TEXT, TranPaties.ICD_TEXT)));
                }
                keyValues.Add((new KeyValue(Mps000007ExtendSingleKey.LOGIN_USER_NAME, userName)));
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ_1>(vHisExamServiceReq1, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<HIS_TREATMENT>(currentTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_EXAM_SERVICE_REQ>(ExamServiceReqs, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(currentHispatientTypeAlter, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(TranPaties, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_DEPARTMENT_TRAN>(departmentTran, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERE_SERV>(sereServs, keyValues, false);
                //GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    //public class DepartmentCFG
    //{
    //    private static string departmentCC;
    //    public static string DepartmentCC
    //    {
    //        get
    //        {
    //            if (departmentCC == "")
    //            {
    //                departmentCC = SdaConfigs.Get<string>("MRS.HIS_DEPARTMENT.DEPARTMENT_CODE.KCC");
    //            }
    //            return departmentCC;
    //        }
    //        set
    //        {
    //            departmentCC = value;
    //        }
    //    }
    //}
}

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
using MOS.EFMODEL.DataModels;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000001
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    public class Mps000001RDO : RDOBase
    {
        internal V_HIS_PATIENT_TYPE_ALTER PatyAlterBhyt { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReq { get; set; }
        internal HIS_PATIENT_TYPE PatientType { get; set; }
        internal V_HIS_PATIENT currentPatient { get; set; }
        internal string ratio_text = "";
        internal V_HIS_TRAN_PATI currentTranPati { get; set; }
        internal TreatmentADO currentTreatment { get; set; }
        internal string firstExamRoomName { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }

        public Mps000001RDO(V_HIS_SERVICE_REQ serviceReq,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            HIS_PATIENT_TYPE patientType,
            V_HIS_PATIENT currentPatient,
            string ratio_text,
            V_HIS_TRAN_PATI currentTranPati,
            List<V_HIS_SERE_SERV> sereServs)
        {
            try
            {
                ServiceReq = serviceReq;
                PatyAlterBhyt = patyAlterBhyt;
                PatientType = patientType;
                this.currentPatient = currentPatient;
                this.ratio_text = ratio_text;
                this.currentTranPati = currentTranPati;
                this.sereServs = sereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000001RDO(V_HIS_SERVICE_REQ serviceReq,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt,
            HIS_PATIENT_TYPE patientType,
            V_HIS_PATIENT currentPatient,
            string ratio_text,
            V_HIS_TRAN_PATI currentTranPati,
            TreatmentADO treatment,
            string firstExamRoom,
            List<V_HIS_SERE_SERV> sereServs
            )
        {
            try
            {
                ServiceReq = serviceReq;
                PatyAlterBhyt = patyAlterBhyt;
                PatientType = patientType;
                this.currentPatient = currentPatient;
                this.ratio_text = ratio_text;
                this.currentTranPati = currentTranPati;
                this.currentTreatment = treatment;
                this.firstExamRoomName = firstExamRoom;
                this.sereServs = sereServs;
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
                if (ServiceReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReq.FINISH_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReq.START_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        ServiceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(ServiceReq.FINISH_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        ServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        ServiceReq.INTRUCTION_TIME)));

                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(ServiceReq.DOB)));
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.INTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString((ServiceReq.INTRUCTION_TIME))));
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.DOB_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString((ServiceReq.DOB))));

                    if (ServiceReq.ICD_ID != null)
                    {
                        GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReq, keyValues, true);
                        GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(currentTranPati, keyValues, false);
                    }
                    else
                    {
                        GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReq, keyValues, false);
                        if (this.currentTranPati != null)
                        {
                            GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TRAN_PATI>(currentTranPati, keyValues, true);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(firstExamRoomName))
                {
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.FIRST_EXAM_ROOM_NAME, firstExamRoomName));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.FIRST_EXAM_ROOM_NAME, ""));
                }

                keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.LOGIN_USER_NAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));
                keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.RATIO_NG, ratio_text));

                if (PatyAlterBhyt != null)
                {
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.FROM_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_FROM_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.TO_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(PatyAlterBhyt.HEIN_CARD_TO_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.HEIN_CARD_NUMBER_SEPERATOR, GlobalQuery.TrimHeinCardNumber(PatyAlterBhyt.HEIN_CARD_NUMBER)));
                    keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.HEIN_CARD_ADDRESS, PatyAlterBhyt.ADDRESS));
                    if (this.currentTranPati != null && currentTranPati.TRAN_PATI_TYPE_ID == 1)
                    {
                        keyValues.Add(new KeyValue(Mps000001ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, currentTranPati.MEDI_ORG_CODE + "-" + currentTranPati.MEDI_ORG_NAME));
                    }
                }
                GlobalQuery.AddObjectKeyIntoListkey<HIS_PATIENT_TYPE>(PatientType, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT_TYPE_ALTER>(PatyAlterBhyt, keyValues, false);

                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_PATIENT>(currentPatient, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentTreatment, keyValues, false);

                if (sereServs != null && sereServs.Count > 0)
                {
                    sereServs = sereServs.OrderBy(o => o.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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

namespace MPS.Core.Mps000029
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000029RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal TreatmentADO currentHisTreatment { get; set; }
        internal List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran { get; set; }
        internal V_HIS_TRAN_PATI TranPaties { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        internal List<V_HIS_SERE_SERV> sereServs { get; set; }
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        string bebRoomName;
        internal string firstExamRoomName { get; set; }
        internal decimal ratio { get; set; }

        public Mps000029RDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            PatyAlterBhytADO PatyAlterBhyt
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlterBhyt = PatyAlterBhyt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000029RDO(
            V_HIS_SERVICE_REQ ServiceReqPrint,
            List<V_HIS_SERE_SERV> sereServs,
            PatyAlterBhytADO PatyAlterBhyt,
            TreatmentADO treatment,
            string firstExamRoom,
            decimal ratio
            )
        {
            try
            {
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServs = sereServs;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.currentHisTreatment = treatment;
                this.firstExamRoomName = firstExamRoom;
                this.ratio = ratio;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000029RDO(
            PatientADO patient,
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_TRAN_PATI tranPaties,
            List<V_HIS_SERE_SERV> sereServs,
            PatyAlterBhytADO PatyAlterBhyt,
            string bebRoomName,
            TreatmentADO currentHisTreatment
            )
        {
            try
            {
                this.Patient = patient;
                this.lstDepartmentTran = lstDepartmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.TranPaties = tranPaties;
                this.sereServs = sereServs;
                this.PatyAlterBhyt = PatyAlterBhyt;
                this.bebRoomName = bebRoomName;
                this.currentHisTreatment = currentHisTreatment;
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
                if (ServiceReqPrint != null)
                {
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.INTRUCTION_TIME)));
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.AGE, AgeUtil.CalculateFullAge(ServiceReqPrint.DOB)));
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.STR_YEAR, ServiceReqPrint.DOB.ToString().Substring(0, 4)));
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.STR_DOB, Inventec.Common.DateTime.Convert.TimeNumberToDateString(ServiceReqPrint.DOB)));

                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.FINISH_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.FINISH_TIME ?? 0)));
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.START_TIME_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ServiceReqPrint.START_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.FINISH_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        ServiceReqPrint.FINISH_TIME ?? 0) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.FINISH_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(ServiceReqPrint.FINISH_TIME ?? 0)));

                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        ServiceReqPrint.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        ServiceReqPrint.INTRUCTION_TIME)));
                }

                if (!String.IsNullOrEmpty(firstExamRoomName))
                {
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.FIRST_EXAM_ROOM_NAME, firstExamRoomName));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.FIRST_EXAM_ROOM_NAME, ""));
                }

                decimal bhytthanhtoan_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (sereServs != null && sereServs.Count > 0)
                {
                    bhytthanhtoan_tong = sereServs.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = sereServs.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;

                    sereServs = sereServs.OrderBy(o => o.ID).ToList();
                }

                if (PatyAlterBhyt != null)
                {
                    if (PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (PatyAlterBhyt.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (PatyAlterBhyt.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }

                    if (PatyAlterBhyt.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.IS_NOT_HEIN, "X"));
                }

                keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));

                keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RATIO, ratio));
                keyValues.Add(new KeyValue(Mps000029ExtendSingleKey.RATIO_STR, (ratio * 100) + "%"));

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(PatyAlterBhyt, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReqPrint, keyValues, true);
                GlobalQuery.AddObjectKeyIntoListkey<TreatmentADO>(currentHisTreatment, keyValues, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}

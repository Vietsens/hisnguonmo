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
using MOS.SDO;

namespace MPS.Core.Mps000045
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000045RDO : RDOBase
    {
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal string departmentName;
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServ2s;
        internal List<SereServADO> sereServAdos { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        internal V_HIS_TREATMENT currentTreatment { get; set; }
        string currentDateSeparateFullTime;
        internal List<MPS.ADO.ServiceGroupPrintADO> HeinServiceTypes { get; set; }

        // highTech
        internal List<MPS.ADO.ServiceGroupPrintADO> HighTechServiceReports { get; set; }
        internal List<SereServADO> HightTechServices { get; set; }
        internal List<V_HIS_SERE_SERV> HighTechDepartments { get; set; }
        internal List<SereServADO> ServiceInHightTechs { get; set; }

        public Mps000045RDO(
           PatientADO patientADO,
           PatyAlterBhytADO patyAlterBhytADO,
           List<SereServADO> SereServAdos,
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> SereServ2s,

            List<MPS.ADO.ServiceGroupPrintADO> highTechServiceReports,
            List<SereServADO> hightTechServices,
            List<V_HIS_SERE_SERV> highTechDepartments,
            List<SereServADO> serviceInHightTechs,

           List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees,
           string departmentName,
           List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
           MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
           V_HIS_TREATMENT currentTreatment,
           string currentDateSeparateFullTime,
            List<MPS.ADO.ServiceGroupPrintADO> HeinServiceTypes
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.sereServAdos = SereServAdos;
                this.sereServ2s = SereServ2s;

                this.departmentName = departmentName;
                this.treatmentFees = treatmentFees;
                this.departmentTrans = departmentTrans;
                this.hisTranPati = hisTranPati;
                this.currentDateSeparateFullTime = currentDateSeparateFullTime;
                this.currentTreatment = currentTreatment;
                this.HeinServiceTypes = HeinServiceTypes;

                this.HighTechServiceReports = highTechServiceReports;
                this.HightTechServices = hightTechServices;
                this.HighTechDepartments = highTechDepartments;
                this.ServiceInHightTechs = serviceInHightTechs;
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
                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.OPEN_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[0].LOG_TIME)));
                    if (departmentTrans[departmentTrans.Count - 1] != null)
                    {
                        keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.CLOSE_TIME_SEPARATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(departmentTrans[departmentTrans.Count - 1].LOG_TIME)));
                    }
                }

                if (patyAlterBhytADO != null)
                {
                    if (patyAlterBhytADO.IS_HEIN != null)
                        keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.IS_HEIN, "X"));
                    else
                        keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.IS_NOT_HEIN, "X"));

                    if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)// la dung tuyen cap cuu
                        {
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, "X"));
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else if (patyAlterBhytADO.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)// la dung tuyen: gioi thieu,
                        {
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                        else
                        {
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, "X"));
                            keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, ""));
                        }
                    }
                    else if (patyAlterBhytADO.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE)//trai tuyen
                    {
                        keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME_CC, ""));
                        keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.RIGHT_ROUTE_TYPE_NAME, ""));
                        keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.NOT_RIGHT_ROUTE_TYPE_NAME, "X"));
                    }
                }
                else
                    keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.IS_NOT_HEIN, "X"));

                if (hisTranPati != null)
                {
                    keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TRAN_PATI_MEDI_ORG_CODE, hisTranPati.MEDI_ORG_CODE));
                    keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TRAN_PATI_MEDI_ORG_NAME, hisTranPati.MEDI_ORG_NAME));
                }

                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.CURRENT_DATE_SEPARATE_FULL_STR, currentDateSeparateFullTime));

                decimal thanhtien_tong = 0;
                decimal bhytthanhtoan_tong = 0;
                decimal nguonkhac_tong = 0;
                decimal bnthanhtoan_tong = 0;

                if (this.sereServ2s != null && this.sereServ2s.Count > 0)
                {
                    thanhtien_tong = this.sereServ2s.Sum(o => o.VIR_TOTAL_PRICE) ?? 0;
                    bhytthanhtoan_tong = this.sereServ2s.Sum(o => o.VIR_TOTAL_HEIN_PRICE) ?? 0;
                    bnthanhtoan_tong = this.sereServ2s.Sum(o => o.VIR_TOTAL_PATIENT_PRICE) ?? 0;
                    nguonkhac_tong = 0;
                }

                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToStringRoundAuto(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToStringRoundAuto(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_HEIN_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bhytthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_PATIENT_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(bnthanhtoan_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_PRICE_OTHER_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(nguonkhac_tong).ToString())));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.CREATOR_USERNAME, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()));

                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TREATMENT_CODE, currentTreatment.TREATMENT_CODE));

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(currentTreatment, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);

                GlobalQuery.AddObjectKeyIntoListkey<List<MPS.ADO.ServiceGroupPrintADO>>(HighTechServiceReports, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<List<MPS.ADO.SereServADO>>(HightTechServices, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<List<V_HIS_SERE_SERV>>(HighTechDepartments, keyValues);
                GlobalQuery.AddObjectKeyIntoListkey<List<SereServADO>>(ServiceInHightTechs, keyValues);

                if (treatmentFees != null)
                {
                    keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, Inventec.Common.Number.Convert.NumberToStringRoundAuto(treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0, 0)));
                }
                else
                {
                    keyValues.Add(new KeyValue(Mps000045ExtendSingleKey.TOTAL_DEPOSIT_AMOUNT, "0"));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}

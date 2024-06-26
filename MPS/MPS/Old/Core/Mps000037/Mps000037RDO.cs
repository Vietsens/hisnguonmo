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

namespace MPS.Core.Mps000037
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000037RDO : RDOBase
    {
        internal MPS.ADO.HisServiceReqCombo hisServiceReqCombo { get; set; }
        internal List<MPS.ADO.SereServGroupPlusSDO> sereServs_All { get; set; }
        internal List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> lstSereServResult { get; set; }
        internal PatientADO patientADO { get; set; }
        internal PatyAlterBhytADO patyAlterBhytADO { get; set; }
        internal string bebRoomName;
        internal string departmentName;
        internal V_HIS_SERVICE_REQ lstServiceReq { get; set; }
        internal List<SereServGroupPlusSDO> ExecuteRoomGroups { get; set; }
        internal decimal ratio { get; set; }

        public Mps000037RDO(
           MPS.ADO.HisServiceReqCombo hisServiceReqCombo,
            List<MPS.ADO.SereServGroupPlusSDO> sereServs_All,
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> lstSereServResult,
            PatientADO patientADO,
            PatyAlterBhytADO patyAlterBhytADO,
            string bebRoomName,
            string departmentName,
            V_HIS_SERVICE_REQ lstServiceReq,
            decimal ratio
            )
        {
            try
            {
                this.hisServiceReqCombo = hisServiceReqCombo;
                this.sereServs_All = sereServs_All;
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.lstSereServResult = lstSereServResult;
                this.bebRoomName = bebRoomName;
                this.departmentName = departmentName;
                this.lstServiceReq = lstServiceReq;
                this.ratio = ratio;
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


                decimal thanhtien_tong = 0, bhytthanhtoan_tong = 0, nguonkhac_tong = 0, bnthanhtoan_tong = 0;
                for (int i = 0; i < lstSereServResult.Count; i++)
                {
                    thanhtien_tong += lstSereServResult[i].VIR_TOTAL_PRICE ?? 0;
                    bhytthanhtoan_tong += lstSereServResult[i].VIR_TOTAL_HEIN_PRICE ?? 0;
                    bnthanhtoan_tong += lstSereServResult[i].VIR_TOTAL_PATIENT_PRICE ?? 0;
                }

                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE, Inventec.Common.Number.Convert.NumberToString(thanhtien_tong, 0)));
                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_HEIN, Inventec.Common.Number.Convert.NumberToString(bhytthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_PATIENT, Inventec.Common.Number.Convert.NumberToString(bnthanhtoan_tong, 0)));
                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_OTHER, Inventec.Common.Number.Convert.NumberToString(nguonkhac_tong, 0)));
                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(Math.Round(thanhtien_tong).ToString())));

                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.BED_ROOM_NAME, bebRoomName));
                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.DEPARTMENT_NAME, departmentName));
                
                if (hisServiceReqCombo != null)
                {
                    keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.INSTRUCTION_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(hisServiceReqCombo.INTRUCTION_TIME)));

                    GlobalQuery.AddObjectKeyIntoListkey<HisServiceReqCombo>(hisServiceReqCombo, keyValues, false);
                }

                if (lstServiceReq != null)
                {

                    keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.INTRUCTION_TIME_FULL_STR,
                        GlobalQuery.GetCurrentTimeSeparateBeginTime(
                        Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(
                        lstServiceReq.INTRUCTION_TIME) ?? DateTime.MinValue)));

                    keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.INTRUCTION_DATE_FULL_STR,
                        Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(
                        lstServiceReq.INTRUCTION_TIME)));
                }

                GlobalQuery.AddObjectKeyIntoListkey<PatyAlterBhytADO>(patyAlterBhytADO, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(patientADO, keyValues);

                if (lstServiceReq != null)
                {
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(lstServiceReq, keyValues);
                }

                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.RATIO, ratio));
                keyValues.Add(new KeyValue(Mps000037ExtendSingleKey.RATIO_STR, (ratio * 100) + "%"));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ProcessGroupSereServ()
        {
            try
            {


                ExecuteRoomGroups = new List<SereServGroupPlusSDO>();
                var sExecuteRoomGroups = sereServs_All.GroupBy(o => o.EXECUTE_ROOM_ID).ToList();
                foreach (var sExecuteRoomGroup in sExecuteRoomGroups)
                {
                    SereServGroupPlusSDO itemSExecuteGroup = sExecuteRoomGroup.First();
                    ExecuteRoomGroups.Add(itemSExecuteGroup);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}

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
using MPS.Processor.Mps000285.PDO.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000285
{
    public class PatientTypeAlterProcessor
    {
        public static HIS_PATIENT_TYPE_ALTER GetPatientTypeAlter(HIS_SERE_SERV s, 
            List<HIS_SERVICE_REQ> serviceReqs, 
            HisConfigValue hisConfigValue, 
            PatientTypeCFG cfg, 
            List<HIS_PATIENT_TYPE_ALTER> ListPta, 
            long treatmentTypeId, 
            ref string key)
        {
            HIS_PATIENT_TYPE_ALTER result = null;
            try
            {
                if (s.IS_NO_EXECUTE != 1)
                {
                    if (s.PATIENT_TYPE_ID == cfg.PATIENT_TYPE__BHYT && s.JSON_PATIENT_TYPE_ALTER != null)
                    {
                        result = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(s.JSON_PATIENT_TYPE_ALTER);
                        if (result != null)
                        {
                            key = ToString(result, s, treatmentTypeId);
                        }
                    }
                    if (hisConfigValue != null && hisConfigValue.IsGroupHeinServiceByUseTime)
                    {
                        if (s.SERVICE_REQ_ID.HasValue && serviceReqs != null && serviceReqs.Count > 0 && ListPta != null && ListPta.Count > 0)
                        {
                            HIS_SERVICE_REQ sr = serviceReqs.FirstOrDefault(x => x.ID == s.SERVICE_REQ_ID.Value);
                            long useTime = (sr != null && sr.USE_TIME.HasValue) ? sr.USE_TIME.Value : 0;
                            if (useTime > 0)
                            {
                                HIS_PATIENT_TYPE_ALTER ptaApplied = ListPta
                                    .Where(x =>
                                        (!x.HEIN_CARD_FROM_TIME.HasValue || x.HEIN_CARD_FROM_TIME.Value <= useTime)
                                        && (!x.HEIN_CARD_TO_TIME.HasValue || x.HEIN_CARD_TO_TIME.Value >= useTime))
                                    .OrderByDescending(x => x.HEIN_CARD_FROM_TIME ?? 0)
                                    .ThenByDescending(x => x.LOG_TIME)
                                    .FirstOrDefault();
                                if (ptaApplied != null)
                                {
                                    result = ptaApplied;
                                    key = PatientTypeAlterProcessor.ToString(ptaApplied, s, treatmentTypeId);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public static string ToString(HIS_PATIENT_TYPE_ALTER patyAlter, HIS_SERE_SERV s, long treatmentTypeId)
        {
            if (patyAlter != null)
            {
                string key = NVL(patyAlter.HEIN_CARD_NUMBER) + "|"
                    + NVL(patyAlter.HEIN_MEDI_ORG_CODE) + "|"
                    + NVL(patyAlter.LEVEL_CODE) + "|"
                    + NVL(patyAlter.RIGHT_ROUTE_CODE) + "|"
                    + NVL(patyAlter.RIGHT_ROUTE_TYPE_CODE) + "|"
                    + NVL(patyAlter.JOIN_5_YEAR) + "|"
                    + NVL(patyAlter.PAID_6_MONTH) + "|"
                    + NVL(patyAlter.LIVE_AREA_CODE) + "|"
                    + NVL(patyAlter.HNCODE) + "|"
                    + NVL((patyAlter.HEIN_CARD_FROM_TIME ?? 0).ToString()) + "|"
                    + NVL((patyAlter.HEIN_CARD_TO_TIME ?? 0).ToString());

                if (patyAlter.LEVEL_CODE == MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE
                    && patyAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE
                    && treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    if (s.TDL_INTRUCTION_DATE < 20210101000000)
                    {
                        key += "|false";
                    }
                    else
                    {
                        key += "|true";
                    }
                }
                return key;
            }
            return null;
        }

        private static string ToString(HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            if (patyAlter != null)
            {
                return NVL(patyAlter.HEIN_CARD_NUMBER) + "|"
                    + NVL(patyAlter.HEIN_MEDI_ORG_CODE) + "|"
                    + NVL(patyAlter.LEVEL_CODE) + "|"
                    + NVL(patyAlter.RIGHT_ROUTE_CODE) + "|"
                    + NVL(patyAlter.RIGHT_ROUTE_TYPE_CODE) + "|"
                    + NVL(patyAlter.JOIN_5_YEAR) + "|"
                    + NVL(patyAlter.PAID_6_MONTH) + "|"
                    + NVL(patyAlter.LIVE_AREA_CODE) + "|"
                    + NVL(patyAlter.HNCODE)
                    + NVL((patyAlter.HEIN_CARD_FROM_TIME ?? 0).ToString())
                    + NVL((patyAlter.HEIN_CARD_TO_TIME ?? 0).ToString()); ;
            }
            return null;
        }

        private static string NVL(string s)
        {
            return !string.IsNullOrWhiteSpace(s) ? s : "";
        }
    }
}

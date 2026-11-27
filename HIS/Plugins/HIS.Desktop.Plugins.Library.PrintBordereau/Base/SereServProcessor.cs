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
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Base
{
    public class SereServProcessor
    {
        public static Dictionary<string, List<HIS_SERE_SERV>> GroupSereServByPatyAlterBhyt(List<HIS_SERE_SERV> listSereServ)
        {
            Dictionary<string, List<HIS_SERE_SERV>> dicSereServ = new Dictionary<string, List<HIS_SERE_SERV>>();
            try
            {
                listSereServ = listSereServ.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                if (listSereServ == null || listSereServ.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay dich vu nao duoc thuc hien");
                    return null;
                }

                foreach (HIS_SERE_SERV s in listSereServ)
                {
                    if (s.PATIENT_TYPE_ID == HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT && s.JSON_PATIENT_TYPE_ALTER != null)
                    {
                        HIS_PATIENT_TYPE_ALTER patyAlter = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(s.JSON_PATIENT_TYPE_ALTER);
                        if (patyAlter != null)
                        {
                            string key = ToString(patyAlter);
                            List<HIS_SERE_SERV> list;
                            if (dicSereServ.ContainsKey(key))
                            {
                                list = dicSereServ[key];
                            }
                            else
                            {
                                list = new List<HIS_SERE_SERV>();
                                dicSereServ.Add(key, list);
                            }
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dicSereServ = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return dicSereServ;
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

        public static string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode, string facilityClassCode, string formerLevelCode = null, long point = 0, long clinicalInTime = 0)
        {
            string result = "";
            try
            {
                // TT BHYT moi: truyen CLINICAL_IN_TIME
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode, facilityClassCode, formerLevelCode, point, clinicalInTime) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal const string KEY__IsAllowOddConvertUnitAmount = "HIS.Desktop.Plugins.AssignPrescription.IsAllowOddConvertUnitAmount";

        /// <summary>
        /// #48132: Bat cau hinh IsAllowOddConvertUnitAmount -> dong thuoc co don vi quy doi (khong ke theo don vi goc) hien thi tren bang ke theo don vi quy doi:
        /// - Don vi tinh = don vi quy doi; So luong = so luong goc * ty le (lam tron 6 - so chu so phan nguyen cua ty le);
        /// - Cac cot don gia (ten co PRICE, khong phai cot TOTAL) = don gia goc / ty le; cac cot thanh tien (TOTAL) giu nguyen.
        /// Khong sua doi tuong goc va danh muc dich vu dung chung: tao ban sao. convertedItems danh dau cac dong da doi de khong doi lan 2 khi in lai.
        /// </summary>
        public static void ProcessConvertUnit(List<HIS_SERE_SERV> sereServs, List<V_HIS_SERVICE> services, HashSet<HIS_SERE_SERV> convertedItems, out List<HIS_SERE_SERV> resultSereServs, out List<V_HIS_SERVICE> resultServices)
        {
            resultSereServs = sereServs;
            resultServices = services;
            try
            {
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KEY__IsAllowOddConvertUnitAmount) != "1"
                    || sereServs == null || sereServs.Count == 0 || convertedItems == null)
                    return;

                List<HIS_SERVICE_UNIT> units = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                if (units == null || units.Count == 0)
                    return;
                Dictionary<long, HIS_SERVICE_UNIT> dicUnit = units.GroupBy(o => o.ID).ToDictionary(o => o.Key, o => o.First());

                //Dich vu co dong ke theo don vi goc thi giu nguyen ca dich vu (bang ke lay ten don vi theo dich vu)
                HashSet<long> originalServiceIds = new HashSet<long>(sereServs.Where(o => o != null && o.MEDICINE_ID.HasValue && o.USE_ORIGINAL_UNIT_FOR_PRES == 1).Select(o => o.SERVICE_ID));

                List<HIS_SERE_SERV> newSereServs = new List<HIS_SERE_SERV>();
                Dictionary<long, HIS_SERVICE_UNIT> dicServiceConvertUnit = new Dictionary<long, HIS_SERVICE_UNIT>();
                foreach (var ss in sereServs)
                {
                    HIS_SERVICE_UNIT unit = null;
                    HIS_SERVICE_UNIT convertUnit = null;
                    bool canConvert = ss != null && ss.MEDICINE_ID.HasValue && ss.USE_ORIGINAL_UNIT_FOR_PRES != 1 && !originalServiceIds.Contains(ss.SERVICE_ID)
                        && dicUnit.TryGetValue(ss.TDL_SERVICE_UNIT_ID, out unit) && unit.CONVERT_ID.HasValue && (unit.CONVERT_RATIO ?? 0) > 0
                        && dicUnit.TryGetValue(unit.CONVERT_ID.Value, out convertUnit);
                    if (!canConvert)
                    {
                        newSereServs.Add(ss);
                        continue;
                    }

                    dicServiceConvertUnit[ss.SERVICE_ID] = convertUnit;
                    if (convertedItems.Contains(ss))
                    {
                        newSereServs.Add(ss);
                        continue;
                    }

                    HIS_SERE_SERV clone = CloneConvertSereServ(ss, unit.CONVERT_RATIO.Value);
                    convertedItems.Add(clone);
                    newSereServs.Add(clone);
                }

                if (dicServiceConvertUnit.Count == 0)
                    return;

                List<V_HIS_SERVICE> newServices = services;
                if (services != null)
                {
                    newServices = new List<V_HIS_SERVICE>(services.Count);
                    foreach (var sv in services)
                    {
                        HIS_SERVICE_UNIT convertUnit = null;
                        if (sv != null && dicServiceConvertUnit.TryGetValue(sv.ID, out convertUnit) && sv.SERVICE_UNIT_NAME != convertUnit.SERVICE_UNIT_NAME)
                        {
                            V_HIS_SERVICE svClone = CloneObject(sv);
                            svClone.SERVICE_UNIT_CODE = convertUnit.SERVICE_UNIT_CODE;
                            svClone.SERVICE_UNIT_NAME = convertUnit.SERVICE_UNIT_NAME;
                            newServices.Add(svClone);
                        }
                        else
                            newServices.Add(sv);
                    }
                }

                resultSereServs = newSereServs;
                resultServices = newServices;
                Inventec.Common.Logging.LogSystem.Info("#48132 ProcessConvertUnit: so dich vu doi sang don vi quy doi = " + dicServiceConvertUnit.Count);
            }
            catch (Exception ex)
            {
                resultSereServs = sereServs;
                resultServices = services;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static HIS_SERE_SERV CloneConvertSereServ(HIS_SERE_SERV data, decimal ratio)
        {
            HIS_SERE_SERV clone = CloneObject(data);
            decimal intPart = Math.Floor(Math.Abs(ratio));
            int intDigits = intPart >= 1 ? intPart.ToString("0", System.Globalization.CultureInfo.InvariantCulture).Length : 0;
            int digits = 6 - intDigits;
            if (digits < 0) digits = 0;

            clone.AMOUNT = Math.Round(data.AMOUNT * ratio, digits, MidpointRounding.AwayFromZero);
            if (data.AMOUNT_TEMP.HasValue)
                clone.AMOUNT_TEMP = Math.Round(data.AMOUNT_TEMP.Value * ratio, digits, MidpointRounding.AwayFromZero);

            foreach (var pi in typeof(HIS_SERE_SERV).GetProperties())
            {
                if (!pi.CanRead || !pi.CanWrite || !pi.Name.Contains("PRICE") || pi.Name.Contains("TOTAL"))
                    continue;
                if (pi.PropertyType == typeof(decimal))
                {
                    pi.SetValue(clone, (decimal)pi.GetValue(data) / ratio);
                }
                else if (pi.PropertyType == typeof(decimal?))
                {
                    decimal? v = (decimal?)pi.GetValue(data);
                    if (v.HasValue)
                        pi.SetValue(clone, (decimal?)(v.Value / ratio));
                }
            }
            return clone;
        }

        private static T CloneObject<T>(T data) where T : new()
        {
            T clone = new T();
            foreach (var pi in typeof(T).GetProperties())
            {
                if (pi.CanRead && pi.CanWrite && pi.GetIndexParameters().Length == 0)
                    pi.SetValue(clone, pi.GetValue(data));
            }
            return clone;
        }
    }
}

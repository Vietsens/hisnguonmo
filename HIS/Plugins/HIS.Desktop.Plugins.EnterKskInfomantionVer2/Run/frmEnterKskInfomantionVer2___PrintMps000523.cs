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
/*
 * Task 60377 (BV Xuyên Á Củ Chi): "Giấy khám sức khỏe định kỳ cho người cao tuổi" — hospital form "Mẫu 4"
 * of the HCMC health department program (body: Mẫu 03 TT 25/2026/TT-BYT), printed as Mps000523.
 * The "In phiếu" button of tab "Ksk trên 18 tuổi" prints it instead of Mps000452 when the record is an
 * elderly M4 record (same rule as the portal push: SYT_PATIENT_TYPES holds "Người cao tuổi") and the
 * template of Mps000523 is installed on this workstation; otherwise Mps000452 as before.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private const string PRINT_TYPE_CODE__MPS000523 = "Mps000523";
        private const string SYT_ELDERLY_NAME = "Người cao tuổi";
        /// <summary>CLS map of the HCMC portal, kept in the ControlState of the KSK sync module (per workstation).</summary>
        private const string KSK_SYNC_MODULE_LINK = "HIS.Desktop.Plugins.KskSyncList";
        private const string KSK_SYNC_CLS_MAP_KEY = "KskSytClsMap";
        private const string CLS_MAP_KIND__TEST_INDEX = "XN";

        /// <summary>Interview JSON keys stored as portal catalog ids -> catalog code.</summary>
        private static readonly Dictionary<string, string> PRINT_M4_JSON_CATALOGS = BuildJsonCatalogMap();

        private static Dictionary<string, string> BuildJsonCatalogMap()
        {
            var rs = new Dictionary<string, string>();
            foreach (string k in new[] { "rltc_ithungthu", "rltc_channan", "rltc_khongu", "rltc_metmoi", "rltc_khongngonmieng", "rltc_camthayte",
                "rltc_khotaptrung", "rltc_chamchap", "rltc_ynghi", "rlla_canthang", "rlla_kiemsoat_lolang", "rlla_lolang_nhieuthu",
                "rlla_khothugian", "rlla_bucrut", "rlla_bucboi", "rlla_lolang" })
                rs[k] = IHCM_CAT__TAN_SUAT;
            rs["ttsy_metmoi"] = IHCM_CAT__SUY_YEU;
            foreach (string k in new[] { "benh_tanghuyetap", "benh_daithaoduong", "benh_phoi", "benh_hen", "benh_ungthu", "benh_suytim", "benh_khop",
                "benh_thanmam", "benh_timthieumau", "benh_roiloan_tramcam", "benh_roiloan_loau", "benh_sasut_tritue" })
                rs[k + "_cskcb"] = IHCM_CAT__CSKCB;
            rs[WPHCM_FIELD__WORK_PLACE] = WPHCM_CAT__WORK_PLACE;
            rs[WPHCM_FIELD__WORK_PLACE_WARD] = WPHCM_CAT__WARD;
            return rs;
        }

        // ================================================================ which form

        /// <summary>Print Mps000523 instead of Mps000452 for an elderly M4 record when its template is installed here.</summary>
        private bool IsElderlyM4Print()
        {
            try
            {
                if (!IsSytHcmDeclared() || currentKskOverEight == null || currentKskSytHcm == null) return false;
                if (!IsElderlyPatientType()) return false;
                if (!IsPrintTemplateInstalled(PRINT_TYPE_CODE__MPS000523))
                {
                    LogSystem.Warn("Mps000523: ho so nguoi cao tuoi nhung may chua co mau in Tmp/Mps/Mps000523 -> in Mps000452");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>Same rule as the portal push (KskSytHcmBodyBuilder.IsElderlyForm): "Người cao tuổi" among the patient types.</summary>
        private bool IsElderlyPatientType()
        {
            string sytTypes = GetSytHcmStr(currentKskSytHcm, "SYT_PATIENT_TYPES");
            if (!string.IsNullOrWhiteSpace(sytTypes))
            {
                SytCatalogItem elderly = GetSytCatalog(SYT_CODE__DOI_TUONG_KHAM).FirstOrDefault(o => o != null && NormName(o.Name) == NormName(SYT_ELDERLY_NAME));
                return elderly != null && SplitIds(sytTypes).Contains((elderly.Id ?? "").Trim());
            }
            // Saved while the portal catalog was missing: HIS codes (QĐ 1551) in KSK_PATIENT_TYPES.
            KskCodeNameADO hisElderly = BuildKskObjectList().FirstOrDefault(o => NormName(o.NAME) == NormName(SYT_ELDERLY_NAME));
            return hisElderly != null && SplitIds(currentKskOverEight.KSK_PATIENT_TYPES).Contains(hisElderly.ID.ToString());
        }

        private static bool IsPrintTemplateInstalled(string printTypeCode)
        {
            try
            {
                string root = Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath ?? "";
                if (!Path.IsPathRooted(root)) root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, root);
                string dir = Path.Combine(root, printTypeCode);
                return Directory.Exists(dir) && Directory.GetFiles(dir).Any(f => !Path.GetFileName(f).StartsWith("~$"));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        // ================================================================ PDO

        private void LoadBieuMauPhieuMps000523(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT_4 treatment = null;
                if (currentServiceReq.TREATMENT_ID > 0)
                {
                    var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4",
                        ApiConsumers.MosConsumer, new HisTreatmentView4Filter { ID = currentServiceReq.TREATMENT_ID }, param);
                    treatment = treatments != null ? treatments.FirstOrDefault() : null;
                }

                MPS.Processor.Mps000523.PDO.Mps000523PDO rdo = new MPS.Processor.Mps000523.PDO.Mps000523PDO(
                    currentServiceReq, currentKskOverEight, currentKskSytHcm, BuildKskGeneralForPrint(), GetDhstForM4Print(param), treatment,
                    BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                rdo.KskServiceReq = printKskServiceReq;
                rdo.KskPatient = printKskPatient;
                rdo.PersonalHistories = BuildM4PersonalHistories(param);
                rdo.InterviewAnswers = ParseInterviewAnswers(GetSytHcmStr(currentKskSytHcm, IHCM_COL__JSON));
                rdo.ResolvedNames = BuildM4ResolvedNames(rdo.InterviewAnswers);
                rdo.PatientTypesText = BuildM4PatientTypesText();
                rdo.EmployeeNames = BuildM4EmployeeNames();
                rdo.ClsValues = BuildM4ClsValues(param);
                WaitingManager.Hide();
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private HIS_DHST GetDhstForM4Print(CommonParam param)
        {
            try
            {
                if (currentKskOverEight == null || !(currentKskOverEight.DHST_ID > 0)) return null;
                List<HIS_DHST> cached = PreGetDhst(currentKskOverEight.DHST_ID);
                if (cached != null && cached.Count > 0) return cached[0];
                var list = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer,
                    new HisDhstFilter { ID = currentKskOverEight.DHST_ID }, param);
                return list != null ? list.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// 22 rows of the form = HIS_DISEASE_TYPE (IS_KSK_OVER_EIGHTEEN) in code order + the SAVED answers
        /// (HIS_PERIOD_DRIVER_DITY of the record), like every other part of this print.
        /// </summary>
        private List<MPS.Processor.Mps000523.PDO.Mps000523HistoryADO> BuildM4PersonalHistories(CommonParam param)
        {
            var rs = new List<MPS.Processor.Mps000523.PDO.Mps000523HistoryADO>();
            try
            {
                List<HIS_DISEASE_TYPE> types = preDiseaseTypesOverE;
                if (types == null || types.Count == 0)
                    types = new BackendAdapter(param).Get<List<HIS_DISEASE_TYPE>>("api/HisDiseaseType/Get", ApiConsumers.MosConsumer,
                        new HisDiseaseTypeFilter { IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE, IS_KSK_OVER_EIGHTEEN = 1 }, param);
                if (types == null || currentKskOverEight == null) return rs;
                var ditys = new BackendAdapter(param).Get<List<HIS_PERIOD_DRIVER_DITY>>("api/HisPeriodDriverDity/Get", ApiConsumers.MosConsumer,
                    new HisPeriodDriverDityFilter { KSK_OVER_EIGHTEEN_ID = currentKskOverEight.ID }, param) ?? new List<HIS_PERIOD_DRIVER_DITY>();
                Dictionary<long, string> answers = ditys.GroupBy(o => o.DISEASE_TYPE_ID).ToDictionary(g => g.Key, g => g.First().IS_YES_NO);
                foreach (HIS_DISEASE_TYPE t in types.OrderBy(o => o.DISEASE_TYPE_CODE))
                {
                    string yn;
                    answers.TryGetValue(t.ID, out yn);
                    rs.Add(new MPS.Processor.Mps000523.PDO.Mps000523HistoryADO { DISEASE_TYPE_CODE = t.DISEASE_TYPE_CODE, DISEASE_TYPE_NAME = t.DISEASE_TYPE_NAME, IS_YES_NO = yn });
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return rs;
        }

        private static Dictionary<string, string> ParseInterviewAnswers(string json)
        {
            var rs = new Dictionary<string, string>();
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return rs;
                Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(json);
                foreach (var p in o.Properties())
                {
                    if (p.Value == null || p.Value.Type == Newtonsoft.Json.Linq.JTokenType.Null) continue;
                    rs[p.Name] = Convert.ToString(((Newtonsoft.Json.Linq.JValue)p.Value).Value, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Mps000523: INTERVIEW_JSON khong doc duoc -> in trong muc hoi benh", ex);
            }
            return rs;
        }

        /// <summary>Display names of every catalog value of the record, keyed by the field that stores it (see Mps000523PDO).</summary>
        private Dictionary<string, string> BuildM4ResolvedNames(Dictionary<string, string> answers)
        {
            var rs = new Dictionary<string, string>();
            try
            {
                foreach (var kv in answers)
                {
                    string catalog;
                    if (PRINT_M4_JSON_CATALOGS.TryGetValue(kv.Key, out catalog)) PutName(rs, kv.Key, SytName(catalog, kv.Value));
                }
                var h = currentKskSytHcm;
                PutName(rs, "SYT_PAYSOURCE_ID", SytName(SYT_CODE__NGUON_CHI_TRA, Convert.ToString(GetSytHcmLong(h, "SYT_PAYSOURCE_ID"))));
                PutName(rs, "SYT_PAY_SOURCE_DETAIL_ID", SytName(SYT_CODE__HINH_THUC_CHI_TRA_CT, Convert.ToString(GetSytHcmLong(h, "SYT_PAY_SOURCE_DETAIL_ID"))));
                PutName(rs, "SYT_EXAM_PLACE_ID", SytOrLocalName(SYT_CODE__DIA_DIEM_KHAM, GetSytHcmLong(h, "SYT_EXAM_PLACE_ID"), BuildKskExamPlaceList()));
                PutName(rs, "SYT_SUGGEST_ID", SytOrLocalName(SYT_CODE__KET_LUAN_DE_NGHI, GetSytHcmLong(h, "SYT_SUGGEST_ID"), BuildKskSuggestList()));
                short? hisPay = currentKskOverEight != null ? currentKskOverEight.KSK_PAY_SOURCE : null;
                KskCodeNameADO pay = hisPay.HasValue ? BuildKskPaymentSourceList().FirstOrDefault(o => o.ID == hisPay.Value) : null;
                PutName(rs, "KSK_PAY_SOURCE", pay != null ? pay.NAME : null);
                PutName(rs, "PATHOLOGICAL_HISTORY_FAMILY", FamilyHistoryNames(currentKskOverEight != null ? currentKskOverEight.PATHOLOGICAL_HISTORY_FAMILY : null));
                foreach (int tooth in new[] { 18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28, 48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38 })
                    PutName(rs, "TOOTH_" + tooth, ToothStatusName(GetSytHcmLong(h, "TOOTH_" + tooth)));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return rs;
        }

        private static void PutName(Dictionary<string, string> target, string field, string name)
        {
            if (!string.IsNullOrWhiteSpace(name)) target[field] = name.Trim();
        }

        private static string SytName(string catalog, string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            SytCatalogItem it = GetSytCatalog(catalog).FirstOrDefault(o => o != null && (o.Id ?? "").Trim() == id.Trim());
            return it != null ? it.Name : null;
        }

        /// <summary>Portal name; a value saved from the temporary HIS list (portal catalog not loaded yet) falls back to that list.</summary>
        private static string SytOrLocalName(string catalog, long? id, List<KskCodeNameADO> localList)
        {
            if (!id.HasValue) return null;
            string name = SytName(catalog, id.Value.ToString());
            if (name != null) return name;
            KskCodeNameADO local = localList.FirstOrDefault(o => o.ID == id.Value);
            return local != null ? local.NAME : null;
        }

        /// <summary>Family history saved as portal ids ("12,15") becomes names; free text is kept as typed.</summary>
        private static string FamilyHistoryNames(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            List<string> ids = SplitIds(value);
            if (ids.Count == 0 || ids.Any(o => !o.All(char.IsDigit))) return value.Trim();
            List<string> names = ids.Select(o => SytName(SYT_CODE__TS_GIA_DINH, o)).Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            return names.Count > 0 ? string.Join("; ", names) : value.Trim();
        }

        /// <summary>Tooth status: portal catalog id, else an old HIS code (1-10) of the screen.</summary>
        private static string ToothStatusName(long? code)
        {
            if (!code.HasValue) return null;
            string name = SytName(SYT_CODE__TINH_TRANG_RANG, code.Value.ToString());
            if (name != null) return name;
            HcmToothStatus his = HCM_TOOTH_STATUSES_HIS.FirstOrDefault(o => o.Code == code.Value);
            return his != null ? his.Name : null;
        }

        /// <summary>"Đối tượng": "code. name" of the portal catalog (fallback: HIS list QĐ 1551), several joined by "; ".</summary>
        private string BuildM4PatientTypesText()
        {
            try
            {
                var parts = new List<string>();
                string sytTypes = GetSytHcmStr(currentKskSytHcm, "SYT_PATIENT_TYPES");
                if (!string.IsNullOrWhiteSpace(sytTypes))
                {
                    List<SytCatalogItem> catalog = GetSytCatalog(SYT_CODE__DOI_TUONG_KHAM);
                    foreach (string id in SplitIds(sytTypes))
                    {
                        SytCatalogItem it = catalog.FirstOrDefault(o => o != null && (o.Id ?? "").Trim() == id);
                        parts.Add(it == null ? id : (string.IsNullOrWhiteSpace(it.Code) ? it.Name : it.Code.Trim() + ". " + it.Name));
                    }
                }
                else if (currentKskOverEight != null)
                {
                    List<KskCodeNameADO> his = BuildKskObjectList();
                    foreach (string id in SplitIds(currentKskOverEight.KSK_PATIENT_TYPES))
                    {
                        KskCodeNameADO it = his.FirstOrDefault(o => o.ID.ToString() == id);
                        parts.Add(it != null ? it.ID + ". " + it.NAME : id);
                    }
                }
                return string.Join("; ", parts);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>LOGINNAME -> user name of every doctor referenced by the record (from the employee cache).</summary>
        private Dictionary<string, string> BuildM4EmployeeNames()
        {
            var rs = new Dictionary<string, string>();
            try
            {
                var logins = new HashSet<string>();
                if (currentKskOverEight != null)
                    foreach (var pi in typeof(HIS_KSK_OVER_EIGHTEEN).GetProperties())
                        if (pi.PropertyType == typeof(string) && (pi.Name.EndsWith("_LOGINNAME") || pi.Name == "TEST_BLOOD"))
                            AddLogin(logins, pi.GetValue(currentKskOverEight, null) as string);
                AddLogin(logins, GetSytHcmStr(currentKskSytHcm, "EXAM_GYNECOLOGY_LOGINNAME"));
                AddLogin(logins, currentKskGeneral != null ? currentKskGeneral.CONCLUDER_LOGINNAME : null);
                if (logins.Count == 0) return rs;
                foreach (V_HIS_EMPLOYEE e in BackendDataWorker.Get<V_HIS_EMPLOYEE>())
                    if (e != null && e.LOGINNAME != null && logins.Contains(e.LOGINNAME) && !string.IsNullOrWhiteSpace(e.TDL_USERNAME))
                        rs[e.LOGINNAME] = e.TDL_USERNAME;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return rs;
        }

        private static void AddLogin(HashSet<string> target, string login)
        {
            if (!string.IsNullOrWhiteSpace(login)) target.Add(login.Trim());
        }

        /// <summary>
        /// Lab values per portal field (xnm_*, shm_*, xnnt_*) through the CLS map of the KSK sync module on this
        /// workstation — the same source as the portal push. No map here = empty (the processor prints the screen memo).
        /// </summary>
        private Dictionary<string, string> BuildM4ClsValues(CommonParam param)
        {
            var rs = new Dictionary<string, string>();
            try
            {
                Dictionary<string, string> map = ReadClsMap();
                if (map.Count == 0 || currentServiceReq == null || currentServiceReq.TREATMENT_ID <= 0) return rs;
                var sereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer,
                    new HisSereServFilter { TREATMENT_ID = currentServiceReq.TREATMENT_ID, TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN, HAS_EXECUTE = true }, param);
                List<long> ids = sereServs != null ? sereServs.Select(o => o.ID).Distinct().ToList() : new List<long>();
                if (ids.Count == 0) return rs;
                var teins = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer,
                    new HisSereServTeinViewFilter { SERE_SERV_IDs = ids, IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE }, param);
                var byCode = new Dictionary<string, string>();
                foreach (V_HIS_SERE_SERV_TEIN t in teins ?? new List<V_HIS_SERE_SERV_TEIN>())
                {
                    // First non-empty value per index, like the portal push (a repeated test keeps its first result).
                    if (t == null || string.IsNullOrWhiteSpace(t.TEST_INDEX_CODE) || string.IsNullOrWhiteSpace(t.VALUE)) continue;
                    string code = t.TEST_INDEX_CODE.Trim().ToUpperInvariant();
                    if (!byCode.ContainsKey(code)) byCode[code] = t.VALUE.Trim();
                }
                foreach (var kv in map)
                {
                    string v;
                    if (byCode.TryGetValue(kv.Value, out v)) rs[kv.Key] = v;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return rs;
        }

        /// <summary>Portal field code -> HIS test index code (upper case) from ControlState "KskSytClsMap" (only "XN" rows).</summary>
        private static Dictionary<string, string> ReadClsMap()
        {
            var rs = new Dictionary<string, string>();
            try
            {
                var states = new HIS.Desktop.Library.CacheClient.ControlStateWorker().GetData(KSK_SYNC_MODULE_LINK);
                var state = states != null ? states.FirstOrDefault(o => o != null && o.KEY == KSK_SYNC_CLS_MAP_KEY) : null;
                if (state == null || string.IsNullOrWhiteSpace(state.VALUE)) return rs;
                var items = Newtonsoft.Json.Linq.JObject.Parse(state.VALUE)["Items"] as Newtonsoft.Json.Linq.JArray;
                if (items == null) return rs;
                foreach (var it in items)
                {
                    string field = (string)it["FieldCode"], code = (string)it["TestIndexCode"], kind = (string)it["SourceKind"];
                    if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(code)) continue;
                    if (!string.IsNullOrWhiteSpace(kind) && kind.Trim() != CLS_MAP_KIND__TEST_INDEX) continue;
                    rs[field.Trim()] = code.Trim().ToUpperInvariant();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return rs;
        }

        // ================================================================ helpers

        private static List<string> SplitIds(string value)
        {
            return (value ?? "").Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).Where(o => o.Length > 0).ToList();
        }

        private static string NormName(string s)
        {
            return (s ?? "").Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}

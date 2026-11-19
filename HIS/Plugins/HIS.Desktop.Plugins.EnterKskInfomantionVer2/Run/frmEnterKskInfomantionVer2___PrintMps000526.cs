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
 * Task 60376 (BV Xuyên Á Củ Chi): "Giấy khám sức khỏe và khám sức khỏe định kỳ dùng cho người từ đủ 18 tuổi trở lên" —
 * hospital form "Mẫu 3" of the HCMC health department program (body: Mẫu 03 TT 25/2026/TT-BYT), printed as Mps000526.
 * The "In phiếu" button of tab "Ksk trên 18 tuổi" prints it instead of Mps000452 when the record is an HCMC record that is
 * NOT elderly (the elderly one prints Mps000523) and the template of Mps000526 is installed on this workstation.
 * The data is prepared by the builders of Mps000523 (frmEnterKskInfomantionVer2___PrintMps000523.cs): saved data only.
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
using System.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        private const string PRINT_TYPE_CODE__MPS000526 = "Mps000526";
        /// <summary>CLS map rows whose value is the result text of a service (X-quang nhũ, siêu âm tuyến vú...).</summary>
        private const string CLS_MAP_KIND__SERVICE = "DV";

        /// <summary>Print Mps000526 instead of Mps000452 for an HCMC record that is not elderly when its template is installed here.</summary>
        private bool IsAdultM3Print()
        {
            try
            {
                if (!IsSytHcmDeclared() || currentKskOverEight == null || currentKskSytHcm == null) return false;
                if (IsElderlyPatientType()) return false;
                // Patient types saved as portal ids but no portal catalog on this workstation: the record may be an elderly one
                // that cannot be recognised here -> keep the old form rather than print the wrong new one.
                if (!string.IsNullOrWhiteSpace(GetSytHcmStr(currentKskSytHcm, "SYT_PATIENT_TYPES")) && GetSytCatalog(SYT_CODE__DOI_TUONG_KHAM).Count == 0)
                {
                    LogSystem.Warn("Mps000526: may chua co danh muc cong " + SYT_CODE__DOI_TUONG_KHAM + " -> khong biet ho so co phai nguoi cao tuoi khong, in Mps000452");
                    return false;
                }
                if (!IsPrintTemplateInstalled(PRINT_TYPE_CODE__MPS000526))
                {
                    LogSystem.Warn("Mps000526: ho so mau 3 nhung may chua co mau in Tmp/Mps/Mps000526 hoac loai in -> in Mps000452");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Mps000526: IsAdultM3Print loi -> in Mps000452", ex);
                return false;
            }
        }

        private void LoadBieuMauPhieuMps000526(string printTypeCode, string fileName, ref bool result)
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

                // Saved rows only, exactly like Mps000523 (saved conclusion, DHST read again after Save).
                MPS.Processor.Mps000526.PDO.Mps000526PDO rdo = new MPS.Processor.Mps000526.PDO.Mps000526PDO(
                    currentServiceReq, currentKskOverEight, currentKskSytHcm, currentKskGeneral, GetDhstForM4Print(param), treatment,
                    BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                rdo.KskServiceReq = printKskServiceReq;
                rdo.KskPatient = printKskPatient;
                rdo.PersonalHistories = BuildM4PersonalHistories(param).Select(o => new MPS.Processor.Mps000526.PDO.Mps000526HistoryADO
                {
                    DISEASE_TYPE_CODE = o.DISEASE_TYPE_CODE,
                    DISEASE_TYPE_NAME = o.DISEASE_TYPE_NAME,
                    IS_YES_NO = o.IS_YES_NO
                }).ToList();
                // The work place of the form is kept in the interview JSON, also for a record without interview answers.
                rdo.ResolvedNames = BuildM4ResolvedNames(ParseInterviewAnswers(GetSytHcmStr(currentKskSytHcm, IHCM_COL__JSON)));
                rdo.PatientTypesText = BuildM4PatientTypesText();
                rdo.EmployeeNames = BuildM4EmployeeNames();
                rdo.ClsValues = BuildM3ClsValues(param);
                WaitingManager.Hide();
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error("Mps000526: LoadBieuMauPhieuMps000526 loi -> khong in duoc", ex);
            }
        }

        /// <summary>
        /// Lab values of this form = the test index values of Mps000523 + the result text (conclusion, else description)
        /// of the services that the CLS map links by SERVICE (screening items of women) — the rule of the portal push
        /// (KskSytHcmBodyBuilder.ServiceResultValues): first executed service with a result wins.
        /// </summary>
        private Dictionary<string, string> BuildM3ClsValues(CommonParam param)
        {
            Dictionary<string, string> rs = BuildM4ClsValues(param);
            try
            {
                Dictionary<string, string> map = ReadClsMap(CLS_MAP_KIND__SERVICE);
                if (map.Count == 0 || currentServiceReq == null || currentServiceReq.TREATMENT_ID <= 0) return rs;
                var wanted = new HashSet<string>(map.Values);
                List<HIS_SERE_SERV> sereServs = (new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer,
                    new HisSereServFilter { TREATMENT_ID = currentServiceReq.TREATMENT_ID, HAS_EXECUTE = true }, param) ?? new List<HIS_SERE_SERV>())
                    .Where(o => o != null && !string.IsNullOrWhiteSpace(o.TDL_SERVICE_CODE) && wanted.Contains(o.TDL_SERVICE_CODE.Trim().ToUpperInvariant())).ToList();
                if (sereServs.Count == 0) return rs;
                var exts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer,
                    new HisSereServExtFilter { TDL_TREATMENT_IDs = new List<long> { currentServiceReq.TREATMENT_ID } }, param) ?? new List<HIS_SERE_SERV_EXT>();
                var extBySereServ = new Dictionary<long, HIS_SERE_SERV_EXT>();
                foreach (HIS_SERE_SERV_EXT e in exts)
                    if (e != null && !extBySereServ.ContainsKey(e.SERE_SERV_ID)) extBySereServ[e.SERE_SERV_ID] = e;
                var byCode = new Dictionary<string, string>();
                foreach (HIS_SERE_SERV s in sereServs)
                {
                    HIS_SERE_SERV_EXT e;
                    if (!extBySereServ.TryGetValue(s.ID, out e)) continue;
                    string v = !string.IsNullOrWhiteSpace(e.CONCLUDE) ? e.CONCLUDE : e.DESCRIPTION;
                    string code = s.TDL_SERVICE_CODE.Trim().ToUpperInvariant();
                    if (!string.IsNullOrWhiteSpace(v) && !byCode.ContainsKey(code)) byCode[code] = v.Trim();
                }
                foreach (var kv in map)
                {
                    string v;
                    if (byCode.TryGetValue(kv.Value, out v)) rs[kv.Key] = v;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Mps000526: BuildM3ClsValues loi -> in trong ket qua can lam sang lay tu dich vu", ex);
            }
            return rs;
        }
    }
}

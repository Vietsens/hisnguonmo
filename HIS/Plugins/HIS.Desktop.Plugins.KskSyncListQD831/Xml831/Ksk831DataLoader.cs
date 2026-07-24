/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>
    /// Nạp ĐỦ dữ liệu để dựng XML QĐ831. api/HisKskProfile/GetFull trả theo TỪNG bệnh nhân (patient/profile/
    /// exam[general,dhst,disease-result,vaccination]/relations). Dữ liệu phụ nạp THEO LÔ (filter nhiều ID):
    /// HIS_SERVICE_REQ (hành chính lượt khám), CLS (V_HIS_SERE_SERV_2 + TEIN), và DHST fallback nếu SDO thiếu.
    /// Danh mục HIS_DISEASE_DETAIL / HIS_BRANCH lấy cache local.
    /// </summary>
    internal static class Ksk831DataLoader
    {
        /// <summary>Dựng Data cho 1 hồ sơ (xem trước).</summary>
        internal static Data BuildDataForRow(V_HIS_KSK_PROFILE row)
        {
            var list = BuildDataForRows(new List<V_HIS_KSK_PROFILE> { row });
            return (list != null && list.Count > 0) ? list[0].Value : null;
        }

        /// <summary>
        /// Dựng Data cho NHIỀU hồ sơ: GetFull từng hồ sơ; nạp HIS_SERVICE_REQ + CLS + DHST thiếu THEO LÔ 1 lần.
        /// </summary>
        internal static List<KeyValuePair<V_HIS_KSK_PROFILE, Data>> BuildDataForRows(IEnumerable<V_HIS_KSK_PROFILE> rows)
        {
            var result = new List<KeyValuePair<V_HIS_KSK_PROFILE, Data>>();
            var rowList = (rows != null) ? rows.Where(r => r != null).ToList() : new List<V_HIS_KSK_PROFILE>();
            if (rowList.Count == 0) return result;

            // 1) GetFull theo từng hồ sơ; gom SERVICE_REQ_IDs, DHST có sẵn + DHST_ID cần.
            var sdoByRow = new List<KeyValuePair<V_HIS_KSK_PROFILE, HisKskProfileFullSDO>>();
            var allSrIds = new List<long>();
            var dhstById = new Dictionary<long, HIS_DHST>();
            var neededDhstIds = new List<long>();
            foreach (var row in rowList)
            {
                HisKskProfileFullSDO sdo = GetFull(BuildFilter(row));
                sdoByRow.Add(new KeyValuePair<V_HIS_KSK_PROFILE, HisKskProfileFullSDO>(row, sdo));
                if (sdo == null || sdo.ExamHistory == null) continue;
                foreach (var e in sdo.ExamHistory)
                {
                    if (e == null) continue;
                    if (e.HisKskGeneral != null)
                    {
                        if (e.HisKskGeneral.SERVICE_REQ_ID > 0) allSrIds.Add(e.HisKskGeneral.SERVICE_REQ_ID);
                        if (e.HisKskGeneral.DHST_ID.HasValue && e.HisKskGeneral.DHST_ID.Value > 0)
                            neededDhstIds.Add(e.HisKskGeneral.DHST_ID.Value);
                    }
                    if (e.HisDhst != null && !dhstById.ContainsKey(e.HisDhst.ID)) dhstById[e.HisDhst.ID] = e.HisDhst;
                }
            }
            allSrIds = allSrIds.Distinct().ToList();

            // 2) Batch HIS_SERVICE_REQ theo tất cả SERVICE_REQ_IDs.
            var srById = new Dictionary<long, HIS_SERVICE_REQ>();
            if (allSrIds.Count > 0)
            {
                var srs = GetList<HIS_SERVICE_REQ>("api/HisServiceReq/Get", new HisServiceReqFilter { IDs = allSrIds });
                if (srs != null) foreach (var s in srs) if (s != null && !srById.ContainsKey(s.ID)) srById[s.ID] = s;

                // Bổ sung DHST_ID từ chính service_req (nếu general không có).
                foreach (var s in srById.Values)
                    if (s.DHST_ID.HasValue && s.DHST_ID.Value > 0) neededDhstIds.Add(s.DHST_ID.Value);
            }

            // 3) DHST fallback: nạp các DHST_ID còn thiếu (SDO không trả) -> đảm bảo đủ sinh hiệu.
            var missingDhstIds = neededDhstIds.Distinct().Where(id => !dhstById.ContainsKey(id)).ToList();
            if (missingDhstIds.Count > 0)
            {
                var dhsts = GetList<HIS_DHST>("api/HisDhst/Get", new HisDhstFilter { IDs = missingDhstIds });
                if (dhsts != null) foreach (var d in dhsts) if (d != null && !dhstById.ContainsKey(d.ID)) dhstById[d.ID] = d;
            }

            // 4) Batch CLS (XN + CĐHA) theo tất cả TREATMENT_IDs.
            var clsByTr = new Dictionary<long, List<DichVu>>();
            var treatmentIds = srById.Values.Select(s => s.TREATMENT_ID).Where(x => x > 0).Distinct().ToList();
            if (treatmentIds.Count > 0)
            {
                var sereServs = GetList<V_HIS_SERE_SERV_2>("api/HisSereServ/GetView2",
                    new HisSereServView2Filter { TREATMENT_IDs = treatmentIds, HAS_EXECUTE = true });
                var ssIds = (sereServs != null) ? sereServs.Where(s => s != null).Select(s => s.ID).Distinct().ToList() : new List<long>();
                var teins = (ssIds.Count > 0)
                    ? GetList<V_HIS_SERE_SERV_TEIN>("api/HisSereServTein/GetView",
                        new HisSereServTeinViewFilter { SERE_SERV_IDs = ssIds, IS_ACTIVE = 1 })
                    : null;
                clsByTr = Ksk831Builder.BuildClsByTreatment(sereServs, teins);
            }

            // 5) Danh mục cache local.
            var detailById = LoadDiseaseDetailCache();

            // 6) Assemble từng hồ sơ — bọc try/catch RIÊNG mỗi hồ sơ để 1 hồ sơ lỗi KHÔNG làm hỏng cả lô.
            foreach (var kv in sdoByRow)
            {
                Data data = null;
                try
                {
                    if (kv.Value != null) data = AssembleData(kv.Value, kv.Key, srById, clsByTr, detailById, dhstById);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    data = null;
                }
                result.Add(new KeyValuePair<V_HIS_KSK_PROFILE, Data>(kv.Key, data));
            }
            return result;
        }

        private static Data AssembleData(HisKskProfileFullSDO sdo, V_HIS_KSK_PROFILE row,
            IDictionary<long, HIS_SERVICE_REQ> srById, IDictionary<long, List<DichVu>> clsByTr,
            IDictionary<long, HIS_DISEASE_DETAIL> detailById, IDictionary<long, HIS_DHST> dhstById)
        {
            HIS_PATIENT patient = (sdo.PatientInfo != null) ? sdo.PatientInfo.Patient : null;
            List<HIS_KSK_PROFILE> profiles = (sdo.PatientInfo != null) ? sdo.PatientInfo.Profiles : null;
            List<HisKskProfileExamSDO> exams = sdo.ExamHistory ?? new List<HisKskProfileExamSDO>();

            long srId = row.SERVICE_REQ_ID.HasValue ? row.SERVICE_REQ_ID.Value : 0;

            HIS_KSK_PROFILE profile = null;
            if (profiles != null && profiles.Count > 0)
                profile = profiles.FirstOrDefault(p => p != null && (p.SERVICE_REQ_ID ?? 0) == srId) ?? profiles[0];

            HisKskProfileExamSDO curExam =
                exams.FirstOrDefault(e => e != null && e.HisKskGeneral != null && e.HisKskGeneral.SERVICE_REQ_ID == srId)
                ?? exams.FirstOrDefault();
            HIS_KSK_GENERAL curGeneral = (curExam != null) ? curExam.HisKskGeneral : null;
            List<HIS_DISEASE_DETAIL_RESULT> curResults = (curExam != null) ? curExam.HisDiseaseDetailResults : null;

            var generalBySrId = new Dictionary<long, HIS_KSK_GENERAL>();
            var serviceReqs = new List<HIS_SERVICE_REQ>();
            var vaccinations = new Dictionary<long, HIS_HEALTH_VACCINATION>();   // gộp tránh trùng theo ID
            foreach (var e in exams)
            {
                if (e == null) continue;
                if (e.HisKskGeneral != null)
                {
                    long gsr = e.HisKskGeneral.SERVICE_REQ_ID;
                    if (gsr > 0 && !generalBySrId.ContainsKey(gsr))
                    {
                        generalBySrId[gsr] = e.HisKskGeneral;
                        HIS_SERVICE_REQ sr;
                        if (srById != null && srById.TryGetValue(gsr, out sr) && sr != null) serviceReqs.Add(sr);
                    }
                }
                if (e.HisHealthVaccinations != null)
                    foreach (var v in e.HisHealthVaccinations)
                        if (v != null && !vaccinations.ContainsKey(v.ID)) vaccinations[v.ID] = v;
            }
            // Vaccination cấp SDO (nếu backend trả ở đây) — gộp thêm.
            if (sdo.Vaccination != null)
                foreach (var v in sdo.Vaccination)
                    if (v != null && !vaccinations.ContainsKey(v.ID)) vaccinations[v.ID] = v;

            HIS_BRANCH branch = null;
            try
            {
                long brId = (patient != null && patient.BRANCH_ID.HasValue) ? patient.BRANCH_ID.Value : 0;
                if (brId > 0) branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(b => b != null && b.ID == brId);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            Header header = Ksk831Builder.BuildHeader(branch);
            ThongTinChung ttc = Ksk831Builder.BuildThongTinChung(patient, profile, sdo.Relations);
            TienSu tienSu = Ksk831Builder.BuildTienSu(profile, curGeneral, curResults, detailById);
            TiemChung tiemChung = Ksk831Builder.BuildTiemChung(vaccinations.Values.ToList(), profile);
            List<HoSoKhamChuaBenh> hoSoList = Ksk831Builder.BuildHoSoKhamChuaBenhList(
                serviceReqs, generalBySrId, dhstById, clsByTr, profile);

            return Ksk831Builder.BuildData(header, ttc, tienSu, tiemChung, hoSoList, "");
        }

        private static HisKskProfileFilter BuildFilter(V_HIS_KSK_PROFILE row)
        {
            var filter = new HisKskProfileFilter { IS_ACTIVE = 1 };
            if (row.SERVICE_REQ_ID.HasValue && row.SERVICE_REQ_ID.Value > 0)
                filter.SERVICE_REQ_ID = row.SERVICE_REQ_ID.Value;
            else if (!string.IsNullOrEmpty(row.TDL_PATIENT_CODE))
                filter.TDL_PATIENT_CODE__EXACT = row.TDL_PATIENT_CODE;
            return filter;
        }

        private static Dictionary<long, HIS_DISEASE_DETAIL> LoadDiseaseDetailCache()
        {
            var detailById = new Dictionary<long, HIS_DISEASE_DETAIL>();
            try
            {
                foreach (var d in BackendDataWorker.Get<HIS_DISEASE_DETAIL>())
                    if (d != null && !detailById.ContainsKey(d.ID)) detailById[d.ID] = d;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return detailById;
        }

        private static HisKskProfileFullSDO GetFull(HisKskProfileFilter filter)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<HisKskProfileFullSDO>(
                    "api/HisKskProfile/GetFull", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static List<T> GetList<T>(string uri, object filter)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<List<T>>(uri, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }
    }
}

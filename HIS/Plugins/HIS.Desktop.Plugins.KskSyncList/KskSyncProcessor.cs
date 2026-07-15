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
using System.Security.Cryptography.X509Certificates;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.KskSyncList.ADO;
using His.Ksk.QD2062;
using His.Ksk.QD2062.Base;
using His.Ksk.QD2062.Builder;
using His.Ksk.QD2062.Transport.Model;
using Inventec.Common.Adapter;
using Inventec.Core;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Diem rap noi thu vien dong bo QD 1551 (His.Ksk.QD2062 - thiet ke BD_046, muc 3.4 PTTK_44350).
    ///
    /// Plugin: (1) map ban ghi V_HIS_KSK_SYNC -> mau phieu QD2062 (KskSyncModelMapper);
    /// (2) goi thu vien CreateQd1551Main (BuildPreview / PushList): build XML/JSON -> ky envelope
    /// SHA256RSA -> xac thuc OAuth2 -> POST /api/platform/data-sync/push;
    /// (3) map ket qua tung ho so -> KskSyncResultADO de UC luu qua api/HisKskSync/SaveSyncResult.
    /// </summary>
    internal class KskSyncProcessor
    {
        private readonly string connectionInfo;        // cong BYT (MOS.HIS_KSK_SYNC.CONNECTION_INFO)
        private readonly string hsskConnectionInfo;     // cong HSSK (MOS.HIS_KSK_SYNC.HSSK_HN_2062_CONNECTION_INFO)
        private readonly bool pushByt;                  // co day cong BYT
        private readonly bool pushHssk;                 // co day cong HSSK
        private readonly bool sign;
        private readonly SettingSignADO signSetting;

        // Ket qua dong bo (khop KskSyncResultADO.SYNC_RESULT_TYPE): 2 = thanh cong, 3 = that bai.
        private const short RESULT_SUCCESS = 2;
        private const short RESULT_FAILED = 3;

        /// <summary>Ctor cu (chi cong BYT) — giu tuong thich cho preview / cac loi goi khac.</summary>
        internal KskSyncProcessor(string connectionInfo, bool sign, SettingSignADO signSetting)
            : this(connectionInfo, null, true, false, sign, signSetting)
        {
        }

        /// <summary>
        /// Ctor day da cong: chon day BYT (pushByt) va/hoac HSSK (pushHssk). base64 XML dung CHUNG cho ca 2 cong,
        /// chi khac API dang nhap + endpoint day (thu vien CreateQd1551Main.PushListMulti xu ly).
        /// </summary>
        internal KskSyncProcessor(string connectionInfo, string hsskConnectionInfo, bool pushByt, bool pushHssk,
            bool sign, SettingSignADO signSetting)
        {
            this.connectionInfo = connectionInfo;
            this.hsskConnectionInfo = hsskConnectionInfo;
            this.pushByt = pushByt;
            this.pushHssk = pushHssk;
            this.sign = sign;
            this.signSetting = signSetting;
        }

        /// <summary>
        /// Xem truoc du lieu se day cua mot ho so (Scene 3): map -> mau phieu roi goi
        /// CreateQd1551Main.BuildPreview (khong ky, khong gui) -> chuoi XML/JSON.
        /// </summary>
        internal string BuildPreview(V_HIS_KSK_SYNC row)
        {
            try
            {
                if (row == null) return "";

                CreateQd1551Main main = new CreateQd1551Main(BuildConfig());
                List<V_HIS_KSK_SYNC> one = new List<V_HIS_KSK_SYNC> { row };
                List<Qd1551KskInput> inputs = BuildInputs(one);

                ResultADO result = main.BuildPreview(inputs);
                if (result == null)
                    return "Không tạo được dữ liệu xem trước.";
                if (!result.Success)
                    return string.IsNullOrEmpty(result.Message) ? "Không tạo được dữ liệu xem trước." : result.Message;

                return (result.Data != null && result.Data.Length > 0 && result.Data[0] != null)
                    ? result.Data[0].ToString()
                    : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return "Lỗi tạo dữ liệu xem trước: " + ex.Message;
            }
        }

        /// <summary>
        /// Xuat file XML cho danh sach ho so ra thu muc dirPath — MOI ho so 1 file (envelope KHAMSUCKHOE
        /// SOLUONGHOSO=1). Goi API nap du lieu chi tiet BATCH 1 lan (BuildInputs), roi build tung envelope.
        /// XML KHONG mask chu ky (dung BuildEnvelope). Tra so file xuat thanh cong; failed = so ho so loi.
        /// </summary>
        internal int ExportXmlFiles(IEnumerable<V_HIS_KSK_SYNC> rows, string dirPath, out int failed, out string error)
        {
            failed = 0; error = null;
            List<V_HIS_KSK_SYNC> rowList = (rows != null) ? rows.Where(r => r != null).ToList() : new List<V_HIS_KSK_SYNC>();
            if (rowList.Count == 0) return 0;
            int ok = 0;
            try
            {
                CreateQd1551Main main = new CreateQd1551Main(BuildConfig());
                List<Qd1551KskInput> inputs = BuildInputs(rowList);   // 1 lan nap batch (1:1 voi rowList)
                var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < rowList.Count; i++)
                {
                    try
                    {
                        Qd1551KskInput inp = (i < inputs.Count) ? inputs[i] : null;
                        if (inp == null) { failed++; continue; }
                        ResultADO r = main.BuildEnvelope(new List<Qd1551KskInput> { inp });
                        if (r == null || !r.Success || r.Data == null || r.Data.Length == 0 || r.Data[0] == null)
                        { failed++; continue; }
                        string xml = r.Data[0].ToString();
                        if (string.IsNullOrEmpty(xml)) { failed++; continue; }

                        string baseName = MakeExportFileName(rowList[i]);
                        string name = baseName; int k = 1;
                        while (used.Contains(name)) { name = baseName + "_" + (++k); }
                        used.Add(name);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(dirPath, name + ".xml"), xml, new System.Text.UTF8Encoding(false));
                        ok++;
                    }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); failed++; }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); error = ex.Message; }
            return ok;
        }

        /// <summary>Ten file XML xuat = MaBN_MaDot_MaHoSo (bo ky tu khong hop le); trong -> KSK_MaHoSo.</summary>
        private static string MakeExportFileName(V_HIS_KSK_SYNC row)
        {
            string pat = SafeString(GetProp(row, "TDL_PATIENT_CODE"));
            string tre = SafeString(GetProp(row, "TDL_TREATMENT_CODE"));
            string rid = SafeString(GetProp(row, "KSK_RECORD_ID"));
            string s = string.Join("_", new[] { pat, tre, rid }.Where(x => !string.IsNullOrEmpty(x)).ToArray());
            if (string.IsNullOrEmpty(s)) s = "KSK_" + rid;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
            return string.IsNullOrEmpty(s) ? "KSK" : s;
        }

        /// <summary>
        /// Day lo nhieu ho so (Scene 4): dang nhap 1 lan, moi ho so map -> mau phieu roi goi
        /// CreateQd1551Main.PushList (build -> ky -> day cong).
        /// </summary>
        internal List<KskSyncResultADO> PushList(IEnumerable<V_HIS_KSK_SYNC> rows, long syncTime)
        {
            List<KskSyncResultADO> results = new List<KskSyncResultADO>();
            if (rows == null) return results;

            List<V_HIS_KSK_SYNC> rowList = rows.Where(r => r != null).ToList();
            if (rowList.Count == 0) return results;

            try
            {
                CreateQd1551Main main = new CreateQd1551Main(BuildConfig());
                X509Certificate2 certificate = LoadCertificate();
                // Moi ho so -> 1 Qd1551KskInput. Nap du lieu chi tiet BATCH (goi API theo danh sach ID, song song).
                List<Qd1551KskInput> inputs = BuildInputs(rowList);

                // Ky DU LIEU vao the CKS_BENH_VIEN (HSM/USB) - xu ly nhu ExportXmlQD130, thuc hien o plugin.
                Func<string, string> dataSigner = null;
                if (this.sign && this.signSetting != null)
                    dataSigner = new KskSyncSigner(this.signSetting).SignCksBenhVien;

                // Cong HSSK: parse cau hinh rieng (cung dinh dang CSV nhu BYT). base64 XML dung CHUNG.
                Qd1551Config hsskConfig = null;
                if (this.pushHssk && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo))
                    hsskConfig = Qd1551ConfigParser.Parse(this.hsskConnectionInfo, null);

                List<ResultADO> pushResults = main.PushListMulti(inputs, certificate, dataSigner, this.pushByt, hsskConfig);
                // PushList tra ket qua theo dung thu tu inputs (1:1 voi rowList) -> ghep theo chi so.
                for (int i = 0; i < rowList.Count; i++)
                {
                    ResultADO pr = (pushResults != null && i < pushResults.Count) ? pushResults[i] : null;
                    results.Add(BuildResultAdo(rowList[i], pr, syncTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                // Loi chung (chua cau hinh / khoi tao that bai): tra ve that bai cho tung ho so.
                foreach (var row in rowList)
                    results.Add(BuildFailedResult(row, syncTime, ex.Message));
            }
            return results;
        }

        /// <summary>
        /// Dung day du Qd1551KskInput cho danh sach ho so (cac dong tich chon). Goi API BATCH theo danh sach ID
        /// (SERVICE_REQ_IDs / TREATMENT_IDs / KSK_*_IDs) va SONG SONG (Task.WaitAll) — KHONG for goi tung ho so mot.
        /// Sau do index theo khoa va gan vao tung Qd1551KskInput. Chay tren tien trinh nen cua PushList.
        /// XML1/XML2 (hanh chinh + lan kham) do THU VIEN tu dung tu Patient/Treatment/KSK entity
        /// + MaCskcb/MaGtinCskcb/MaLoaiKcb — plugin khong con dung Admin1/Admin2 thu cong.
        /// </summary>
        // ===== TEMP FAKE: true = sinh XML tu DU LIEU GIA (test), KHONG doc DB. false = du lieu THAT (doc DB). =====
        private const bool USE_FAKE_DATA = false;

        private List<Qd1551KskInput> BuildInputs(List<V_HIS_KSK_SYNC> rowList)
        {
            if (USE_FAKE_DATA) return BuildFakeInputs();   // TEMP FAKE

            var inputs = new List<Qd1551KskInput>();
            if (rowList == null || rowList.Count == 0) return inputs;

            List<long> serviceReqIds = rowList.Select(r => ToLong(GetProp(r, "SERVICE_REQ_ID"))).Where(x => x > 0).Distinct().ToList();
            List<long> treatmentIds = rowList.Select(r => ToLong(GetProp(r, "TDL_TREATMENT_ID"))).Where(x => x > 0).Distinct().ToList();

            // .NET Framework mac dinh CHI cho 2 ket noi HTTP/host -> nang gioi han cho cac call song song.
            if (System.Net.ServicePointManager.DefaultConnectionLimit < 20)
                System.Net.ServicePointManager.DefaultConnectionLimit = 20;

            // === DOT 1 (song song): 1 CALL GOP api/HisKskSync/GetKskData (input/output nhu EnterKskVer2) ===
            // HisKskDataSDO bung du du lieu KSC 1 luot cho CA LIST ho so: General/UnderSix/Under18/Over18/DHST/
            // Treatment + UneiVaty(tiem chung) + PeriodDriverDity(tien su) + VaccineType + DiseaseType.
            // Cac du lieu NGOAI pham vi KSK (CLS, doi tuong dieu tri) van goi rieng theo TREATMENT_IDs (song song).
            MOS.SDO.HisKskDataSDO sdo = null;
            List<HIS_SERE_SERV> clsSereServs = null;
            List<V_HIS_SERE_SERV_TEIN> clsTeins = null;
            List<HIS_SERE_SERV_EXT> clsExts = null;
            List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters = null;

            var tasks = new List<System.Threading.Tasks.Task>();
            if (serviceReqIds.Count > 0 || treatmentIds.Count > 0)
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    sdo = GetKskDataSdo(new MOS.Filter.HisKskDataFilter
                    {
                        SERVICE_REQ_IDs = serviceReqIds,
                        TREATMENT_IDs = treatmentIds,
                        IS_ACTIVE = 1
                    })));
            if (treatmentIds.Count > 0)
            {
                // CLS (XML11): dich vu can lam sang DA THUC HIEN theo dot dieu tri (bo loai chuan nhu
                // TreatmentList: XN/CDHA/NS/SA/TDCN) + chi so xet nghiem (TEIN) + ket qua mo ta/ket luan (EXT).
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsSereServs = GetList<HIS_SERE_SERV>("api/HisSereServ/Get", new HisSereServFilter
                    {
                        TREATMENT_IDs = treatmentIds,
                        TDL_SERVICE_TYPE_IDs = new List<long>
                        {
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                        },
                        HAS_EXECUTE = true
                    })));
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsTeins = GetList<V_HIS_SERE_SERV_TEIN>("api/HisSereServTein/GetView", new HisSereServTeinViewFilter { TDL_TREATMENT_IDs = treatmentIds })));
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    clsExts = GetList<HIS_SERE_SERV_EXT>("api/HisSereServExt/Get", new HisSereServExtFilter { TDL_TREATMENT_IDs = treatmentIds })));
                // Dien doi tuong hien tai cua dot dieu tri — phuc vu suy MA_LOAI_KCB=100 (doi tuong KSK, nhu XML130).
                tasks.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    patientTypeAlters = GetList<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetView", new HisPatientTypeAlterViewFilter { TREATMENT_IDs = treatmentIds })));
            }
            if (tasks.Count > 0) System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            // Bung du lieu KSK tu SDO (null-safe). Loi call gop -> tat ca null -> input rong (khong sai du lieu).
            List<HIS_KSK_GENERAL> generals = (sdo != null) ? sdo.HisKskGenerals : null;
            List<HIS_KSK_UNDER_SIX> underSixes = (sdo != null) ? sdo.HisKskUnderSixs : null;
            List<HIS_KSK_UNDER_EIGHTEEN> under18s = (sdo != null) ? sdo.HisKskUnderEighteens : null;
            List<HIS_KSK_OVER_EIGHTEEN> over18s = (sdo != null) ? sdo.HisKskOverEighteens : null;
            List<HIS_DHST> dhsts = (sdo != null) ? sdo.HisDhsts : null;
            List<HIS_TREATMENT> treatments = (sdo != null) ? sdo.HisTreatments : null;
            List<HIS_KSK_UNEI_VATY> vatys = (sdo != null) ? sdo.HisKskUneiVatys : null;
            List<HIS_PERIOD_DRIVER_DITY> ditys = (sdo != null) ? sdo.HisPeriodDriverDitys : null;
            List<HIS_VACCINE_TYPE> vaccineTypes = (sdo != null) ? sdo.HisVaccineTypes : null;
            List<HIS_DISEASE_TYPE> diseaseTypes = (sdo != null) ? sdo.HisDiseaseTypes : null;

            // === DOT 2 (phu thuoc treatments/KSK entity tu SDO): benh nhan (XML1) + ngoai tru man tinh
            //     (MA_LOAI_KCB 05/08) + chu ky dien tu bac si (emr_signer.SIGN_IMAGE theo LOGINNAMEs). ===
            List<HIS_PATIENT> patients = null;
            List<HIS_SERE_SERV> chronicSereServs = null;
            List<EMR.EFMODEL.DataModels.EMR_SIGNER> emrSigners = null;
            var tasks2 = new List<System.Threading.Tasks.Task>();
            List<long> patientIds = (treatments != null) ? treatments.Select(t => t.PATIENT_ID).Distinct().ToList() : new List<long>();
            List<string> loginnames = CollectLoginnames(underSixes, under18s, over18s, generals, dhsts, clsExts);
            List<long> chronicTrIds = (treatments != null)
                ? treatments.Where(t => t != null && (t.TDL_TREATMENT_TYPE_ID ?? 0) == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU && t.IS_CHRONIC == 1)
                            .Select(t => t.ID).Distinct().ToList()
                : new List<long>();
            if (patientIds.Count > 0)
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    patients = GetList<HIS_PATIENT>("api/HisPatient/Get", new HisPatientFilter { IDs = patientIds })));
            if (chronicTrIds.Count > 0)
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    chronicSereServs = GetList<HIS_SERE_SERV>("api/HisSereServ/Get", new HisSereServFilter { TREATMENT_IDs = chronicTrIds })));
            if (loginnames.Count > 0)
                tasks2.Add(System.Threading.Tasks.Task.Factory.StartNew(() =>
                    emrSigners = GetList<EMR.EFMODEL.DataModels.EMR_SIGNER>("api/EmrSigner/Get",
                        new EMR.Filter.EmrSignerFilter { LOGINNAMEs = loginnames, IS_ACTIVE = 1 }, ApiConsumers.EmrConsumer)));
            if (tasks2.Count > 0) System.Threading.Tasks.Task.WaitAll(tasks2.ToArray());

            // Chu ky: loginname -> base64(SIGN_IMAGE) — thu vien (Qd1551SignResolver) dien vao cac the CKDT_.
            Dictionary<string, string> signImageByLogin = BuildSignMap(emrSigners);
            // CLS: dung List<Qd1551ClsRow> theo TREATMENT_ID (XN: 1 dong/chi so TEIN; CDHA/NS/SA/TDCN: 1 dong/dich vu).
            Dictionary<long, List<Qd1551ClsRow>> clsByTr = BuildClsByTreatment(clsSereServs, clsTeins, clsExts);

            List<HIS_HEALTH_EXAM_RANK> ranks = null;
            try { ranks = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            var genBySr = IndexBy(generals, g => g.SERVICE_REQ_ID);
            var u6BySr = IndexBy(underSixes, x => x.SERVICE_REQ_ID);
            var u18BySr = IndexBy(under18s, x => x.SERVICE_REQ_ID);
            var o18BySr = IndexBy(over18s, x => x.SERVICE_REQ_ID);
            var dhstById = IndexBy(dhsts, d => d.ID);            // DHST theo ID (chinh xac tung ban ghi KSK)
            var dhstByTr = GroupByKey(dhsts, d => d.TREATMENT_ID); // fallback theo dot dieu tri
            var treaById = IndexBy(treatments, t => t.ID);
            var patById = IndexBy(patients, p => p.ID);
            var vatyByU18 = GroupByKey(vatys, v => v.KSK_UNDER_EIGHTEEN_ID);
            var dityByO18 = GroupByKey(ditys, d => d.KSK_OVER_EIGHTEEN_ID ?? 0);

            // Danh muc chi nhanh (cache local) — MA_CSKCB = HEIN_MEDI_ORG_CODE theo BRANCH_ID.
            Dictionary<long, HIS_BRANCH> branchById = null;
            try { branchById = IndexBy(BackendDataWorker.Get<HIS_BRANCH>(), b => b.ID); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            // Ma GTIN/GLN co so — SenderId trong CONNECTION_INFO (BYT); neu rong -> fallback SenderId cong HSSK.
            string maGtinCskcb = "";
            try
            {
                var cfg = BuildConfig();
                string sid = (cfg != null) ? cfg.SenderId : null;
                if (string.IsNullOrWhiteSpace(sid) && !string.IsNullOrWhiteSpace(this.hsskConnectionInfo))
                {
                    var h = Qd1551ConfigParser.Parse(this.hsskConnectionInfo, null);
                    if (h != null && !string.IsNullOrWhiteSpace(h.SenderId)) sid = h.SenderId;
                }
                maGtinCskcb = sid ?? "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            // Ma doi tuong KSK (config) — dung cho quy tac MA_LOAI_KCB=100 nhu XML130.
            string keyKsk = "";
            try
            {
                var cfgKsk = BackendDataWorker.Get<HIS_CONFIG>().FirstOrDefault(o => o.KEY == "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK");
                if (cfgKsk != null) keyKsk = cfgKsk.VALUE ?? "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            var alterByTr = GroupByKey(patientTypeAlters, a => a.TREATMENT_ID);
            var chronicServByTr = GroupByKey(chronicSereServs, x => x.TDL_TREATMENT_ID ?? 0);

            foreach (var row in rowList)
            {
                long sr = ToLong(GetProp(row, "SERVICE_REQ_ID"));
                long tr = ToLong(GetProp(row, "TDL_TREATMENT_ID"));

                HIS_KSK_GENERAL general = ValOrNull(genBySr, sr);
                HIS_KSK_UNDER_SIX underSix = ValOrNull(u6BySr, sr);
                HIS_KSK_UNDER_EIGHTEEN under18 = ValOrNull(u18BySr, sr);
                HIS_KSK_OVER_EIGHTEEN over18 = ValOrNull(o18BySr, sr);

                // DHST: uu tien lay DUNG ban ghi theo DHST_ID cua ho so KSK (khong lay nham sinh hieu cua lan do
                // khac cung TREATMENT). Neu ban ghi khong co DHST_ID -> fallback theo dot dieu tri.
                long dhstId = 0;
                if (underSix != null && underSix.DHST_ID.HasValue) dhstId = underSix.DHST_ID.Value;
                else if (under18 != null && under18.DHST_ID.HasValue) dhstId = under18.DHST_ID.Value;
                else if (over18 != null && over18.DHST_ID.HasValue) dhstId = over18.DHST_ID.Value;
                else if (general != null && general.DHST_ID.HasValue) dhstId = general.DHST_ID.Value;
                HIS_DHST dhstOne = ValOrNull(dhstById, dhstId);
                List<HIS_DHST> dhstForInput = (dhstOne != null) ? new List<HIS_DHST> { dhstOne } : ListOrNull(dhstByTr, tr);

                HIS_TREATMENT trea = ValOrNull(treaById, tr);
                HIS_BRANCH branch = (trea != null) ? ValOrNull(branchById, trea.BRANCH_ID) : null;

                inputs.Add(new Qd1551KskInput
                {
                    FormType = Qd1551FormMapper.ResolveFormType(ToLong(GetProp(row, "KSK_TYPE_ID"))),
                    // XML1/XML2: thu vien tu dung tu Patient + Treatment + KSK entity + 3 gia tri duoi day
                    Patient = (trea != null) ? ValOrNull(patById, trea.PATIENT_ID) : null,
                    MaCskcb = (branch != null) ? (branch.HEIN_MEDI_ORG_CODE ?? "") : "", // MA_CSKCB thật theo BRANCH_ID
                    MaGtinCskcb = maGtinCskcb,
                    MaLoaiKcb = ResolveMaLoaiKcb(trea, ListOrNull(chronicServByTr, tr), ListOrNull(alterByTr, tr), keyKsk),
                    General = general,
                    UnderSix = underSix,
                    UnderEighteen = under18,
                    OverEighteen = over18,
                    Dhst = dhstForInput,
                    Treatment = trea,
                    HealthExamRanks = ranks,
                    // Tiem chung 6-18 + danh muc vac-xin (mapper quy doi VACCINE_TYPE_CODE KSK01-07 -> the TIEM_CHUNG_*)
                    Vaccinations = (under18 != null) ? ListOrNull(vatyByU18, under18.ID) : null,
                    VaccineTypes = vaccineTypes,
                    // Tien su ban than >=18 (grid) + danh muc loai benh (mapper quy doi ma 01-22 -> the TSBT_*)
                    PersonalHistoryDity = (over18 != null) ? ListOrNull(dityByO18, over18.ID) : null,
                    DiseaseTypes = diseaseTypes,
                    // Chu ky dien tu bac si kham (CKDT_) + danh sach chi so CLS (XML11)
                    SignImageByLoginName = signImageByLogin,
                    ClsList = ListOrNull(clsByTr, tr)
                });
            }
            return inputs;
        }

        /// <summary>
        /// TEMP FAKE — dung 1 Qd1551KskInput DU LIEU GIA (mau nguoi >=18 tuoi) de sinh XML thu,
        /// KHONG doc DB. Bat/tat bang USE_FAKE_DATA. Xoa method + co khi khong con test.
        /// </summary>
        private List<Qd1551KskInput> BuildFakeInputs()
        {
            const string BT = "Bình thường";
            HIS_PATIENT patient = new HIS_PATIENT
            {
                VIR_PATIENT_NAME = "NGUYỄN VĂN TEST",
                GENDER_ID = 1,
                DOB = 19900101000000L,
                ETHNIC_CODE = "01",
                CCCD_NUMBER = "079090012345",
                CCCD_DATE = 20200315L,
                CCCD_PLACE = "Cục CSDLQG về dân cư",
                BLOOD_ABO_CODE = "O",   // NHOM_MAU chi lay ABO (A/B/AB/O) — khong noi Rh
                VIR_ADDRESS = "Số 1, Xã An Minh, An Giang",
                PROVINCE_CODE = "91",
                COMMUNE_CODE = "31018",
                MOBILE = "0912345678",
                CAREER_CODE = "04",
                WORK_PLACE = "Công ty TNHH ABC"
            };
            HIS_TREATMENT treatment = new HIS_TREATMENT
            {
                ID = 1000,
                PATIENT_ID = 1,
                TREATMENT_CODE = "000026007788",
                IN_TIME = 20260709081500L,
                HOSPITALIZATION_REASON = "Khám sức khỏe định kỳ",
                TDL_PATIENT_CAREER_CODE = "0412"   // MA_NGHE_NGHIEP se cat con "04" (nhu XML130)
            };
            HIS_DHST dhst = new HIS_DHST
            {
                ID = 555,
                TREATMENT_ID = 1000,
                HEIGHT = 168m,
                WEIGHT = 60m,
                PULSE = 78L,
                BLOOD_PRESSURE_MAX = 120L,
                BLOOD_PRESSURE_MIN = 80L
            };
            HIS_KSK_OVER_EIGHTEEN oe = new HIS_KSK_OVER_EIGHTEEN
            {
                ID = 2000,
                SERVICE_REQ_ID = 3000,
                DHST_ID = 555L,
                KSK_PATIENT_TYPES = "1;2",
                KSK_PAY_SOURCE = (short)2,
                HEALTH_EXAM_RANK_DESCRIPTION = "Đủ sức khỏe làm việc",
                DHST_RANK = 2L,
                // Tien su ban than (3 o text) — TSBT_MA_BENH_KHAC / TEN_THUOC / THAI_SAN
                PATHOLOGICAL_HISTORY = "Viêm dạ dày mạn",
                MEDICINE_USING = "Omeprazol 20mg",
                MATERNITY_HISTORY = "Không",
                // Noi khoa (ket qua text + phan loai _RANK)
                EXAM_CIRCULATION = BT, EXAM_CIRCULATION_RANK = 2L,
                EXAM_RESPIRATORY = BT, EXAM_RESPIRATORY_RANK = 2L,
                EXAM_DIGESTION = BT, EXAM_DIGESTION_RANK = 2L,
                EXAM_KIDNEY_UROLOGY = BT, EXAM_KIDNEY_UROLOGY_RANK = 2L,
                EXAM_OEND = BT, EXAM_OEND_RANK = 2L,
                EXAM_MUSCLE_BONE = BT, EXAM_MUSCLE_BONE_RANK = 2L,
                EXAM_NEUROLOGICAL = BT, EXAM_NEUROLOGICAL_RANK = 2L,
                EXAM_MENTAL = BT, EXAM_MENTAL_RANK = 2L,
                EXAM_SURGERY = BT, EXAM_SURGERY_RANK = 2L,
                EXAM_DERMATOLOGY = BT, EXAM_DERMATOLOGY_RANK = 2L,
                EXAM_OBSTETRIC = BT, EXAM_OBSTETRIC_RANK = 2L,
                // Mat
                EXAM_EYESIGHT_RIGHT = "10/10", EXAM_EYESIGHT_LEFT = "10/10",
                EXAM_EYESIGHT_GLASS_RIGHT = "10/10", EXAM_EYESIGHT_GLASS_LEFT = "10/10",
                EXAM_EYE_DISEASE = "Không", EXAM_EYE_RANK = 2L,
                // Tai mui hong
                // Tai (do suc nghe noi thuong/noi tham) — do dai toi da 10, dung gia tri khoang cach
                EXAM_ENT_LEFT_NORMAL = "5/5", EXAM_ENT_LEFT_WHISPER = "5/5",
                EXAM_ENT_RIGHT_NORMAL = "5/5", EXAM_ENT_RIGHT_WHISPER = "5/5",
                EXAM_ENT_DISEASE = "Không", EXAM_ENT_RANK = 2L,
                // Rang ham mat
                EXAM_STOMATOLOGY_UPPER = BT, EXAM_STOMATOLOGY_LOWER = BT,
                EXAM_STOMATOLOGY_DISEASE = "Không", EXAM_STOMATOLOGY_RANK = 2L,
                // CKDT_: loginname bac si kham -> tra base64 chu ky (fake) o SignImageByLoginName ben duoi
                EXAM_CIRCULATION_LOGINNAME = "fakebs",
                EXAM_RESPIRATORY_LOGINNAME = "fakebs",
                EXAM_DIGESTION_LOGINNAME = "fakebs",
                EXAM_KIDNEY_UROLOGY_LOGINNAME = "fakebs",
                EXAM_OEND_LOGINNAME = "fakebs",
                EXAM_MUSCLE_BONE_LOGINNAME = "fakebs",
                EXAM_NEUROLOGICAL_LOGINNAME = "fakebs",
                EXAM_MENTAL_LOGINNAME = "fakebs",
                EXAM_SURGERY_LOGINNAME = "fakebs",
                EXAM_DERMATOLOGY_LOGINNAME = "fakebs",
                EXAM_OBSTETRIC_LOGINNAME = "fakebs",
                EXAM_EYE_LOGINNAME = "fakebs",
                EXAM_ENT_LOGINNAME = "fakebs",
                EXAM_STOMATOLOGY_LOGINNAME = "fakebs"
            };
            HIS_KSK_GENERAL general = new HIS_KSK_GENERAL
            {
                ID = 4000,
                SERVICE_REQ_ID = 3000,
                DHST_ID = 555L,
                HEALTH_CONCLUSION_TYPE = (short)1,
                HEALTH_EXAM_RANK_ID = 2L,                                     // -> PHAN_LOAI_SK = "2"
                DISEASES = "Không phát hiện bệnh lý cấp tính",                 // -> CAC_BENH_TAT_NEU_CO
                CONCLUSION_ICD_CODE = "Z00.0",                                // -> KET_LUAN_BENH (ma ICD)
                FAMILY_HISTORY_ICD_CODE = "I10",                              // -> TSGD_MA_BENH + co
                PERSONAL_HISTORY_ICD_CODE = "K29",                            // -> TSBT_MA_BENH + co
                TREATING_DISEASE_ICD_CODE = "K29",                            // -> co TSBT_DANG_DIEU_TRI_BENH
                OBSTETRIC_DISEASE_ICD_CODE = ""                               // nam gioi -> de trong
            };
            List<HIS_HEALTH_EXAM_RANK> ranks = new List<HIS_HEALTH_EXAM_RANK>
            {
                new HIS_HEALTH_EXAM_RANK { ID = 1, HEALTH_EXAM_RANK_CODE = "1" },
                new HIS_HEALTH_EXAM_RANK { ID = 2, HEALTH_EXAM_RANK_CODE = "2" },
                new HIS_HEALTH_EXAM_RANK { ID = 3, HEALTH_EXAM_RANK_CODE = "3" },
                new HIS_HEALTH_EXAM_RANK { ID = 4, HEALTH_EXAM_RANK_CODE = "4" },
                new HIS_HEALTH_EXAM_RANK { ID = 5, HEALTH_EXAM_RANK_CODE = "5" }
            };
            // Tien su ban than (grid) -> co TSBT_* : ma 5=tim, 7=tang huyet ap, 12=dai thao duong (DefaultTsbtByCode)
            List<HIS_DISEASE_TYPE> diseaseTypes = new List<HIS_DISEASE_TYPE>
            {
                new HIS_DISEASE_TYPE { ID = 5, DISEASE_TYPE_CODE = "5" },
                new HIS_DISEASE_TYPE { ID = 7, DISEASE_TYPE_CODE = "7" },
                new HIS_DISEASE_TYPE { ID = 12, DISEASE_TYPE_CODE = "12" }
            };
            List<HIS_PERIOD_DRIVER_DITY> ditys = new List<HIS_PERIOD_DRIVER_DITY>
            {
                new HIS_PERIOD_DRIVER_DITY { ID = 1, DISEASE_TYPE_ID = 5, IS_YES_NO = "1", KSK_OVER_EIGHTEEN_ID = 2000L },
                new HIS_PERIOD_DRIVER_DITY { ID = 2, DISEASE_TYPE_ID = 7, IS_YES_NO = "1", KSK_OVER_EIGHTEEN_ID = 2000L },
                new HIS_PERIOD_DRIVER_DITY { ID = 3, DISEASE_TYPE_ID = 12, IS_YES_NO = "1", KSK_OVER_EIGHTEEN_ID = 2000L }
            };
            List<Qd1551ClsRow> cls = new List<Qd1551ClsRow>
            {
                new Qd1551ClsRow { MA_DICH_VU = "03C3.1.89", TEN_DICH_VU = "Tổng phân tích tế bào máu ngoại vi", MA_CHI_SO = "H02", TEN_CHI_SO = "Huyết sắc tố", GIA_TRI = "130", DON_VI_DO = "g/L", MO_TA = "Trong giới hạn bình thường", KET_LUAN = "Bình thường" },
                new Qd1551ClsRow { MA_DICH_VU = "18.0068.0013", TEN_DICH_VU = "X Quang phổi thẳng", MA_CHI_SO = "X01", TEN_CHI_SO = "X Quang phổi thẳng", GIA_TRI = "Không", DON_VI_DO = "Không", MO_TA = "Không thấy tổn thương nhu mô phổi", KET_LUAN = "Bình thường" }
            };
            Qd1551KskInput input = new Qd1551KskInput
            {
                FormType = FormType.Tren18,
                Patient = patient,
                Treatment = treatment,
                OverEighteen = oe,
                General = general,
                Dhst = new List<HIS_DHST> { dhst },
                HealthExamRanks = ranks,
                PersonalHistoryDity = ditys,
                DiseaseTypes = diseaseTypes,
                ClsList = cls,
                // CKDT_ (fake): loginname "fakebs" -> base64 anh NinhThuan/dvvvv.jpg
                SignImageByLoginName = new Dictionary<string, string> { { "fakebs", FakeSignImage.ABC_JPG_BASE64 } },
                MaCskcb = "01816",
                MaGtinCskcb = "8934285005264",
                MaLoaiKcb = "01"
            };
            return new List<Qd1551KskInput> { input };
        }

        /// <summary>
        /// Suy MA_LOAI_KCB tu loai dieu tri — port DUNG logic bo XML BHYT (XML130 Xml1Processor):
        /// KHAM=01; DTNOITRU: noi tru duoi 4h (OUT_TIME - CLINICAL_IN_TIME) =09, nguoc lai =03;
        /// DTBANNGAY=04; TYTXA=06; NHANTHUOC=07; DTNGOAITRU: khong man tinh =02, man tinh co DV
        /// ngoai kham/don (KH/DONDT/DONTT/DONK) =08 nguoc lai =05; mac dinh =10.
        /// Rieng KSK: neu MA_LOAI_KCB=01 va doi tuong hien tai la KSK (PATIENT_TYPE_CODE = config
        /// MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK, khop TDL_PATIENT_TYPE_ID) -> "100".
        /// </summary>
        private static string ResolveMaLoaiKcb(HIS_TREATMENT t, List<HIS_SERE_SERV> allSereServs,
            List<V_HIS_PATIENT_TYPE_ALTER> alters, string keyKsk)
        {
            if (t == null) return "";
            string maLoaiKcb = "10";
            long type = t.TDL_TREATMENT_TYPE_ID ?? 0;
            if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
            {
                maLoaiKcb = "01";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
            {
                if (t.OUT_TIME.HasValue && t.CLINICAL_IN_TIME.HasValue
                    && (t.OUT_TIME.Value - t.CLINICAL_IN_TIME.Value) > 0
                    && Inventec.Common.DateTime.Calculation.DifferenceTime(t.CLINICAL_IN_TIME.Value, t.OUT_TIME.Value,
                        Inventec.Common.DateTime.Calculation.UnitDifferenceTime.HOUR) < 4)
                    maLoaiKcb = "09";
                else
                    maLoaiKcb = "03";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)
            {
                maLoaiKcb = "04";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TYTXA)
            {
                maLoaiKcb = "06";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__NHANTHUOC)
            {
                maLoaiKcb = "07";
            }
            else if (type == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
            {
                if (t.IS_CHRONIC != 1)
                    maLoaiKcb = "02";
                else if (allSereServs != null && allSereServs.Count > 0)
                {
                    if (allSereServs.Exists(o => o != null
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                        && o.TDL_SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK))
                        maLoaiKcb = "08";
                    else
                        maLoaiKcb = "05";
                }
            }
            // Kham suc khoe: dien dieu tri kham + doi tuong benh nhan la KSK -> 100 (nhu XML130).
            if (maLoaiKcb == "01" && !string.IsNullOrEmpty(keyKsk)
                && alters != null && alters.Count > 0
                && alters.Exists(o => o != null && o.PATIENT_TYPE_CODE == keyKsk && o.PATIENT_TYPE_ID == t.TDL_PATIENT_TYPE_ID))
            {
                maLoaiKcb = "100";
            }
            return maLoaiKcb;
        }

        /// <summary>Goi API danh sach (Get) — MosConsumer. Loi -> null (khong chan cac call khac).</summary>
        private static List<T> GetList<T>(string uri, object filter)
        {
            return GetList<T>(uri, filter, ApiConsumers.MosConsumer);
        }

        /// <summary>Goi API danh sach (Get) theo consumer chi dinh (Mos/Emr...). Loi -> null.</summary>
        private static List<T> GetList<T>(string uri, object filter, Inventec.Common.WebApiClient.ApiConsumer consumer)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<List<T>>(uri, consumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Gom moi gia tri cot string ket thuc "LOGINNAME" (EXAM_*_LOGINNAME, CONCLUDER_LOGINNAME,
        /// EXECUTE_LOGINNAME, SUBCLINICAL_RESULT_LOGINNAME...) tu cac danh sach entity — dung tra emr_signer.
        /// </summary>
        private static List<string> CollectLoginnames(params System.Collections.IEnumerable[] lists)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (lists != null)
                foreach (var list in lists)
                {
                    if (list == null) continue;
                    foreach (var item in list)
                    {
                        if (item == null) continue;
                        foreach (var p in item.GetType().GetProperties())
                        {
                            if (p.PropertyType != typeof(string) || !p.Name.EndsWith("LOGINNAME")) continue;
                            string v = null;
                            try { v = p.GetValue(item, null) as string; } catch { }
                            if (!string.IsNullOrEmpty(v)) set.Add(v.Trim());
                        }
                    }
                }
            return set.ToList();
        }

        /// <summary>emr_signer -> map loginname -> base64(SIGN_IMAGE). Khong co chu ky -> bo qua loginname do.</summary>
        private static Dictionary<string, string> BuildSignMap(List<EMR.EFMODEL.DataModels.EMR_SIGNER> signers)
        {
            if (signers == null || signers.Count == 0) return null;
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var s in signers)
            {
                if (s == null || string.IsNullOrEmpty(s.LOGINNAME)) continue;
                if (s.SIGN_IMAGE == null || s.SIGN_IMAGE.Length == 0) continue;
                if (!map.ContainsKey(s.LOGINNAME))
                    map[s.LOGINNAME] = Convert.ToBase64String(s.SIGN_IMAGE);
            }
            return map.Count > 0 ? map : null;
        }

        /// <summary>
        /// Dung danh sach dong CLS (Qd1551ClsRow) theo TREATMENT_ID: dich vu XN co chi so -> 1 dong/chi so
        /// (MA_CHI_SO/GIA_TRI/DON_VI_DO tu V_HIS_SERE_SERV_TEIN); dich vu khong co chi so (CDHA/NS/SA/TDCN)
        /// -> 1 dong/dich vu. MO_TA/KET_LUAN/bac si ket qua tu HIS_SERE_SERV_EXT.
        /// </summary>
        private static Dictionary<long, List<Qd1551ClsRow>> BuildClsByTreatment(
            List<HIS_SERE_SERV> sereServs, List<V_HIS_SERE_SERV_TEIN> teins, List<HIS_SERE_SERV_EXT> exts)
        {
            var result = new Dictionary<long, List<Qd1551ClsRow>>();
            if (sereServs == null || sereServs.Count == 0) return result;

            // Chi lay dich vu CLS theo LOAI DICH VU BHYT (HEIN_SERVICE_TYPE): CDHA / TDCN / XN.
            var allowedHein = new HashSet<long>
            {
                IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA,
                IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TDCN,
                IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__XN
            };

            var teinBySs = GroupByKey(teins, t => t.SERE_SERV_ID);
            var extBySs = IndexBy(exts, e => e.SERE_SERV_ID);

            foreach (var ss in sereServs)
            {
                if (ss == null || ss.IS_NO_EXECUTE != null) continue;   // chi lay dich vu DA thuc hien
                if (!allowedHein.Contains(ss.TDL_HEIN_SERVICE_TYPE_ID ?? 0)) continue;   // chi CDHA/TDCN/XN
                long tr = ss.TDL_TREATMENT_ID ?? 0;
                if (tr <= 0) continue;

                HIS_SERE_SERV_EXT ext = ValOrNull(extBySs, ss.ID);
                string moTa = (ext != null) ? (ext.DESCRIPTION ?? "") : "";
                string ketLuan = (ext != null) ? (ext.CONCLUDE ?? "") : "";
                string bacSi = (ext != null) ? (ext.SUBCLINICAL_RESULT_LOGINNAME ?? "") : "";

                List<Qd1551ClsRow> rows;
                if (!result.TryGetValue(tr, out rows)) { rows = new List<Qd1551ClsRow>(); result[tr] = rows; }

                List<V_HIS_SERE_SERV_TEIN> ssTeins = ListOrNull(teinBySs, ss.ID);
                if (ssTeins != null && ssTeins.Count > 0)
                {
                    foreach (var tein in ssTeins)
                    {
                        if (tein == null) continue;
                        rows.Add(new Qd1551ClsRow
                        {
                            MA_DICH_VU = ss.TDL_SERVICE_CODE ?? "",
                            TEN_DICH_VU = ss.TDL_SERVICE_NAME ?? "",
                            MA_CHI_SO = tein.TEST_INDEX_CODE ?? "",
                            TEN_CHI_SO = tein.TEST_INDEX_NAME ?? "",
                            GIA_TRI = tein.VALUE ?? "",
                            DON_VI_DO = tein.TEST_INDEX_UNIT_NAME ?? "",
                            MO_TA = moTa,
                            KET_LUAN = ketLuan,
                            LoginNameBacSi = bacSi
                        });
                    }
                }
                else
                {
                    rows.Add(new Qd1551ClsRow
                    {
                        MA_DICH_VU = ss.TDL_SERVICE_CODE ?? "",
                        TEN_DICH_VU = ss.TDL_SERVICE_NAME ?? "",
                        MA_CHI_SO = "",
                        TEN_CHI_SO = ss.TDL_SERVICE_NAME ?? "",
                        GIA_TRI = "",
                        DON_VI_DO = "",
                        MO_TA = moTa,
                        KET_LUAN = ketLuan,
                        LoginNameBacSi = bacSi
                    });
                }
            }
            return result;
        }

        /// <summary>Goi API danh sach kieu GetRO (co ApiResultObject) — dung cho HIS_TREATMENT_GET.</summary>
        private static List<T> GetListRO<T>(string uri, object filter)
        {
            try
            {
                var param = new CommonParam();
                ApiResultObject<List<T>> rs = new BackendAdapter(param).GetRO<List<T>>(uri, ApiConsumers.MosConsumer, filter, param);
                return (rs != null) ? rs.Data : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Goi 1 call gop api/HisKskSync/GetKskData -> HisKskDataSDO (chua toan bo du lieu KSK cua CA LIST
        /// ho so theo SERVICE_REQ_IDs + TREATMENT_IDs). Loi -> null (BuildInputs coi nhu du lieu KSK rong).
        /// </summary>
        private static MOS.SDO.HisKskDataSDO GetKskDataSdo(MOS.Filter.HisKskDataFilter filter)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<MOS.SDO.HisKskDataSDO>(
                    "api/HisKskSync/GetKskData", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static Dictionary<long, T> IndexBy<T>(List<T> list, Func<T, long> key)
        {
            var d = new Dictionary<long, T>();
            if (list != null)
                foreach (var x in list) { long k = key(x); if (k > 0 && !d.ContainsKey(k)) d[k] = x; }
            return d;
        }

        private static Dictionary<long, List<T>> GroupByKey<T>(List<T> list, Func<T, long> key)
        {
            var d = new Dictionary<long, List<T>>();
            if (list != null)
                foreach (var x in list)
                {
                    long k = key(x); if (k <= 0) continue;
                    List<T> l; if (!d.TryGetValue(k, out l)) { l = new List<T>(); d[k] = l; }
                    l.Add(x);
                }
            return d;
        }

        private static T ValOrNull<T>(Dictionary<long, T> d, long k) where T : class
        {
            T v; return (d != null && d.TryGetValue(k, out v)) ? v : null;
        }

        private static List<T> ListOrNull<T>(Dictionary<long, List<T>> d, long k)
        {
            List<T> v; return (d != null && d.TryGetValue(k, out v)) ? v : null;
        }

        #region build config / certificate / result
        /// <summary>
        /// Parse chuoi HIS_CONFIG (theo vien) -> Qd1551Config. branchCode null -> lay cau hinh dau tien.
        /// Toan bo thong tin (ke ca khoa bi mat ky checksum = truong cuoi) lay tu cau hinh he thong
        /// MOS.HIS_KSK_SYNC.CONNECTION_INFO — khong con gia tri fix cung trong code.
        /// </summary>
        private Qd1551Config BuildConfig()
        {
            return Qd1551ConfigParser.Parse(this.connectionInfo, null) ?? new Qd1551Config();
        }

        /// <summary>Lay chung thu so (co private key) theo serial da chon o SettingSignInfo khi bat ky so.</summary>
        private X509Certificate2 LoadCertificate()
        {
            try
            {
                if (!this.sign || this.signSetting == null || string.IsNullOrEmpty(this.signSetting.SerialNumber))
                    return null;
                return Inventec.Common.SignFile.CertUtil.GetBySerial(this.signSetting.SerialNumber, requirePrivateKey: true, validOnly: false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private KskSyncResultADO BuildResultAdo(V_HIS_KSK_SYNC row, ResultADO pushResult, long syncTime)
        {
            KskSyncResultADO ado = NewResult(row, syncTime);
            PushResponse resp = ExtractResponse(pushResult);
            bool success = pushResult != null && pushResult.Success;

            ado.SYNC_RESULT_TYPE = success ? RESULT_SUCCESS : RESULT_FAILED;
            // Ma giao dich / trang thai: uu tien tu PushResponse cong BYT; fallback Data[2]/Data[3]
            // (chuoi do PushListMulti chuan hoa — dung cho cong HSSK, response khac kieu).
            string txn = (resp != null) ? resp.TxnId : null;
            string regState = (resp != null && resp.Data != null) ? resp.Data.DataState : null;
            if (pushResult != null && pushResult.Data != null)
            {
                if (string.IsNullOrEmpty(txn) && pushResult.Data.Length > 2) txn = pushResult.Data[2] as string;
                if (string.IsNullOrEmpty(regState) && pushResult.Data.Length > 3) regState = pushResult.Data[3] as string;
            }
            ado.TRANSACTION_CODE = txn;
            ado.REGISTRATION_NO = regState;
            if (!success)
                ado.SYNC_FAILD_REASON = (pushResult != null && !string.IsNullOrEmpty(pushResult.Message))
                    ? pushResult.Message : "Đồng bộ thất bại";
            return ado;
        }

        private KskSyncResultADO BuildFailedResult(V_HIS_KSK_SYNC row, long syncTime, string reason)
        {
            KskSyncResultADO ado = NewResult(row, syncTime);
            ado.SYNC_RESULT_TYPE = RESULT_FAILED;
            ado.SYNC_FAILD_REASON = reason;
            return ado;
        }

        private static KskSyncResultADO NewResult(V_HIS_KSK_SYNC row, long syncTime)
        {
            return new KskSyncResultADO
            {
                KSK_TYPE_ID = ToLong(GetProp(row, "KSK_TYPE_ID")),
                KSK_RECORD_ID = ToLong(GetProp(row, "KSK_RECORD_ID")),
                PATIENT_CODE = SafeString(GetProp(row, "TDL_PATIENT_CODE")),
                KskTypeName = SafeString(GetProp(row, "KSK_TYPE_NAME")),
                SYNC_TIME = syncTime
            };
        }

        /// <summary>ResultADO.Data cua PushList = [PushResponse (hoac null), tag].</summary>
        private static PushResponse ExtractResponse(ResultADO r)
        {
            if (r == null || r.Data == null || r.Data.Length == 0) return null;
            return r.Data[0] as PushResponse;
        }

        private static V_HIS_KSK_SYNC ExtractTag(ResultADO r)
        {
            if (r == null || r.Data == null || r.Data.Length < 2) return null;
            return r.Data[1] as V_HIS_KSK_SYNC;
        }
        #endregion

        #region helper
        private static object GetProp(object obj, string name)
        {
            try
            {
                if (obj == null) return null;
                var p = obj.GetType().GetProperty(name);
                return p != null ? p.GetValue(obj, null) : null;
            }
            catch { return null; }
        }
        private static string SafeString(object o) { return o == null ? "" : o.ToString(); }
        private static long ToLong(object o)
        {
            try { return o == null ? 0 : Convert.ToInt64(o); }
            catch { return 0; }
        }
        #endregion
    }
}

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
        private readonly string connectionInfo;
        private readonly bool sign;
        private readonly SettingSignADO signSetting;

        // Ket qua dong bo (khop KskSyncResultADO.SYNC_RESULT_TYPE): 2 = thanh cong, 3 = that bai.
        private const short RESULT_SUCCESS = 2;
        private const short RESULT_FAILED = 3;

        internal KskSyncProcessor(string connectionInfo, bool sign, SettingSignADO signSetting)
        {
            this.connectionInfo = connectionInfo;
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

                List<ResultADO> pushResults = main.PushList(inputs, certificate, dataSigner);
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
        private List<Qd1551KskInput> BuildInputs(List<V_HIS_KSK_SYNC> rowList)
        {
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
            // Ma GTIN/GLN co so — cau hinh he thong (SenderId trong CONNECTION_INFO).
            string maGtinCskcb = "";
            try { var cfg = BuildConfig(); maGtinCskcb = (cfg != null && cfg.SenderId != null) ? cfg.SenderId : ""; }
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
                    MaCskcb = (branch != null) ? (branch.HEIN_MEDI_ORG_CODE ?? "") : "",
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

            var teinBySs = GroupByKey(teins, t => t.SERE_SERV_ID);
            var extBySs = IndexBy(exts, e => e.SERE_SERV_ID);

            foreach (var ss in sereServs)
            {
                if (ss == null || ss.IS_NO_EXECUTE != null) continue;   // chi lay dich vu DA thuc hien
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
                            MA_CHI_SO = tein.TEST_INDEX_CODE ?? "",
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
                        MA_CHI_SO = "",
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
            if (resp != null)
            {
                ado.TRANSACTION_CODE = resp.TxnId;
                if (resp.Data != null)
                    ado.REGISTRATION_NO = resp.Data.DataState;
            }
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

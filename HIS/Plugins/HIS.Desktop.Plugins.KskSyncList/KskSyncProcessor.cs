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
using HIS.Desktop.Plugins.KskSyncList.ADO;
using His.Ksk.QD1551;
using His.Ksk.QD1551.Base;
using His.Ksk.QD1551.Builder;
using His.Ksk.QD1551.Transport.Model;
using Inventec.Common.Adapter;
using Inventec.Core;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Diem rap noi thu vien dong bo QD 1551 (His.Ksk.QD1551 - thiet ke BD_046, muc 3.4 PTTK_44350).
    ///
    /// Plugin: (1) map ban ghi V_HIS_KSK_SYNC -> mau phieu QD1551 (KskSyncModelMapper);
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
                Dictionary<long, long> inTimes = LoadInTimes(new[] { row });
                FormType formType = Qd1551FormMapper.ResolveFormType(ToLong(GetProp(row, "KSK_TYPE_ID")));
                IKsk1551Form model = Qd1551FormMapper.BuildModel(formType, ToSourceData(row, inTimes));

                ResultADO result = main.BuildPreview(formType, model);
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
                Dictionary<long, long> inTimes = LoadInTimes(rowList);

                List<Qd1551PushItem> items = new List<Qd1551PushItem>();
                foreach (var row in rowList)
                {
                    FormType formType = Qd1551FormMapper.ResolveFormType(ToLong(GetProp(row, "KSK_TYPE_ID")));
                    IKsk1551Form model = Qd1551FormMapper.BuildModel(formType, ToSourceData(row, inTimes));
                    items.Add(new Qd1551PushItem
                    {
                        FormType = formType,
                        Model = model,
                        Tag = row
                    });
                }

                // Ky DU LIEU vao the CKS_BENH_VIEN (HSM/USB) - xu ly nhu ExportXmlQD130, thuc hien o plugin.
                Func<string, string> dataSigner = null;
                if (this.sign && this.signSetting != null)
                    dataSigner = new KskSyncSigner(this.signSetting).SignCksBenhVien;

                List<ResultADO> pushResults = main.PushList(items, certificate, dataSigner);
                foreach (var pr in pushResults)
                {
                    V_HIS_KSK_SYNC row = ExtractTag(pr);
                    results.Add(BuildResultAdo(row, pr, syncTime));
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

        /// <summary>Trich gia tri tu ban ghi HIS -> DTO nguon trung lap cua thu vien (input cho Qd1551FormMapper).
        /// inTimes: map TDL_TREATMENT_ID -> HIS_TREATMENT.IN_TIME (ngay vao) da nap truoc qua API.</summary>
        private static Qd1551SourceData ToSourceData(V_HIS_KSK_SYNC row, Dictionary<long, long> inTimes)
        {
            long treatmentId = ToLong(GetProp(row, "TDL_TREATMENT_ID"));
            long inTime = 0;
            if (inTimes != null) inTimes.TryGetValue(treatmentId, out inTime);

            return new Qd1551SourceData
            {
                KskTypeId = ToLong(GetProp(row, "KSK_TYPE_ID")),
                PatientName = SafeString(GetProp(row, "TDL_PATIENT_NAME")),
                PatientCode = SafeString(GetProp(row, "TDL_PATIENT_CODE")),
                PatientDob = ToLong(GetProp(row, "TDL_PATIENT_DOB")),
                GenderName = SafeString(GetProp(row, "TDL_PATIENT_GENDER_NAME")),
                ConclusionTime = ToLong(GetProp(row, "CONCLUSION_TIME")),
                InTime = inTime,
                Conclusion = SafeString(GetProp(row, "CONCLUSION")),
                TreatmentCode = SafeString(GetProp(row, "TDL_TREATMENT_CODE"))
            };
        }

        /// <summary>Nap ngay vao (HIS_TREATMENT.IN_TIME) theo TDL_TREATMENT_ID cua cac ho so (1 lan goi API).</summary>
        private Dictionary<long, long> LoadInTimes(IEnumerable<V_HIS_KSK_SYNC> rows)
        {
            var map = new Dictionary<long, long>();
            try
            {
                if (rows == null) return map;
                List<long> ids = rows.Where(r => r != null)
                                     .Select(r => ToLong(GetProp(r, "TDL_TREATMENT_ID")))
                                     .Where(id => id > 0)
                                     .Distinct()
                                     .ToList();
                if (ids.Count == 0) return map;

                HisTreatmentFilter filter = new HisTreatmentFilter { IDs = ids };
                CommonParam param = new CommonParam();
                ApiResultObject<List<HIS_TREATMENT>> apiResult = new BackendAdapter(param)
                    .GetRO<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);

                if (apiResult != null && apiResult.Data != null)
                {
                    foreach (var t in apiResult.Data)
                        if (t != null && !map.ContainsKey(t.ID)) map[t.ID] = t.IN_TIME;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return map;
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

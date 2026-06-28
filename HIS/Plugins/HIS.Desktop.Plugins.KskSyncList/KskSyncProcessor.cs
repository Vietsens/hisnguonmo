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
using System.Text;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.KskSyncList.ADO;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Diem rap noi thu vien dong bo QD 1551 (His.Ksk.QD1551 - thiet ke BD_046, muc 3.4 PTTK_44350).
    ///
    /// Thu vien QD1551 chiu trach nhiem: build XML/JSON tach rieng theo loai KSK -> ky du lieu
    /// (USB token nguoi ket luan + chung thu to chuc) -> dong envelope + ky checksum SHA256RSA
    /// -> xac thuc OAuth2 -> POST /api/platform/data-sync/push.
    ///
    /// Plugin chi: (1) map ban ghi KSK -> goi thu vien; (2) nhan ket qua tung ho so;
    /// (3) goi api/HisKskSync/SaveSyncResult de luu trang thai (muc 3.2.2).
    ///
    /// Khi thu vien BD_046 san sang: dat LIBRARY_INTEGRATED = true va thay than cac ham
    /// duoi day bang loi goi His.Ksk.QD1551.CreateQd1551Main (BuildPreview / Push / PushList).
    /// </summary>
    internal class KskSyncProcessor
    {
        /// <summary>Co bao thu vien His.Ksk.QD1551 (BD_046) da duoc tich hop hay chua.</summary>
        internal const bool LIBRARY_INTEGRATED = false;

        private readonly string connectionInfo;
        private readonly bool sign;
        private readonly SettingSignADO signSetting;

        internal KskSyncProcessor(string connectionInfo, bool sign, SettingSignADO signSetting)
        {
            this.connectionInfo = connectionInfo;
            this.sign = sign;
            this.signSetting = signSetting;
        }

        /// <summary>
        /// Xem truoc du lieu se day cua mot ho so (Scene 3). Tuong ung
        /// His.Ksk.QD1551.CreateQd1551Main.BuildPreview(row) - build (khong ky, khong gui).
        /// </summary>
        internal string BuildPreview(V_HIS_KSK_SYNC row)
        {
            // BD_046: return His.Ksk.QD1551.CreateQd1551Main.BuildPreview(row);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0,-14}: {1}", "SO", SafeString(GetProp(row, "TRANSACTION_CODE"))));
            sb.AppendLine(string.Format("{0,-14}: {1}", "HOTEN", SafeString(GetProp(row, "TDL_PATIENT_NAME"))));
            sb.AppendLine(string.Format("{0,-14}: {1}", "NGAYSINH", FormatDob(row)));
            sb.AppendLine(string.Format("{0,-14}: {1}", "GIOITINH", SafeString(GetProp(row, "TDL_PATIENT_GENDER_NAME"))));
            sb.AppendLine(string.Format("{0,-14}: {1}", "SOCMND/CCCD", "xxxxxxxxxxxx"));
            sb.AppendLine(string.Format("{0,-14}: {1}", "DIACHI", "Mã tỉnh / huyện / xã thường trú"));
            sb.AppendLine(string.Format("{0,-14}: {1}", "IDBENHVIEN", "(mã cơ sở KCB)"));
            sb.AppendLine(string.Format("{0,-14}: {1}", "NGAYKETLUAN", FormatTime(GetProp(row, "CONCLUSION_TIME"))));
            sb.AppendLine(string.Format("{0,-14}: {1}", "BACSYKETLUAN", SafeString(GetProp(row, "CONCLUDER_NAME"))));
            sb.AppendLine(string.Format("{0,-14}: {1}", "KETLUAN", SafeString(GetProp(row, "CONCLUSION"))));
            sb.AppendLine(string.Format("{0,-14}: {1}", "STATE", "ADD"));
            sb.AppendLine(string.Format("{0,-14}: {1}", "SIGNDATA", "(chữ ký số - sinh khi bấm Đồng bộ)"));
            return sb.ToString();
        }

        /// <summary>
        /// Day lo nhieu ho so (Scene 4). Tuong ung
        /// His.Ksk.QD1551.CreateQd1551Main.PushList(rows, signSetting) - moi ho so xu ly doc lap.
        /// </summary>
        internal List<KskSyncResultADO> PushList(IEnumerable<V_HIS_KSK_SYNC> rows, long syncTime)
        {
            List<KskSyncResultADO> results = new List<KskSyncResultADO>();
            foreach (var row in rows)
            {
                results.Add(PushOne(row, syncTime));
            }
            return results;
        }

        /// <summary>Day rieng mot ho so. Tuong ung CreateQd1551Main.Push(row, signSetting).</summary>
        internal KskSyncResultADO PushOne(V_HIS_KSK_SYNC row, long syncTime)
        {
            KskSyncResultADO result = new KskSyncResultADO();
            result.KSK_TYPE_ID = ToLong(GetProp(row, "KSK_TYPE_ID"));
            result.KSK_RECORD_ID = ToLong(GetProp(row, "KSK_RECORD_ID"));
            result.PATIENT_CODE = SafeString(GetProp(row, "TDL_PATIENT_CODE"));
            result.KskTypeName = SafeString(GetProp(row, "KSK_TYPE_NAME"));
            result.SYNC_TIME = syncTime;

            try
            {
                if (LIBRARY_INTEGRATED)
                {
                    // BD_046: var rs = His.Ksk.QD1551.CreateQd1551Main.Push(row, sign, signSetting, connectionInfo);
                    //         result.SYNC_RESULT_TYPE = rs.Success ? (short)2 : (short)3;
                    //         result.TRANSACTION_CODE = rs.TransactionCode;
                    //         result.REGISTRATION_NO  = rs.RegistrationNo;
                    //         result.SYNC_FAILD_REASON = rs.Message;
                    throw new NotImplementedException();
                }
                else
                {
                    result.SYNC_RESULT_TYPE = 3;
                    result.SYNC_FAILD_REASON = "Chưa tích hợp thư viện đẩy cổng QĐ1551 (His.Ksk.QD1551 - BD_046).";
                }
            }
            catch (Exception ex)
            {
                result.SYNC_RESULT_TYPE = 3;
                result.SYNC_FAILD_REASON = ex.Message;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

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
        private static string FormatTime(object o)
        {
            try { return Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(o)); }
            catch { return ""; }
        }
        private static string FormatDob(V_HIS_KSK_SYNC row)
        {
            try
            {
                object dob = GetProp(row, "TDL_PATIENT_DOB");
                object noDay = GetProp(row, "TDL_PATIENT_IS_HAS_NOT_DAY_DOB");
                if (noDay != null && Convert.ToInt32(noDay) == 1)
                    return SafeString(dob).Length >= 4 ? SafeString(dob).Substring(0, 4) : SafeString(dob);
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(dob));
            }
            catch { return ""; }
        }
        #endregion
    }
}

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
using His.Ksk.QD2062.Base;
using His.Ksk.QD2062.Sign;
using His.Ksk.QD2062.Transport;
using His.Ksk.QD2062.Transport.Model;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Phan tich chuoi cau hinh cong HCC — khoa <c>MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO</c>.
    /// Dinh dang (cac truong cach '|', cung ho voi cau hinh cong HOC):
    /// <code>
    /// MaCsyt|Username|Password|ReceiverId|DataType|Version|TokenUrl|PushUrl|PrivateKey
    /// </code>
    /// <list type="bullet">
    /// <item>MaCsyt: ma don vi 13 so HCC cap — dung cho CA <c>header.sender_id</c> VA <c>thongtindonvi/macskcb</c>.</item>
    /// <item>Username/Password: tai khoan lien thong HCC cap (POST /api/auth/login).</item>
    /// <item>ReceiverId: bo trong -> "HCC". DataType: bo trong -> "json/base64". Version: bo trong -> "1.0.6".</item>
    /// <item>TokenUrl/PushUrl: URL DAY DU (doi cong chi can doi 2 URL nay); bo trong -> URL mac dinh HCC.</item>
    /// <item>PrivateKey: khoa bi mat PEM cua don vi da dang ky PublicKey voi HCC; bo trong -> signature rong.</item>
    /// </list>
    /// Toi thieu 3 truong dau (MaCsyt, Username, Password) — thieu thi tra null (coi nhu chua cau hinh).
    /// Ket qua map vao Qd1551Config de tai su dung Qd1551Consumer/EnvelopeSigner cua thu vien:
    /// SenderId = MaCsyt, LoginUri = TokenUrl, PushUri = PushUrl, BaseUrl = RONG (CombineUrl tra nguyen URL).
    /// </summary>
    internal static class KskHccConfigParser
    {
        internal const string DEFAULT_BASE_URL = "https://ermhub.healthcarecenter.asia";
        internal const string DEFAULT_TOKEN_URL = DEFAULT_BASE_URL + "/api/auth/login";
        internal const string DEFAULT_PUSH_URL = DEFAULT_BASE_URL + "/api/platform/data-sync/push";
        private const int MIN_FIELD_COUNT = 3;

        internal static Qd1551Config Parse(string configValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(configValue)) return null;
                string[] f = configValue.Split('|');
                if (f.Length < MIN_FIELD_COUNT) return null;

                string maCsyt = Get(f, 0);
                string username = Get(f, 1);
                string password = Get(f, 2);
                if (string.IsNullOrWhiteSpace(maCsyt) || string.IsNullOrWhiteSpace(username)
                    || string.IsNullOrWhiteSpace(password))
                    return null;

                Qd1551Config cfg = new Qd1551Config();
                cfg.BaseUrl = "";                 // PHAI rong: LoginUri/PushUri la URL day du (xem CombineUrl)
                cfg.SenderId = maCsyt;            // header.sender_id + thongtindonvi/macskcb
                cfg.Username = username;
                cfg.Password = password;

                string receiverId = Get(f, 3);
                cfg.ReceiverId = !string.IsNullOrWhiteSpace(receiverId) ? receiverId : KskHccPusher.RECEIVER_ID;

                string dataType = Get(f, 4);
                cfg.DataType = !string.IsNullOrWhiteSpace(dataType) ? dataType : KskHccPusher.DATA_TYPE_JSON;

                string version = Get(f, 5);
                if (!string.IsNullOrWhiteSpace(version)) cfg.Version = version;

                string tokenUrl = Get(f, 6);
                cfg.LoginUri = !string.IsNullOrWhiteSpace(tokenUrl) ? tokenUrl : DEFAULT_TOKEN_URL;

                string pushUrl = Get(f, 7);
                cfg.PushUri = !string.IsNullOrWhiteSpace(pushUrl) ? pushUrl : DEFAULT_PUSH_URL;

                string privateKey = Get(f, 8);
                if (!string.IsNullOrWhiteSpace(privateKey)) cfg.ChecksumPrivateKeyPem = privateKey;

                return cfg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private static string Get(string[] arr, int index)
        {
            return (arr != null && index < arr.Length && arr[index] != null) ? arr[index].Trim() : null;
        }
    }

    /// <summary>Ket qua day 1 ho so len cong HCC (chuan hoa de gop voi ket qua cac cong khac).</summary>
    internal class KskHccPushResult
    {
        internal bool Success { get; set; }
        internal string Message { get; set; }
        internal string TxnCode { get; set; }   // header.txn_id
        internal string State { get; set; }     // data.data_state

        internal static KskHccPushResult Failure(string message)
        {
            return new KskHccPushResult { Success = false, Message = message };
        }
    }

    /// <summary>
    /// Day ban tin KSK len cong HCC (Health Care Center - https://ermhub.healthcarecenter.asia) theo
    /// "HUONG DAN TICH HOP API - Lien thong du lieu KSK -> HCC" v1.0 (21/07/2026), chuan Phu luc 02
    /// (Quyet dinh sua doi 1551/QD-BYT). Cau hinh: MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO.
    ///
    /// HCC dung DUNG giao thuc truc Bo Y te (tai lieu: "chuyen cong chi can doi domain"):
    ///   1. POST {BaseUrl}/api/auth/login  {username,password} -> data.token (duration ~6000s)
    ///   2. POST {BaseUrl}/api/platform/data-sync/push  (Authorization: Bearer, service-type: 100)
    ///      body = { header, data = base64(ban tin khamsuckhoe), signature }
    ///   3. Chu ky: A = UPPER(SHA256(headerJson)), B = UPPER(SHA256(trim(data))), C = A + "." + B,
    ///      signature = BASE64(RSA-SHA256(C, privateKey cua don vi)) -> dung EnvelopeSigner cua thu vien.
    ///   4. Thanh cong: header.res_code = PS_SYNC_SUCCESS (PushResponse.IsSuccess() = res_code EndsWith SUCCESS).
    ///
    /// Vi giao thuc trung voi cong BYT nen lop nay TAI SU DUNG ha tang thu vien His.Ksk.QD2062
    /// (Qd1551Consumer.Login/PushData + EnvelopeSigner + PushEnvelope/PushHeader), chi khac:
    /// receiver_id = "HCC", data_type mac dinh json/base64, khoa ky lay tu cau hinh RIENG cua cong HCC.
    /// (Khi co source thu vien: chuyen logic nay vao CreateQd1551Main.PushListMulti nhu BYT/HSSK/HOC.)
    /// </summary>
    internal class KskHccPusher
    {
        internal const string RECEIVER_ID = "HCC";              // tai lieu muc 3.2: gia tri co dinh
        internal const string DATA_TYPE_JSON = "json/base64";   // tai lieu muc 3.2: gia tri co dinh
        private const string TXN_TYPE = "sync_checkup";
        private const string MSG_TYPE = "101";
        private const string VERSION = "1.0.6";
        private const long TOKEN_TTL_DEFAULT_SECONDS = 6000;    // tai lieu muc 2: duration = 6000
        private const long TOKEN_TTL_SAFETY_SECONDS = 60;       // tru bien de khong het han giua lo
        private const long TOKEN_TTL_MIN_SECONDS = 60;
        private const int MAX_ATTEMPT = 2;                      // 401 / token het han -> dang nhap lai 1 lan
        private const int HTTP_UNAUTHORIZED = 401;
        private const string RES_CODE_AUTH_PERMISSION = "CM_AUTH_PERMISSION";      // token sai/het han
        private const string RES_CODE_AUTH_EXPIRED = "CM_AUTH_EXPIRED";
        private const string RES_CODE_AUTH_ACCOUNT_FAIL = "CM_AUTH_ACCOUNT_FAIL";  // sai tai khoan -> khong retry

        private readonly Qd1551Config config;
        private readonly Qd1551Consumer consumer = new Qd1551Consumer();
        private readonly EnvelopeSigner signer = new EnvelopeSigner();
        private string cachedToken;
        private DateTime tokenExpireAt = DateTime.MinValue;

        internal KskHccPusher(Qd1551Config config)
        {
            this.config = config;
        }

        /// <summary>
        /// Day base64 cua MOT ho so (envelope SOLUONGHOSO = 1) len cong HCC. Token duoc cache trong
        /// instance nay (dung chung ca lo); gap 401/het han thi dang nhap lai va day lai 1 lan.
        /// </summary>
        internal KskHccPushResult Push(string dataBase64)
        {
            try
            {
                string configError = ValidateConfig();
                if (configError != null) return KskHccPushResult.Failure(configError);
                if (string.IsNullOrEmpty(dataBase64))
                    return KskHccPushResult.Failure("HCC: không dựng được dữ liệu đẩy.");

                for (int attempt = 0; attempt < MAX_ATTEMPT; attempt++)
                {
                    string token = GetToken();
                    if (string.IsNullOrWhiteSpace(token))
                        return KskHccPushResult.Failure("HCC: đăng nhập thất bại (kiểm tra tài khoản liên thông).");

                    PushHeader header = BuildHeader();
                    string headerJson = Newtonsoft.Json.JsonConvert.SerializeObject(header);
                    // Chu ky RSA-SHA256 tren (SHA256 header . SHA256 data) — khoa RIENG cua don vi cap cho HCC
                    // (khac khoa EMRHUB dung cho cong BYT). Chua dang ky PublicKey -> de trong.
                    string signature = this.signer.Sign(headerJson, dataBase64, this.config.ChecksumPrivateKeyPem);
                    PushEnvelope envelope = new PushEnvelope
                    {
                        Header = header,
                        Data = dataBase64,
                        Signature = signature ?? ""
                    };

                    PushResponse response = this.consumer.PushData(this.config, envelope, token);
                    if (IsAuthExpired(response) && attempt + 1 < MAX_ATTEMPT)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("HCC: token hết hạn (401/CM_AUTH_PERMISSION) -> đăng nhập lại và đẩy lại.");
                        ResetToken();
                        continue;
                    }
                    if (response == null)
                        return KskHccPushResult.Failure("HCC: không nhận được phản hồi từ cổng.");

                    bool success = response.IsSuccess();
                    return new KskHccPushResult
                    {
                        Success = success,
                        Message = success
                            ? null
                            : (!string.IsNullOrEmpty(response.ResCode) || !string.IsNullOrEmpty(response.ResMsg)
                                ? string.Format("HCC: {0} {1}", response.ResCode, response.ResMsg)
                                : "HCC: đồng bộ thất bại"),
                        TxnCode = response.TxnId,
                        State = (response.Data != null) ? response.Data.DataState : null
                    };
                }
                return KskHccPushResult.Failure("HCC: xác thực thất bại (token hết hạn sau khi đăng nhập lại).");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return KskHccPushResult.Failure("HCC: " + ex.Message);
            }
        }

        /// <summary>
        /// Kiem tra cau hinh du de day: tai khoan + ma don vi 13 so + 2 URL. KHONG dung Qd1551Config.IsValid()
        /// vi ham do doi BaseUrl khac rong, con cau hinh HCC dung LoginUri/PushUri la URL DAY DU (BaseUrl rong).
        /// Tra null neu hop le, nguoc lai tra thong bao loi.
        /// </summary>
        private string ValidateConfig()
        {
            if (this.config == null)
                return "HCC: chưa cấu hình kết nối (MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO).";
            var missing = new List<string>();
            if (string.IsNullOrWhiteSpace(this.config.SenderId)) missing.Add("mã đơn vị (13 số)");
            if (string.IsNullOrWhiteSpace(this.config.Username)) missing.Add("tài khoản");
            if (string.IsNullOrWhiteSpace(this.config.Password)) missing.Add("mật khẩu");
            if (string.IsNullOrWhiteSpace(this.config.LoginUri)) missing.Add("URL đăng nhập");
            if (string.IsNullOrWhiteSpace(this.config.PushUri)) missing.Add("URL đẩy dữ liệu");
            if (missing.Count == 0) return null;
            return "HCC: cấu hình kết nối thiếu " + string.Join(", ", missing.ToArray())
                 + " (MOS.HIS_KSK_SYNC.HSSK_HCC_2062_CONNECTION_INFO).";
        }

        /// <summary>Token cache theo duration cong tra ve (mac dinh 6000s, tru 60s an toan, toi thieu 60s).</summary>
        private string GetToken()
        {
            if (!string.IsNullOrWhiteSpace(this.cachedToken) && DateTime.Now < this.tokenExpireAt)
                return this.cachedToken;

            AuthResponse auth = this.consumer.Login(this.config);
            string token = (auth != null && auth.Data != null) ? auth.Data.Token : null;
            if (string.IsNullOrWhiteSpace(token))
            {
                string resCode = (auth != null && auth.Header != null) ? auth.Header.ResCode : "(null)";
                Inventec.Common.Logging.LogSystem.Error("HCC login that bai. res_code: " + resCode);
                ResetToken();
                return null;
            }

            long ttl = (auth.Data.Duration > 0) ? auth.Data.Duration : TOKEN_TTL_DEFAULT_SECONDS;
            ttl = Math.Max(TOKEN_TTL_MIN_SECONDS, ttl - TOKEN_TTL_SAFETY_SECONDS);
            this.cachedToken = token;
            this.tokenExpireAt = DateTime.Now.AddSeconds(ttl);
            return this.cachedToken;
        }

        private void ResetToken()
        {
            this.cachedToken = null;
            this.tokenExpireAt = DateTime.MinValue;
        }

        /// <summary>HTTP 401 hoac res_code bao token sai/het han (sai tai khoan thi KHONG dang nhap lai).</summary>
        private bool IsAuthExpired(PushResponse response)
        {
            string resCode = (response != null) ? response.ResCode : null;
            if (!string.IsNullOrEmpty(resCode))
            {
                if (string.Equals(resCode, RES_CODE_AUTH_ACCOUNT_FAIL, StringComparison.OrdinalIgnoreCase)) return false;
                if (string.Equals(resCode, RES_CODE_AUTH_PERMISSION, StringComparison.OrdinalIgnoreCase)) return true;
                if (string.Equals(resCode, RES_CODE_AUTH_EXPIRED, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return this.consumer.LastStatusCode == HTTP_UNAUTHORIZED;
        }

        /// <summary>
        /// Header theo tai lieu muc 3.2 — cac gia tri co dinh co the ghi de bang cau hinh (truong DataType/
        /// ReceiverId trong chuoi cau hinh) de sau nay chuyen sang truc BYT chi can doi cau hinh.
        /// </summary>
        private PushHeader BuildHeader()
        {
            string senderId = this.config.SenderId ?? "";
            return new PushHeader
            {
                Version = !string.IsNullOrWhiteSpace(this.config.Version) ? this.config.Version : VERSION,
                SenderId = senderId,
                ReceiverId = !string.IsNullOrWhiteSpace(this.config.ReceiverId) ? this.config.ReceiverId : RECEIVER_ID,
                TxnType = !string.IsNullOrWhiteSpace(this.config.TxnType) ? this.config.TxnType : TXN_TYPE,
                MsgType = !string.IsNullOrWhiteSpace(this.config.MsgType) ? this.config.MsgType : MSG_TYPE,
                DataType = !string.IsNullOrWhiteSpace(this.config.DataType) ? this.config.DataType : DATA_TYPE_JSON,
                SendDatetime = GetUnixMilliseconds(),
                MsgId = GenerateMsgId(senderId)
            };
        }

        /// <summary>msg_id = sender_id + YYMMDD + UUIDv4 (bo dau gach) — KHONG duoc trung giua cac lan gui.</summary>
        private static string GenerateMsgId(string senderId)
        {
            return (senderId ?? "")
                 + DateTime.Now.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture)
                 + Guid.NewGuid().ToString("N");
        }

        /// <summary>send_datetime = unix timestamp 13 so (mili giay).</summary>
        private static long GetUnixMilliseconds()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}

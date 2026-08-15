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

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Tien ich chuan hoa khoa bi mat RSA (PEM) truoc khi dua vao EnvelopeSigner cua thu vien QD2062.
    ///
    /// LY DO: thu vien doc khoa bang <c>PrivateKeyFactory.CreateKey(der)</c> — chi hieu DER dang
    /// PKCS#8 (PEM header <c>-----BEGIN PRIVATE KEY-----</c>). Trong khi tai lieu HCC muc 5.2 huong dan
    /// sinh khoa bang <c>openssl genrsa</c> — OpenSSL 1.x xuat ra PKCS#1 (<c>-----BEGIN RSA PRIVATE KEY-----</c>).
    /// Khoa PKCS#1 se lam thu vien nem loi -> chu ky RONG -> cong tra PS_SIGNATURE_INVALID.
    ///
    /// Lop nay phat hien PKCS#1 va boc lai thanh PKCS#8 (thuan DER, khong can thu vien mat ma):
    ///   PrivateKeyInfo ::= SEQUENCE { version INTEGER 0,
    ///                                 algorithm SEQUENCE { OID 1.2.840.113549.1.1.1, NULL },
    ///                                 privateKey OCTET STRING (chua nguyen ban DER PKCS#1) }
    /// Khoa da la PKCS#8 (hoac khong nhan dang duoc) -> GIU NGUYEN.
    /// </summary>
    internal static class KskPemUtil
    {
        private const string HEADER_PKCS1 = "RSA PRIVATE KEY";
        private const string HEADER_PKCS8 = "BEGIN PRIVATE KEY";

        /// <summary>DER cua AlgorithmIdentifier { OID rsaEncryption (1.2.840.113549.1.1.1), NULL }.</summary>
        private static readonly byte[] ALG_RSA_ENCRYPTION = new byte[]
        {
            0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00
        };

        /// <summary>
        /// Tra ve chuoi khoa dung dinh dang PKCS#8 (base64, khong header) de thu vien doc duoc.
        /// Khong nhan dang duoc / da dung dinh dang -> tra nguyen chuoi dau vao.
        /// </summary>
        internal static string EnsurePkcs8(string pem)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pem)) return pem;

                bool looksPkcs1 = pem.IndexOf(HEADER_PKCS1, StringComparison.OrdinalIgnoreCase) >= 0;
                bool looksPkcs8 = pem.IndexOf(HEADER_PKCS8, StringComparison.OrdinalIgnoreCase) >= 0;

                byte[] der = DecodePemBody(pem);
                if (der == null || der.Length == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "HCC: khoa bi mat khong giai duoc base64 -> giu nguyen (kiem tra lai chuoi cau hinh).");
                    return pem;
                }

                if (!looksPkcs1 && (looksPkcs8 || !IsPkcs1(der))) return pem;   // da PKCS#8 / khong ro -> giu nguyen

                byte[] pkcs8 = WrapPkcs1AsPkcs8(der);
                Inventec.Common.Logging.LogSystem.Info(
                    "HCC: khoa bi mat dang PKCS#1 (BEGIN RSA PRIVATE KEY) -> tu dong chuyen sang PKCS#8 de ky.");
                return Convert.ToBase64String(pkcs8);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return pem;
            }
        }

        /// <summary>
        /// TU KIEM TRA chu ky vua tao: tinh lai C = SHA256(headerJson) + "." + SHA256(trim(data)) roi verify
        /// bang PUBLIC KEY suy ra tu chinh private key dang khai. Ket qua noi ro loi nam o dau:
        ///   - hop le  -> thuat toan/khoa phia minh DUNG; cong tu choi = public key da dang ky KHAC cap khoa nay.
        ///   - khong hop le -> loi o khau ky phia minh.
        /// Ghi kem PUBLIC KEY (PEM 1 dong) de doi chieu voi public_key.pem da gui cong.
        /// </summary>
        /// <returns>true = chu ky hop le; false = khong hop le; null = khong kiem tra duoc.</returns>
        internal static bool? SelfCheckSignature(string headerJson, string dataBase64, string signatureBase64, string privateKeyPem)
        {
            try
            {
                if (string.IsNullOrEmpty(signatureBase64)) return null;
                byte[] der = DecodePemBody(EnsurePkcs8(privateKeyPem));
                if (der == null) return null;

                var privateKey = Org.BouncyCastle.Security.PrivateKeyFactory.CreateKey(der);
                var crt = privateKey as Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters;
                if (crt == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("HCC: khoa bi mat khong phai RSA -> bo qua tu kiem tra chu ky.");
                    return null;
                }
                var publicKey = new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(false, crt.Modulus, crt.PublicExponent);

                string c = Sha256HexUpper(string.IsNullOrEmpty(headerJson) ? "{}" : headerJson)
                         + "." + Sha256HexUpper((dataBase64 ?? "").Trim());
                var verifier = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA256WITHRSA");
                verifier.Init(false, publicKey);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(c);
                verifier.BlockUpdate(bytes, 0, bytes.Length);
                bool ok = verifier.VerifySignature(Convert.FromBase64String(signatureBase64));

                string publicPem = Convert.ToBase64String(
                    Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey).GetDerEncoded());

                if (ok)
                    Inventec.Common.Logging.LogSystem.Info(
                        "HCC: chu ky TU KIEM TRA HOP LE (dung thuat toan SHA256 header + \".\" + SHA256 data, RSA base64)."
                        + " Neu cong van tra PS_SIGNATURE_INVALID => PUBLIC KEY da dang ky voi cong KHAC cap khoa dang khai."
                        + " Public key tuong ung khoa dang khai (so voi public_key.pem da gui cong): " + publicPem);
                else
                    Inventec.Common.Logging.LogSystem.Error(
                        "HCC: chu ky TU KIEM TRA KHONG HOP LE -> loi o khau ky phia HIS (bao dev). Public key suy ra: " + publicPem);
                return ok;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("HCC: khong tu kiem tra duoc chu ky: " + ex.Message);
                return null;
            }
        }

        /// <summary>SHA-256 cua chuoi (UTF-8) -> hex IN HOA (giong EnvelopeSigner cua thu vien).</summary>
        /// <summary>
        /// Băm SHA256 rồi đưa về hex VIẾT HOA — dùng cho chữ ký của cổng Sở Y tế TP.HCM.
        /// Chỉ là lớp bọc ngoài hàm sẵn có, không đổi hàm gốc để 4 cổng đang chạy không bị ảnh hưởng.
        /// </summary>
        internal static string Sha256HexUpperForSyt(string value)
        {
            return Sha256HexUpper(value);
        }

        /// <summary>
        /// Ký chuỗi bằng RSA-SHA256 (đệm PKCS#1 v1.5) rồi đưa kết quả về hex VIẾT HOA.
        ///
        /// Khác 4 cổng cũ ở ĐỊNH DẠNG ĐẦU RA: cổng Sở Y tế TP.HCM nhận hex viết hoa, không phải
        /// base64. Trả chuỗi rỗng nếu khóa không đọc được — nơi gọi tự báo lỗi, không ném ra ngoài.
        /// </summary>
        internal static string SignRsaSha256HexUpper(string content, string privateKeyPem)
        {
            try
            {
                if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(privateKeyPem)) return "";

                // Chan doan truoc khi giai ma, de log noi ro nguyen nhan thay vi chi "khoa khong hop le".
                string trimmed = privateKeyPem.Trim();
                string bodyOnly = System.Text.RegularExpressions.Regex.Replace(trimmed, "-----[^-]+-----", "")
                    .Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
                if (bodyOnly.Length == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: khoa rieng trong cau hinh CHI CO dong tieu de, KHONG co phan than. "
                        + "Tep .pem co 28 dong; o nhap cau hinh thuong chi lay dong dau. "
                        + "Hay GOP TOAN BO phan than thanh MOT DONG (bo 2 dong -----BEGIN/END-----) "
                        + "roi dan vao truong thu 7. Do dai chuoi hien tai = " + trimmed.Length + " ky tu.");
                    return "";
                }
                if (trimmed.IndexOf("PUBLIC KEY", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: truong thu 7 dang la KHOA CONG KHAI (PUBLIC KEY). Khoa cong khai gui cho So, "
                        + "con cau hinh phai dien KHOA RIENG (client_private_key.pem).");
                    return "";
                }

                byte[] der = DecodePemBody(EnsurePkcs8(privateKeyPem));
                if (der == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: khong giai duoc base64 cua khoa rieng -> chuoi bi CAT BOT hoac lan ky tu la. "
                        + "Do dai phan than = " + bodyOnly.Length + " ky tu; khoa RSA 2048 bit dang PKCS#8 "
                        + "thuong dai khoang 1600-1640 ky tu.");
                    return "";
                }

                object privateKey;
                try
                {
                    privateKey = Org.BouncyCastle.Security.PrivateKeyFactory.CreateKey(der);
                }
                catch (Exception exKey)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: doc duoc base64 nhung KHONG dung dinh dang khoa rieng ("
                        + exKey.GetType().Name + "). Kiem tra lai dung tep client_private_key.pem, "
                        + "sinh boi: openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048");
                    return "";
                }
                var signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner("SHA256WITHRSA");
                signer.Init(true, (Org.BouncyCastle.Crypto.AsymmetricKeyParameter)privateKey);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
                signer.BlockUpdate(bytes, 0, bytes.Length);
                byte[] sig = signer.GenerateSignature();

                System.Text.StringBuilder sb = new System.Text.StringBuilder(sig.Length * 2);
                for (int i = 0; i < sig.Length; i++) sb.Append(sig[i].ToString("X2"));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private static string Sha256HexUpper(string value)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value ?? ""));
                var sb = new System.Text.StringBuilder(hash.Length * 2);
                foreach (byte b in hash) sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }

        /// <summary>Mo ta dinh dang khoa de ghi log chan doan (KHONG lo noi dung khoa).</summary>
        internal static string DescribeKey(string pem)
        {
            if (string.IsNullOrWhiteSpace(pem)) return "(rong)";
            if (pem.IndexOf(HEADER_PKCS1, StringComparison.OrdinalIgnoreCase) >= 0) return "PKCS#1 (BEGIN RSA PRIVATE KEY)";
            if (pem.IndexOf(HEADER_PKCS8, StringComparison.OrdinalIgnoreCase) >= 0) return "PKCS#8 (BEGIN PRIVATE KEY)";
            if (pem.IndexOf("ENCRYPTED", StringComparison.OrdinalIgnoreCase) >= 0) return "PEM DA MA HOA (co passphrase — thu vien KHONG doc duoc)";
            byte[] der = DecodePemBody(pem);
            if (der == null || der.Length == 0) return "khong giai duoc base64 (len=" + pem.Length + ")";
            return (IsPkcs1(der) ? "PKCS#1 (khong co header)" : "PKCS#8 / khac (khong co header)") + ", DER " + der.Length + " byte";
        }

        /// <summary>Bo header/footer -----XXX----- va moi ky tu trang, roi giai base64.</summary>
        private static byte[] DecodePemBody(string pem)
        {
            try
            {
                string body = System.Text.RegularExpressions.Regex.Replace(pem, "-----[^-]+-----", "");
                body = body.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", "");
                return string.IsNullOrEmpty(body) ? null : Convert.FromBase64String(body);
            }
            catch { return null; }
        }

        /// <summary>
        /// PKCS#1 RSAPrivateKey = SEQUENCE { INTEGER 0, INTEGER modulus, INTEGER publicExponent, ... }
        /// -> phan tu thu 2 la INTEGER (0x02). PKCS#8 PrivateKeyInfo co phan tu thu 2 la SEQUENCE (0x30).
        /// </summary>
        private static bool IsPkcs1(byte[] der)
        {
            try
            {
                int pos = 0;
                if (der[pos++] != 0x30) return false;          // SEQUENCE
                ReadLength(der, ref pos);
                if (der[pos++] != 0x02) return false;          // version INTEGER
                int versionLen = ReadLength(der, ref pos);
                pos += versionLen;
                return der[pos] == 0x02;                       // PKCS#1: modulus INTEGER; PKCS#8: 0x30
            }
            catch { return false; }
        }

        /// <summary>Boc DER PKCS#1 vao PrivateKeyInfo (PKCS#8).</summary>
        private static byte[] WrapPkcs1AsPkcs8(byte[] pkcs1Der)
        {
            var content = new List<byte>();
            content.AddRange(new byte[] { 0x02, 0x01, 0x00 });                  // version INTEGER 0
            content.AddRange(ALG_RSA_ENCRYPTION);                               // AlgorithmIdentifier
            content.AddRange(DerTagged(0x04, pkcs1Der));                        // privateKey OCTET STRING
            return DerTagged(0x30, content.ToArray());                          // SEQUENCE bao ngoai
        }

        /// <summary>Tao 1 phan tu DER: tag + do dai + noi dung.</summary>
        private static byte[] DerTagged(byte tag, byte[] content)
        {
            var result = new List<byte>();
            result.Add(tag);
            result.AddRange(EncodeLength(content.Length));
            result.AddRange(content);
            return result.ToArray();
        }

        /// <summary>Ma hoa do dai theo DER (dang ngan &lt; 128, hoac dang dai nhieu byte).</summary>
        private static byte[] EncodeLength(int length)
        {
            if (length < 0x80) return new byte[] { (byte)length };
            var bytes = new List<byte>();
            int value = length;
            while (value > 0) { bytes.Insert(0, (byte)(value & 0xFF)); value >>= 8; }
            var result = new List<byte> { (byte)(0x80 | bytes.Count) };
            result.AddRange(bytes);
            return result.ToArray();
        }

        /// <summary>Doc do dai DER tai vi tri pos (pos se nhay qua phan do dai).</summary>
        private static int ReadLength(byte[] der, ref int pos)
        {
            int first = der[pos++];
            if (first < 0x80) return first;
            int count = first & 0x7F;
            int length = 0;
            for (int i = 0; i < count; i++) length = (length << 8) | der[pos++];
            return length;
        }
    }
}

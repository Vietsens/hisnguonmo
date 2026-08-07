using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.Sign
{
    /// <summary>
    /// Ký XML hồ sơ điều chỉnh mẫu 09/BH ĐÚNG profile mà cổng giám định BHXH đã tiếp nhận.
    /// Tham chiếu: file mẫu hsdc09_24664226_11_SIGNED.xml (BV 108) - cấu trúc bắt buộc:
    ///
    ///   Signature Id = "CHUKYDONVI-&lt;tagId&gt;"      (tagId = attribute Id của thẻ TT_HOSO)
    ///   CanonicalizationMethod = c14n 20010315
    ///   SignatureMethod        = rsa-sha256
    ///   Reference[0] URI="#Object-CHUKYDONVI-&lt;tagId&gt;"  KHÔNG Transform  (chứa SigningTime)
    ///   Reference[1] URI="#&lt;tagId&gt;"                     KHÔNG Transform  (trỏ vào TT_HOSO)
    ///   KeyInfo/X509Data = X509SubjectName + X509Certificate
    ///   Object Id="Object-CHUKYDONVI-&lt;tagId&gt;" chứa SignatureProperties/SignatureProperty/SigningTime
    ///           (các thẻ này nằm NGOÀI namespace xmldsig -> serialize ra xmlns="")
    ///
    /// KHÁC HẲN Inventec.Common.SignFile.SignData.SignXml130 của thư viện dùng chung
    /// (1 Reference URI="" + Transform enveloped-signature, không có SigningTime).
    ///
    /// ĐÂY LÀ ĐƯỜNG DỰ PHÒNG: đường chính là hàm ký dùng chung
    /// Inventec.Common.SignFile.SignData.SignXml09BH, gọi qua service EMR.SignProcessor (xem
    /// frmImportXmlAdjust.SignFileByService09BH). Lớp này ký ngay tại máy trạm bằng chứng thư trong kho chứng thư
    /// của Windows, dùng khi service ký số không chạy được hoặc chưa cập nhật thư viện ký.
    /// Sửa profile chữ ký thì phải sửa CẢ HAI nơi cho khớp nhau.
    ///
    /// Dùng cert.GetRSAPrivateKey() chứ KHÔNG dùng cert.PrivateKey: API cũ chỉ hỗ trợ CSP nên ném
    /// CryptographicException "Invalid provider type specified" với USB Token/chứng thư dùng khóa CNG.
    /// </summary>
    public static class Xml09BHSigner
    {
        public const string TAG_CHUKYDONVI = "CHUKYDONVI";
        private const string TAG_TT_HOSO = "TT_HOSO";
        private const string DIGEST_SHA256 = "http://www.w3.org/2001/04/xmlenc#sha256";
        private const string SIGNATURE_RSA_SHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

        /// <summary>
        /// Ký file XML tại chỗ (ghi đè chính file đó). Trả về false kèm error nếu không ký được.
        /// </summary>
        public static bool SignFile(string filePath, string serialNumber, out string error)
        {
            error = null;
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    error = "Không tìm thấy file XML cần ký: " + filePath;
                    return false;
                }

                X509Certificate2 cert = FindCertificate(serialNumber, out error);
                if (cert == null) return false;

                RSA rsa = GetRsaPrivateKeyCompat(cert, out error);
                if (rsa == null) return false;

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true; // bắt buộc, nếu không digest sẽ lệch khi ghi lại file
                doc.Load(filePath);

                XmlElement ttHoSo = FirstElement(doc, TAG_TT_HOSO);
                if (ttHoSo == null)
                {
                    error = "File XML không có thẻ " + TAG_TT_HOSO + ".";
                    return false;
                }
                // Reference trỏ vào TT_HOSO qua attribute Id -> phải có Id trước khi tính digest
                string tagId = ttHoSo.GetAttribute("Id");
                if (string.IsNullOrEmpty(tagId))
                {
                    tagId = "Id-" + Guid.NewGuid().ToString("N");
                    ttHoSo.SetAttribute("Id", tagId);
                }

                XmlElement chuKy = FirstElement(doc, TAG_CHUKYDONVI);
                if (chuKy == null)
                {
                    error = "File XML không có thẻ " + TAG_CHUKYDONVI + ".";
                    return false;
                }
                chuKy.IsEmpty = false;
                chuKy.RemoveAll(); // ký lại thì bỏ chữ ký cũ đi

                string signatureId = TAG_CHUKYDONVI + "-" + tagId;
                string objectId = "Object-" + signatureId;

                SignedXml signedXml = new SignedXml(doc);
                signedXml.SigningKey = rsa;
                signedXml.Signature.Id = signatureId;
                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
                signedXml.SignedInfo.SignatureMethod = SIGNATURE_RSA_SHA256;

                DataObject dataObject = new DataObject();
                dataObject.Id = objectId;
                dataObject.Data = BuildSignatureProperties(doc, signatureId);
                signedXml.AddObject(dataObject);

                // Đúng thứ tự file mẫu: Object trước, TT_HOSO sau. Tuyệt đối KHÔNG thêm Transform nào -
                // enveloped-signature đặt trên Reference trỏ vào Object (nằm trong chính Signature)
                // sẽ loại Object khỏi node-set và tính digest trên rỗng.
                Reference refObject = new Reference("#" + objectId);
                refObject.DigestMethod = DIGEST_SHA256;
                signedXml.AddReference(refObject);

                Reference refTag = new Reference("#" + tagId);
                refTag.DigestMethod = DIGEST_SHA256;
                signedXml.AddReference(refTag);

                KeyInfo keyInfo = new KeyInfo();
                KeyInfoX509Data x509Data = new KeyInfoX509Data(cert);
                x509Data.AddSubjectName(cert.Subject);
                keyInfo.AddClause(x509Data);
                signedXml.KeyInfo = keyInfo;

                try
                {
                    signedXml.ComputeSignature();
                }
                catch (CryptographicException exSign)
                {
                    error = "Không tạo được chữ ký (" + exSign.Message + "). Kiểm tra token còn cắm, đúng PIN và chứng thư còn hạn.";
                    Inventec.Common.Logging.LogSystem.Error(exSign);
                    return false;
                }

                chuKy.AppendChild(doc.ImportNode(signedXml.GetXml(), true));

                SaveWithoutBom(doc, filePath);

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "[KY_09BH] Đã ký {0} | tagId={1} | serial={2} | subject={3}",
                    Path.GetFileName(filePath), tagId, cert.SerialNumber, cert.Subject));
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Lấy khóa bí mật RSA của chứng thư.
        /// Ưu tiên RSACertificateExtensions.GetRSAPrivateKey (.NET 4.6+) vì nó hỗ trợ cả khóa CNG/KSP -
        /// đây chính là thứ USB Token đời mới dùng. Project target 4.5.2 nên không tham chiếu tĩnh được,
        /// gọi qua reflection (runtime thực tế của HIS.exe là .NET 4.8 nên luôn có sẵn).
        /// Chỉ khi không lấy được mới rơi về cert.PrivateKey (API cũ, ném
        /// "Invalid provider type specified" với khóa CNG - đúng lỗi đã gặp trong log ngày 29/07).
        /// </summary>
        private static RSA GetRsaPrivateKeyCompat(X509Certificate2 cert, out string error)
        {
            error = null;
            try
            {
                Type extensions = Type.GetType(
                    "System.Security.Cryptography.X509Certificates.RSACertificateExtensions, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    false);
                if (extensions != null)
                {
                    System.Reflection.MethodInfo method = extensions.GetMethod("GetRSAPrivateKey",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                        null, new Type[] { typeof(X509Certificate2) }, null);
                    if (method != null)
                    {
                        RSA rsa = method.Invoke(null, new object[] { cert }) as RSA;
                        if (rsa != null) return rsa;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex.InnerException ?? ex);
            }

            try
            {
                RSA legacy = cert.PrivateKey as RSA;
                if (legacy != null) return legacy;
                error = "Chứng thư " + cert.Subject + " không có khóa bí mật RSA khả dụng.";
                return null;
            }
            catch (Exception ex)
            {
                error = "Không truy cập được khóa bí mật của chứng thư: " + ex.Message
                    + " (khóa dạng CNG cần .NET 4.6+ trên máy trạm).";
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Dựng &lt;SignatureProperties&gt;&lt;SignatureProperty&gt;&lt;SigningTime&gt; nằm ngoài namespace xmldsig.
        /// Trả về XmlNodeList để gán vào DataObject.Data.
        /// </summary>
        private static XmlNodeList BuildSignatureProperties(XmlDocument doc, string signatureId)
        {
            XmlElement holder = doc.CreateElement("holder");
            XmlElement properties = doc.CreateElement("SignatureProperties");
            XmlElement property = doc.CreateElement("SignatureProperty");
            property.SetAttribute("Target", "#" + signatureId);
            property.SetAttribute("Id", "SignatureProperty-" + signatureId);
            XmlElement signingTime = doc.CreateElement("SigningTime");
            signingTime.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            property.AppendChild(signingTime);
            properties.AppendChild(property);
            holder.AppendChild(properties);
            return holder.ChildNodes;
        }

        /// <summary>
        /// Tìm chứng thư theo serial trong kho Personal của user rồi tới máy.
        /// Serial của USB Token là chuỗi hex hoa, so sánh không phân biệt hoa thường và bỏ khoảng trắng.
        /// </summary>
        private static X509Certificate2 FindCertificate(string serialNumber, out string error)
        {
            error = null;
            string serial = (serialNumber ?? "").Replace(" ", "").Trim();
            if (string.IsNullOrEmpty(serial))
            {
                error = "Chưa chọn chứng thư ký số (serial rỗng).";
                return null;
            }

            StoreLocation[] locations = new StoreLocation[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine };
            foreach (StoreLocation location in locations)
            {
                X509Store store = new X509Store(StoreName.My, location);
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    foreach (X509Certificate2 candidate in store.Certificates)
                    {
                        if (string.Equals((candidate.SerialNumber ?? "").Replace(" ", "").Trim(), serial,
                                StringComparison.OrdinalIgnoreCase))
                            return candidate;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    try { store.Close(); }
                    catch { }
                }
            }

            error = "Không tìm thấy chứng thư số serial " + serial + " trong kho chứng thư (Personal). Cắm USB Token, cài driver rồi chọn lại chứng thư.";
            return null;
        }

        private static XmlElement FirstElement(XmlDocument doc, string name)
        {
            XmlNodeList nodes = doc.GetElementsByTagName(name);
            return (nodes != null && nodes.Count > 0) ? nodes[0] as XmlElement : null;
        }

        /// <summary>Ghi lại file: UTF-8 KHÔNG BOM, không thêm indent (thêm khoảng trắng là hỏng digest).</summary>
        private static void SaveWithoutBom(XmlDocument doc, string filePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;
            settings.Encoding = new UTF8Encoding(false);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (XmlWriter writer = XmlWriter.Create(fs, settings))
            {
                doc.Save(writer);
            }
        }
    }
}

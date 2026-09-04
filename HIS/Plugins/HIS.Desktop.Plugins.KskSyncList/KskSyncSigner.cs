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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EMR.WCF.DCO;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary.ServiceSign;
using Inventec.Core;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Ký số DỮ LIỆU cho gói QĐ 1551 vào thẻ CKS_BENH_VIEN — xử lý y hệt HIS.Desktop.Plugins.ExportXmlQD130
    /// (SettingSignADO): HSM ký qua api/EmrSign/SignXmlBhyt; USB token ký qua WCF SignProcessorClient.SignXml130.
    /// Khác ExportXmlQD130 ở chỗ tag chữ ký là CKS_BENH_VIEN (thay vì CHUKYDONVI).
    /// Nhận chuỗi XML gốc -> trả chuỗi XML đã chèn chữ ký (dùng làm dataSigner cho CreateQd1551Main).
    /// </summary>
    internal class KskSyncSigner
    {
        private const string SIGN_TAG = "CKS_BENH_VIEN";
        private readonly SettingSignADO setting;

        internal KskSyncSigner(SettingSignADO setting)
        {
            this.setting = setting;
        }

        /// <summary>Ký chuỗi XML vào thẻ CKS_BENH_VIEN. Trả null nếu ký thất bại.</summary>
        internal string SignCksBenhVien(string xml)
        {
            try
            {
                if (this.setting == null || string.IsNullOrEmpty(xml)) return xml;
                string signed = this.setting.IsHsm ? SignByHsm(xml) : SignByUsbToken(xml);
                Inventec.Common.Logging.LogSystem.Info("CKS_BENH_VIEN: ky bang "
                    + (this.setting.IsHsm ? "HSM" : "USB token") + " -> "
                    + ((signed == null) ? "THAT BAI (null)"
                        : ("do dai chu ky trong the = " + TagValueLength(signed, SIGN_TAG) + " ky tu")));
                return signed;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        // HSM: ký qua backend EMR (api/EmrSign/SignXmlBhyt) — như ExportXmlQD130.SourceFileSignApi
        private string SignByHsm(string xml)
        {
            try
            {
                string xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO sdo = new EMR.SDO.SignXmlBhytSDO();
                sdo.XmlBase64 = xmlBase64;
                sdo.TagStoreSignatureValue = SIGN_TAG;
                sdo.ConfigData = new EMR.SDO.XmlConfigDataSDO()
                {
                    HsmSerialNumber = setting.SerialNumber,
                    HsmType = setting.Id,
                    HsmUserCode = setting.Name,
                    Password = setting.Password,
                    SecretKey = setting.SercetKey,
                    IdentityNumber = setting.CccdNumber
                };
                string signedBase64 = new BackendAdapter(param).Post<string>(
                    "api/EmrSign/SignXmlBhyt", ApiConsumers.EmrConsumer, sdo, SessionManager.ActionLostToken, param);
                SessionManager.ProcessTokenLost(param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                    Inventec.Common.Logging.LogSystem.Warn(string.Join(Environment.NewLine, param.Messages));
                if (string.IsNullOrEmpty(signedBase64)) return null;
                return Encoding.UTF8.GetString(Convert.FromBase64String(signedBase64));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private const string CONCLUDER_SIGN_TAG = "CKS_NGUOI_KET_LUAN";

        /// <summary>
        /// Ký số thẻ CKS_NGUOI_KET_LUAN NGAY TRÊN chuỗi XML thật của hồ sơ (không ký file rác) bằng chứng thư
        /// HSM của NGƯỜI KẾT LUẬN — thông tin lấy từ EMR_SIGNER (tài khoản/mật khẩu/CCCD/khóa bí mật/serial),
        /// HsmType theo cấu hình ký số của viện (setting.Id). Gọi api/EmrSign/SignXmlBhyt.
        /// LUÔN ký bằng HSM, KHÔNG phụ thuộc cấu hình USB token/HSM của thẻ CKS_BENH_VIEN (USB token của máy
        /// trạm không thể ký hộ người kết luận) — chỉ cần đã chọn hệ thống HSM (setting.Id > 0).
        /// Trả chuỗi XML đã chèn chữ ký; nếu thiếu thông tin/ký thất bại -> trả nguyên chuỗi XML gốc (không chặn).
        /// GỌI TRƯỚC khi ký CKS_BENH_VIEN (để chữ ký viện bao trùm cả chữ ký người kết luận).
        /// </summary>
        internal string SignXmlByConcluder(string xml, EMR.EFMODEL.DataModels.EMR_SIGNER signer)
        {
            try
            {
                if (string.IsNullOrEmpty(xml) || this.setting == null)
                {
                    Inventec.Common.Logging.LogSystem.Info("CKS_NGUOI_KET_LUAN: BO QUA — xml rong hoac chua"
                        + " cau hinh ky so.");
                    return xml;
                }
                // KHONG phu thuoc checkbox USB token/HSM cua the CKS_BENH_VIEN: the CKS_NGUOI_KET_LUAN
                // LUON ky bang HSM cua nguoi ket luan (USB token khong the ky ho nguoi khac). Chi can HsmType
                // (loai HSM cua vien) — frmSetting luon luu cboSystem vao setting.Id ke ca khi chon USB token.
                if (this.setting.Id <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CKS_NGUOI_KET_LUAN: BO QUA — chua chon HE THONG HSM"
                        + " trong form cau hinh ky so (HsmType=0). Mo lai form 'Ky so' va chon he thong HSM.");
                    return xml;
                }
                if (signer == null || string.IsNullOrEmpty(signer.PCA_SERIAL))
                {
                    Inventec.Common.Logging.LogSystem.Warn("CKS_NGUOI_KET_LUAN: BO QUA — nguoi ket luan "
                        + ((signer != null) ? signer.LOGINNAME : "(null)") + " khong co EMR_SIGNER.PCA_SERIAL.");
                    return xml;   // khong co chung thu HSM
                }
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO sdo = new EMR.SDO.SignXmlBhytSDO();
                sdo.XmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
                sdo.TagStoreSignatureValue = CONCLUDER_SIGN_TAG;
                sdo.ConfigData = new EMR.SDO.XmlConfigDataSDO()
                {
                    HsmSerialNumber = signer.PCA_SERIAL,       // serialNumber
                    HsmType = setting.Id,                      // loại HSM (cấu hình viện)
                    HsmUserCode = signer.HSM_USER_CODE,        // tài khoản
                    Password = signer.PASSWORD,                // mật khẩu
                    SecretKey = signer.SECRET_KEY,             // khóa bí mật
                    IdentityNumber = signer.CMND_NUMBER        // cccd
                };
                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "CKS_NGUOI_KET_LUAN: goi api/EmrSign/SignXmlBhyt bang HSM cua nguoi ket luan {0}"
                    + " (HsmType={1}; the CKS_BENH_VIEN ky bang {2}).",
                    signer.LOGINNAME, setting.Id, setting.IsHsm ? "HSM" : "USB token"));
                string signedBase64 = new BackendAdapter(param).Post<string>(
                    "api/EmrSign/SignXmlBhyt", ApiConsumers.EmrConsumer, sdo, SessionManager.ActionLostToken, param);
                SessionManager.ProcessTokenLost(param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                    Inventec.Common.Logging.LogSystem.Warn(string.Join(Environment.NewLine, param.Messages));
                if (string.IsNullOrEmpty(signedBase64))
                {
                    Inventec.Common.Logging.LogSystem.Warn("CKS_NGUOI_KET_LUAN: api/EmrSign/SignXmlBhyt tra ve RONG"
                        + " cho nguoi ket luan " + signer.LOGINNAME + " (HsmType=" + setting.Id + ") -> the DE TRONG.");
                    return xml;
                }
                string signedXml = Encoding.UTF8.GetString(Convert.FromBase64String(signedBase64));
                Inventec.Common.Logging.LogSystem.Info("CKS_NGUOI_KET_LUAN: DA KY cho nguoi ket luan "
                    + signer.LOGINNAME + "; do dai chu ky trong the = " + TagValueLength(signedXml, CONCLUDER_SIGN_TAG)
                    + " ky tu.");
                return signedXml;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); return xml; }
        }

        /// <summary>Do dai gia tri cua 1 the XML (0 = the trong / khong thay the) — chi dung de ghi log doi soat.</summary>
        private static int TagValueLength(string xml, string tag)
        {
            try
            {
                if (string.IsNullOrEmpty(xml) || string.IsNullOrEmpty(tag)) return 0;
                int open = xml.IndexOf("<" + tag + ">", StringComparison.OrdinalIgnoreCase);
                if (open < 0) return 0;
                open += tag.Length + 2;
                int close = xml.IndexOf("</" + tag + ">", open, StringComparison.OrdinalIgnoreCase);
                if (close < 0) return 0;
                return close - open;
            }
            catch { return 0; }
        }

        // USB token: ký qua WCF SignProcessorClient.SignXml130 (ký file) — như ExportXmlQD130 nhánh USB
        private string SignByUsbToken(string xml)
        {
            string srcPath = null, outPath = null;
            try
            {
                if (!VerifyServiceSignProcessorIsRunning())
                {
                    Inventec.Common.Logging.LogSystem.Warn("EMR.SignProcessor chưa chạy — không ký USB token được.");
                    return null;
                }
                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
                Directory.CreateDirectory(tempFolder);
                srcPath = Path.Combine(tempFolder, "ksk_" + Guid.NewGuid().ToString("N") + ".xml");
                outPath = Path.Combine(tempFolder, "ksk_" + Guid.NewGuid().ToString("N") + "_signed.xml");
                File.WriteAllText(srcPath, xml, new UTF8Encoding(false));

                WcfSignDCO dco = new WcfSignDCO();
                dco.SerialNumber = setting.SerialNumber;
                dco.SourceFile = srcPath;
                dco.OutputFile = outPath;
                dco.PIN = "";
                dco.fieldSigned = SIGN_TAG;

                SignProcessorClient client = new SignProcessorClient();
                var result = client.SignXml130(JsonConvert.SerializeObject(dco));
                if (result != null && result.Success && File.Exists(result.OutputFile) && new FileInfo(result.OutputFile).Length > 0)
                    return File.ReadAllText(result.OutputFile, Encoding.UTF8);
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            finally
            {
                TryDelete(srcPath);
                TryDelete(outPath);
            }
        }

        private static void TryDelete(string path)
        {
            try { if (!string.IsNullOrEmpty(path) && File.Exists(path)) File.Delete(path); }
            catch { }
        }

        #region service sign processor (mô phỏng ExportXmlQD130 / frmSetting)
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        valid = true;
                    }
                    else
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Thread.Sleep(500);
                            valid = true;
                        }
                        catch (Exception exx) { Inventec.Common.Logging.LogSystem.Warn(exx); }
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return valid;
        }

        private string AppFilePathSignService()
        {
            try
            {
                return Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error temp file: " + exception.Message);
                return "";
            }
        }

        private bool IsProcessOpen(string name)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == name || p.ProcessName == string.Format("{0}.exe", name)
                    || p.ProcessName == string.Format("{0} (32 bit)", name) || p.ProcessName == string.Format("{0}.exe (32 bit)", name))
                    return true;
            }
            return false;
        }
        #endregion
    }
}

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
                return this.setting.IsHsm ? SignByHsm(xml) : SignByUsbToken(xml);
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

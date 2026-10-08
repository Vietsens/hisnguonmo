/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using EMR.WCF.DCO;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ServiceSign;
using Newtonsoft.Json;
using System;
using System.IO;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Ký số đơn vị (thẻ CHUKYDONVI) cho file XML 3176 gửi Cổng CSDL Y tế Cần Thơ — TUỲ CHỌN: chỉ ký khi màn hình
    /// có cấu hình "Ký file"; không có thì gửi file không chữ ký (đã thử: cổng vẫn nhận, 06/10/2026).
    /// Dùng lại cấu hình ký số của màn XML 130 (HSM qua EMR / USB token qua dịch vụ ký) như khi gửi cổng BHXH,
    /// nhưng:
    ///  - chỉ ký bản gửi Cần Thơ (4750 / Vĩnh Long / HOC vẫn nhận file chưa ký như trước);
    ///  - tệp tạm nằm ở thư mục riêng Temp\CsdlCt3176 và chỉ xoá tệp của chính lượt ký — hàm ký BHXH xoá sạch
    ///    thư mục Temp, dùng chung sẽ xoá nhầm tệp của luồng đang chạy song song.
    /// </summary>
    public partial class UCExportXml
    {
        private const string CSDL_CT_SIGN_TEMP_FOLDER = "CsdlCt3176";

        /// <summary>Đã có cấu hình ký số (Ký file) chưa — cùng điều kiện các luồng ký BHXH đang dùng.</summary>
        private bool HasSignSettingForCt()
        {
            return SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
        }

        /// <summary>
        /// Ký file XML tổng GIAMDINHHS (bytes) cho cổng Cần Thơ. Trả bytes đã ký; null nếu không ký được
        /// (error = lý do, có tiền tố "CT:") — KHÔNG gửi file chưa ký.
        /// </summary>
        private byte[] SignXmlForCsdlCt(byte[] xmlBytes, string treatmentCode, out string error)
        {
            error = null;
            string srcPath = null, outPath = null, signedPath = null;
            try
            {
                if (xmlBytes == null || xmlBytes.Length == 0)
                {
                    error = "CT: không có dữ liệu XML để ký";
                    return null;
                }
                if (!HasSignSettingForCt())
                {
                    error = "CT: chưa cấu hình ký số (ô Ký file) — Cổng CSDL Y tế Cần Thơ yêu cầu chữ ký số đơn vị";
                    return null;
                }

                string folder = Path.Combine(Directory.GetCurrentDirectory(), "Temp", CSDL_CT_SIGN_TEMP_FOLDER);
                Directory.CreateDirectory(folder);
                string id = (string.IsNullOrEmpty(treatmentCode) ? "hs" : treatmentCode) + "_" + Guid.NewGuid().ToString("N");
                srcPath = Path.Combine(folder, id + ".xml");
                outPath = Path.Combine(folder, id + "_signed.xml");
                File.WriteAllBytes(srcPath, xmlBytes);

                if (SettingSignADO.IsHsm)
                {
                    string signedBase64 = SourceFileSignApi(ReadFileContent(srcPath));
                    if (string.IsNullOrEmpty(signedBase64))
                    {
                        error = "CT: ký số HSM thất bại";
                        return null;
                    }
                    return Convert.FromBase64String(signedBase64);
                }

                if (!VerifyServiceSignProcessorIsRunning())
                {
                    error = "CT: không chạy được dịch vụ ký số (USB token)";
                    return null;
                }
                File.Create(outPath).Close();
                WcfSignDCO wcfSignDCO = new WcfSignDCO
                {
                    SerialNumber = SettingSignADO.SerialNumber,
                    OutputFile = outPath,
                    PIN = "",
                    SourceFile = srcPath,
                    fieldSigned = "CHUKYDONVI"
                };
                var signResult = new SignProcessorClient().SignXml130(JsonConvert.SerializeObject(wcfSignDCO));
                if (signResult == null || !signResult.Success)
                {
                    error = "CT: ký số USB token thất bại";
                    return null;
                }
                signedPath = !string.IsNullOrEmpty(signResult.OutputFile) ? signResult.OutputFile : outPath;
                if (!File.Exists(signedPath))
                {
                    error = "CT: không tìm thấy file sau khi ký";
                    return null;
                }
                return File.ReadAllBytes(signedPath);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                error = "CT: lỗi ký số: " + ex.Message;
                return null;
            }
            finally
            {
                TryDeleteCtTempFile(srcPath);
                TryDeleteCtTempFile(outPath);
                if (signedPath != outPath) TryDeleteCtTempFile(signedPath);
            }
        }

        private static void TryDeleteCtTempFile(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    public class ElectronicBillResultUtil
    {
        public static void Set(ref ElectronicBillResult electronicBillResult, bool success, string message)
        {
            try
            {
                if (electronicBillResult == null)
                {
                    electronicBillResult = new ElectronicBillResult();
                }

                if (electronicBillResult.Messages == null)
                {
                    electronicBillResult.Messages = new List<string>();
                }
                electronicBillResult.Success = success;
                if (!String.IsNullOrEmpty(message))
                {
                    electronicBillResult.Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Set(ref ElectronicBillResult electronicBillResult, bool success, List<string> messages)
        {
            try
            {
                if (electronicBillResult == null)
                {
                    electronicBillResult = new ElectronicBillResult();
                }

                if (electronicBillResult.Messages == null)
                {
                    electronicBillResult.Messages = new List<string>();
                }
                electronicBillResult.Success = success;
                if (messages != null && messages.Count > 0)
                {
                    electronicBillResult.Messages.AddRange(messages);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Thông báo lỗi có phân loại nguồn (vCong 62522)

        /// <summary>
        /// Gán lỗi từ exception: tự phân loại nguồn (kết nối / nhà cung cấp / máy trạm / cấu hình / dữ liệu / phần mềm),
        /// dựng thông báo cho người dùng biết liên hệ ai, vẫn kèm ex.Message ở phần "Chi tiết".
        /// </summary>
        /// <param name="provider">Mã ProviderType.* (có thể null)</param>
        /// <param name="action">Thao tác đang làm, VD "phát hành hóa đơn" (có thể null)</param>
        public static void SetException(ref ElectronicBillResult electronicBillResult, Exception ex, string provider, string action = null)
        {
            try
            {
                Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildFromException(ex, provider, action));
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
                Set(ref electronicBillResult, false, ex != null ? ex.Message : "");
            }
        }

        /// <summary>
        /// Cổng nhà cung cấp nhận yêu cầu nhưng trả về lỗi nghiệp vụ (mã lỗi + mô tả của họ).
        /// </summary>
        /// <param name="apiMessage">Nguyên văn mô tả lỗi API trả về (có thể null/rỗng)</param>
        /// <param name="fallbackSummary">Mô tả thao tác thất bại, VD "Gửi hóa đơn gốc thất bại"</param>
        public static void SetProviderError(ref ElectronicBillResult electronicBillResult, string provider, string apiMessage, string fallbackSummary)
        {
            try
            {
                if (ElectronicBillErrorUtil.IsBuiltMessage(apiMessage))
                {
                    // Đã là thông báo phân loại sẵn (VD từ DoSignAndReleaseInvoice) → không bọc lại
                    Set(ref electronicBillResult, false, apiMessage);
                }
                else if (String.IsNullOrWhiteSpace(apiMessage))
                {
                    // Cổng không nói lý do → có thể không nhận được phản hồi
                    Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Connection, provider,
                        (String.IsNullOrWhiteSpace(fallbackSummary) ? "Thao tác thất bại" : fallbackSummary) + " và cổng hóa đơn điện tử không trả về mô tả lỗi", null));
                }
                else
                {
                    // Thư viện ngoài (VNPT/BKAV/Viettel...) đôi khi trả ex.Message kết nối dưới dạng chuỗi mô tả → nhận diện lại
                    ElectronicBillErrorSource source = ElectronicBillErrorUtil.IsConnectionMessage(apiMessage)
                        ? ElectronicBillErrorSource.Connection
                        : ElectronicBillErrorSource.Provider;
                    if (String.IsNullOrWhiteSpace(fallbackSummary))
                    {
                        fallbackSummary = source == ElectronicBillErrorSource.Connection
                            ? "Không kết nối được tới cổng hóa đơn điện tử"
                            : "Cổng hóa đơn điện tử trả về lỗi";
                    }
                    Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(source, provider, fallbackSummary, apiMessage));
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
                Set(ref electronicBillResult, false, String.IsNullOrWhiteSpace(apiMessage) ? fallbackSummary : apiMessage);
            }
        }

        /// <summary>
        /// Đăng nhập cổng nhà cung cấp thất bại. Nguyên nhân thường là tài khoản/mật khẩu/mã đơn vị thiết lập trong HIS không đúng
        /// → xếp vào THIẾT LẬP TẠI VIỆN; nếu cổng không phản hồi thì là KẾT NỐI.
        /// </summary>
        /// <param name="apiMessage">Mô tả lỗi cổng trả về (null = không có phản hồi)</param>
        public static void SetLoginError(ref ElectronicBillResult electronicBillResult, string provider, string apiMessage)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(apiMessage))
                {
                    SetNoResponse(ref electronicBillResult, provider, "đăng nhập cổng hóa đơn điện tử");
                }
                else if (ElectronicBillErrorUtil.IsConnectionMessage(apiMessage))
                {
                    Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Connection, provider, "Không kết nối được tới cổng hóa đơn điện tử khi đăng nhập", apiMessage));
                }
                else
                {
                    Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Config, provider,
                        "Đăng nhập cổng hóa đơn điện tử thất bại: kiểm tra tài khoản, mật khẩu, mã số thuế, mã đơn vị đã thiết lập trong HIS cho máy/người dùng này; nếu tất cả đã đúng thì liên hệ nhà cung cấp", apiMessage));
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
                Set(ref electronicBillResult, false, apiMessage);
            }
        }

        /// <summary>
        /// Thiếu/sai cấu hình tích hợp hóa đơn điện tử trong HIS.
        /// </summary>
        public static void SetConfigError(ref ElectronicBillResult electronicBillResult, string provider, string summary)
        {
            Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Config, provider, summary, null));
        }

        /// <summary>
        /// Dữ liệu/giao dịch chưa hợp lệ về nghiệp vụ, người dùng tự kiểm tra được.
        /// </summary>
        public static void SetBusinessError(ref ElectronicBillResult electronicBillResult, string provider, string summary)
        {
            Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Business, provider, summary, null));
        }

        /// <summary>
        /// Lỗi xử lý nội bộ phần mềm HIS (không phải do người dùng, máy trạm hay nhà cung cấp).
        /// </summary>
        public static void SetSoftwareError(ref ElectronicBillResult electronicBillResult, string provider, string summary, string detail)
        {
            Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Software, provider, summary, detail));
        }

        /// <summary>
        /// Không nhận được phản hồi (null) từ thư viện/cổng nhà cung cấp.
        /// </summary>
        public static void SetNoResponse(ref ElectronicBillResult electronicBillResult, string provider, string action)
        {
            string summary = String.IsNullOrWhiteSpace(action)
                ? "Không nhận được phản hồi từ cổng hóa đơn điện tử"
                : string.Format("Không nhận được phản hồi từ cổng hóa đơn điện tử khi {0}", action);
            Set(ref electronicBillResult, false, ElectronicBillErrorUtil.BuildMessage(ElectronicBillErrorSource.Connection, provider, summary, null));
        }

        #endregion

        #region file
        public static string ProcessPdfFileResult(string base64string)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(base64string))
                {
                    byte[] data = System.Convert.FromBase64String(base64string);
                    result = ProcessPdfFileResult(data);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public static string ProcessPdfFileResult(byte[] fileToBytes)
        {
            string result = "";
            try
            {
                string tempFileName = Path.GetTempFileName();
                tempFileName = tempFileName.Replace("tmp", "pdf");
                try
                {
                    File.WriteAllBytes(tempFileName, fileToBytes);
                    result = tempFileName;
                }
                catch (Exception ex)
                {
                    result = "";
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion
    }
}

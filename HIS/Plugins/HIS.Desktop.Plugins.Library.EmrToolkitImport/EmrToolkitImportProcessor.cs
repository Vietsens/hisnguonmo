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
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Popup;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Service;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport
{
    /// <summary>
    /// Điểm truy cập công khai của thư viện EMRTOOLKIT Import.
    /// Mọi plugin khác chỉ cần tham chiếu DLL này và gọi 1 trong các method dưới.
    ///
    /// Ví dụ:
    /// <code>
    /// var proc = new EmrToolkitImportProcessor();
    /// var result = proc.ImportEmrAndShowResult(model);
    /// </code>
    /// </summary>
    public class EmrToolkitImportProcessor
    {
        /// <summary>
        /// Kiểm tra hệ thống đã khai báo thông tin kết nối EMRTOOLKIT chưa
        /// (key HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo có giá trị).
        /// Dùng cho điều kiện hiển thị menu/checkbox ở các plugin gọi.
        /// </summary>
        public static bool IsConfigured()
        {
            try
            {
                return Config.EmrToolkitConfigCFG.CheckHasConnectionInfo();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Gửi dữ liệu qua EMRTOOLKIT (CreateToken -> MaHoaJson -> Import) và
        /// trả về kết quả. KHÔNG hiển thị UI.
        /// </summary>
        /// <param name="model">Dữ liệu cần import</param>
        /// <returns>Kết quả tổng hợp (không null)</returns>
        public EmrToolkitImportResult ImportEmr(EmrImportModel model)
        {
            try
            {
                return new EmrToolkitApiService().ImportEmr(model);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return new EmrToolkitImportResult
                {
                    Success = false,
                    Message = ex.Message,
                    Step = EmrToolkitImportStep.None
                };
            }
        }

        /// <summary>
        /// Gửi dữ liệu qua EMRTOOLKIT và hiển thị cửa sổ kết quả (JSON trả về).
        /// Lưu ý: phần gọi mạng chạy đồng bộ — nếu cần WaitingManager nên dùng
        /// <see cref="ImportEmr"/> + <see cref="ShowResult"/> để tự kiểm soát.
        /// </summary>
        /// <param name="model">Dữ liệu cần import</param>
        /// <param name="owner">Form cha để canh giữa (có thể null)</param>
        /// <returns>Kết quả tổng hợp (không null)</returns>
        public EmrToolkitImportResult ImportEmrAndShowResult(EmrImportModel model, IWin32Window owner = null)
        {
            EmrToolkitImportResult result = ImportEmr(model);
            ShowResult(result, owner);
            return result;
        }

        /// <summary>
        /// Hiển thị cửa sổ kết quả cho 1 EmrToolkitImportResult đã có.
        /// </summary>
        /// <param name="result">Kết quả cần hiển thị</param>
        /// <param name="owner">Form cha để canh giữa (có thể null)</param>
        public void ShowResult(EmrToolkitImportResult result, IWin32Window owner = null)
        {
            try
            {
                using (frmEmrToolkitImportResult frm = new frmEmrToolkitImportResult(result))
                {
                    if (owner != null)
                        frm.ShowDialog(owner);
                    else
                        frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}

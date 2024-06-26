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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    internal class ServiceExecuteCFG
    {
        internal const string ThoiGianKetThuc = "HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc";
        internal const string HideTimePrint = "HIS.Desktop.Plugins.ServiceExecute.HideTimePrint";
        internal const string ConnectPacsByFss = "HIS.Desktop.Plugins.ServiceExecute.ConnectPacsByFss";
        internal const string ConnectImageOption = "HIS.Desktop.Plugins.ServiceExecute.OptionImage";
        internal const string CaptureType = "HIS.Desktop.Camera.CaptureType";
        internal const string OptionPrint = "HIS.Desktop.Plugins.ServiceExecute.PrintOption";
        internal const string NumberOfFilmCFG = "HIS.Desktop.Plugins.ServiceExecute.CĐHA.ValidNumberOfFilm";

        //1: Ẩn vùng hiển thị ảnh trong màn hình xử lý dịch vụ chẩn đoán hình ảnh (xquang, CT, MRI)
        internal const string ShowImageCFG = "HIS.Desktop.Plugins.ServiceExecute.IsNotShowingImgAreaForDiagnosticImgServiceReq";

        //1: Bắt buộc phải chỉ định thuốc/vật tư tiêu hao trước khi xử lý dịch vụ "chẩn đoán hình ảnh"
        internal const string LockExecuteCFG = "HIS.Desktop.Plugins.ServiceExecute.MustHavePresBeforeExecuteWithDiagnosticImgServiceReq";

        internal static string MPS000354 { get { return "Mps000354"; } }
    }
}

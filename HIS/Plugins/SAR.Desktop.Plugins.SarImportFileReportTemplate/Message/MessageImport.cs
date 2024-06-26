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

namespace SAR.Desktop.Plugins.SarImportFileReportTemplate.Message
{
    class MessageImport
    {
        internal const string KoTonTai = "Mã báo cáo {0} không tồn tại|";
        internal const string Maxlength = "{0} vượt quá maxlength|";
        internal const string DinhDangFileKoDung = "{0} không phải file excel|";
        internal const string FileImportDaTonTai = "File import tồn tại file báo cáo giống nhau {0} | ";
        internal const string FileImportDaTonTaiMaBaoCao = "File import tồn tại mã báo cáo giống nhau {0} | ";
    }
}

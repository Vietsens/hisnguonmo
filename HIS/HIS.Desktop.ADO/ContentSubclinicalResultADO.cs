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

namespace HIS.Desktop.ADO
{
    /// <summary>
    /// Kết quả CLS dạng CHUỖI kèm người thực hiện y lệnh. Plugin ContentSubclinical trả object này
    /// (thay cho string thuần) qua DelegateSelectData khi plugin gọi truyền cờ "WITH_EXECUTOR"
    /// trong List&lt;string&gt; args. Dùng cho nhập KSK: ô "Người khám" của mục CLS lấy theo
    /// người thực hiện y lệnh của dịch vụ được lấy kết quả. Không truyền cờ thì vẫn trả string như cũ.
    /// </summary>
    public class ContentSubclinicalResultADO
    {
        /// <summary>Chuỗi kết quả — đúng nội dung trước đây trả bằng string.</summary>
        public string RESULT { get; set; }

        /// <summary>
        /// Người thực hiện y lệnh (HIS_SERVICE_REQ.EXECUTE_LOGINNAME) của dịch vụ đầu tiên (theo thứ tự)
        /// đã có người thực hiện trong các dòng được lấy; null khi chưa dịch vụ nào được thực hiện.
        /// </summary>
        public string EXECUTE_LOGINNAME { get; set; }

        public string EXECUTE_USERNAME { get; set; }

        /// <summary>
        /// Tất cả người thực hiện y lệnh (loginname, KHÔNG trùng, theo thứ tự dòng) của các dịch vụ được lấy —
        /// để plugin gọi nạp datasource combo "Người khám" cho bác sĩ tự chọn. Rỗng khi chưa ai thực hiện.
        /// </summary>
        public List<string> EXECUTE_LOGINNAMES { get; set; }
    }
}

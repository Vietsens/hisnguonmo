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

namespace HIS.Desktop.Plugins.CustomerRequest
{
    class Data
    {
        public string id { get; set; }
        public string stt { get; set; }
        public string tiêu_đề { get; set; }
        public string người_tiếp_nhận { get; set; }
        public string thời_gian_tạo { get; set; }
        public string thời_hạn_hoàn_thành { get; set; }
        public string thời_gian_hoàn_thành { get; set; }
        public string nội_dung { get; set; }
        public string trạng_thái_yckh { get; set; }
        public string loại_yckh { get; set; }
    }
}

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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SAFECERT.ADO
{
    class EinvoiceLine
    {
        public string idMaster { get; set; }
        public string sothutu { get; set; }
        public int sothutuIdx { get; set; }
        public string loaihhdv { get; set; }
        public string mahang { get; set; }
        public string tenhang { get; set; }
        public string donvitinh { get; set; }
        public string soluong { get; set; }
        public string dongia { get; set; }
        public string thanhtien { get; set; }
        public string thuesuat { get; set; }
        public string tienthue { get; set; }
        public string tongtien { get; set; }
        //public int khuyenmai { get; set; }
        public string thuettdb { get; set; }
        public int khonghienthi { get; set; }

        public string hoaDonMoRongCT { get; set; }
        public int? tinhchat { get; set; }
        public string tileckgg { get; set; }
        public string thanhtienckgg { get; set; }
        public int? loaidieuchinh { get; set; }
    }
}

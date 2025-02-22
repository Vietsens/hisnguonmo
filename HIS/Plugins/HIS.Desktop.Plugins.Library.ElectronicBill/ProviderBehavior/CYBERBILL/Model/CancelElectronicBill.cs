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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.CYBERBILL.Model
{
    public class InputCancelElectronicBill
    {
        public string doanhnghiep_mst { get; set; }
        public short hoadon_loai { get; set; }
        public string loaihoadon_ma { get; set; }
        public string mauso { get; set; }
        public string kyhieu { get; set; }
        public string ma_hoadon { get; set; }
        public string hoadon_goc { get; set; }
        public string ngaylap { get; set; }
        public string hopdong_so { get; set; }
        public string hopdong_ngayky { get; set; }
        public string file_hopdong { get; set; }
        public string nguoilap { get; set; }
    }
    public class OutputCancelElectronicBill
    {
        public ResultCancelElectronicBill result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public ErrorCyberbill error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }
    public class ResultCancelElectronicBill
    {
        public string maketqua { get; set; }
        public string motaketqua { get; set; }
        public string magiaodich { get; set; }
        public string tct_manoquan { get; set; }
    }
    public class CancelElectronicBill
    {
     
    }
}

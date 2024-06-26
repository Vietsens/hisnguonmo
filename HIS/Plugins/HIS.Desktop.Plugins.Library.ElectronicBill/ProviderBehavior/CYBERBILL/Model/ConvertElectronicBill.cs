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
    public class InputConvertElectronicBill
    {
        public string doanhnghiep_mst { get; set; }
        public string magiaodich { get; set; }
        public string ma_hoadon { get; set; }
    }
    public class OutputConvertElectronicBill
    {
        public ResultConvertElectronicBill result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public ErrorCyberbill error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }

    }
    public class ResultConvertElectronicBill
    {
        public string maketqua { get; set; }
        public string motaketqua { get; set; }
        public string macapcqt { get; set; }
        public string base64pdf { get; set; }
        public string nguoichuyendoi { get; set; }
        public string ngaychuyendoi { get; set; }
    }
}

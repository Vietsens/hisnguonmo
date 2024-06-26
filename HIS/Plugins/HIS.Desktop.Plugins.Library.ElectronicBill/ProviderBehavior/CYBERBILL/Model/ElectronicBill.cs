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
    public class InputElectronicBill
    {
        public string doanhnghiep_mst { get; set; }
        public string loaihoadon_ma { get; set; }
        public string mauso { get; set; }
        public string ma_tracuu { get; set; }
        public string kyhieu { get; set; }
        public string sophieu { get; set; }
        public string ma_hoadon { get; set; }
        public string ngaylap { get; set; }
        public string dnmua_mst { get; set; }
        public string dnmua_ten { get; set; }
        public string dnmua_tennguoimua { get; set; }
        public string dnmua_diachi { get; set; }
        public string dnmua_sdt { get; set; }
        public string dnmua_email { get; set; }
        public short thanhtoan_phuongthuc { get; set; }
        public string thanhtoan_phuongthuc_ten { get; set; }
        public string thanhtoan_taikhoan { get; set; }
        public string thanhtoan_nganhang { get; set; }
        public string tiente_ma { get; set; }
        public decimal tygiangoaite { get; set; }
        public string thanhtoan_thoihan { get; set; }
        public decimal tongtien_chietkhau { get; set; }
        public decimal tongtien_chietkhau_thuongmai { get; set; }
        public string ghichu { get; set; }
        public decimal tongtien_chuavat { get; set; }
        public decimal tienthue { get; set; }
        public decimal tongtien_covat { get; set; }
        public string nguoilap { get; set; }
        //public bool is_download_file { get; set; }
        //public bool trinhky { get; set; }
        //public int phikhac_tyle { get; set; }
        //public int phikhac_sotien { get; set; }
        public List<DanhSachChiTiet> dschitiet { get; set; }
        public List<DanhSachThue> dsthuesuat { get; set; }
        public int nghiquyetapdung { get; set; }
        public string dulieudacthu01 { get; set; }
        public string dulieudacthu02 { get; set; }
        public string dulieudacthu03 { get; set; }
        public string dulieudacthu04 { get; set; }
        public string dulieudacthu05 { get; set; }
        public string khachhang_ma { get; set; }
        public string matracuuhtkhac { get; set; }
    }
    public class OutputElectronicBill
    {
        public ResultElectronicBill result { get; set; }
        public string targetUrl { get; set; }
        public bool success { get; set; }
        public ErrorCyberbill error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }
    public class ResultElectronicBill
    {
        public string maketqua { get; set; }
        public string motaketqua { get; set; }
        public string magiaodich { get; set; }
        public string sohoadon { get; set; }
        public string mauso { get; set; }
        public string kyhieu { get; set; }
        public string ngayky { get; set; }
        public string tct_macoquan { get; set; }
    }
    public class HoaDon
    {
        
    }
    public class DanhSachChiTiet
    {
        public int stt { get; set; }
        public int hanghoa_loai { get; set; }
        public int khuyenmai { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string donvitinh { get; set; }
        public decimal? soluong { get; set; }
        public decimal? dongia { get; set; }
        public decimal phantram_chietkhau { get; set; }
        public decimal tongtien_chietkhau { get; set; }
        public decimal phikhac_tyle { get; set; }
        public decimal phikhac_sotien { get; set; }
        public decimal? tongtien_chuathue { get; set; }
        public string mathue { get; set; }
        public decimal? tongtien_cothue { get; set; }
        public short tyletinhthue { get; set; }
    }
    public class DanhSachThue
    {
        public string mathue { get; set; }
        public decimal tongtien_chiuthue { get; set; }
        public decimal tongtien_thue { get; set; }
    }
}

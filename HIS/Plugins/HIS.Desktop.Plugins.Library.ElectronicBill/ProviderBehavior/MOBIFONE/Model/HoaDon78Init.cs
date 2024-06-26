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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MOBIFONE.Model
{
    public class HoaDon78Init
    {
        public int editmode { get; set; }
        public List<HoaDon78Data> data { get; set; }
    }
    public class HoaDon78Data
    {
        public string cctbao_id { get; set; }
        public string hdon_id { get; set; }
        public string nlap { get; set; }
        public string sdhang { get; set; }
        public string dvtte { get; set; }
        public int tgia { get; set; }
        public string htttoan { get; set; }
        public string stknban { get; set; }
        public string tnhban { get; set; }
        public string mnmua { get; set; }
        public string mst { get; set; }
        public string tnmua { get; set; }
        public string email { get; set; }
        public string ten { get; set; }
        public string dchi { get; set; }
        public string stknmua { get; set; }
        public string sdtnmua { get; set; }
        public string tnhmua { get; set; }
        public decimal tgtcthue { get; set; }
        public decimal tgtthue { get; set; }
        public decimal tgtttbso { get; set; }
        public decimal tgtttbso_last { get; set; }
        public decimal tkcktmn { get; set; }
        public decimal ttcktmai { get; set; }
        public decimal tgtphi { get; set; }
        public string mdvi { get; set; }
        public int tthdon { get; set; }
        public int is_hdcma { get; set; }
        public string hdon_id_old { get; set; }
        public int lhdclquan { get; set; }
        public string khmshdclquan { get; set; }
        public string khhdclquan { get; set; }
        public string shdclquan { get; set; }
        public DateTime nlhdclquan { get; set; }
        public List<HoaDon78Details> details { get; set; }
        public List<HoaDon78Phi> hoadon68_phi { get; set; }
        public List<HoaDon78Khac> hoadon68_khac { get; set; }
    }

    public class HoaDon78Details
    {
        public List<HoaDon78DetailsData> data { get; set; }
    }
    public class HoaDon78DetailsData
    {
        public int stt { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string mdvtinh { get; set; }
        public string dvtinh { get; set; }
        public decimal? sluong { get; set; }
        public decimal? dgia { get; set; }
        public decimal? thtien { get; set; }
        public decimal? tlckhau { get; set; }
        public decimal? stckhau { get; set; }
        public string tsuat { get; set; }
        public decimal? tthue { get; set; }
        public decimal? tgtien { get; set; }
        public decimal kmai { get; set; }
    }
    public class HoaDon78Phi
    {
        public List<HoaDon78PhiData> data { get; set; }
    }
    public class HoaDon78PhiData
    {
        public string tnphi { get; set; }
        public decimal tienphi { get; set; }
    }
    public class HoaDon78Khac
    {
        public List<HoaDon78KhacData> data { get; set; }
    }
    public class HoaDon78KhacData
    {
        public string dlieu { get; set; }
        public string kdlieu { get; set; }
        public string ttruong { get; set; }
    }
    public enum TaxRateCode
    {
        Tax10 = 10, // Thuế 10%
        Tax8 =8,//Thuế 8%
        Tax5 = 5,//Thuế 5%
        Tax0 = 0,//Thuế 0%
        NotObjectToTax = -1,//Không chịu thuế
        DontPayTaxes = -2//Không nộp thuế
    }
    public enum ServiceType
    {
        Services = 1, //Hàng hóa dịch vụ
        Promotion = 2,//Khuyến mại
        Tradediscount = 3,//Chiết khấu thương mại
        Notes = 4//Ghi chú
    }
    public enum StatusInvoice
    {
        Original = 0,//Hóa đơn gốc
        Replace = 2,//Thay thế
        UpwardAdj = 19,//Điều chỉnh tăng
        DownAdj = 21,//Điều chỉnh giảm
        InformationAdj = 23//Điều chỉnh thông tin
    }
    public enum InvoiceHasCode
    {
        No = 0,
        Yes = 1
    }
    public enum RelevantInvoiceType
    {
        HoaDon32 = 3//Với hóa đơn theo nghị định 32 mặc iđnhj truyền giá trị sang là 3
    }
    public enum EditMode
    {
        Create = 1,//Tạo mới
        Edit = 2,// Sửa
        Delete = 3//Xóa
    }
}

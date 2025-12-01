using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.CYBERBILL.Model
{
        public class InputSendAndSignElectronicBill
        {
            public string doanhnghiep_mst { get; set; }
            public string loaihoadon_ma { get; set; }
            public string mauso { get; set; }
            public string kyhieu { get; set; }
            public string sophieu { get; set; }
            public string ma_tracuu { get; set; }
            public string ma_hoadon { get; set; }
            public string ngaylap { get; set; }
            public string tct_macoquan { get; set; }
            public string dnmua_mst { get; set; }
            public string dnmua_ten { get; set; }
            public string dnmua_tennguoimua { get; set; }
            public string dnmua_diachi { get; set; }
            public string dnmua_sdt { get; set; }
            public string dnmua_email { get; set; }
            public string khachhang_ma { get; set; }
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
            public string dulieudacthu01 { get; set; }
            public string dulieudacthu02 { get; set; }
            public string dulieudacthu03 { get; set; }
            public string dulieudacthu04 { get; set; }
            public string dulieudacthu05 { get; set; }
            public int nghiquyetapdung { get; set; }
            public string trinhky { get; set; }
            public short dnmua_cccd { get; set; }
            public string sohochieu { get; set; } // trong tài liệu chưa cho biết kiểu dữ liệu
            public short hoadonchietkhau { get; set; }
            public string sobangke { get; set; }
            public string ngaybangke { get; set; }
            public List<DanhSachChiTiet> dschitiet { get; set; }
            public List<DanhSachThue> dsthuesuat { get; set; }
        }

        public class OutputSendAndSignElectronicBill
        {
            public ResultSendAndSignElectronicBill result { get; set; }
            public string targetUrl { get; set; }
            public bool success { get; set; }
            public ErrorCyberbill error { get; set; }
            public bool unAuthorizedRequest { get; set; }
            public bool __abp { get; set; }
        }
        public class ResultSendAndSignElectronicBill
        {
            public string maketqua { get; set; }
            public string motaketqua { get; set; }
            public string magiaodich { get; set; }
            public string sohoadon { get; set; }
        }
    }

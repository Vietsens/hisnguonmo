using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.MInvoice
{
    partial class InvoiceADO
    {
        public string editmode { get; set; }
        public List<InvoiceData> data { get; set; }
    }

    partial class InvoiceData
    {
        public DateTime tdlap { get; set; }
        public string khieu { get; set; }
        public string shdon { get; set; }
        public string sdhang { get; set; }
        public string dvtte { get; set; }
        public decimal tgia { get; set; }
        public string htttoan { get; set; }
        public string tnmua { get; set; }
        public string mnmua { get; set; }
        public string ten { get; set; }
        public string cccdan { get; set; }
        public string sdtnmua { get; set; }
        public string mst { get; set; }
        public string dchi { get; set; }
        public string email { get; set; }
        public string stknmua { get; set; }
        public string nganhang_ngmua { get; set; }
        public decimal ttcktmai { get; set; }
        public decimal tgtcthue { get; set; }
        public decimal tgtthue { get; set; }
        public decimal tgtttbso { get; set; }
        public string tgtttbchu { get; set; }
        public string nguoi_thu { get; set; }
        public List<InvoiceDetails> details { get; set; }
    }

    partial class InvoiceDetails
    {
        public List<InvoiceItem> data { get; set; }
    }

    partial class InvoiceItem
    {
        public int tchat { get; set; }
        public string stt { get; set; }
        public string ma { get; set; }
        public string ten { get; set; }
        public string dvtinh { get; set; }
        public decimal sluong { get; set; }
        public decimal dgia { get; set; }
        public decimal tlckhau { get; set; }
        public decimal stckhau { get; set; }
        public decimal thtien { get; set; }
        public decimal tsuat { get; set; }
        public decimal tthue { get; set; }
        public decimal tgtien { get; set; }
    }

    partial class LoginDataMinvoice
    {
        public string username { get; set; }
        public string password { get; set; }
        public string ma_dvcs { get; set; }
    }

    partial class ApiDataResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public InvoiceResult data { get; set; }
        public string error { get; set; }
        public string token { get; set; }
    }

    partial class InvoiceResult
    {
        public string hoadon68_id { get; set; }
        public string cctbao_id { get; set; }
        public string hdlket_id { get; set; }
        public string tthai { get; set; }
        public int tthdon { get; set; }
        public string khieu { get; set; }
        public double shdon { get; set; }
        public DateTime tdlap { get; set; }
        public string dvtte { get; set; }
        public double tgia { get; set; }
        public string gchu { get; set; }
        public string tnmua { get; set; }
        public string mnmua { get; set; }
        public string ten { get; set; }
        public string mst { get; set; }
        public string dchi { get; set; }
        public string email { get; set; }
        public string cccdan { get; set; }
        public string sdtnmua { get; set; }
        public string stknmua { get; set; }
        public string htttoan { get; set; }
        public string stknban { get; set; }
        public string sbmat { get; set; }
        public string mdvi { get; set; }
        public string nglap { get; set; }
        public DateTime nlap { get; set; }
        public DateTime? ngsua { get; set; }
        public string nsua { get; set; }
        public double tgtcthue { get; set; }
        public double tgtthue { get; set; }
        public double ttcktmai { get; set; }
        public double tgtttbso { get; set; }
        public string tgtttbchu { get; set; }
        public string dlqrcode { get; set; }
        public string sdhang { get; set; }
        public string shdon1 { get; set; }
        public string mccqthue { get; set; }
        public DateTime? ngky { get; set; }
        public string nky { get; set; }
        public string signature { get; set; }
        public string hthdbtthe { get; set; }
        public DateTime? tdlhdbtthe { get; set; }
        public string khmshdbtthe { get; set; }
        public string khhdbtthe { get; set; }
        public string shdbtthe { get; set; }
        public double? tgtphi { get; set; }
        public double tgtcthue0 { get; set; }
        public double tgtthue0 { get; set; }
        public double ttcktmai0 { get; set; }
        public double tgtttbso0 { get; set; }
        public double tgtcthue5 { get; set; }
        public double tgtthue5 { get; set; }
        public double ttcktmai5 { get; set; }
        public double tgtttbso5 { get; set; }
        public double tgtcthue10 { get; set; }
        public double tgtthue10 { get; set; }
        public double ttcktmai10 { get; set; }
        public double tgtttbso10 { get; set; }
        public double tgtcthuekct { get; set; }
        public double tgtthuekct { get; set; }
        public double ttcktmaikct { get; set; }
        public double? tgtttbsokct { get; set; }
        public double tgtcthuekkk { get; set; }
        public double tgtthuekkk { get; set; }
        public double ttcktmaikkk { get; set; }
        public double? tgtttbsokkk { get; set; }
        public double? tgtphi0 { get; set; }
        public double? tgtphi5 { get; set; }
        public double? tgtphi10 { get; set; }
        public double? tgtphikct { get; set; }
        public double? tgtphikkk { get; set; }
        public string lhdon { get; set; }
        public string lddnbo { get; set; }
        public string tnvchuyen { get; set; }
        public string ptvchuyen { get; set; }
        public string dckhoxuat { get; set; }
        public string dckhonhap { get; set; }
        public string tennguoinhanhang { get; set; }
        public string mstnguoinhanhang { get; set; }
        public string phongban { get; set; }
        public string veviec { get; set; }
        public string sohopdong { get; set; }
        public string hdon68_id_lk { get; set; }
        public string phanHoiCQT { get; set; }
        public DateTime? ngay_duyet { get; set; }
        public string nguoi_duyet { get; set; }
        public double tgtcthuekhac { get; set; }
        public double ttcktmaikhac { get; set; }
        public double tgtthuekhac { get; set; }
        public double tgtttbsokhac { get; set; }
        public string hvtnxhang { get; set; }
        public string sovb { get; set; }
        public string ghi_chu { get; set; }
        public DateTime? ngayvb { get; set; }
        public bool? is_bangtonghop { get; set; }
        public int is_tthdon { get; set; }
        public bool? is_mau04 { get; set; }
        public string sbke { get; set; }
        public string nbke { get; set; }
        public string ma_dvcs { get; set; }
        public string nganhang_ngmua { get; set; }
        public string nganhang_ngban { get; set; }
        public string matdiep { get; set; }
        public string maloaitdiep { get; set; }
        public string matdtc { get; set; }
        public string macqt { get; set; }
        public string cuaHang { get; set; }
        public string maSoNhanVien { get; set; }
        public string soCMT { get; set; }
        public string soContainer { get; set; }
        public string soChi { get; set; }
        public double? tgtctkckhau { get; set; }
        public DateTime? ngayhopdong { get; set; }
        public string theomh { get; set; }
        public DateTime? hdktngay { get; set; }
        public string hdktso { get; set; }
        public DateTime? ngay_hd32 { get; set; }
        public string mauso_hd32 { get; set; }
        public string kyhieu_hd32 { get; set; }
        public string so_hd32 { get; set; }
        public string loai_hd32 { get; set; }
        public string tinhchat_hd32 { get; set; }
        public string ma1 { get; set; }
        public bool? is_success { get; set; }
        public string note_error { get; set; }
        public double? tgtcthue8 { get; set; }
        public double? tgtthue8 { get; set; }
        public double? ttcktmai8 { get; set; }
        public double? tgtttbso8 { get; set; }
        public double? tlptdoanhthu20 { get; set; }
        public double? tgtck20 { get; set; }
        public string txtck20 { get; set; }
        public DateTime? rms_arrival_date { get; set; }
        public DateTime? rms_departure_date { get; set; }
        public string rms_room_no { get; set; }
        public string rms_res_no { get; set; }
        public string rms_invoice { get; set; }
        public string rms_property { get; set; }
        public string rms_ext_ref { get; set; }
        public double? inv_TotalSVC { get; set; }
        public double? inv_TotalSST { get; set; }
        public string kptquan { get; set; }
        public string routeName { get; set; }
        public string departText { get; set; }
        public string arrivalText { get; set; }
        public DateTime? startDate { get; set; }
        public string startTime { get; set; }
        public string licensePlate { get; set; }
        public string ticketType { get; set; }
        public string busType { get; set; }
        public string seatNumber { get; set; }
        public string so_folio { get; set; }
        public DateTime? inv_arrivalDate { get; set; }
        public double? inv_amount { get; set; }
        public DateTime? inv_departureDate { get; set; }
        public string so_phong { get; set; }
        public string ma_nganh { get; set; }
        public string ma_kho { get; set; }
        public string user_so { get; set; }
        public string dia_chi_giao_hang { get; set; }
        public string PO { get; set; }
        public bool? is_print { get; set; }
        public bool? is_send_email { get; set; }
        public string khdly { get; set; }
        public string spxkho { get; set; }
        public string pbvmtruong { get; set; }
        public string pbvmtruong_test { get; set; }
        public string sopo { get; set; }
        public string GT_KCT1 { get; set; }
        public string GT_KCT2 { get; set; }
        public string GT_khac1 { get; set; }
        public string GT_khac2 { get; set; }
        public double? ttthue_hkdcn { get; set; }
        public double? ts_hkdcn { get; set; }
        public string so_number { get; set; }
        public DateTime? Due_date { get; set; }
        public string id { get; set; }
    }

}

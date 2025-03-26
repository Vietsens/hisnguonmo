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

namespace Inventec.Common.EHoaDon
{
    [Serializable]
    public class InvoiceDataWS
    {
        public InvoiceWS Invoice { get; set; }
        public List<InvoiceDetailsWS> ListInvoiceDetailsWS { get; set; }
        public List<InvoiceAttachFileWS> ListInvoiceAttachFileWS { get; set; }

        // Định danh duy nhất cho hóa đơn ở hệ thống của Partner (Phía Client).
        // Chỉ dùng 1 trong 2 trường PartnerInvoiceID, PartnerInvoiceStringID theo 1 trong 2 cách sau:
        // Cách 1: Dùng PartnerInvoiceID: Set PartnerInvoiceID > 0 và PartnerInvoiceStringID = null
        // Cách 2: Dùng PartnerInvoiceStringID: Set PartnerInvoiceID = 0 và PartnerInvoiceStringID != null
        public long PartnerInvoiceID { get; set; }
        public string PartnerInvoiceStringID { get; set; }
        
        //Lý do hủy
        public string Reason { get; set; }
    }

    [Serializable]
    public class InvoiceWS
    {
        /// <summary>
        /// Loại Hoá đơn: luôn là 1	(Hóa đơn giá trị gia tăng)
        /// </summary>
        public int InvoiceTypeID { get; set; }
        /// <summary>
        /// Ngày trên Hoá đơn
        /// </summary>
        public DateTime InvoiceDate { get; set; }
        /// <summary>
        /// Tên người mua hàng
        /// </summary>
        public string BuyerName { get; set; }
        /// <summary>
        /// Mã số thuế Người mua hàng
        /// </summary>
        public string BuyerTaxCode { get; set; }
        /// <summary>
        /// Tên đơn vị mua hàng
        /// </summary>
        public string BuyerUnitName { get; set; }
        /// <summary>
        /// Địa chỉ đơn vị mua hàng
        /// </summary>
        public string BuyerAddress { get; set; }
        /// <summary>
        /// Thông tin tài khoản ngân hàng người mua ví dụ: 11111111111 - BIDV chi nhánh Tây Hồ
        /// </summary>
        public string BuyerBankAccount { get; set; }
        /// <summary>
        /// Hình thức thanh toán: 1	Tiền mặt (mặc định), 2	Chuyển khoản, 3	Tiền mặt/Chuyển khoản, 4	Xuất hàng cho chi nhánh, 5	Hàng biếu tặng
        /// </summary>
        public int PayMethodID { get; set; }
        /// <summary>
        /// Hình thức nhận Hoá đơn: 1	Email , 2	Tin nhắn, 3	Email và tin nhắn, 4	Chuyển phát nhanh
        /// </summary>
        public int ReceiveTypeID { get; set; }
        /// <summary>
        /// eMail nhận Hoá đơn
        /// </summary>
        public string ReceiverEmail { get; set; }
        /// <summary>
        /// Số điện thoại nhận Hoá đơn
        /// </summary>
        public string ReceiverMobile { get; set; }
        /// <summary>
        /// Địa chỉ nhận Hoá đơn (Hoá đơn in chuyển đổi)
        /// </summary>
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// Tên người nhận Hoá đơn (Hoá đơn in chuyển đổi)
        /// </summary>
        public string ReceiverName { get; set; }
        /// <summary>
        /// Ghi chú Hoá đơn
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Dữ liệu KH tự định nghĩa (dạng json)
        /// </summary>
        public string UserDefine { get; set; }
        /// <summary>
        /// Mã ID chứng từ kế toán hoặc số Bill code của Hoá đơn Bán hàng
        /// </summary>
        public string BillCode { get; set; }
        /// <summary>
        /// ID tiền tệ: VND	- Việt Nam đồng (mặc định), USD - Đô la Mỹ, EUR - Đồng Euro, GBP - Bảng Anh, CNY - Nhân dân tệ,CHF - Phơ răng Thuỵ Sĩ ...
        /// </summary>
        public string CurrencyID { get; set; }
        /// <summary>
        /// Tỷ giá ngoại tệ so với VND: mặc định là 1
        /// </summary>
        public double ExchangeRate { get; set; }
        /// <summary>
        /// ID hệ thống tự sinh dùng để giao tiếp giữa các hệ thống
        /// </summary>
        public Guid InvoiceGUID { get; set; }
        /// <summary>
        /// Trạng thái của hóa đơn
        /// </summary>
        public int InvoiceStatusID { get; set; }
        /// <summary>
        /// Mẫu số Hóa đơn
        /// </summary>
        public string InvoiceForm { get; set; }
        /// <summary>
        /// Ký hiệu Hóa đơn
        /// </summary>
        public string InvoiceSerial { get; set; }
        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public int InvoiceNo { get; set; }
        /// <summary>
        /// Mã tra cứu
        /// </summary>
        public string InvoiceCode { get; set; }
        /// <summary>
        /// Ngày ký
        /// </summary>
        public DateTime SignedDate { get; set; }
        /// <summary>
        /// default 0: Tạo Hoá đơn, 1: Tạo Hoá đơn thay thế, 2: Tạo Hoá đơn điều chỉnh thông tin, 3: Tạo Hoá đơn điều chỉnh tăng, 4: Tạo Hoá đơn điều chỉnh giảm
        /// </summary>
        public int TypeCreateInvoice { get; set; }
        /// <summary>
        /// Thông tin Hoá đơn gốc dùng trong trường hợp thay thế, điều chỉnh. Định dạng như sau: [Mẫu Số]_[Ký hiệu]_[Số Hoá đơn], ví dụ: [01GTKT0/001]_[AA/17E]_[0000001]
        /// </summary>
        public string OriginalInvoiceIdentify { get; set; }

    }

    [Serializable]
    public class InvoiceDetailsWS
    {
        /// Tên hàng hóa, dịch vụ hoặc nội dung giảm giá chiết khấu (IsDiscount = 1)
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// Đơn vị tính hàng hóa, dịch vụ
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// Số lượng hàng hóa dịch vụ
        /// </summary>
        public double Qty { get; set; }
        /// <summary>
        /// Giá của hàng hóa
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Thành tiền hàng hóa dịch vụ hoặc số tiền chiết khấu
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// ID thuế suất: 1	0%, 2	5%, 3	10%, 4	Không chịu thuế, 5	Không kê khai thuế
        /// </summary>
        public int TaxRateID { get; set; }
        /// <summary>
        /// Thành tiền thuế
        /// </summary>
        public double TaxAmount { get; set; }
        /// <summary>
        /// Là chiết khấu ghi trên Hoá đơn: 1 - là chiết khấu, mặc định là 0 
        /// </summary>
        public bool IsDiscount { get; set; }
        /// <summary>
        /// Dữ liệu KH tự định nghĩa (dạng json/xml)
        /// </summary>
        public string UserDefineDetails { get; set; }
        /// <summary>
        /// Dùng mã lệnh 121: Điều chỉnh thì Báo tăng là True, Báo giảm là False
        /// </summary>
        public bool IsIncrease { get; set; }
    }

    [Serializable]
    public class InvoiceAttachFileWS
    {
        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName { set; get; }
        /// <summary>
        /// Phần mở rộng (docx,pdf...)
        /// </summary>
        public string FileExtension { set; get; }
        /// <summary>
        /// Nội dung file dạng Base64
        /// </summary>
        public string FileContent { set; get; }
    }
}

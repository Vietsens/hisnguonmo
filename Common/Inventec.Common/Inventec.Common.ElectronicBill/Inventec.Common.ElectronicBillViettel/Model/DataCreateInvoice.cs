using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    /// <summary>
    /// Dữ liệu đầu vào đối với các API lập hóa đơn, điều chỉnh hóa đơn, thay thế hóa đơn, lập hóa đơn nháp, lập hóa đơn usb token, xem trước hóa đơn nháp các trường dữ liệu truyền vào sẽ có dạng chung
    /// </summary>
    [Serializable]
    public class DataCreateInvoice
    {
        /// <summary>
        /// Thông tin chung của hóa đơn
        /// </summary>
        public GeneralInvoiceInfo generalInvoiceInfo { get; set; }

        /// <summary>
        /// Thông tin người mua
        /// </summary>
        public BuyerInfo buyerInfo { get; set; }

        /// <summary>
        /// Thông tin người bán
        /// </summary>
        public SellerInfo sellerInfo { get; set; }

        /// <summary>
        /// thông tin thanh toán
        /// </summary>
        public List<Payments> payments { get; set; }

        /// <summary>
        /// thông tin hàng hóa
        /// </summary>
        public List<ItemInfo> itemInfo { get; set; }

        /// <summary>
        /// thông tin tổng hợp tiền của hóa đơn
        /// </summary>
        public SummarizeInfo summarizeInfo { get; set; }

        /// <summary>
        /// thông tin gom nhóm tiền hóa đơn theo thuế suất
        /// </summary>
        public List<TaxBreakdowns> taxBreakdowns { get; set; }

        /// <summary>
        /// thông tin trường động
        /// </summary>
        public List<Metadata> metadata { get; set; }
    }
}

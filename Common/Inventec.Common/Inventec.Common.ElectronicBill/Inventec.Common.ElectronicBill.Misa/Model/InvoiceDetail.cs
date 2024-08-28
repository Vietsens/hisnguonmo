using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class InvoiceDetail
    {
        /// <summary>
        /// Số thứ tự dòng mặt hàng, bắt đầu từ 1.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Tên mặt hàng
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Số lượng mặt hàng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Tiền hàng nguyên tệ không bao gồm thuế
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Mức thuế GTGT.
        /// - 0: 0%
        /// - 5: 5%
        /// - 10: 10%
        /// - -1: Không chịu thuế
        /// </summary>
        public decimal VatPercentage { get; set; }

        /// <summary>
        /// Tiền thuế GTGT
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// Đơn giá mặt hàng không bao gồm thuế
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Loại hàng hóa:
        /// 0: Hàng hóa vật tư
        /// 1: Hàng hóa thành phẩm
        /// 2: Hàng hóa dịch vụ
        /// 3: Đánh dấu là dòng diễn giải
        /// 4: Đánh dấu là dòng chiết khấu
        /// </summary>
        //public int InventoryItemType { get; set; }

        /// <summary>
        /// Có phải là hàng khuyến mại hay không.
        /// 0: Không phải là hàng khuyến mại
        /// 1: Là hàng khuyến mại
        /// </summary>
        public bool Promotion { get; set; }
    }
}

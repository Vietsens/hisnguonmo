using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider
{
    class CreatQrData
    {
        /// <summary>
        /// Mã Merchant (BIDV cấp)
        /// Max(100)
        /// </summary>
        public string merchantId { get; set; }

        /// <summary>
        /// Tên viêt tắt của Merchant (BIDV cấp)
        /// Max(25)
        /// </summary>
        public string merchantName { get; set; }

        /// <summary>
        /// Merchant Guid (Mặc định là: 970488 )
        /// Max(20)
        /// </summary>
        public string merchantGuid { get; set; }

        /// <summary>
        /// Mã Vùng: default VN
        /// 2
        /// </summary>
        public string countryCode { get; set; }

        /// <summary>
        /// Tên điểm thu (BIDV cấp)
        /// Max(20)
        /// </summary>
        public string storeLable { get; set; }

        /// <summary>
        /// Mã điểm thu(BIDV cấp)
        /// Max(8)
        /// </summary>
        public string terminalLable { get; set; }

        /// <summary>
        /// Mã tỉnh, thành phố mặc định mặc định là HANOI
        /// Max(4)
        /// </summary>
        public string merchantCity { get; set; }

        /// <summary>
        /// Số hóa đơn QR terminal.
        /// Max(15)
        /// </summary>
        public string billNumber { get; set; }

        /// <summary>
        /// Số tiền 
        /// Max(13)
        /// </summary>
        public string amount { get; set; }

        /// <summary>
        /// Mã tiền tệ : Giá trị mặc định 704
        /// Max(3)
        /// </summary>
        public string ccy { get; set; }

        /// <summary>
        /// Mã merchant Catelory Code Mặc định: 8062
        /// 
        /// </summary>
        public string merchantCateloryCode { get; set; }

        /// <summary>
        /// Mã bưu chính quốc gia. Hà nội: 10000
        /// </summary>
        public string postalCode { get; set; }

        /// <summary>
        /// Phương thức khởi tạo. 11: QR tĩnh. 12: QR động
        /// </summary>
        public string methodCode { get; set; }

        /// <summary>
        /// Acquier ID/BNB ID
        /// </summary>
        public string acquierOrBnb { get; set; }

        /// <summary>
        /// GUID
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// Merchant/Consumer
        /// </summary>
        public string merchantOrConsumer { get; set; }

        /// <summary>
        /// QRIBFTTC: dịch vụ chuyển nhanh NAPAS247 bằng mã QR đến thẻ
        /// QRIBFTTA: dịch vụ chuyển nhanh NAPAS247 bằng mã QR đến Tài khoản.
        /// </summary>
        public string qrType { get; set; }

        public string payLoad { get; set; }
        public string pointOTMethod { get; set; }
        public string merchantCode { get; set; }
        public string terminalId { get; set; }
        public string storeLabel { get; set; }
        public string purpose { get; set; }


    }
}

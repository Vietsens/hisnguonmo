using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.BIDV.Model
{
    class ResponseData
    {
        /// <summary>
        /// Mã lỗi trả về
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Mô tả mã lỗi chi đính kèm(Bảng mã lỗi)
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Dữ liệu qrcode trả về
        /// </summary>
        public string data { get; set; }
    }
}

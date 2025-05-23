﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class CancelInvoice
    {
        /// <summary>
        /// Mã số thuế của doanh nghiệp/chi nhánh phát hành hóa đơn. Một doanh nghiệp có thể có nhiều mã số thuế
        /// Maxlength : 11
        /// </summary>
        public string supplierTaxCode { get; set; }

        /// <summary>
        /// Mã mẫu hóa đơn, tuân thủ theo quy định ký hiệu mẫu hóa đơn của Thông tư hướng dẫn thi hành nghị định số 51/2010/NĐ-CP
        /// Minlength : 11
        /// Maxlength : 11
        /// </summary>
        public string templateCode { get; set; }

        /// <summary>
        /// Là ký hiệu hóa đơn + số hóa đơn vd : AA/16E0000001
        /// Minlength : 7
        /// Maxlength : 13
        /// </summary>
        public string invoiceNo { get; set; }

        /// <summary>
        /// Ngày lập hóa đơn, không bắt buộc. (không vượt quá ngày hiện tại) - yyyyMMddHHmmss
        /// </summary>
        public string strIssueDate { get; set; }

        /// <summary>
        /// Tên văn bản thỏa thuận hủy hóa đơn
        /// Maxlength : 100
        /// </summary>
        public string additionalReferenceDesc { get; set; }

        /// <summary>
        /// Ngày thỏa thuận (không vượt quá ngày hiện tại) - yyyyMMddHHmmss
        /// </summary>
        public string additionalReferenceDate { get; set; }
    }
}

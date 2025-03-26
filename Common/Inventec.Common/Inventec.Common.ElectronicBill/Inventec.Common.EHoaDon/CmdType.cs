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
    public class CmdType
    {
        public const int Undefined = 0;

        //1xx: Thêm mới
        public const int CreateInvoiceMT = 100; // Tạo Hóa đơn, eHD tự cấp InvoiceForm, InvoiceSerial; InvoiceNo = 0 (tạo Hóa đơn mới)
        public const int CreateInvoiceTR = 101; // Tạo Hóa đơn, eHD tự cấp InvoiceForm, InvoiceSerial và cấp InvoiceNo (tạo Hóa đơn Trống)
        public const int CreateInvoiceWithFormSerial = 110; // Tạo Hóa đơn, Client tự cấp InvoiceForm, InvoiceSerial; InvoiceNo = 0 (tạo Hóa đơn mới)
        public const int CreateInvoiceWithFormSerialNo = 111; // Tạo Hóa đơn, Client tự cấp InvoiceForm, InvoiceSerial, InvoiceNo (tạo Hóa đơn mới, có sẵn Số Hóa đơn)
        public const int CreateInvoiceWithFormSerialReturnNo = 112; // Tạo Hóa đơn, Client tự cấp InvoiceForm, InvoiceSerial, bkav cấp InvoiceNo (tạo Hóa đơn mới)

        public const int CreateInvoiceReplace = 120; // Tạo Hóa đơn thay thế cho 1 Hoá đơn khác
        public const int CreateInvoiceAdjust = 121; // Tạo Hóa đơn điều chỉnh cho 1 Hoá đơn khác
        // 2xx: Cập nhật
        public const int UpdateInvoiceByPartnerInvoiceID = 200; // Cập nhật thông tin của Hoá đơn
        public const int UpdateInvoiceByInvoiceGUID = 204; // Cập nhật thông tin của Hoá đơn

        public const int CancelInvoiceByInvoiceGUID = 201; // Hủy hóa đơn
        public const int CancelInvoiceByPartnerInvoiceID = 202; // Hủy hóa đơn

        // 3xx: Xóa
        public const int DeleteInvoiceByPartnerInvoiceID = 301; // Xoá hóa đơn chưa phát hành
        public const int DeleteInvoiceByInvoiceGUID = 303; // Xoá hóa đơn chưa phát hành

        // 5xx: File
        public const int UploadFile = 500;//Upload file excel dữ liệu Hóa đơn
        // 6xx: View hoá đơn
        public const int ViewInvoice = 600;//Trả về cho Partner file PDF bản thể hiện của Hoá đơn theo trạng thái
        public const int ViewInvoiceConversion = 601;//Trả về cho Partner file PDF chuyển đổi
        // 8xx: Các hàm lấy thông tin, báo cáo
        public const int GetInvoiceDataWS = 800; // Lấy thông tin của tờ Hóa đơn
        public const int GetInvoiceStatusID = 801; // Lấy trạng thái của tờ Hóa đơn
        public const int GetInvoiceHistory = 802; // Lấy lịch sử thay đổi của 1 tờ Hóa đơn
        public const int GetInvoiceLink = 804;//Lấy link để tải file hóa đơn in chuyển đổi
        public const int GetInvoiceShow = 816;//In hóa đơn bản thể hiện
        public const int CreateAccount = 902;//Tạo tài khoản Demo trên eHD demo
        public const int UpdateAccount = 903; // Cập nhật thông tin tài khoản
        public const int GetUnitInforByTaxCode = 904; // Lấy thông tin doanh nghiệp từ thuế

        //10xx Trao đổi với tool
        public const int GetRunTypeInfo = 1000; // Lấy kiểu xử lý và FileName, ClassName.
        public const int GetDLLContent = 1001; // Lấy nội dung file DLL.
    }
}

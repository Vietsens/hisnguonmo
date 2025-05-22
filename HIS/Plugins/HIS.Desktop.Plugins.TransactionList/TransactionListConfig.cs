using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionList
{
    class TransactionListConfig
    {
        private const string Key__InvoiceTypeCreate = "HIS.Desktop.ElectronicBill.Type";

        private const string Key__InvoiceTemplateCreate = "HIS.Desktop.Plugins.Library.ElectronicBill.Template";


        /// <summary>
        /// key cấu hình HIS.Desktop.Plugins.Library.ElectronicBill.Template
        /// </summary>
        private static string invoiceTemplateCreate;
        public static string InvoiceTemplateCreate
        {
            get
            {
                invoiceTemplateCreate = GetValue(Key__InvoiceTemplateCreate);
                return invoiceTemplateCreate;
            }
            set
            {
                invoiceTemplateCreate = value;
            }
        }

        /// <summary>
        /// Cấu hình chế độ tạo hóa đơn điện tử, chữ ký điện tử
        //- Đặt 1: Chỉ tạo hóa đơn điện tử trên hệ thống của vnpt, không tạo trên hệ thống HIS
        //- Đặt 2: Tạo giao dịch trên hệ thống HIS, tự tạo hóa đơn + ký điện tử trên hóa đơn lưu trên hệ thống HIS
        //- Mặc định là ẩn chức năng đi
        /// </summary>
        private static string invoiceTypeCreate;
        public static string InvoiceTypeCreate
        {
            get
            {
                invoiceTypeCreate = GetValue(Key__InvoiceTypeCreate);
                return invoiceTypeCreate;
            }
            set
            {
                invoiceTypeCreate = value;
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = null;
            }
            return result;
        }

    }
}

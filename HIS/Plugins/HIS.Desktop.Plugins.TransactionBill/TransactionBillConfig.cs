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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBill
{
    class TransactionBillConfig
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

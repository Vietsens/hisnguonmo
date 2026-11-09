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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    /// <summary>
    /// Exception đã biết rõ nguồn lỗi (kết nối / nhà cung cấp / máy trạm / cấu hình...).
    /// Các ApiConsumer ném exception này để lớp trên không phải đoán lại từ ex.Message.
    /// </summary>
    public class ElectronicBillException : Exception
    {
        /// <summary>Nguồn gây lỗi.</summary>
        public ElectronicBillErrorSource Source { get; private set; }

        /// <summary>Mã nhà cung cấp (ProviderType.*), có thể null.</summary>
        public string Provider { get; private set; }

        /// <summary>Mã HTTP trả về (nếu có), 0 nếu không áp dụng.</summary>
        public int StatusCode { get; private set; }

        public ElectronicBillException(ElectronicBillErrorSource source, string message)
            : base(message)
        {
            this.Source = source;
        }

        public ElectronicBillException(ElectronicBillErrorSource source, string message, Exception inner)
            : base(message, inner)
        {
            this.Source = source;
        }

        public ElectronicBillException(ElectronicBillErrorSource source, string provider, string message, Exception inner)
            : base(message, inner)
        {
            this.Source = source;
            this.Provider = provider;
        }

        public ElectronicBillException(ElectronicBillErrorSource source, string provider, int statusCode, string message)
            : base(message)
        {
            this.Source = source;
            this.Provider = provider;
            this.StatusCode = statusCode;
        }
    }
}

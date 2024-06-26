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
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    internal class ADO
    {

    }

    internal class PageTotal
    {
        public static long SoDongTrangDauTienMauPhieuHoaDonChiTiet { get; set; }
        public static long SoDongTrangTiepTheoMauPhieuHoaDonChiTiet { get; set; }
    }

    internal class ResultValidateControl
    {
        public bool Result { get; set; }

        public string Notification { get; set; }
    }

    public class HIS_INVOICE_DETAIL_NEW : HIS_INVOICE_DETAIL
    {
        public long ADD_TIME { get; set; }

        public int ACTION { get; set; }

        public decimal SUM_PRICE_STR { get; set; }
    }

    internal enum MenuPrintType
    {
        PrintInvoice,
        PrintInvoiceOrder
    }
}

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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.BKAV
{
    /// <summary>
    /// Bo sung cac truong BKAV moi ma Inventec.Common.EHoaDon.InvoiceWS chua co.
    /// EHoaDon serialize bang JsonConvert.SerializeObject theo kieu runtime nen cac truong o lop con van duoc gui di.
    /// </summary>
    [Serializable]
    class BkavInvoiceWS : Inventec.Common.EHoaDon.InvoiceWS
    {
        public string CCCD { get; set; }
        public string PassportNumber { get; set; }
        public string FiscalCodes { get; set; }
    }
}

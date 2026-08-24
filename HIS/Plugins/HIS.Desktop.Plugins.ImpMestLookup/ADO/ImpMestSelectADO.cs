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
using System;

namespace HIS.Desktop.Plugins.ImpMestLookup.ADO
{
    /// <summary>
    /// Dữ liệu 1 dòng trên lưới chọn phiếu nhập - dùng khi tra cứu theo số hóa đơn
    /// trả về nhiều phiếu nhập trùng số hóa đơn.
    /// </summary>
    public class ImpMestSelectADO
    {
        public V_HIS_IMP_MEST ImpMest { get; set; }
        public string IMP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string IMP_TIME_STR { get; set; }
        public string IMP_USER_NAME { get; set; }
        public Nullable<decimal> DOCUMENT_PRICE { get; set; }

        public ImpMestSelectADO()
        {
        }

        public ImpMestSelectADO(V_HIS_IMP_MEST impMest)
        {
            try
            {
                if (impMest == null) return;
                this.ImpMest = impMest;
                this.IMP_MEST_CODE = impMest.IMP_MEST_CODE;
                this.MEDI_STOCK_NAME = impMest.MEDI_STOCK_CODE + " - " + impMest.MEDI_STOCK_NAME;
                this.IMP_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(impMest.IMP_TIME ?? 0);
                this.IMP_USER_NAME = impMest.IMP_LOGINNAME + " - " + impMest.IMP_USERNAME;
                this.DOCUMENT_PRICE = impMest.DOCUMENT_PRICE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

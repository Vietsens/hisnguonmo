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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000111
{
    /// <summary>
    /// Mot dong cua bang chiet khau giao dich (HIS_TRANSACTION_DISCOUNT)
    /// </summary>
    public class Mps000111DiscountADO
    {
        /// <summary>
        /// So thu tu dong
        /// </summary>
        public long NUM_ORDER { get; set; }

        /// <summary>
        /// Tien chiet khau
        /// </summary>
        public decimal DISCOUNT { get; set; }

        /// <summary>
        /// Ti le chiet khau (%)
        /// </summary>
        public decimal DISCOUNT_RATIO { get; set; }

        /// <summary>
        /// Ti le chiet khau dang chuoi, vd "10%"
        /// </summary>
        public string DISCOUNT_RATIO_STR { get; set; }

        /// <summary>
        /// Ly do chiet khau
        /// </summary>
        public string REASON { get; set; }

        public Mps000111DiscountADO() { }

        public Mps000111DiscountADO(HIS_TRANSACTION_DISCOUNT data)
        {
            try
            {
                if (data == null) return;

                this.DISCOUNT = data.DISCOUNT ?? 0;
                this.DISCOUNT_RATIO = data.DISCOUNT_RATIO ?? 0;
                this.DISCOUNT_RATIO_STR = (data.DISCOUNT_RATIO ?? 0) + "%";
                this.REASON = data.REASON;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

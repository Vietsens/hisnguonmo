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

namespace MPS.Processor.Mps000112
{
    /// <summary>
    /// Mot dong cua bang hinh thuc thanh toan (HIS_TRANSACTION_PAYFORM)
    /// </summary>
    public class Mps000112PayformADO
    {
        /// <summary>
        /// So thu tu dong
        /// </summary>
        public long NUM_ORDER { get; set; }

        /// <summary>
        /// Ma hinh thuc thanh toan
        /// </summary>
        public string PAY_FORM_CODE { get; set; }

        /// <summary>
        /// Ten hinh thuc thanh toan
        /// </summary>
        public string PAY_FORM_NAME { get; set; }

        /// <summary>
        /// Ma ngan hang (neu co)
        /// </summary>
        public string BANK_CODE { get; set; }

        /// <summary>
        /// Ten ngan hang (neu co)
        /// </summary>
        public string BANK_NAME { get; set; }

        /// <summary>
        /// So tien cua dong nay
        /// </summary>
        public decimal AMOUNT { get; set; }

        /// <summary>
        /// Ten phu phi
        /// </summary>
        public string SURCHARGE_NAME { get; set; }

        /// <summary>
        /// Tien phu phi
        /// </summary>
        public decimal SURCHARGE_AMOUNT { get; set; }

        /// <summary>
        /// Thanh tien quy doi VND (so tien + phu phi)
        /// </summary>
        public decimal TOTAL_AMOUNT { get; set; }

        /// <summary>
        /// Ma loai tien (rong = VND)
        /// </summary>
        public string CURRENCY_CODE { get; set; }

        /// <summary>
        /// Ti gia quy doi sang VND
        /// </summary>
        public decimal EXCHANGE_RATE { get; set; }

        /// <summary>
        /// So tien nguyen te
        /// </summary>
        public decimal FOREIGN_AMOUNT { get; set; }

        public Mps000112PayformADO() { }

        public Mps000112PayformADO(HIS_TRANSACTION_PAYFORM data, List<HIS_PAY_FORM> payForms, List<HIS_BANK> banks)
        {
            try
            {
                if (data == null) return;

                this.AMOUNT = data.AMOUNT;
                this.SURCHARGE_NAME = data.SURCHARGE_NAME;
                this.SURCHARGE_AMOUNT = data.SURCHARGE_AMOUNT ?? 0;
                this.TOTAL_AMOUNT = data.TOTAL_AMOUNT;
                this.CURRENCY_CODE = data.CURRENCY_CODE;
                this.EXCHANGE_RATE = data.EXCHANGE_RATE ?? 0;
                this.FOREIGN_AMOUNT = data.FOREIGN_AMOUNT ?? 0;

                if (payForms != null && payForms.Count > 0)
                {
                    var payForm = payForms.FirstOrDefault(o => o.ID == data.PAY_FORM_ID);
                    if (payForm != null)
                    {
                        this.PAY_FORM_CODE = payForm.PAY_FORM_CODE;
                        this.PAY_FORM_NAME = payForm.PAY_FORM_NAME;
                    }
                }

                if (data.BANK_ID.HasValue && banks != null && banks.Count > 0)
                {
                    var bank = banks.FirstOrDefault(o => o.ID == data.BANK_ID.Value);
                    if (bank != null)
                    {
                        this.BANK_CODE = bank.BANK_CODE;
                        this.BANK_NAME = bank.BANK_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dong hinh thuc thanh toan suy ra tu chinh giao dich - dung khi giao dich khong co
        /// dong HIS_TRANSACTION_PAYFORM nao (config MULTI_PAYFORM tat).
        /// Giao dich khong luu phu phi / loai tien / ti gia nen cac truong do de mac dinh.
        /// </summary>
        public Mps000112PayformADO(V_HIS_TRANSACTION tran)
        {
            try
            {
                if (tran == null) return;

                this.PAY_FORM_CODE = tran.PAY_FORM_CODE;
                this.PAY_FORM_NAME = tran.PAY_FORM_NAME;
                this.BANK_CODE = tran.BANK_CODE;
                this.BANK_NAME = tran.BANK_NAME;
                this.AMOUNT = tran.AMOUNT;
                this.TOTAL_AMOUNT = tran.AMOUNT;
                this.SURCHARGE_AMOUNT = 0;
                this.EXCHANGE_RATE = 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.UC.TransactionPayformGrid.ADO
{
    /// <summary>
    /// Mot loai tien (ngoai te) kem ti gia mac dinh cho cot "Loai tien" / "Ti gia".
    /// Form cha cung cap danh sach nay (vd tu cau hinh ngoai te cua vien).
    /// </summary>
    public class CurrencyItemADO
    {
        /// <summary>ID loai tien (HIS_CURRENCY.ID)</summary>
        public long CURRENCY_ID { get; set; }

        /// <summary>Ma loai tien (VD: USD, EUR, VND)</summary>
        public string CURRENCY_CODE { get; set; }

        /// <summary>Ten loai tien (hien thi)</summary>
        public string CURRENCY_NAME { get; set; }

        /// <summary>Ti gia quy doi sang VND (1 don vi ngoai te = ? VND). Nguoi dung co the sua lai.</summary>
        public decimal EXCHANGE_RATE { get; set; }
    }
}

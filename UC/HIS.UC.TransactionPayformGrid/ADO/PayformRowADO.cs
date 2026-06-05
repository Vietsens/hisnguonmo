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
    /// Mot dong trong luoi hinh thuc thanh toan (1 dong = 1 hinh thuc).
    /// Vua la du lieu binding cho grid (in-place edit), vua la du lieu form cha
    /// doc ra qua Processor.GetData() khi bam nut tao giao dich.
    /// </summary>
    public class PayformRowADO
    {
        /// <summary>ID hinh thuc thanh toan duoc chon (cot "Hinh thuc TT")</summary>
        public long PAY_FORM_ID { get; set; }

        /// <summary>Ten hinh thuc thanh toan (de hien thi / log)</summary>
        public string PAY_FORM_NAME { get; set; }

        /// <summary>ID ngan hang duoc chon (cot "Ngan hang"). null neu khong ap dung</summary>
        public long? BANK_ID { get; set; }

        /// <summary>Ten ngan hang (de hien thi / log)</summary>
        public string BANK_NAME { get; set; }

        /// <summary>Phu phi ngan hang (VND) (cot "Phu phi"). Tu dien theo cau hinh, nguoi dung sua duoc</summary>
        public decimal BANK_FEE_AMOUNT { get; set; }

        /// <summary>Ti le phu phi (%) ap dung cho dong nay (de hien thi "(2.7%)"). null neu khong co cau hinh</summary>
        public decimal? BANK_FEE_RATIO { get; set; }

        /// <summary>So tien benh nhan dua / so tien thanh toan cua dong (cot "So tien")</summary>
        public decimal AMOUNT { get; set; }

        /// <summary>ID loai tien duoc chon (de map sang SDO). null = VND</summary>
        public long? CURRENCY_ID { get; set; }

        /// <summary>Ma loai tien (cot "Loai tien"). Rong = VND</summary>
        public string CURRENCY_CODE { get; set; }

        /// <summary>Ten phu phi (tu cau hinh) - de luu sang SDO</summary>
        public string BANK_FEE_NAME { get; set; }

        /// <summary>Ti gia quy doi sang VND (cot "Ti gia"). Mac dinh 1 cho VND</summary>
        public decimal EXCHANGE_RATE { get; set; }

        /// <summary>Thanh tien quy doi sang VND (cot "Thanh tien (VND)") - chi doc, tu tinh</summary>
        public decimal TOTAL_AMOUNT_VND { get; set; }

        /// <summary>Tich "Con lai" - dong nay tu tinh So tien = Phai thu - tong cac dong khac. Chi 1 dong duoc tich</summary>
        public bool IS_REMAINING { get; set; }
    }
}

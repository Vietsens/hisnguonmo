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
    /// Mot dong cau hinh phu phi ngan hang (HisPayFormBankFee).
    /// Phu phi = So tien * FEE_RATIO / 100.
    /// Form cha parse cau hinh HisPayFormBankFee sang danh sach ADO nay.
    /// </summary>
    public class BankFeeConfigADO
    {
        /// <summary>ID hinh thuc thanh toan ap dung phu phi</summary>
        public long PAY_FORM_ID { get; set; }

        /// <summary>ID ngan hang ap dung phu phi. null = ap dung cho moi ngan hang cua hinh thuc</summary>
        public long? BANK_ID { get; set; }

        /// <summary>Ti le phu phi (%) tren so tien. VD: 2.7 nghia la 2.7%</summary>
        public decimal FEE_RATIO { get; set; }

        /// <summary>Ten phu phi (HIS_PAY_FORM_BANK_FEE.FEE_NAME) - de hien thi/luu</summary>
        public string FEE_NAME { get; set; }
    }
}

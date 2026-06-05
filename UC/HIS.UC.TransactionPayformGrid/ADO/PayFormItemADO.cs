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
    /// Mot hinh thuc thanh toan kha dung de hien thi trong combo cot "Hinh thuc TT".
    /// Form cha map tu HIS_PAY_FORM (vd payFormList cua TransactionBill) sang ADO nay.
    /// </summary>
    public class PayFormItemADO
    {
        /// <summary>ID hinh thuc thanh toan (HIS_PAY_FORM.ID)</summary>
        public long PAY_FORM_ID { get; set; }

        /// <summary>Ma hinh thuc thanh toan</summary>
        public string PAY_FORM_CODE { get; set; }

        /// <summary>Ten hinh thuc thanh toan (hien thi)</summary>
        public string PAY_FORM_NAME { get; set; }

        /// <summary>True neu hinh thuc nay BAT BUOC chon ngan hang (chuyen khoan, quet the)</summary>
        public bool IsRequiredBank { get; set; }

        /// <summary>True neu hinh thuc nay cho phep chon ngan hang (hien cot Ngan hang)</summary>
        public bool IsShowBank { get; set; }

        /// <summary>True neu hinh thuc la tien mat ngoai te (hien cot Loai tien + Ti gia)</summary>
        public bool IsForeignCurrency { get; set; }
    }
}

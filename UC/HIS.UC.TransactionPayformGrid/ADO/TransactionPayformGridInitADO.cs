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
    /// Du lieu dau vao khoi tao UC luoi hinh thuc thanh toan.
    /// Form cha nap day du danh muc + cau hinh roi truyen vao - UC KHONG goi API.
    /// </summary>
    public class TransactionPayformGridInitADO
    {
        /// <summary>Danh sach hinh thuc thanh toan kha dung (combo cot "Hinh thuc TT")</summary>
        public List<PayFormItemADO> ListPayForm { get; set; }

        /// <summary>Danh sach ngan hang kha dung (combo cot "Ngan hang")</summary>
        public List<BankItemADO> ListBank { get; set; }

        /// <summary>Danh sach loai tien / ti gia (combo cot "Loai tien")</summary>
        public List<CurrencyItemADO> ListCurrency { get; set; }

        /// <summary>Cau hinh phu phi ngan hang (HisPayFormBankFee) da parse</summary>
        public List<BankFeeConfigADO> ListBankFeeConfig { get; set; }

        /// <summary>So tien phai thu (Can thu). Dung cho cot "Con lai" va canh bao "Con thieu"</summary>
        public decimal RequiredAmount { get; set; }

        /// <summary>Cac dong khoi tao san (vd khi mo lai giao dich da luu). Co the null/rong</summary>
        public List<PayformRowADO> InitRows { get; set; }

        /// <summary>True: hien cot "Con lai" (o tich tu tinh). Mac dinh true theo tai lieu 4.1.1</summary>
        public bool IsShowRemainingColumn { get; set; }

        /// <summary>Callback bao form cha khi Tong thanh tien / Con thieu thay doi (de active nut tao giao dich)</summary>
        public DelegateTotalAmountChanged DelegateTotalAmountChanged { get; set; }

        /// <summary>Icon nut Xoa dong (X do) - form cha truyen de dong bo voi grid Chiet khau/Quy ho tro</summary>
        public System.Drawing.Image DeleteButtonImage { get; set; }

        public TransactionPayformGridInitADO()
        {
            this.ListPayForm = new List<PayFormItemADO>();
            this.ListBank = new List<BankItemADO>();
            this.ListCurrency = new List<CurrencyItemADO>();
            this.ListBankFeeConfig = new List<BankFeeConfigADO>();
            this.InitRows = new List<PayformRowADO>();
            this.IsShowRemainingColumn = true;
        }
    }
}

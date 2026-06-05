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
    /// UC TU LAY danh muc (PayForm/Bank tu cache, Currency/BankFee tu API) - form cha CHI truyen sizing,
    /// so tien phai thu, callback va (tuy chon) cac dong khoi tao.
    /// </summary>
    public class  TransactionPayformGridInitADO
    {
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

        #region Sizing (giong HIS.UC.Icd) - form cha truyen de fix kich thuoc. <= 0 = bo qua (giu mac dinh)
        /// <summary>Chieu rong UC (px). > 0 va Height > 0 thi set this.Size</summary>
        public int Width { get; set; }

        /// <summary>Chieu cao UC (px). > 0 va Width > 0 thi set this.Size</summary>
        public int Height { get; set; }

        /// <summary>Co chu trong luoi (font size). > 0 thi ap cho Row/Header/Footer</summary>
        public float SizeText { get; set; }

        /// <summary>Do rong vung caption/label (px). Du tru theo chuan Icd - UC luoi chua co label rieng nen hien chua dung</summary>
        public int LabelTextSize { get; set; }

        /// <summary>Chieu rong toi thieu UC (px). > 0 thi set this.MinimumSize</summary>
        public int MinSize { get; set; }
        #endregion

        public TransactionPayformGridInitADO()
        {
            this.InitRows = new List<PayformRowADO>();
            this.IsShowRemainingColumn = true;
        }
    }
}

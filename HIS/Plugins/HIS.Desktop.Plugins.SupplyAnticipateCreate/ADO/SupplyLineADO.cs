using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate.ADO
{
    /// <summary>
    /// One line of the supply anticipate (HIS_ANTICIPATE_METY or HIS_ANTICIPATE_MATY) + reference numbers
    /// </summary>
    class SupplyLineADO
    {
        public int STT { get; set; }
        public string KEY { get; set; }
        public long? LINE_ID { get; set; }
        public long TYPE_ID { get; set; }
        public bool IS_MATERIAL { get; set; }
        public string TYPE_CODE { get; set; }
        public string TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKING { get; set; }
        public string KIND_NAME { get; set; }
        public List<long> SUPPLIER_IDS { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public string NOTE { get; set; }

        public decimal? TOTAL_IN_STOCK { get; set; }
        public string STOCK_TIME_DISPLAY { get; set; }
        public decimal? EXP_MONTH_1 { get; set; }
        public decimal? EXP_MONTH_2 { get; set; }
        public decimal? EXP_MONTH_3 { get; set; }
        public decimal? LAST_APPROVED_AMOUNT { get; set; }
        public string LAST_APPROVED_MONTH_DISPLAY { get; set; }
        public decimal? LAST_UNAPPROVED_AMOUNT { get; set; }

        /// <summary>
        /// SL xin
        /// </summary>
        public decimal? AMOUNT { get; set; }

        /// <summary>
        /// SL duyet
        /// </summary>
        public decimal? APPROVAL_AMOUNT { get; set; }

        public decimal? IMP_PRICE { get; set; }

        /// <summary>
        /// Ratio as stored in DB (0.1 = 10%)
        /// </summary>
        public decimal? IMP_VAT_RATIO { get; set; }

        /// <summary>
        /// Money is computed on SL duyet (approve mode, approved anticipate) or on SL xin (otherwise)
        /// </summary>
        public bool UseApprovalAmount { get; set; }

        public decimal? VAT_PERCENT
        {
            get { return this.IMP_VAT_RATIO.HasValue ? (decimal?)(this.IMP_VAT_RATIO.Value * 100) : null; }
        }

        public decimal CalcAmount
        {
            get { return (this.UseApprovalAmount ? this.APPROVAL_AMOUNT : this.AMOUNT) ?? 0; }
        }

        public decimal TOTAL_PRICE
        {
            get { return Math.Round(this.CalcAmount * (this.IMP_PRICE ?? 0), 4); }
        }

        public decimal VAT_PRICE
        {
            get { return Math.Round(this.TOTAL_PRICE * (this.IMP_VAT_RATIO ?? 0), 4); }
        }

        public decimal TOTAL_PRICE_WITH_VAT
        {
            get { return this.TOTAL_PRICE + this.VAT_PRICE; }
        }

        public bool IsApprovalDifferent
        {
            get { return this.APPROVAL_AMOUNT.HasValue && this.APPROVAL_AMOUNT != this.AMOUNT; }
        }
    }
}

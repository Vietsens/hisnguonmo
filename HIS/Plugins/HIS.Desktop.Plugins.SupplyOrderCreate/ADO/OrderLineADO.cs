using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.SupplyOrderCreate.ADO
{
    /// <summary>
    /// One line of a supply order (HIS_SUPPLY_ORDER_DETAIL) + balance F/O/R/N of the medi stock
    /// </summary>
    class OrderLineADO
    {
        public int STT { get; set; }
        public string KEY { get; set; }
        public long TYPE_ID { get; set; }
        public bool IS_MATERIAL { get; set; }
        public string TYPE_CODE { get; set; }
        public string TYPE_NAME { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string IMP_UNIT_NAME { get; set; }
        public string PACKING { get; set; }
        public string KIND_NAME { get; set; }
        public List<long> SUPPLIER_IDS { get; set; }
        public long? SUPPLIER_ID { get; set; }

        /// <summary>
        /// Group caption of the preview by supplier
        /// </summary>
        public string SUPPLIER_NAME { get; set; }

        /// <summary>
        /// Code of the draft order holding this line (empty = not saved yet)
        /// </summary>
        public string SUPPLY_ORDER_CODE { get; set; }

        /// <summary>
        /// k: amount in service unit = amount in import unit x k
        /// </summary>
        public decimal? IMP_UNIT_CONVERT_RATIO { get; set; }

        /// <summary>
        /// SL (import unit)
        /// </summary>
        public decimal? IMP_UNIT_AMOUNT { get; set; }

        /// <summary>
        /// SL le (service unit) - the standard amount
        /// </summary>
        public decimal? AMOUNT { get; set; }

        public decimal? IMP_PRICE { get; set; }

        /// <summary>
        /// Ratio as stored in DB (0.1 = 10%)
        /// </summary>
        public decimal? IMP_VAT_RATIO { get; set; }

        /// <summary>
        /// F: approved anticipate amount
        /// </summary>
        public decimal? APPROVED_AMOUNT { get; set; }

        /// <summary>
        /// O: ordered amount (confirmed orders)
        /// </summary>
        public decimal? ORDERED_AMOUNT { get; set; }

        /// <summary>
        /// R: remain = F - O, may be negative
        /// </summary>
        public decimal? REMAIN_AMOUNT { get; set; }

        /// <summary>
        /// N: arrived with invoice, not yet imported
        /// </summary>
        public decimal? NEW_AMOUNT { get; set; }

        public decimal? ARRIVED_AMOUNT { get; set; }
        public decimal? IMPORTED_AMOUNT { get; set; }

        public bool HasRatio
        {
            get { return this.IMP_UNIT_CONVERT_RATIO.HasValue && this.IMP_UNIT_CONVERT_RATIO.Value > 0; }
        }

        public decimal? VAT_PERCENT
        {
            get { return this.IMP_VAT_RATIO.HasValue ? (decimal?)(this.IMP_VAT_RATIO.Value * 100) : null; }
        }

        public decimal TOTAL_PRICE
        {
            get { return Math.Round((this.AMOUNT ?? 0) * (this.IMP_PRICE ?? 0), 4); }
        }

        public decimal VAT_PRICE
        {
            get { return Math.Round(this.TOTAL_PRICE * (this.IMP_VAT_RATIO ?? 0), 4); }
        }

        public decimal TOTAL_PRICE_WITH_VAT
        {
            get { return this.TOTAL_PRICE + this.VAT_PRICE; }
        }

        public decimal? WAIT_AMOUNT
        {
            get
            {
                if (!this.AMOUNT.HasValue)
                {
                    return null;
                }
                decimal wait = this.AMOUNT.Value - (this.ARRIVED_AMOUNT ?? 0) - (this.IMPORTED_AMOUNT ?? 0);
                return wait > 0 ? wait : 0;
            }
        }

        public bool IsOverRemain
        {
            get { return this.AMOUNT.HasValue && this.REMAIN_AMOUNT.HasValue && this.AMOUNT.Value > this.REMAIN_AMOUNT.Value; }
        }
    }
}

using MOS.SDO;
using System;

namespace HIS.Desktop.Plugins.SupplyOrderList.ADO
{
    class OrderDetailADO : HisSupplyOrderDetailSDO
    {
        public int STT { get; set; }
        public string PACKING { get; set; }

        public decimal? VAT_PERCENT
        {
            get { return this.IMP_VAT_RATIO.HasValue ? (decimal?)(this.IMP_VAT_RATIO.Value * 100) : null; }
        }

        public decimal TOTAL_PRICE
        {
            get { return Math.Round(this.AMOUNT * (this.IMP_PRICE ?? 0), 4); }
        }

        public decimal VAT_PRICE
        {
            get { return Math.Round(this.TOTAL_PRICE * (this.IMP_VAT_RATIO ?? 0), 4); }
        }

        /// <summary>
        /// Ordered minus arrived (request/approved imp mest) minus imported; ARRIVED_AMOUNT does not include imported
        /// </summary>
        public decimal WAIT_AMOUNT
        {
            get
            {
                decimal wait = this.AMOUNT - this.ARRIVED_AMOUNT - this.IMPORTED_AMOUNT;
                return wait > 0 ? wait : 0;
            }
        }

        public OrderDetailADO()
        {
        }

        public OrderDetailADO(HisSupplyOrderDetailSDO data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<OrderDetailADO>(this, data);
                this.PACKING = !String.IsNullOrWhiteSpace(data.PACKING_TYPE_NAME) ? data.PACKING_TYPE_NAME : data.CONCENTRA;
            }
        }
    }
}

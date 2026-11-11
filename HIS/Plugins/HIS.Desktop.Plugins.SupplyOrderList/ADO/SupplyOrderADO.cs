using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.SupplyOrderList.ADO
{
    class SupplyOrderADO : V_HIS_SUPPLY_ORDER
    {
        public int STT { get; set; }
        public string ORDER_TIME_DISPLAY { get; set; }
        public string STATUS_DISPLAY { get; set; }
        public string MAIL_STATUS_DISPLAY { get; set; }
        public string LAST_MAIL_TIME_DISPLAY { get; set; }

        /// <summary>
        /// Sums of the order lines, in service unit
        /// </summary>
        public decimal SUM_AMOUNT { get; set; }
        public decimal SUM_ARRIVED_AMOUNT { get; set; }
        public decimal SUM_IMPORTED_AMOUNT { get; set; }

        public decimal TOTAL_WITH_VAT
        {
            get { return (this.TOTAL_PRICE ?? 0) + (this.TOTAL_VAT_PRICE ?? 0); }
        }

        public SupplyOrderADO()
        {
        }

        public SupplyOrderADO(V_HIS_SUPPLY_ORDER data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<SupplyOrderADO>(this, data);
            }
        }
    }
}

using MOS.EFMODEL.DataModels;
using System;
namespace HIS.UC.ServiceRoom.ADO
{
    public class ServiceRoomWithPriceADO
    {
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public decimal? PRICE { get; set; }
        public string PRICE_DISPLAY { get; set; }
        public ServiceRoomWithPriceADO() { }
        public ServiceRoomWithPriceADO(V_HIS_SERVICE_ROOM serviceRoom, decimal? price)
        {
            try
            {
                if (serviceRoom != null)
                {
                    this.SERVICE_ID = serviceRoom.SERVICE_ID;
                    this.SERVICE_CODE = serviceRoom.SERVICE_CODE;
                    this.SERVICE_NAME = serviceRoom.SERVICE_NAME;
                    this.PRICE = price;
                    this.PRICE_DISPLAY = price.HasValue ? string.Format("{0:#,##0}", price.Value) : "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
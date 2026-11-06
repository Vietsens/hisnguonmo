using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.SupplyAnticipateList.ADO
{
    class SupplyAnticipateADO : V_HIS_ANTICIPATE
    {
        public int STT { get; set; }
        public string SUPPLY_PERIOD_DISPLAY { get; set; }
        public string CREATE_TIME_DISPLAY { get; set; }
        public string SUPPLY_STT_DISPLAY { get; set; }
        public string SUPPLY_APPROVE_TIME_DISPLAY { get; set; }

        public SupplyAnticipateADO(V_HIS_ANTICIPATE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SupplyAnticipateADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

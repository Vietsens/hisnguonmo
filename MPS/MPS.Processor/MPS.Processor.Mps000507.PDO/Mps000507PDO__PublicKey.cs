using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000507.PDO
{
    public class SereServADO : V_HIS_SERE_SERV
    {
        public string CONCLUDE { get; set; }
        public string DISPLAY_VALUE { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? FUEX_TYPE_ID { get; set; }
        public long? DIIM_TYPE_ID { get; set; }
        public long? TEST_TYPE_ID { get; set; }

        public SereServADO() { }

        public SereServADO(V_HIS_SERE_SERV data, HIS_SERVICE Service, HIS_SERE_SERV_EXT SereSErvExt)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);

                    if (Service != null)
                    {
                        this.NUM_ORDER = Service.NUM_ORDER;
                        this.FUEX_TYPE_ID = Service.FUEX_TYPE_ID;
                        this.DIIM_TYPE_ID = Service.DIIM_TYPE_ID;
                        this.TEST_TYPE_ID = Service.TEST_TYPE_ID;
                    }

                    if (SereSErvExt != null)
                    {
                        this.CONCLUDE = SereSErvExt.CONCLUDE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, HIS_SERVICE Service)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);

                    if (Service != null)
                    {
                        this.NUM_ORDER = Service.NUM_ORDER;
                        this.FUEX_TYPE_ID = Service.FUEX_TYPE_ID;
                        this.DIIM_TYPE_ID = Service.DIIM_TYPE_ID;
                        this.TEST_TYPE_ID = Service.TEST_TYPE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SereServTeinADO : V_HIS_SERE_SERV_TEIN
    {
        public short? IS_IMPORTANT { get; set; }

        public SereServTeinADO() { }

        public SereServTeinADO(V_HIS_SERE_SERV_TEIN data, V_HIS_TEST_INDEX TestIndex)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServTeinADO>(this, data);

                    if (TestIndex != null)
                    {
                        this.IS_IMPORTANT = TestIndex.IS_IMPORTANT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.ADO
{
    public class ServiceReqADO : V_HIS_SERVICE_REQ_2
    {
        public bool IsChecked { get; set; }

        public HIS_KSK_GENERAL KSK_GENERAL { get; set; }

        public HIS_KSK_OCCUPATIONAL KSK_OCCUPATIONAL { get; set; }

        public ServiceReqADO()
        {

        }

        public ServiceReqADO(V_HIS_SERVICE_REQ_2 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(this, data);

                    var serviceReqStt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == data.SERVICE_REQ_STT_ID);
                    if (serviceReqStt != null)
                    {
                        this.SERVICE_REQ_STT_CODE = serviceReqStt.SERVICE_REQ_STT_CODE;
                        this.SERVICE_REQ_STT_NAME = serviceReqStt.SERVICE_REQ_STT_NAME;
                    }
                    var serviceReqType = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == data.SERVICE_REQ_TYPE_ID);
                    if (serviceReqType != null)
                    {
                        this.SERVICE_REQ_TYPE_CODE = serviceReqType.SERVICE_REQ_TYPE_CODE;
                        this.SERVICE_REQ_TYPE_NAME = serviceReqType.SERVICE_REQ_TYPE_NAME;
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

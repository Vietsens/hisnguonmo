using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SamplePathologyReq.ADO
{
    public class ServiceReqADO : MOS.EFMODEL.DataModels.HIS_SERVICE_REQ
    {
        public string ErrorMessageBlock { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeBlock { get; set; }
        public string ErrorMessageTDN { get; set; }
        public DevExpress.XtraEditors.DXErrorProvider.ErrorType ErrorTypeTDN { get; set; }
        public ServiceReqADO() { }
        public ServiceReqADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

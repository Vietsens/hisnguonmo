using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.ADO
{
    public class SereServADO : DHisSereServ2
    {

        public bool IsEnableEdit { get; set; }
        public bool IsEnableDelete { get; set; }
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public string NOTE_ADO { get; set; }
        public string AMOUNT_SER { get; set; }
        public long SERVICE_REQ_TYPE_ID { get; set; }
        public long child { get; set; }
        public long REQUEST_DEPARTMENT_ID { get; set; }
        public long EXECUTE_DEPARTMENT_ID { get; set; }
        //ServiceReq
        public long? SAMPLE_TIME { get; set; }
        public long? RECEIVE_SAMPLE_TIME { get; set; }
        public short? IS_TEMPORARY_PRES { get; set; }

        public SereServADO() { }

        public SereServADO(DHisSereServ2 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);
                    this.PARENT_ID__IN_SETY = data.SERVICE_REQ_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

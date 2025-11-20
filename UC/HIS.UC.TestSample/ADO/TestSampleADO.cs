using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TestSample.ADO
{
    public class TestSampleADO : MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE
    {
        public TestSampleADO() { }
        public TestSampleADO(MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<TestSampleADO>(this, data);
            }
        }

        public bool check1 { get; set; }
        public bool isKeyChoose { get; set; }
        public bool radio1 { get; set; }
    }
}

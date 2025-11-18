using HIS.UC.TestSample.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TestSample.Run
{
    internal class RunBehavior : IRun
    {
        TestSampleInitADO entity;
        public RunBehavior()
            : base()
        {
        }

        public RunBehavior(CommonParam param, TestSampleInitADO data)
            : base()
        {
            this.entity = data;
        }

        object IRun.Run()
        {
            try
            {
                return new UCTestSample(this.entity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

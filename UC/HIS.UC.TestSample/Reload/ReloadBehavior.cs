using HIS.UC.TestSample.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TestSample.Reload
{
    internal class ReloadBehavior : IReload
    {
        UserControl control;
        List<TestSampleADO> entity;
        public ReloadBehavior()
            : base()
        {
        }

        public ReloadBehavior(CommonParam param, UserControl uc, List<TestSampleADO> data)
            : base()
        {
            this.control = uc;
            this.entity = data;
        }

        void IReload.Run()
        {
            try
            {
                ((UCTestSample)this.control).Reload(entity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

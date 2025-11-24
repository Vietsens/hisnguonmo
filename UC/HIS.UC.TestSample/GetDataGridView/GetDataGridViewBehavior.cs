using HIS.UC.TestSample.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TestSample.GetDataGridView
{
    public sealed class GetDataGridViewBehavior : IGetDataGridView
    {
        UserControl control;
        List<TestSampleADO> entity;
        public GetDataGridViewBehavior()
            : base()
        {
        }

        public GetDataGridViewBehavior(CommonParam param, UserControl uc)
            : base()
        {
            this.control = uc;
        }

        object IGetDataGridView.Run()
        {
            try
            {
                return ((UCTestSample)this.control).GetDataGridView();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

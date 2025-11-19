using HIS.UC.TestSample.ADO;
using HIS.UC.TestSample.GetDataGridView;
using HIS.UC.TestSample.Reload;
using HIS.UC.TestSample.Run;
using HIS.UC.TestSample.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TestSample
{
    public class UCTestSampleProcessor : BussinessBase
    {
        object uc;
        public UCTestSampleProcessor()
            : base()
        {
        }

        public UCTestSampleProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(TestSampleInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeITestSampleGrid(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, List<TestSampleADO> data)    
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object GetDataGridView(UserControl control)
        {
            object result = null;
            try
            {
                IGetDataGridView behavior = GetDataGridViewFactory.MakeIGetDataGridView(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

using Inventec.UC.Loading.Init;
using Inventec.UC.Loading.Set.Delegate.SetDelegateBWDoWorker;
using Inventec.UC.Loading.Set.Delegate.SetDelegateBWRunWorkerCompleted;
using Inventec.UC.Loading.Set.SetReportProgressChanged;
using Inventec.UC.Loading.Start.StartBackgroundWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Loading
{
    public partial class MainLoginLoading
    {
        public static string TEMPLATE1 = "Template1";


        public UserControl Init(string Template)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool SetDelegateDoWorker(UserControl UC, BWDoWorker DoWorker)
        {
            bool result = false;
            try
            {
                result = SetDelegateBWDoWorkerFactory.MakeISetDelegateBWDoWorker().SetDelegateDoWorker(UC, DoWorker);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool SetDelegateRunWorkerCompleted(UserControl UC, BWRunWorkerCompleted RunCompleted)
        {
            bool result = false;
            try
            {
                result = SetDelegateBWRunWorkerCompletedFactory.MakeISetDelegateBWRunWorkerCompleted().SetDelegateRunCompleted(UC, RunCompleted);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void StartRunWorker(UserControl UC)
        {
            try
            {
                StartBackgroundWorkerFactory.MakeIStartBackgroundWorker().StartRunWorkerAsync(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetReportProgress(UserControl UC, int i)
        {
            try
            {
                SetReportProgressChangedFactory.MakeISetReportProgressChanged().SetReportProgress(UC, i);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Start.StartBackgroundWorker
{
    class StartBackgroundWorker : IStartBackgroundWorker
    {
        public void StartRunWorkerAsync(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLoading = (Design.Template1.Template1)UC;
                    UCLoading.StartRunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

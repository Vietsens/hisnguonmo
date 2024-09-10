using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Set.SetReportProgressChanged
{
    class SetReportProgressChanged : ISetReportProgressChanged
    {

        public void SetReportProgress(System.Windows.Forms.UserControl UC, int i)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCLoading = (Design.Template1.Template1)UC;
                    UCLoading.ReportProgress(i);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

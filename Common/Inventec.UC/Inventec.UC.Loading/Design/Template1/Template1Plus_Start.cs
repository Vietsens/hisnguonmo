using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Design.Template1
{
    internal partial class Template1
    {
        internal void StartRunWorkerAsync()
        {
            try
            {
                if (bw.IsBusy == false)
                {
                    bw.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.ListEmrDocumentScaned.ListEmrDocumentScaned
{
    class ListEmrDocumentSanedBehavior : Tool<IDesktopToolContext>, IListEmrDocumentScaned
    {
        Inventec.Desktop.Common.Modules.Module Module;

        string treatmentCode = "";
        internal ListEmrDocumentSanedBehavior()
        : base()
        {

        }
        internal ListEmrDocumentSanedBehavior(Inventec.Desktop.Common.Modules.Module moduleData, CommonParam param, string tmCode) : base()
        {
            treatmentCode = tmCode;
            Module = moduleData;
        }
        object IListEmrDocumentScaned.Run()
        {
            object result = null;
            try
            {
                result = new frmListEmrDocumentScaned(Module, treatmentCode);
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}

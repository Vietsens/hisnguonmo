using Inventec.Core;
using Inventec.Desktop.Common.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTransReqList.HisTransReqList
{
    class TransReqBehavior : Tool<IDesktopToolContext>, ITransReq
    {
        Inventec.Desktop.Common.Modules.Module Module;
        V_HIS_TREATMENT_FEE treatment = null;
        long dataID = 0;
        internal TransReqBehavior()
            : base()
        {

        }
        //internal TransReqBehavior(Inventec.Desktop.Common.Modules.Module moduleData, CommonParam param) : base()
        //{
        //    Module = moduleData;
        //}

        internal TransReqBehavior(Inventec.Desktop.Common.Modules.Module moduleData, CommonParam param,  long treatmentID) : base()
        {
            dataID = treatmentID;
            Module = moduleData;
        }
        object ITransReq.Run()
        {
            object result = null;
            try
            {
                result = new frmTransReqList(Module, dataID);
                //else
                //{
                //    result = new frmTransReqList(Module);
                //}
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

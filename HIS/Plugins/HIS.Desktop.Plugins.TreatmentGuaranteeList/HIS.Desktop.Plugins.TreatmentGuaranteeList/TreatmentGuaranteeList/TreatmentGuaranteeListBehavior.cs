using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentGuaranteeList.TreatmentGuaranteeList
{
    class TreatmentGuaranteeListBehavior : Tool<IDesktopToolContext>, ITreatmentGuaranteeList
    {
        Inventec.Desktop.Common.Modules.Module Module;
        //V_HIS_TREATMENT_FEE_1 treatment = null;
        internal TreatmentGuaranteeListBehavior() : base()
        {

        }
        internal TreatmentGuaranteeListBehavior(Inventec.Desktop.Common.Modules.Module moduleData, CommonParam param) : base()
        {
            //this.treatment = treatmentFee;
            Module = moduleData;
        }
        object ITreatmentGuaranteeList.Run()
        {
            object result = null;
            try
            {
                result = new UCTreatmentGuaranteeList(Module);
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

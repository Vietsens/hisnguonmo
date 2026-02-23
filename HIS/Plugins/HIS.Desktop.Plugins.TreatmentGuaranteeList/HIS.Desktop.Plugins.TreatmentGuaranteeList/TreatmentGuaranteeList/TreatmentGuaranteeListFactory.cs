using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentGuaranteeList.TreatmentGuaranteeList
{
    class TreatmentGuaranteeListFactory
    {
        internal static ITreatmentGuaranteeList MakeIControl(CommonParam commonParam, object[] data)
        {
            ITreatmentGuaranteeList result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            //V_HIS_TREATMENT_FEE_1 treatmentFee= null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            //else if (data[i] is V_HIS_TREATMENT_FEE_1)
                            //{
                            //    treatmentFee = (V_HIS_TREATMENT_FEE_1)data[i];
                            //}
                        }

                        if (moduleData != null)
                        {
                            result = new TreatmentGuaranteeListBehavior(moduleData, commonParam);
                        }
                    }
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
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

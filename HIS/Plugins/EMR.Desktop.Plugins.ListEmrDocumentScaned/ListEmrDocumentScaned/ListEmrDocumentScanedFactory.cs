using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.ListEmrDocumentScaned.ListEmrDocumentScaned
{
    class ListEmrDocumentScanedFactory
    {
        internal static IListEmrDocumentScaned MakeIControl(CommonParam commonParam, object[] data)
        {
            IListEmrDocumentScaned result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            //V_HIS_TREATMENT_FEE hisTreatment = null;
            string treatmentCode = "";
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            //if (data[i] is V_HIS_TREATMENT_FEE)
                            //{
                            //    hisTreatment = (V_HIS_TREATMENT_FEE)data[i];
                            //}
                            if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is string)
                            {
                                treatmentCode = (string)data[i];
                            }
                        }

                        if (moduleData != null)
                        {
                            result = new ListEmrDocumentSanedBehavior(moduleData, commonParam, treatmentCode);
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

using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Common;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KskServiceEditList.KskServiceEditList
{
    /// <summary>
    /// Args: Module (bắt buộc), List&lt;V_HIS_TREATMENT_4&gt; hồ sơ đã tick (bắt buộc),
    /// V_HIS_KSK_CONTRACT hợp đồng đang lọc (bắt buộc), RefeshReference (tùy chọn).
    /// </summary>
    class KskServiceEditListBehavior : BusinessBase, IKskServiceEditList
    {
        object[] entity;

        internal KskServiceEditListBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IKskServiceEditList.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                List<V_HIS_TREATMENT_4> treatments = null;
                V_HIS_KSK_CONTRACT kskContract = null;
                RefeshReference refeshReference = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        else if (entity[i] is List<V_HIS_TREATMENT_4>)
                            treatments = (List<V_HIS_TREATMENT_4>)entity[i];
                        else if (entity[i] is V_HIS_KSK_CONTRACT)
                            kskContract = (V_HIS_KSK_CONTRACT)entity[i];
                        else if (entity[i] is RefeshReference)
                            refeshReference = (RefeshReference)entity[i];
                    }
                }

                if (moduleData != null && treatments != null && treatments.Any() && kskContract != null)
                {
                    result = new frmKskServiceEditList(moduleData, treatments, kskContract, refeshReference);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("KskServiceEditList thieu tham so dau vao."
                        + Inventec.Common.Logging.LogUtil.TraceData("TreatmentCount", treatments != null ? treatments.Count : 0)
                        + Inventec.Common.Logging.LogUtil.TraceData("HasContract", kskContract != null));
                }
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

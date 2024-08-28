using Inventec.Common.WordContent.Base;
using Inventec.Common.WordContent.Sar.SarPrintType.Get.List;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Sar.SarPrintType
{
    partial class SarPrintTypeGet : BeanObjectBase, IDelegacyT
    {
        object entity;

        internal SarPrintTypeGet(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        T IDelegacyT.Execute<T>()
        {
            T result = default(T);
            try
            {
                if (typeof(T) == typeof(List<SAR_PRINT_TYPE>))
                {
                    ISarPrintTypeGetList behavior = SarPrintTypeBehaviorFactory.MakeIGetList(param, entity);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}

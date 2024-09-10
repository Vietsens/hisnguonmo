using Inventec.Common.Adapter;
using Inventec.Common.WordContent.Base;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Sar.SarPrintType.Get.List
{
    class SarPrintTypeBehavior : BusinessLogicBase, ISarPrintTypeGetList
    {
        SarPrintTypeFilter entity;
        internal SarPrintTypeBehavior(CommonParam param, SarPrintTypeFilter filter)
            : base()
        {
            this.entity = filter;
        }

        List<SAR_PRINT_TYPE> ISarPrintTypeGetList.Run()
        {
            try
            {
                return new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>(RequestUriStore.SAR_PRINT_TYPE_GET, WordContentStorage.SarConsumer, entity, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

    }
}

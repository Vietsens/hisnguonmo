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

namespace Inventec.Common.WordContent.SarPrint.Get.List
{
    class SarPrintBehavior : BusinessLogicBase, ISarPrintGetList
    {
        SarPrintFilter entity;
        internal SarPrintBehavior(CommonParam param, SarPrintFilter filter)
            : base()
        {
            this.entity = filter;
        }

        List<SAR_PRINT> ISarPrintGetList.Run()
        {
            try
            {
                return new BackendAdapter(param).Get<List<SAR_PRINT>>(RequestUriStore.SAR_PRINT_GET, WordContentStorage.SarConsumer, entity, param);
                //return new SarPrintDAL(param).GetRequest<List<SAR_PRINT>>(RequestUriStore.SAR_PRINT_GET, ApiConsumerStore.SarConsumer, entity, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

    }
}

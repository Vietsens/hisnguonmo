using Inventec.Common.Adapter;
using Inventec.Common.WordContent.Base;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace Inventec.Common.WordContent.SarPrint.Update
{
    class SarPrintUpdateBehavior : BeanObjectBase, ISarPrintUpdate
    {
        SAR_PRINT entity;

        internal SarPrintUpdateBehavior(CommonParam param, SAR_PRINT data)
            : base(param)
        {
            entity = data;
        }

        SAR_PRINT ISarPrintUpdate.Run()
        {
            SAR_PRINT result = null;
            try
            {
                result = new BackendAdapter(param).Post<SAR_PRINT>(RequestUriStore.SAR_PRINT_UPDATE, WordContentStorage.SarConsumer, entity, param);
                //result = new SarPrintDAL(param).PostRequest<SAR_PRINT>(RequestUriStore.SAR_PRINT_UPDATE, ApiConsumerStore.SarConsumer, entity, param);
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

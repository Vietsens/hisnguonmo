using Inventec.Common.Adapter;
using Inventec.Common.WordContent.Base;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace Inventec.Common.WordContent.SarPrint.Create
{
    class SarPrintCreateBehavior : BeanObjectBase, ISarPrintCreate
    {
        SAR_PRINT entity;

        internal SarPrintCreateBehavior(CommonParam param, SAR_PRINT data)
            : base(param)
        {
            entity = data;
        }

        SAR_PRINT ISarPrintCreate.Run()
        {
            SAR_PRINT result = null;
            try
            {
                result = new BackendAdapter(param).Post<SAR_PRINT>(RequestUriStore.SAR_PRINT_CREATE, WordContentStorage.SarConsumer, entity, param);
                //result = new SarPrintDAL(param).PostRequest<SAR_PRINT>(RequestUriStore.SAR_PRINT_CREATE, ApiConsumerStore.SarConsumer, entity, param);
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

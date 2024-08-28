using Inventec.Common.WordContent.Base;
using Inventec.Common.WordContent.SarPrint.Create;
using Inventec.Core;
using System;

namespace Inventec.Common.WordContent.SarPrint
{
    partial class SarPrintCreate : BeanObjectBase, IDelegacy
    {
        object entity;

        internal SarPrintCreate(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        object IDelegacy.Execute()
        {
            object result = null;
            try
            {

                ISarPrintCreate behavior = SarPrintCreateBehaviorFactory.MakeIAggrExpMestCreate(param, entity);
                result = behavior != null ? behavior.Run() : null;
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

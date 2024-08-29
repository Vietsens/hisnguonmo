using Inventec.Common.WordContent.Base;
using Inventec.Common.WordContent.SarPrint.Update;
using Inventec.Core;
using System;

namespace Inventec.Common.WordContent.SarPrint
{
    partial class SarPrintUpdate : BeanObjectBase, IDelegacy
    {
        object entity;

        internal SarPrintUpdate(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        object IDelegacy.Execute()
        {
            object result = null;
            try
            {

                ISarPrintUpdate behavior = SarPrintUpdateBehaviorFactory.MakeIAggrExpMestUpdate(param, entity);
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

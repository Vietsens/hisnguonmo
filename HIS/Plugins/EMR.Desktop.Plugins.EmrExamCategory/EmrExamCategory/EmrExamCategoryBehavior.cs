using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using System;
using System.Linq;

namespace EMR.Desktop.Plugins.EmrExamCategory.EmrExamCategory
{
    class EmrExamCategoryBehavior : HIS.Desktop.Common.BusinessBase, IEmrExamCategory
    {
        object[] entity;

        internal EmrExamCategoryBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IEmrExamCategory.Run()
        {
            try
            {
                Module moduleData = null;
                DelegateSelectData delegateSelect = null;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Module)
                            moduleData = (Module)entity[i];
                        if (entity[i] is DelegateSelectData)
                            delegateSelect = (DelegateSelectData)entity[i];
                    }
                }

                if (delegateSelect != null)
                    return new EmrExamCategoryForm(moduleData, delegateSelect);
                else
                    return new EmrExamCategoryForm(moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

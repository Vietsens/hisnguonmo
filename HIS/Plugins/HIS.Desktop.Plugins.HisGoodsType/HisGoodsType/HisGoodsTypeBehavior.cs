/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.Common;
using Inventec.Core;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.HisGoodsType
{
    class HisGoodsTypeBehavior : BusinessBase, IHisGoodsType
    {
        object[] entity;
        RefeshReference delegateRefresh;

        internal HisGoodsTypeBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisGoodsType.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is RefeshReference)
                        {
                            delegateRefresh = (RefeshReference)entity[i];
                        }
                    }
                }

                if (moduleData != null && delegateRefresh != null)
                {
                    result = new frmHisGoodsType(moduleData, delegateRefresh);
                }
                else
                {
                    result = new frmHisGoodsType(moduleData);
                }
                return result;
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

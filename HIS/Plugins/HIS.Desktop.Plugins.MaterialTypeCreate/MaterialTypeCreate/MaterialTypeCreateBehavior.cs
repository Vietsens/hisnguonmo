/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    class MaterialTypeCreateBehavior : BusinessBase, IMaterialTypeCreate
    {
        object[] entity;
        internal MaterialTypeCreateBehavior(CommonParam param, object[] data)
            : base()
        {
            try
            {
                entity = data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        object IMaterialTypeCreate.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long? currentMaterialTypeIdDefault = null;
                int actionType = 0;
                DelegateSelectData delegateSelect = null;

                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            if (entity[i] is DelegateSelectData)
                            {
                                delegateSelect = (DelegateSelectData)entity[i];
                            }
                            if (entity[i] is long)
                            {
                                currentMaterialTypeIdDefault = (long?)entity[i];
                            }
                            if (entity[i] is int)
                            {
                                actionType = (int)entity[i];
                            }
                        }
                    }
                }
                return new frmMaterialTypeCreate(moduleData, currentMaterialTypeIdDefault, actionType, delegateSelect);
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

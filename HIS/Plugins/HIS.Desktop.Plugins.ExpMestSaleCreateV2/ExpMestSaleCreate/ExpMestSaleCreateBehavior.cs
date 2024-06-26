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
using HIS.Desktop.Controls.Session;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleCreateV2.ExpMestSaleCreateV2
{
    class ExpMestSaleCreateV2Behavior : Tool<IDesktopToolContext>, IExpMestSaleCreateV2
    {
        object[] entity;

        internal ExpMestSaleCreateV2Behavior()
            : base()
        {

        }

        internal ExpMestSaleCreateV2Behavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IExpMestSaleCreateV2.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long? expMestId = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            expMestId = (long)entity[i];
                        }
                    }
                }
                if (moduleData != null)
                {
                    UserControl uc = new UCExpMestSaleCreateV2(moduleData.RoomTypeId, moduleData.RoomId, expMestId);
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), "XUAT_BAN", "Xuất bán", uc, moduleData, ReleaseBeforeClose);
                    return true;

                    //return new UCExpMestSaleCreateV2(moduleData.RoomTypeId, moduleData.RoomId);
                    //return new frmExpMestSaleCreateV2(moduleData.RoomTypeId, moduleData.RoomId, expMestId);
                }
                //if (moduleData != null && expMestId.HasValue)
                //{
                //    return new FormExpMestSaleCreateV2(moduleData, expMestId.Value);
                //}
                //else if (moduleData != null)
                //{
                //    return new UCExpMestSaleCreateV2(moduleData);
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void ReleaseBeforeClose(object uc)
        {
            try
            {
                MethodInfo methodInfo = (uc as UserControl).GetType().GetMethod("FromClosingEvent");
                methodInfo.Invoke(uc, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

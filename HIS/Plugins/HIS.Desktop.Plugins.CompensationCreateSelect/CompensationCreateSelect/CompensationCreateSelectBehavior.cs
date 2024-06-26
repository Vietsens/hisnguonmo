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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CompensationCreateSelect.CompensationCreateSelect
{
    class CompensationCreateSelectBehavior : Tool<IDesktopToolContext>, ICompensationCreateSelect
    {
        object[] entity;
        object extenceInstance = null;
        internal CompensationCreateSelectBehavior()
            : base()
        {

        }

        internal CompensationCreateSelectBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object ICompensationCreateSelect.Run()
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
                    }
                }
                if (moduleData != null)
                {
                    V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == moduleData.RoomId);
                    if (stock == null || stock.IS_CABINET != 1)
                    {
                        MessageBox.Show("Bạn đang không làm việc tại tủ trực", "Thông báo");
                        return null;
                    }
                    if (stock.CABINET_MANAGE_OPTION == IMSys.DbConfig.HIS_RS.HIS_MEDI_STOCK.CABINET_MANAGE_OPTION__PRES_DETAIL)
                    {
                        Inventec.Desktop.Common.Modules.Module module = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.BaseCompensationCreate").FirstOrDefault();
                        if (module == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.BaseCompensationCreate");
                        if (module.IsPlugin && module.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(module, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("extenceInstance is null");
                            return extenceInstance;
                        }
                        else
                        {
                            LogSystem.Info("HIS.Desktop.Plugins.BaseCompensationCreate module.IsPlugin");
                        }
                    }
                    else if (stock.CABINET_MANAGE_OPTION == IMSys.DbConfig.HIS_RS.HIS_MEDI_STOCK.CABINET_MANAGE_OPTION__BASE)
                    {
                        Inventec.Desktop.Common.Modules.Module module = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CompensationByBaseCreate").FirstOrDefault();
                        if (module == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.CompensationByBaseCreate");
                        if (module.IsPlugin && module.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(module, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("extenceInstance is null");
                            return extenceInstance;
                        }
                        else
                        {
                            LogSystem.Info("HIS.Desktop.Plugins.CompensationByBaseCreate module.IsPlugin");
                        }
                    }
                    else
                    {
                        Inventec.Desktop.Common.Modules.Module module = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExpMestBCSCreate").FirstOrDefault();
                        if (module == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ExpMestBCSCreate");
                        if (module.IsPlugin && module.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(module, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("extenceInstance is null");
                            return extenceInstance;
                        }
                        else
                        {
                            LogSystem.Info("HIS.Desktop.Plugins.ExpMestBCSCreate module.IsPlugin");
                        }
                    }
                }
                else
                    return null;
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

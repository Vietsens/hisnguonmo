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
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.ModuleExt;

namespace HIS.Desktop.Plugins.MenuAll.MenuAll
{
    class MenuAllBehavitor : BusinessBase, IMenuAll
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module Module;
        //ImageCollection imageCollection;
        //List<Inventec.Desktop.Common.Modules.Module> modules;
        MenuAllADO menuAllADO;

        internal MenuAllBehavitor()
            : base()
        {
        }

        public MenuAllBehavitor(CommonParam param, object[] data)
            : base()
        {
            this.entity = data;
            //this.Module = module;
            //entity = data;
        }

        object IMenuAll.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            Module = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        
                        else if (item is MenuAllADO)
                        {
                            menuAllADO = (MenuAllADO)item;
                        }
                    }

                    if (Module != null && menuAllADO != null)
                    {
                        result = new FormMenuAll(Module, menuAllADO);                        
                    }
                }
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

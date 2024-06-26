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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.HisArea.HisArea
{
    public sealed class HisAreaBehavior:Tool<IDesktopToolContext>,IHisArea
    {
        object[] entity;

        public HisAreaBehavior() 
            :base() 
        { 
        
        }

        public HisAreaBehavior(CommonParam param, object[] filter) 
            :base() 
        {
            this.entity = filter;
        }

        object IHisArea.Run() {
            try { 
                Inventec.Desktop.Common.Modules.Module moduleData = null;

                if ( entity.GetType() == typeof(object[]) ){
                    if (entity != null && entity.Count() > 0) { 
                        for(int i = 0; i < entity.Count(); i++){
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                        }
                    }
                }

                return new frmHisArea(moduleData);
            }
            catch (Exception ex) {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}

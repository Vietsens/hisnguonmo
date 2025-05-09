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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.LocalCacheManager;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.LocalCacheManager.LocalCacheManager
{
    public sealed class LocalCacheManagerBehavior : Tool<IDesktopToolContext>, ILocalCacheManager
    {              
        Inventec.Desktop.Common.Modules.Module moduleData;

        public LocalCacheManagerBehavior()
            : base()
        {
        }

        public LocalCacheManagerBehavior(CommonParam param,  Inventec.Desktop.Common.Modules.Module moduleData)
            : base()
        {         
            this.moduleData = moduleData;
        }

        object ILocalCacheManager.Run()
        {
            try
            {
                return new frmLocalCacheManager(this.moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

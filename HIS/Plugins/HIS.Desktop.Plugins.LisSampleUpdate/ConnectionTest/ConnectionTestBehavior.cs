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
using LIS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.LisSampleUpdate.ConnectionTest
{
    class ConnectionTestBehavior : Tool<IDesktopToolContext>, IConnectionTest
    {
        object[] entity;        
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal ConnectionTestBehavior()
            : base()
        {

        }

        internal ConnectionTestBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IConnectionTest.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    V_LIS_SAMPLE currentSample = null;

                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is V_LIS_SAMPLE)
                        {
                            currentSample = (V_LIS_SAMPLE)item;
                        }
                    }

                    if (currentModule != null && currentSample != null)
                    {
                        result = new frmUpdateLisSample(currentModule, currentSample);
                    }
                    else if (currentModule != null)
                    {
                        result = new frmUpdateLisSample(currentModule, null);
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

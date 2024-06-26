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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.ConfirmPresBlood;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ConfirmPresBlood.ConfirmPresBlood
{
    public sealed class ConfirmPresBloodBehavior : Tool<IDesktopToolContext>, IConfirmPresBlood
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long expMestId = 0;
        public ConfirmPresBloodBehavior()
            : base()
        {
        }

        public ConfirmPresBloodBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IConfirmPresBlood.Run()
        {
            object result = null;
            DelegateSelectData delegateSelectData = null;
            MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2 expMest = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            expMestId = (long)item;
                        }
                        else if (item is DelegateSelectData)
                        {
                            delegateSelectData = (DelegateSelectData)item;
                        }
                        else if (item is MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)
                        {
                            expMest = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_2)item;
                        }
                    }
                    if (currentModule != null && expMestId > 0)
                    {
                        result = new frmConfirmPresBlood(currentModule, expMestId, delegateSelectData);
                    }
                    else if (currentModule != null && expMest != null)
                    {
                        result = new frmConfirmPresBlood(currentModule, expMest, delegateSelectData);
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

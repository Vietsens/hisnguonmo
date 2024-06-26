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
using HIS.Desktop.Plugins.HisTranPatiToInfo;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.HisTranPatiToInfo.HisTranPatiToInfo
{
    public sealed class HisTranPatiToInfoBehavior : Tool<IDesktopToolContext>, IHisTranPatiToInfo
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        public HisTranPatiToInfoBehavior()
            : base()
        {
        }

        public HisTranPatiToInfoBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisTranPatiToInfo.Run()
        {
            object result = null;
            try
            {
                HIS.Desktop.Common.DelegateRefreshData _dlgRef = null;
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
                            treatmentId = (long)item;
                        }
                        else if (item is DelegateRefreshData)
                        {
                            _dlgRef = (DelegateRefreshData)item;
                        }
                    }
                    if (currentModule != null && treatmentId > 0)
                    {
                        if (_dlgRef != null)
                        {
                            result = new frmHisTranPatiToInfo(currentModule, treatmentId, _dlgRef);
                        }
                        else
                            result = new frmHisTranPatiToInfo(currentModule, treatmentId);
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

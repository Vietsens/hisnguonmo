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
using HIS.Desktop.Plugins.HisSuggestedPayment;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.Plugins.HisSuggestedPayment.Run;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisSuggestedPayment.Run
{
    public sealed class HisSuggestedPaymentBehavior : Tool<IDesktopToolContext>, IHisSuggestedPayment
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public HisSuggestedPaymentBehavior()
            : base()
        {
        }

        public HisSuggestedPaymentBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisSuggestedPayment.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    V_HIS_IMP_MEST_PROPOSE _Edit = new V_HIS_IMP_MEST_PROPOSE();
                    int action = -1;
                    DelegateRefreshData _ref = null;
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is V_HIS_IMP_MEST_PROPOSE)
                        {
                            _Edit = (V_HIS_IMP_MEST_PROPOSE)item;
                        }
                        else if (item is int)
                        {
                            action = (int)item;
                        }
                        else if (item is DelegateRefreshData)
                        {
                            _ref = (DelegateRefreshData)item;
                        }
                    }
                    if (currentModule != null && _Edit != null && action > 0)
                    {
                        result = new frmHisSuggestedPayment(currentModule, _Edit, action);
                    }
                    else if (currentModule != null && _ref != null)
                    {
                        result = new frmHisSuggestedPayment(currentModule, _ref);
                    }
                    else if (currentModule != null)
                    {
                        result = new frmHisSuggestedPayment(currentModule);
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

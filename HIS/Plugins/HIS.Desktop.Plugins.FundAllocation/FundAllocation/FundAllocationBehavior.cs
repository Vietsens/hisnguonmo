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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.FundAllocation.FundAllocation
{
    /// <summary>
    /// Args: Module, TREATMENT_ID (long) or a treatment entity carrying the ID, RefeshReference (optional)
    /// </summary>
    class FundAllocationBehavior : Tool<IDesktopToolContext>, IFundAllocation
    {
        object[] entity;
        internal FundAllocationBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IFundAllocation.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long treatmentId = 0;
            HIS.Desktop.Common.RefeshReference refreshRef = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                        else if (item is HIS_TREATMENT)
                        {
                            treatmentId = ((HIS_TREATMENT)item).ID;
                        }
                        else if (item is V_HIS_TREATMENT)
                        {
                            treatmentId = ((V_HIS_TREATMENT)item).ID;
                        }
                        else if (item is HIS.Desktop.Common.RefeshReference)
                        {
                            refreshRef = (HIS.Desktop.Common.RefeshReference)item;
                        }
                    }
                }

                if (moduleData != null && treatmentId > 0)
                {
                    return new frmFundAllocation(moduleData, treatmentId, refreshRef);
                }
                Inventec.Common.Logging.LogSystem.Warn("FundAllocation: thieu Module hoac TREATMENT_ID");
                return null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}

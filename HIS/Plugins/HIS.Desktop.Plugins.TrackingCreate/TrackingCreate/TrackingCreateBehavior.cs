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
using HIS.Desktop.Plugins.TrackingCreate;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace Inventec.Desktop.Plugins.TrackingCreate.TrackingCreate
{
    public sealed class TrackingCreateBehavior : Tool<IDesktopToolContext>, ITrackingCreate
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_TRACKING currentTracking;
        HIS_DHST currentDhst;
        long treatmentId = 0;
        Action<HIS_TRACKING> actCallBack;
        HisTreatmentBedRoomLViewFilter dataTransferTreatmentBedRoomFilter;

        public TrackingCreateBehavior()
            : base()
        {
        }

        public TrackingCreateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ITrackingCreate.Run()
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
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        if (item is HIS_TRACKING)
                        {
                            currentTracking = (HIS_TRACKING)item;
                        }
                        else if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                        else if (item is HIS_DHST)
                        {
                            currentDhst = (HIS_DHST)item;
                        }
                        else if (item is Action<HIS_TRACKING>)
                        {
                            actCallBack = (Action<HIS_TRACKING>)item;
                        }
                        else if (item is HisTreatmentBedRoomLViewFilter)
                        {
                            dataTransferTreatmentBedRoomFilter = (HisTreatmentBedRoomLViewFilter)item;
                        }
                    }
                    if (currentModule != null && treatmentId > 0 && currentDhst != null)
                    {
                        result = new frmTrackingCreateNew(currentModule, treatmentId, currentDhst, actCallBack, dataTransferTreatmentBedRoomFilter);
                    }
                    else if (currentModule != null && treatmentId > 0)
                    {
                        result = new frmTrackingCreateNew(currentModule, treatmentId, actCallBack, dataTransferTreatmentBedRoomFilter);
                    }
                    else if (currentModule != null && currentTracking != null)
                    {
                        result = new frmTrackingCreateNew(currentModule, currentTracking, actCallBack, dataTransferTreatmentBedRoomFilter);
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

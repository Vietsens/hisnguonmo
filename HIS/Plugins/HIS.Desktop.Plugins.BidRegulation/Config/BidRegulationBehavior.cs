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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BidRegulation.Config
{
    class BidRegulationBehavior : Inventec.Desktop.Common.Core.BusinessBase, IBidRegulationStore
    {
        object[] entity;
        
        internal BidRegulationBehavior(CommonParam param, object[] filter)
                : base()
        {
            this.entity = filter;
        }

        object IBidRegulationStore.Run()
            {
                try
                {
                    MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE dataMaterial = null;
                    MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE dataMedicine = null;
                    Inventec.Desktop.Common.Modules.Module moduleData = null;
                    DelegateSelectData delegateData = null;
                    if (entity.GetType() == typeof(object[]))
                    {
                        if (entity != null && entity.Count() > 0)
                        {
                            for (int i = 0; i < entity.Count(); i++)
                            {
                                if (entity[i] is MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE)
                                    dataMaterial = (MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE)entity[i];
                                if (entity[i] is MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE)
                                    dataMedicine = (MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE)entity[i];
                                if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                                {
                                    moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                                }
                                if (entity[i] is DelegateSelectData)
                                {
                                    if (entity[i] != null) delegateData = (DelegateSelectData)entity[i];
                                }
                            }
                        }
                    }

                    return new frmBidRegulation.frmBidRegulation(moduleData,dataMaterial,dataMedicine,delegateData);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    param.HasException = true;
                    return null;
                }
            }
        }
}

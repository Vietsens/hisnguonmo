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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EpidemiologyInfo.EpidemiologyInfo
{
    class EpidemiologyInfoBehavior : Tool<IDesktopToolContext>, IEpidemiologyInfo
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        long treatmentId = 0;
        HIS_TREATMENT treatment;
        internal EpidemiologyInfoBehavior()
            : base()
        {

        }
        internal EpidemiologyInfoBehavior(CommonParam param, object[] filter)
            : base()
        {
            entity = filter;
        }
        object IEpidemiologyInfo.Run()
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
                            this.moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        } if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                    }

                    if (moduleData != null && treatmentId > 0)
                    {
                        HisTreatmentFilter filter = new HisTreatmentFilter();
                        filter.ID = treatmentId;
                        treatment = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                    }

                    result = new frmEpidemiologyInfo(moduleData, treatment);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
            return result;
        }
    }
}

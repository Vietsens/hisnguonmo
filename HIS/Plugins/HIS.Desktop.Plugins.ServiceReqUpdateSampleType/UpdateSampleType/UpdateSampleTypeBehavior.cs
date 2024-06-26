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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ServiceReqUpdateSampleType.UpdateSampleType
{
    class UpdateSampleTypeBehavior : BusinessBase, IUpdateSampleType
    {
        object[] entity;
        internal UpdateSampleTypeBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IUpdateSampleType.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_SERVICE_REQ req = null;
                HIS.Desktop.Common.RefeshReference refreshClick = null;
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is HIS_SERVICE_REQ)
                            {
                                req = (HIS_SERVICE_REQ)entity[i];
                            }
                            else if (entity[i] is HIS.Desktop.Common.RefeshReference)
                            {
                                refreshClick = (HIS.Desktop.Common.RefeshReference)entity[i];
                            }
                        }
                    }
                }

                return new UpdateSampleTypeFrom(moduleData, req, refreshClick);
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

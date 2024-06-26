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
using HIS.Desktop.Plugins.Camera.ADO;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.Camera.Camera
{
    public sealed class CameraBehavior : Tool<IDesktopToolContext>, ICamera
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        CameraADO cameraADO;
        HIS_SERVICE_REQ serviceReq;
        DelegateSelectData delegateSelectData;
        DelegateRefreshData delegateRefreshData;
        int cameraType = 0;
        public CameraBehavior()
            : base()
        {
        }

        public CameraBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ICamera.Run()
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
                        if (item is CameraADO)
                        {
                            this.cameraADO = (CameraADO)item;
                        }
                        if (item is HIS_SERVICE_REQ)
                        {
                            this.serviceReq = (HIS_SERVICE_REQ)item;
                        }
                        if (item is DelegateSelectData)
                        {
                            this.delegateSelectData = (DelegateSelectData)item;
                        }
                        if (item is DelegateRefreshData)
                        {
                            this.delegateRefreshData = (DelegateRefreshData)item;
                        }
                        if (item is string)
                        {
                            this.cameraType = (string)item == "1" ? 1 : 0;
                        }
                    }
                    if (this.cameraType > 0)
                    {
                        result = new frmCamera(currentModule, this.cameraADO, this.delegateSelectData, this.delegateRefreshData, this.serviceReq, this.cameraType);
                    }
                    else
                    {
                        result = new frmCamera(currentModule, this.cameraADO, this.delegateSelectData, this.delegateRefreshData, this.serviceReq);
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

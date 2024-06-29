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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreeSereServ7V2.ReloadByServiceReq
{
    public sealed class ReloadByServiceReqBehavior : IReloadByServiceReq
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs;
        List<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ> serviceReqList;
        List<SereServADO> sereServAdos;
        public ReloadByServiceReqBehavior()
            : base()
        {
        }

        public ReloadByServiceReqBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs, List<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ> _serviceReqList)
            : base()
        {
            this.control = data;
            this.sereServs = sereServs;
            this.serviceReqList = _serviceReqList;
        }

        public ReloadByServiceReqBehavior(CommonParam param, UserControl data, List<SereServADO> _sereServAdos, List<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ> _serviceReqList)
            : base()
        {
            this.control = data;
            this.sereServAdos = _sereServAdos;
            this.serviceReqList = _serviceReqList;
        }

        void IReloadByServiceReq.Run()
        {
            try
            {
                if (sereServs != null && sereServs.Count() > 0)
                    ((HIS.UC.TreeSereServ7V2.Run.UCTreeSereServ7V2)this.control).Reload(this.sereServs, this.serviceReqList);
                else if (sereServAdos != null)
                {
                    ((HIS.UC.TreeSereServ7V2.Run.UCTreeSereServ7V2)this.control).Reload(this.sereServAdos, this.serviceReqList);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

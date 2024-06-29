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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreeSereServ7.Reload
{
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs;
        List<DHisSereServ2> sereServsNew;
        List<SereServADO> sereServsAdo;
        public ReloadBehavior()
            : base()
        {
        }
        public ReloadBehavior(CommonParam param, UserControl data, List<SereServADO> sereServsAdo)
            : base()
        {
            this.control = data;
            this.sereServsAdo = sereServsAdo;
        }
        public ReloadBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs)
            : base()
        {
            this.control = data;
            this.sereServs = sereServs;
        }
        public ReloadBehavior(CommonParam param, UserControl data, List<DHisSereServ2> _sereServsNew)
            : base()
        {
            this.control = data;
            this.sereServsNew = _sereServsNew;
        }

        void IReload.Run()
        {
            try
            {
                if (sereServsAdo != null && sereServsAdo.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("3");
                    ((HIS.UC.TreeSereServ7.Run.UCTreeSereServ7)this.control).Reload(sereServsAdo);
                }
                else if (sereServsNew != null && sereServsNew.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("1");
                    ((HIS.UC.TreeSereServ7.Run.UCTreeSereServ7)this.control).Reload(sereServsNew);
                }
                else 
                {
                    Inventec.Common.Logging.LogSystem.Debug("2");
                    ((HIS.UC.TreeSereServ7.Run.UCTreeSereServ7)this.control).Reload(sereServs);
                }
                                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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

namespace HIS.UC.SereServTree.Reload
{
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT> sereServDeposits;
        public ReloadBehavior()
            : base()
        {
        }

        public ReloadBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs)
            : base()
        {
            this.control = data;
            this.sereServs = sereServs;
        }
        public ReloadBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT> sereServDeposits)
            : base()
        {
            this.control = data;
            this.sereServDeposits = sereServDeposits;
        }

        void IReload.Run()
        {
            try
            {
                if (sereServs != null)
                {
                    ((HIS.UC.SereServTree.Run.UCSereServTree)this.control).Reload(sereServs);
                }
                else if (sereServDeposits != null)
                {
                    ((HIS.UC.SereServTree.Run.UCSereServTree)this.control).Reload(sereServDeposits);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

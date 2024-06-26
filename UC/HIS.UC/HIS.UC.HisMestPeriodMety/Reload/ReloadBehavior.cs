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

namespace HIS.UC.HisMestPeriodMety.Reload
{
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_METY> HisMestPeriodMetys;
        public ReloadBehavior()
            : base()
        {
        }

        public ReloadBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_METY> HisMestPeriodMetys)
            : base()
        {
            this.control = data;
            this.HisMestPeriodMetys = HisMestPeriodMetys;
        }

        void IReload.Run()
        {
            try
            {
                ((HIS.UC.HisMestPeriodMety.Run.UCHisMestPeriodMety)this.control).Reload(HisMestPeriodMetys);                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using TYT.Desktop.Plugins.FinancePeriod;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.FinancePeriod.TYTFinancePeriod
{
    public sealed class TYTFinancePeriodUpdateBehavior : Tool<IDesktopToolContext>, ITYTFinancePeriod
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        long FinancePeriodId;
        DelegateSelectData refeshData;

        public TYTFinancePeriodUpdateBehavior()
            : base()
        {
        }

        public TYTFinancePeriodUpdateBehavior(CommonParam param,long FinancePeriodId, DelegateSelectData refeshData, Inventec.Desktop.Common.Modules.Module moduleData)
            : base()
        {
            this.moduleData = moduleData;
            this.FinancePeriodId = FinancePeriodId;
            this.refeshData = refeshData;
        }

        object ITYTFinancePeriod.Run()
        {
            try
            {
                return new frmTYTFinancePeriod(this.moduleData, FinancePeriodId, refeshData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}

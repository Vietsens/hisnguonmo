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
using HIS.Desktop.Plugins.ReportCreate;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;

namespace Inventec.Desktop.Plugins.ReportCreate.ReportCreate
{
    public sealed class ReportCreateBehavior : Tool<IDesktopToolContext>, IReportCreate
    {
        object entity;
        public ReportCreateBehavior()
            : base()
        {
        }

        public ReportCreateBehavior(CommonParam param, object filter)
            : base()
        {
            this.entity = filter;
        }

        object IReportCreate.Run()
        {
            try
            {
                return new frmMainReport();
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

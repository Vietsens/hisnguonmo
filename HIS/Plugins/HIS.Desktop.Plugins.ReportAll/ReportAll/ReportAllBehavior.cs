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
using HIS.Desktop.Plugins.ReportAll;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.ReportAll.ReportAll
{
    public sealed class ReportAllBehavior : Tool<IDesktopToolContext>, IReportAll
    {
        object[] entity;
        public ReportAllBehavior()
            : base()
        {
        }

        public ReportAllBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IReportAll.Run()
        {
            try
            {
              Inventec.UC.ListReportType.CreateReport_Click createReportClick = null;
              Inventec.Desktop.Common.Modules.Module moduleData = null;
              if (entity != null && entity.Count() > 0)
              {
                for (int i = 0; i < entity.Count(); i++)
                {
                  if (entity[i] is Inventec.UC.ListReportType.CreateReport_Click)
                  {
                    createReportClick = (Inventec.UC.ListReportType.CreateReport_Click)entity[i];
                  }
                  if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                  {
                      moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                  }
                }
              }
              return new UCReportAll(createReportClick, moduleData);
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

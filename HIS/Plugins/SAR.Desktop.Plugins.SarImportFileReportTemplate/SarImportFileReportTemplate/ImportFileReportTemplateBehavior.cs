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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core.Tools;
using Inventec.Desktop.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core.Actions;
using Inventec.Core;

namespace SAR.Desktop.Plugins.SarImportFileReportTemplate.SarImportFileReportTemplate
{
    class ImportFileReportTemplateBehavior:Tool<IDesktopToolContext>, IImportFileReportTemplate
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        RefeshReference delegateRefresh;
        public ImportFileReportTemplateBehavior()
            : base()
        {
        }

        public ImportFileReportTemplateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IImportFileReportTemplate.Run()
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
                        else if (item is RefeshReference)
                        {
                            delegateRefresh = (RefeshReference)item;
                        }
                    }
                    if (currentModule != null && delegateRefresh != null)
                    {
                        result = new frmSarImportFileReportTemplate(currentModule, delegateRefresh);
                    }
                    else
                    {
                        result = new frmSarImportFileReportTemplate(currentModule);
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

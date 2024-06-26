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
using HIS.Desktop.Plugins.ExpMestAggrExam;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggrExam.ExpMestAggrExam
{
    public sealed class ExpMestAggrExamBehavior : Tool<IDesktopToolContext>, IExpMestAggrExam
    {
        object[] entity;
        public ExpMestAggrExamBehavior()
            : base()
        {
        }

        public ExpMestAggrExamBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IExpMestAggrExam.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 treatment = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)
                        {
                            treatment = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4)entity[i];
                        }
                    }
                }
                if (moduleData != null)
                {
                    return new UCExpMestAggrExam(moduleData, treatment);
                }
                else
                {
                    throw new ArgumentNullException("moduleData is null");
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

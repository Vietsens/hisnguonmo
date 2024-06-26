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
using HIS.Desktop.Plugins.HisTuberclusisTreatment;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisTuberclusisTreatment.Run
{
    public sealed class TuberclusisTreatmentBehavior : Tool<IDesktopToolContext>, ITuberclusisTreatment
    {
        object[] entity;
        public TuberclusisTreatmentBehavior()
            : base()
        {
        }

        public TuberclusisTreatmentBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ITuberclusisTreatment.Run()
        {
            object result = null;
            DelegateSelectData refeshReference = null;
            long treatmentId = 0;
            Inventec.Desktop.Common.Modules.Module currentModule = null;
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] is long)
                        {
                            treatmentId = (long)entity[i];
                        }
                        else if (entity[i] is DelegateSelectData)
                        {
                            refeshReference = (DelegateSelectData)entity[i];
                        }
                        else if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                    }
                    if (treatmentId != 0)
                    {
                        result = new frmTuberclusisTreatment(currentModule, treatmentId, refeshReference);
                    }
                }
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));
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

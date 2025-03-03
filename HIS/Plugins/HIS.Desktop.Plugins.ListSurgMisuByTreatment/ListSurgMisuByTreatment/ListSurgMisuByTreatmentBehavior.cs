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
using HIS.Desktop.Plugins.ListSurgMisuByTreatment;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ListSurgMisuByTreatment.ListSurgMisuByTreatment
{
    public sealed class ListSurgMisuByTreatmentBehavior : Tool<IDesktopToolContext>, IListSurgMisuByTreatment
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        long patientTypeId;
        HIS.Desktop.Common.DelegateLoadPTTT loadPTTT;

        public ListSurgMisuByTreatmentBehavior()
            : base()
        {
        }

        public ListSurgMisuByTreatmentBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IListSurgMisuByTreatment.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                        else if (item is string)
                        {
                            patientTypeId = Convert.ToInt64(item);
                        }
                        else if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is HIS.Desktop.Common.DelegateLoadPTTT)
                        {
                            loadPTTT = (HIS.Desktop.Common.DelegateLoadPTTT)item;
                        }
                    }
                        if (currentModule != null && treatmentId > 0 && patientTypeId > 0)
                        {
                            result = new HIS.Desktop.Plugins.ListSurgMisuByTreatment.Run.frmListSurgMisuByTreatment(currentModule, treatmentId,patientTypeId);
                            //break;
                        }
                        else if (currentModule != null && treatmentId > 0 && loadPTTT != null)
                        {
                            result = new HIS.Desktop.Plugins.ListSurgMisuByTreatment.Run.frmListSurgMisuByTreatment(currentModule, treatmentId, loadPTTT);
                            //break;
                        }
                        else if (currentModule != null && treatmentId > 0)
                        {
                            result = new HIS.Desktop.Plugins.ListSurgMisuByTreatment.Run.frmListSurgMisuByTreatment(currentModule, treatmentId);
                            //break;
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

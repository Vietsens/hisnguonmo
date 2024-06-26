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
using HIS.Desktop.Plugins.PublicServices_NT;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.PublicServices_NT.PublicServices_NT
{
    public sealed class PublicServices_NTBehavior : Tool<IDesktopToolContext>, IPublicServices_NT
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        L_HIS_TREATMENT_BED_ROOM currentTreatment;
        bool isShowPatientList = false;
        public PublicServices_NTBehavior()
            : base()
        {
        }

        public PublicServices_NTBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPublicServices_NT.Run()
        {
            object result = null;
            try
            {
                V_HIS_TREATMENT_4 _treatment4 = null;
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is L_HIS_TREATMENT_BED_ROOM)
                        {
                            currentTreatment = (L_HIS_TREATMENT_BED_ROOM)item;
                        }
                        else if (item is V_HIS_TREATMENT_4)
                        {
                            _treatment4 = (V_HIS_TREATMENT_4)item;
                        }
                        else if (item is bool)
                        {
                            isShowPatientList = (bool)item;
                        }
                    }
                    if (currentModule != null && currentTreatment != null)
                    {
                        result = new frmPublicServices_NT(currentModule, currentTreatment, isShowPatientList);
                    }
                    else if (currentModule != null && _treatment4 != null)
                    {
                        result = new frmPublicServices_NT(currentModule, _treatment4);
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

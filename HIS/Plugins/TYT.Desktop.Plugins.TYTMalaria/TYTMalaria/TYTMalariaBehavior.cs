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
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using TYT.EFMODEL.DataModels;
using TYT.Desktop.Plugins.TYTMalaria;

namespace TYT.Desktop.Plugins.TYTMalaria.TYTMalaria
{
    class TYTMalariaBehavior : Tool<IDesktopToolContext>, ITYTMalaria
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal TYTMalariaBehavior()
            : base()
        {

        }

        internal TYTMalariaBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object ITYTMalaria.Run()
        {
            object result = null;
            try
            {
                V_HIS_PATIENT Patient = new V_HIS_PATIENT();
                TYT_MALARIA _TYT_FETUS_EXAM = new TYT_MALARIA();
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is TYT_MALARIA)
                        {
                            _TYT_FETUS_EXAM = (TYT_MALARIA)item;
                        }
                        else if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is V_HIS_PATIENT)
                        {
                            Patient = (V_HIS_PATIENT)item;
                        }
                    }

                    if (currentModule != null && _TYT_FETUS_EXAM != null && _TYT_FETUS_EXAM.ID > 0)
                    {
                        result = new frmMalaria(currentModule, _TYT_FETUS_EXAM);
                    }
                    else if (currentModule != null && Patient != null)
                    {
                        result = new frmMalaria(currentModule, Patient);
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

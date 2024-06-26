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
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.EventLog
{
    class EventLogBehavior : BusinessBase, IEventLog
    {
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        object[] entity;
        KeyCodeADO ado = null;


        //string treatmentCode;
        //string patientCode;
        //string serviceRequestCode;
        //string impMestCode;
        //string expMestCode;
        internal EventLogBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IEventLog.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is KeyCodeADO)
                        {
                            ado = (KeyCodeADO)entity[i];

                        }
                    }
                }
                if (moduleData != null && ado != null && (!String.IsNullOrEmpty(ado.expMestCode) || !String.IsNullOrEmpty(ado.impMestCode) || !String.IsNullOrEmpty(ado.patientCode) || !String.IsNullOrEmpty(ado.serviceRequestCode) || !String.IsNullOrEmpty(ado.treatmentCode) || !String.IsNullOrEmpty(ado.bidNumber)))
                {
                    return new frmEventLog(ado);
                }
                else if (moduleData != null)
                {
                    return new UCEventLog();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("EventLog NOT contructor");
                    return null;
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

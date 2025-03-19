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

namespace Inventec.Desktop.Plugins.EventLog
{
    class EventLogBehavior : BusinessBase, IEventLog
    {
        object[] entity;
        string treatmentCode;
        string patientCode;
        string serviceRequestCode;
        string impMestCode;
        string expMestCode;
        Inventec.Desktop.Common.Modules.Module moduleData;
        internal EventLogBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IEventLog.Run()
        {
            try
            {
                if (entity == null || entity.Count() <= 2)
                    throw new NullReferenceException("Du lieu truyen vao khong du tham so. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));

                Inventec.UC.EventLogControl.ProcessHasException exceptionApi = null;
                for (int i = 0; i < entity.Count(); i++)
                {
                    if (entity[i] is Inventec.Common.WebApiClient.ApiConsumer)
                    {
                        EventLogConfig.SdaConsumer = (Inventec.Common.WebApiClient.ApiConsumer)entity[i];
                    }
                    else if (entity[i] is long)
                    {
                        EventLogConfig.NumPageSize = (long)(entity[1] == null ? 100 : entity[i]);
                    }
                    else if (entity[i] is Inventec.UC.EventLogControl.ProcessHasException)
                    {
                        exceptionApi = (Inventec.UC.EventLogControl.ProcessHasException)entity[i];
                    }
                    else if (entity[i] is string)
                    {
                        if (treatmentCode != null) 
                        {
                            treatmentCode = (string)entity[i];
                        }
                        else if (patientCode != null)
                        {
                            patientCode = (string)entity[i];
                        }
                        else if (serviceRequestCode != null)
                        {
                            serviceRequestCode = (string)entity[i];
                        }
                        else if (impMestCode != null)
                        {
                            impMestCode = (string)entity[i];
                        }
                        else if (expMestCode != null)
                        {
                            expMestCode = (string)entity[i];
                        }

                    }
                    if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                    {
                        moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                    }
                }
                if (exceptionApi != null && ( treatmentCode != null || patientCode!=null||serviceRequestCode!=null||impMestCode!=null||expMestCode!=null))
                {
                    return new frmEventLog(exceptionApi, treatmentCode, patientCode, serviceRequestCode, impMestCode, expMestCode, moduleData);
                }
                else if (exceptionApi != null)
                {
                    return new UCEventLog(exceptionApi, moduleData);
                }
                else
                {
                    return null;
                }
               

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}

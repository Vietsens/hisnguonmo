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
using HIS.Desktop.ADO;
using HIS.Desktop.Common;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYT.Desktop.Plugins.Nerves.TYTNerves
{
    class TYTNervesFactory
    {
        internal static ITYTNerves MakeITYTNerves(CommonParam param, object[] data)
        {
            ITYTNerves result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            V_HIS_PATIENT patient = null;
            long? nervesId = null;
            DelegateSelectData refeshData = null;
            TYT.EFMODEL.DataModels.TYT_NERVES tytNerverADO = null;

            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is V_HIS_PATIENT)
                            {
                                patient = data[i] as V_HIS_PATIENT;
                            }
                            else if (data[i] is long)
                            {
                                nervesId = (long)data[i];
                            }
                            else if (data[i] is DelegateSelectData)
                            {
                                refeshData = (DelegateSelectData)data[i];
                            }
                            else if (data[i] is TYT.EFMODEL.DataModels.TYT_NERVES)
                            {
                                tytNerverADO = (TYT.EFMODEL.DataModels.TYT_NERVES)data[i];
                            }
                        }
                        if (nervesId > 0)
                        {
                            result = new TYTNervesUpdateBehavior(param, nervesId ?? 0, refeshData, moduleData);
                        }
                        else if (patient != null)
                        {
                            result = new TYTNervesBehavior(param, patient, moduleData);
                        }
                        else if (tytNerverADO != null)
                        {
                            result = new TYTNervesUpdateBehavior(param, tytNerverADO, refeshData, moduleData);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("Don't exist contructor");
                        }
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
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

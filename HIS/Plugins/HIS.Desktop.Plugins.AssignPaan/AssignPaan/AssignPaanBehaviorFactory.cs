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

namespace HIS.Desktop.Plugins.AssignPaan.AssignPaan
{
    class AssignPaanBehaviorFactory
    {
        internal static IAssignPaan MakeIAssignPaan(CommonParam param, object[] data)
        {
            IAssignPaan result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long? hisTreatmentId = null;
            long? serviceReqId = null;
            HIS.UC.Icd.ADO.IcdInputADO icdAdo = null;
            HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo = null;
            MOS.EFMODEL.DataModels.HIS_TRACKING hisTracking = null;
            HIS.Desktop.Common.RefeshReference delegateActionSave = null;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("DATA_INPUT", data));
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 _sereServ = new MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5();
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is long)
                            {
                                if (hisTreatmentId == null)
                                    hisTreatmentId = (long)data[i];
                                else if (serviceReqId == null || serviceReqId <= 0)
                                    serviceReqId = (long)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)
                            {
                                _sereServ = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5)data[i];
                            }
                            else if (data[i] is HIS.UC.Icd.ADO.IcdInputADO)
                            {
                                icdAdo = (HIS.UC.Icd.ADO.IcdInputADO)data[i];
                            }
                            else if (data[i] is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                            {
                                secondIcdAdo = (HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)data[i];
                            }
                            else if (data[i] is MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ && data[i] != null)
                            {
                                serviceReqId = ((MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)data[i]).ID;
                            }
                            else if (data[i] is MOS.EFMODEL.DataModels.HIS_TRACKING)
                            {
                                hisTracking = ((MOS.EFMODEL.DataModels.HIS_TRACKING)data[i]);
                            }
                            else if (data[i] is HIS.Desktop.Common.RefeshReference)
                            {
                                delegateActionSave = ((HIS.Desktop.Common.RefeshReference)data[i]);
                            }
                        }

                        if (moduleData != null && hisTreatmentId.HasValue)
                        {
                            result = new AssignPaanBehavior(moduleData, param, hisTreatmentId.Value, icdAdo, secondIcdAdo, serviceReqId ?? 0, hisTracking, delegateActionSave);
                        }
                        else if (moduleData != null && _sereServ != null)
                        {
                            result = new AssignPaanBehavior(moduleData, param, _sereServ, icdAdo, secondIcdAdo, serviceReqId ?? 0, hisTracking, delegateActionSave);
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

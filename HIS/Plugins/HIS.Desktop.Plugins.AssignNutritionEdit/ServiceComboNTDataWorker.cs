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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignNutritionEdit.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionEdit
{
    public class ServiceComboNTDataWorker
    {
        public static Dictionary<long, ServiceComboNTADO> DicServiceCombo { get; set; }

        public static ServiceComboNTADO GetByPatientType(long patientTypeId)
        {
            return GetByPatientType(patientTypeId, null);
        }

        public static ServiceComboNTADO GetByPatientType(long patientTypeId, Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs,HIS_SERVICE_REQ serviceReq = null)
        {
            try
            {
                ServiceComboNTADO serviceComboADO = new ServiceComboNTADO();

                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>().ToList();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().ToList();
                var pt = patientTypes.FirstOrDefault(o => o.ID == patientTypeId);
                if (patientTypeAllows != null && patientTypeAllows.Count() > 0 && patientTypes != null)
                {
                    long[] serviceTypeIdAllows = new long[1]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN};

                    var listGroup = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o =>
                                    o.IS_ACTIVE == GlobalVariables.CommonNumberTrue
                                   && serviceTypeIdAllows.Contains(o.ID)).ToList().OrderByDescending(o => o.NUM_ORDER).ThenBy(m => m.SERVICE_TYPE_NAME).ToList();
                    LogSystem.Debug("BindTree => Loaded listGroup");

                    var serviceAllADOs = (
                                    from m in BackendDataWorker.Get<V_HIS_SERVICE>()
                                    where
                                     serviceTypeIdAllows.Contains(m.SERVICE_TYPE_ID)
                                     && m.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                    select new ServiceADO(m)
                                    ).Distinct()
                                    .OrderByDescending(o => o.NUM_ORDER)
                                    .ThenBy(o => o.SERVICE_NAME)
                                    .ToList();

                    var roomIdAccepts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => (o.IS_PAUSE == null || o.IS_PAUSE != 1)).Select(o => o.ID).ToArray();

                    serviceComboADO.ServiceRooms = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>()
                                   .Where(o => o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                                     && roomIdAccepts.Contains(o.ROOM_ID)
                                     && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                     && o.ROOM_ID == serviceReq.EXECUTE_ROOM_ID
                                     && serviceTypeIdAllows.Contains(o.SERVICE_TYPE_ID)).ToList();

                    var serviceHasServiceRoomIds = serviceComboADO.ServiceRooms
                                    .GroupBy(o => o.SERVICE_ID)
                                    .ToDictionary(o => o.Key, o => o.ToList());

                    serviceComboADO.ServiceRati = BackendDataWorker.Get<HIS_SERVICE_RATI>()
                                  .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                    && o.RATION_TIME_ID == serviceReq.RATION_TIME_ID).ToList();
                    var serviceHasServiceRati = serviceComboADO.ServiceRati
                                 .GroupBy(o => o.SERVICE_ID)
                                 .ToDictionary(o => o.Key, o => o.ToList());

                    LogSystem.Debug("BindTree => Loaded serviceHasServiceRoomIds");

                    var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == pt.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToArray();
                    if (patientTypeAllow != null && patientTypeAllow.Count() > 0)
                    {
                        var currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                        if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                        {
                            serviceComboADO.PatientTypeId = pt.ID;

                            serviceComboADO.ServiceIsleafADOs = new List<SSServiceADO>();
                            serviceComboADO.ServiceAllADOs = new List<ServiceADO>();
                            serviceComboADO.ServiceAllADOs.AddRange(serviceAllADOs);
                            LogSystem.Debug("BindTree => Loaded ServiceAllADOs");

                            serviceComboADO.ServiceParentADOs = serviceComboADO.ServiceAllADOs
                                .Where(m => (m.IS_LEAF ?? 0) != 1 && CheckExistsParent(m))
                                .ToList();

                            LogSystem.Debug("BindTree => Loaded ServiceParentADOs");
                            var patientTypeIdAllows = currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).Distinct().ToArray();
                            if (servicePatyInBranchs == null || servicePatyInBranchs.Count == 0)
                            {
                                servicePatyInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                                    .Where(o =>
                                     o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                     && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                                     && serviceTypeIdAllows.Contains(o.SERVICE_TYPE_ID)
                                     && patientTypeIdAllows.Contains(o.PATIENT_TYPE_ID))
                                    .GroupBy(o => o.SERVICE_ID)
                                    .ToDictionary(o => o.Key, o => o.ToList());
                            }

                            LogSystem.Debug("BindTree => Loaded serviceHasServicePatyIds");
                            var dicServiceIdParents = serviceComboADO.ServiceParentADOs.ToDictionary(o => o.ID, o => o);

                            serviceComboADO.ServiceIsleafADOs = (
                                from m in serviceComboADO.ServiceAllADOs
                                where
                                    m.IS_LEAF == 1
                                    && servicePatyInBranchs != null && servicePatyInBranchs.ContainsKey(m.ID)
                                    && serviceHasServiceRoomIds != null && serviceHasServiceRoomIds.ContainsKey(m.ID)
                                    && serviceHasServiceRati !=null && serviceHasServiceRati.ContainsKey(m.ID)
                                    && (m.PARENT_ID == null || dicServiceIdParents.ContainsKey(m.PARENT_ID.Value))

                                select new SSServiceADO(m, pt, false)
                                ).Distinct()
                                .OrderByDescending(o => o.NUM_ORDER)
                                .ThenBy(o => o.SERVICE_NAME)
                                .ToList();

                            LogSystem.Debug("BindTree => Loaded ServiceIsleafADOs");
                            foreach (var gr in listGroup)
                            {
                                ServiceADO sety1 = new ServiceADO();
                                sety1.CONCRETE_ID__IN_SETY = gr.ID + ".";
                                sety1.NUM_ORDER = gr.NUM_ORDER;
                                sety1.SERVICE_CODE = gr.SERVICE_TYPE_CODE;
                                sety1.SERVICE_NAME = gr.SERVICE_TYPE_NAME.ToUpper();
                                sety1.SERVICE_TYPE_CODE = gr.SERVICE_TYPE_CODE;
                                sety1.SERVICE_TYPE_NAME = gr.SERVICE_TYPE_NAME.ToUpper();
                                sety1.SERVICE_TYPE_ID = gr.ID;
                                serviceComboADO.ServiceParentADOs.Add(sety1);
                            }
                            serviceComboADO.ServiceParentADOs = serviceComboADO.ServiceParentADOs.OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                        }
                    }
                }

                return serviceComboADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        static bool CheckExistsParent(ServiceADO serviceADO)
        {
            bool valid = true;
            try
            {
                valid = (serviceADO != null && serviceADO.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                if ((serviceADO.PARENT_ID ?? 0) > 0)
                {
                    valid = valid && ExistsParentIsActive((serviceADO.PARENT_ID ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        static bool ExistsParentIsActive(long serviceId)
        {
            bool valid = true;
            try
            {
                if (serviceId > 0)
                {
                    var services = BackendDataWorker.Get<V_HIS_SERVICE>();
                    var parent = services.FirstOrDefault(o => o.ID == serviceId && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    valid = (parent != null);
                    if (valid)
                    {
                        if ((parent.PARENT_ID ?? 0) > 0 && parent.PARENT_ID != parent.ID)
                        {
                            valid = valid && ExistsParentIsActive((parent.PARENT_ID ?? 0));
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}

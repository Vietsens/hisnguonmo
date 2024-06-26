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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo
{
    public class ServiceComboDataWorker
    {
        public static Dictionary<long, ServiceComboADO> DicServiceCombo { get; set; }

        public static ServiceComboADO GetByPatientType(long patientTypeId)
        {
            return GetByPatientType(patientTypeId, null);
        }

        public static ServiceComboADO GetByPatientType(long patientTypeId, Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs)
        {
            try
            {
                ServiceComboADO serviceComboADO = new ServiceComboADO();

                var patientTypeAllows = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var pt = patientTypes.FirstOrDefault(o => o.ID == patientTypeId);
                long branchId = BranchDataWorker.GetCurrentBranchId();

                if (patientTypeAllows != null && patientTypeAllows.Count() > 0 && patientTypes != null)
                {

                    long[] serviceTypeIdAllows = new long[12]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL
                        };

                    var listGroup = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o =>
                                    o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
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

                    var roomIdAccepts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => (o.IS_PAUSE == null || o.IS_PAUSE != GlobalVariables.CommonNumberTrue)).Select(o => o.ID).ToArray();

                    LogSystem.Debug("BindTree => Loaded serviceHasServiceRoomIds");

                    var patientTypeAllow = patientTypeAllows.Where(o => o.PATIENT_TYPE_ID == pt.ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToArray();
                    if (patientTypeAllow != null && patientTypeAllow.Count() > 0)
                    {
                        var currentPatientTypeWithPatientTypeAlter = patientTypes.Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                        if (currentPatientTypeWithPatientTypeAlter != null && currentPatientTypeWithPatientTypeAlter.Count > 0)
                        {
                            serviceComboADO.PatientTypeId = pt.ID;

                            serviceComboADO.ServiceIsleafADOs = new List<SereServADO>();
                            serviceComboADO.ServiceAllADOs = new List<ServiceADO>();
                            serviceComboADO.ServiceAllADOs.AddRange(serviceAllADOs);
                            LogSystem.Debug("BindTree => Loaded ServiceAllADOs");

                            serviceComboADO.ServiceParentADOs = serviceComboADO.ServiceAllADOs
                                .Where(m => (m.IS_LEAF ?? 0) != 1 && CheckExistsParent(m))
                                .ToList();

                            LogSystem.Debug("BindTree => Loaded ServiceParentADOs");
                            if (servicePatyInBranchs == null || servicePatyInBranchs.Count == 0)
                            {
                                var patientTypeIdAllows = currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).Distinct().ToArray();
                                servicePatyInBranchs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                                    .Where(o =>
                                         o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                         && o.BRANCH_ID == branchId
                                         && serviceTypeIdAllows.Contains(o.SERVICE_TYPE_ID)
                                         && (patientTypeIdAllows.Contains(o.PATIENT_TYPE_ID) || BranchDataWorker.CheckPatientTypeInherit(o.INHERIT_PATIENT_TYPE_IDS, patientTypeIdAllows.ToList()))
                                     )
                                    .GroupBy(o => o.SERVICE_ID)
                                    .ToDictionary(o => o.Key, o => o.ToList());
                            }

                            serviceComboADO.ServiceRooms = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>()
                                           .Where(o =>
                                               //o.BRANCH_ID == branchId
                                             servicePatyInBranchs != null && servicePatyInBranchs.ContainsKey(o.SERVICE_ID)
                                             && roomIdAccepts.Contains(o.ROOM_ID)
                                             && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                             && (o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL
                                               || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                                             && serviceTypeIdAllows.Contains(o.SERVICE_TYPE_ID)).ToList();

                            var serviceHasServiceRoomIds = serviceComboADO.ServiceRooms
                                            .GroupBy(o => o.SERVICE_ID)
                                            .ToDictionary(o => o.Key, o => o.ToList());

                            LogSystem.Debug("BindTree => Loaded serviceHasServicePatyIds");
                            var dicServiceIdParents = serviceComboADO.ServiceParentADOs.Select(o => o.ID).ToArray();

                            serviceComboADO.ServiceIsleafADOs = (
                                from m in serviceComboADO.ServiceAllADOs
                                where
                                    m.IS_LEAF == GlobalVariables.CommonNumberTrue
                                    && servicePatyInBranchs != null && servicePatyInBranchs.ContainsKey(m.ID)
                                    && serviceHasServiceRoomIds != null && serviceHasServiceRoomIds.ContainsKey(m.ID)
                                    && (m.PARENT_ID == null || dicServiceIdParents.Contains(m.PARENT_ID.Value))

                                select new SereServADO(m, pt, false, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                ).Distinct()
                                .OrderByDescending(o => o.SERVICE_NUM_ORDER)
                                .ThenBy(o => o.TDL_SERVICE_NAME)
                                .ToList();

                            //var svTTL3014 = serviceAllADOs.Where(k => k.SERVICE_CODE == "TTL3014").ToList();
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => svTTL3014), svTTL3014));

                            //var svInServiceRooms = serviceHasServiceRoomIds != null && serviceHasServiceRoomIds.ContainsKey(svTTL3014[0].ID) ? serviceHasServiceRoomIds[svTTL3014[0].ID] : null;
                            //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("svInServiceRooms: ", svInServiceRooms));

                            //var svInServicePatys = servicePatyInBranchs != null && servicePatyInBranchs.ContainsKey(svTTL3014[0].ID) ? servicePatyInBranchs[svTTL3014[0].ID] : null;
                            //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("svInServicePatys: ", svInServicePatys));

                            //var svInParents = (svTTL3014[0].PARENT_ID == null || (dicServiceIdParents != null && dicServiceIdParents.Contains(svTTL3014[0].PARENT_ID ?? 0))) ? true : false;
                            //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("svInParents: ", svInParents)
                            //    + Inventec.Common.Logging.LogUtil.TraceData("dicServiceIdParents: ", dicServiceIdParents));

                            //var svTTL3014ÀterFilter = serviceComboADO.ServiceIsleafADOs.Where(k => k.TDL_SERVICE_CODE == "TTL3014").ToList();
                            //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("services svTTL3014ÀterFilter: ", svTTL3014ÀterFilter));

                            LogSystem.Debug("BindTree => Loaded ServiceIsleafADOs");
                            foreach (var gr in listGroup)
                            {
                                if (serviceAllADOs != null && serviceAllADOs.Count > 0 && serviceAllADOs.Exists(k => k.SERVICE_TYPE_ID == gr.ID))
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

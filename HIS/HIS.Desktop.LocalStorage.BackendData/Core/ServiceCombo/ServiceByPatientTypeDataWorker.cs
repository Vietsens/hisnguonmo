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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ServiceCombo
{
    public class ServiceByPatientTypeDataWorker
    {
        public static Dictionary<long, List<ServiceADO>> dicServiceByPatientType = new Dictionary<long, List<ServiceADO>>();

        public static List<ServiceADO> GetByPatientType(long patientTypeId)
        {
            try
            {
                if (dicServiceByPatientType.ContainsKey(patientTypeId)) return dicServiceByPatientType[patientTypeId];

                List<ServiceADO> serviceAdos = new List<ServiceADO>();

                long[] serviceTypeIdAllows = new long[11]{IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                                                            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN};

                var serviceAllADOs = (
                                from m in BackendDataWorker.Get<V_HIS_SERVICE>()
                                where
                                 serviceTypeIdAllows.Contains(m.SERVICE_TYPE_ID)
                                 && m.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                 && m.IS_LEAF == 1
                                select new ServiceADO(m)
                                ).Distinct()
                                .OrderByDescending(o => o.NUM_ORDER)
                                .ThenBy(o => o.SERVICE_NAME)
                                .ToList();

                Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs = BackendDataWorker.Get<V_HIS_SERVICE_PATY>()
                      .Where(o =>
                       o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                       && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId()
                       && serviceTypeIdAllows.Contains(o.SERVICE_TYPE_ID)
                       && o.PATIENT_TYPE_ID == patientTypeId)
                      .GroupBy(o => o.SERVICE_ID)
                      .ToDictionary(o => o.Key, o => o.ToList());

                serviceAdos = serviceAllADOs != null ? serviceAllADOs.Where(o => servicePatyInBranchs.ContainsKey(o.ID)).ToList() : new List<ServiceADO>();
                dicServiceByPatientType[patientTypeId] = serviceAdos;

                return serviceAdos;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

    }
}

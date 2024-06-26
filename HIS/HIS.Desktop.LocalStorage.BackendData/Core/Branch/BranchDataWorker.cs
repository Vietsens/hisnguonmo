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
using Inventec.Common.Logging;
using Microsoft.Win32;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public class BranchDataWorker
    {
        private static MOS.EFMODEL.DataModels.HIS_BRANCH branch;
        public static MOS.EFMODEL.DataModels.HIS_BRANCH Branch
        {
            get
            {
                try
                {
                    if (branch == null || branch.ID <= 0)
                    {
                        branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(o => o.ID == GetCurrentBranchId()).SingleOrDefault();
                    }
                    if (branch == null) branch = new MOS.EFMODEL.DataModels.HIS_BRANCH();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                return branch;
            }
            set
            {
                branch = value;
            }
        }

        private static Dictionary<long, List<V_HIS_SERVICE_PATY>> dicServicePatyInBranch;
        public static Dictionary<long, List<V_HIS_SERVICE_PATY>> DicServicePatyInBranch
        {
            get
            {
                try
                {
                    if (dicServicePatyInBranch == null || dicServicePatyInBranch.Count == 0)
                    {
                        dicServicePatyInBranch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                            .Where(t =>
                                t.IS_ACTIVE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonNumberTrue
                                && t.BRANCH_ID == GetCurrentBranchId()
                                )
                            .GroupBy(o => o.SERVICE_ID)
                            .ToDictionary(o => o.Key, o => o.ToList());
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                return dicServicePatyInBranch;
            }
        }

        public static void ResetServicePaty()
        {
            try
            {
                dicServicePatyInBranch = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public static bool HasServicePatyWithListPatientType(long serviceId, List<long> patientTypeIds)
        {
            bool result = false;
            try
            {
                bool valid = DicServicePatyInBranch.ContainsKey(serviceId);
                if (valid)
                {
                    List<V_HIS_SERVICE_PATY> servicePatys = new List<V_HIS_SERVICE_PATY>();
                    if (DicServicePatyInBranch.TryGetValue(serviceId, out servicePatys))
                    {
                        result = servicePatys.Any(o => ((patientTypeIds == null || patientTypeIds.Count == 0) || (patientTypeIds != null && (patientTypeIds.Contains(o.PATIENT_TYPE_ID) || CheckPatientTypeInherit(o.INHERIT_PATIENT_TYPE_IDS, patientTypeIds)))));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("HasServicePatyWithListPatientType error. serviceId=" + serviceId, ex);
            }
            return result;
        }

        public static bool CheckPatientTypeInherit(string INHERIT_PATIENT_TYPE_IDS, List<long> patientTypeIds)
        {
            bool success = false;
            if (!String.IsNullOrEmpty(INHERIT_PATIENT_TYPE_IDS) && patientTypeIds != null && patientTypeIds.Count > 0)
            {
                success = patientTypeIds.Exists(k => ("," + INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + k + ","));
            }

            return success;
        }

        public static List<V_HIS_SERVICE_PATY> ServicePatyWithPatientType(long serviceId, long patientTypeId)
        {
            List<V_HIS_SERVICE_PATY> result = new List<V_HIS_SERVICE_PATY>();
            try
            {
                result = ServicePatyWithListPatientType(serviceId, new List<long>() { patientTypeId });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static List<V_HIS_SERVICE_PATY> ServicePatyWithListPatientType(long serviceId, List<long> patientTypeIds)
        {
            List<V_HIS_SERVICE_PATY> result = new List<V_HIS_SERVICE_PATY>();
            try
            {
                bool valid = DicServicePatyInBranch.ContainsKey(serviceId);
                if (valid)
                {
                    if (DicServicePatyInBranch.TryGetValue(serviceId, out result))
                    {
                        if (patientTypeIds != null && patientTypeIds.Count > 0)
                        {
                            result = result.Where(o => patientTypeIds.Contains(o.PATIENT_TYPE_ID) || CheckPatientTypeInherit(o.INHERIT_PATIENT_TYPE_IDS, patientTypeIds)).ToList();
                        }
                    }
                }
                if (result == null)
                    result = new List<V_HIS_SERVICE_PATY>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static void ChangeBranch(long branchId)
        {
            try
            {
                HIS.Desktop.LocalStorage.Branch.BranchWorker.SetBranchId(branchId);

                LogSystem.Debug("ReLoadServicePatyInBranchInRam => ServicePaty begin load while ChangeBranch");
                //Xóa dữ liệu chính sách giá theo chi nhánh cũ
                HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Reset<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();

                //Khởi tạo lại dữ liệu chính sách giá theo chi nhánh ->lưu vào ram
                var newServicePatyInBranch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                LogSystem.Debug("ReLoadServicePatyInBranchInRam => ServicePaty end load while ChangeBranch");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static long GetCurrentBranchId()
        {
            return HIS.Desktop.LocalStorage.Branch.BranchWorker.GetBranchId();
        }
    }
}

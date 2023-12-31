﻿using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOS.ServicePaty
{
    public class ServicePatyUtil
    {
        /// <summary>
        /// Lay "servicePaty" duoc ap dung
        /// - Trong danh sach service_paty truyen vao
        /// - Dang co hieu luc neu dap ung 1 trong 2 dieu kien sau:
        ///     + Thoi gian chi dinh nam trong khoang (from_time, to_time)
        ///     + Thoi gian dieu tri nam trong khoang (treatment_from_time, treatment_to_time)
        /// - Co priority lon nhat
        /// - Tuong ung voi serviceId, patientTypeId, instructionNumber (lan chi dinh thu may)
        /// </summary>
        /// <param name="treatmentTime"></param>
        /// <returns></returns>
        public static V_HIS_SERVICE_PATY GetApplied(List<V_HIS_SERVICE_PATY> servicePaties, long executeBranchId, long? executeRoomId, long? requestRoomId, long? requestDepartmentId, long instructionTime, long treatmentTime, long serviceId, long patientTypeId, long? instructionNumber)
        {
            return ServicePatyUtil.GetApplied(servicePaties, executeBranchId, executeRoomId, requestRoomId, requestDepartmentId, instructionTime, treatmentTime, serviceId, patientTypeId, instructionNumber, null);
        }

        public static V_HIS_SERVICE_PATY GetApplied(List<V_HIS_SERVICE_PATY> servicePaties, long executeBranchId, long? executeRoomId, long? requestRoomId, long? requestDepartmentId, long instructionTime, long treatmentTime, long serviceId, long patientTypeId, long? instructionNumber, long? instructionNumberByType)
        {
            return ServicePatyUtil.GetApplied(servicePaties, executeBranchId, executeRoomId, requestRoomId, requestDepartmentId, instructionTime, treatmentTime, serviceId, patientTypeId, instructionNumber, instructionNumberByType, null, null, null);
        }


        public static V_HIS_SERVICE_PATY GetApplied(List<V_HIS_SERVICE_PATY> servicePaties, long executeBranchId, long? executeRoomId, long? requestRoomId, long? requestDepartmentId, long instructionTime, long treatmentTime, long serviceId, long patientTypeId, long? instructionNumber, long? instructionNumberByType, long? packageId, long? serviceConditionId)
        {
            return ServicePatyUtil.GetApplied(servicePaties, executeBranchId, executeRoomId, requestRoomId, requestDepartmentId, instructionTime, treatmentTime, serviceId, patientTypeId, instructionNumber, instructionNumberByType, packageId, serviceConditionId, null);
        }

        public static V_HIS_SERVICE_PATY GetApplied(List<V_HIS_SERVICE_PATY> servicePaties, long executeBranchId, long? executeRoomId, long? requestRoomId, long? requestDepartmentId, long instructionTime, long treatmentTime, long serviceId, long patientTypeId, long? instructionNumber, long? instructionNumberByType, long? packageId, long? serviceConditionId, long? patientClassifyId)
        {
            return ServicePatyUtil.GetApplied(servicePaties, executeBranchId, executeRoomId, requestRoomId, requestDepartmentId, instructionTime, treatmentTime, serviceId, patientTypeId, instructionNumber, instructionNumberByType, packageId, serviceConditionId, patientClassifyId, null);
        }

        /// <summary>
        /// Lay "servicePaty" duoc ap dung
        /// - Trong danh sach service_paty truyen vao
        /// - Dang co hieu luc neu dap ung 1 trong 2 dieu kien sau:
        ///     + Thoi gian chi dinh nam trong khoang (from_time, to_time)
        ///     + Thoi gian dieu tri nam trong khoang (treatment_from_time, treatment_to_time)
        /// - Co priority lon nhat
        /// - Tuong ung voi serviceId, patientTypeId, instructionNumber (lan chi dinh thu may), instructionNumberByType (lần chỉ định thứ mấy, tính theo loại dịch vụ)
        /// </summary>
        /// <param name="treatmentTime"></param>
        /// <returns></returns>
        public static V_HIS_SERVICE_PATY GetApplied(List<V_HIS_SERVICE_PATY> servicePaties, long executeBranchId, long? executeRoomId, long? requestRoomId, long? requestDepartmentId, long instructionTime, long treatmentTime, long serviceId, long patientTypeId, long? instructionNumber, long? instructionNumberByType, long? packageId, long? serviceConditionId, long? patientClassifyId, long? rationTimeId)
        {
            V_HIS_SERVICE_PATY result = null;
            try
            {
                if (servicePaties != null && servicePaties.Count > 0)
                {
                    DateTime time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(instructionTime).Value;

                    //gio trong DB luu do dai la 4 (vd: 8h -> 0800, 17h30 -> 1730).
                    //Se co truong hop bat dau la 0, bo sung them 1 phia truoc de co the so sanh duoc
                    int hour = Int32.Parse("1" + instructionTime.ToString().Substring(8, 4));

                    int day = (int)time.DayOfWeek + 1; //luu y: nhan gia tri tu 1 ==> 7 voi 1 tuong ung voi CN
                    string reqRoomIdStr = requestRoomId.HasValue ? string.Format(",{0},", requestRoomId) : "";
                    string requestDepartmentIdStr = requestDepartmentId.HasValue ? string.Format(",{0},", requestDepartmentId) : "";
                    string executeRoomIdStr = executeRoomId.HasValue ? string.Format(",{0},", executeRoomId) : "";

                    result = servicePaties
                        .Where(o => (o.PATIENT_TYPE_ID == patientTypeId) || (o.INHERIT_PATIENT_TYPE_IDS != null && ("," + o.INHERIT_PATIENT_TYPE_IDS + ",").Contains("," + patientTypeId + ",")))
                        .Where(o => o.SERVICE_ID == serviceId && o.BRANCH_ID == executeBranchId && o.IS_ACTIVE == 1)
                        .Where(o => ((!o.FROM_TIME.HasValue || o.FROM_TIME.Value <= instructionTime) && (!o.TO_TIME.HasValue || o.TO_TIME.Value >= instructionTime))
                        || ((!o.TREATMENT_FROM_TIME.HasValue || o.TREATMENT_FROM_TIME.Value <= treatmentTime) && (!o.TREATMENT_TO_TIME.HasValue || o.TREATMENT_TO_TIME.Value >= treatmentTime)))
                        .Where(o => !instructionNumber.HasValue || ((!o.INTRUCTION_NUMBER_FROM.HasValue || o.INTRUCTION_NUMBER_FROM.Value <= instructionNumber.Value) && (!o.INTRUCTION_NUMBER_TO.HasValue || o.INTRUCTION_NUMBER_TO.Value >= instructionNumber.Value)))
                        .Where(o => !instructionNumberByType.HasValue || ((!o.INSTR_NUM_BY_TYPE_FROM.HasValue || o.INSTR_NUM_BY_TYPE_FROM.Value <= instructionNumberByType.Value) && (!o.INSTR_NUM_BY_TYPE_TO.HasValue || o.INSTR_NUM_BY_TYPE_TO.Value >= instructionNumberByType.Value)))
                        .Where(o => (o.HOUR_FROM == null || Int32.Parse("1" + o.HOUR_FROM) <= hour) && (o.HOUR_TO == null || Int32.Parse("1" + o.HOUR_TO) >= hour))
                        .Where(o => (!o.DAY_FROM.HasValue || o.DAY_FROM.Value <= day) && (!o.DAY_TO.HasValue || o.DAY_TO.Value >= day))
                        .Where(o => o.REQUEST_ROOM_IDS == null || ("," + o.REQUEST_ROOM_IDS + ",").Contains(reqRoomIdStr))
                        .Where(o => o.EXECUTE_ROOM_IDS == null || ("," + o.EXECUTE_ROOM_IDS + ",").Contains(executeRoomIdStr))
                        .Where(o => o.REQUEST_DEPARMENT_IDS == null || ("," + o.REQUEST_DEPARMENT_IDS + ",").Contains(requestDepartmentIdStr))
                        .Where(o => (!packageId.HasValue && !o.PACKAGE_ID.HasValue) || (packageId.HasValue && o.PACKAGE_ID == packageId))
                        .Where(o => (!serviceConditionId.HasValue && !o.SERVICE_CONDITION_ID.HasValue) || (serviceConditionId.HasValue && o.SERVICE_CONDITION_ID == serviceConditionId))
                        .Where(o => !o.PATIENT_CLASSIFY_ID.HasValue || o.PATIENT_CLASSIFY_ID == patientClassifyId)
                        .Where(o => !o.RATION_TIME_ID.HasValue || o.RATION_TIME_ID == rationTimeId)
                        .OrderBy(o => o.RATION_TIME_ID.HasValue && rationTimeId.HasValue ? 0 : 1)
                        .OrderByDescending(o => o.PRIORITY)
                        .ThenByDescending(o => o.ID)
                        .FirstOrDefault();
                }
                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}

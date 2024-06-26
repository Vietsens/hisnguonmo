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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000033.PDO
{
    public partial class Mps000033PDO
    {
        public List<string> Manner { get; set; }
        public HIS_SKIN_SURGERY_DESC SkinSurgeryDesc { get; set; }

        public List<HIS_SERE_SERV_FILE> SereServFile { get; set; }
        public List<V_HIS_SESE_PTTT_METHOD> sesePtttMethod { get; set; }
        public Mps000033PDO() { }

        public Mps000033PDO(
                PatientADO patient,
                V_HIS_DEPARTMENT_TRAN departmentTran,
                V_HIS_SERVICE_REQ ServiceReqPrint,
                V_HIS_SERE_SERV_PTTT sereServsPttt,
                V_HIS_BED_LOG bedLog
                )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.BedLog = bedLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000033PDO(
            PatientADO patient,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_SERVICE_REQ ServiceReqPrint,
            V_HIS_SERE_SERV_5 sereServ,
            HIS_SERE_SERV_EXT sereServExt,
            V_HIS_SERE_SERV_PTTT sereServsPttt,
            V_HIS_TREATMENT treatment,
            List<V_HIS_EKIP_USER> ekipUsers,
            HisExecuteRoleCFGPrint executeRoleCFG,
            V_HIS_BED_LOG bedLog
            )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.executeRoleCFG = executeRoleCFG;
                this.ekipUsers = ekipUsers;
                this.SereServExt = sereServExt;
                this.sereServ = sereServ;
                this.BedLog = bedLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000033PDO(
           PatientADO patient,
           V_HIS_DEPARTMENT_TRAN departmentTran,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           V_HIS_SERE_SERV_5 sereServ,
           HIS_SERE_SERV_EXT sereServExt,
           V_HIS_SERE_SERV_PTTT sereServsPttt,
           V_HIS_TREATMENT treatment,
           List<V_HIS_EKIP_USER> ekipUsers,
           HisExecuteRoleCFGPrint executeRoleCFG,
           V_HIS_BED_LOG bedLog,
           V_HIS_BED_LOG lastBedLog
           )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.executeRoleCFG = executeRoleCFG;
                this.ekipUsers = ekipUsers;
                this.SereServExt = sereServExt;
                this.sereServ = sereServ;
                this.BedLog = bedLog;
                this.LastBedLog = lastBedLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000033PDO(
           PatientADO patient,
           V_HIS_DEPARTMENT_TRAN departmentTran,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           V_HIS_SERE_SERV_5 sereServ,
           HIS_SERE_SERV_EXT sereServExt,
           V_HIS_SERE_SERV_PTTT sereServsPttt,
           V_HIS_TREATMENT treatment,
           List<V_HIS_EKIP_USER> ekipUsers,
           HisExecuteRoleCFGPrint executeRoleCFG,
           V_HIS_BED_LOG bedLog,
           V_HIS_BED_LOG lastBedLog,
            List<string> manner,
            HIS_SKIN_SURGERY_DESC skinSurgeryDesc
           )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.executeRoleCFG = executeRoleCFG;
                this.ekipUsers = ekipUsers;
                this.SereServExt = sereServExt;
                this.sereServ = sereServ;
                this.BedLog = bedLog;
                this.LastBedLog = lastBedLog;
                this.Manner = manner;
                this.SkinSurgeryDesc = skinSurgeryDesc;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000033PDO(
           PatientADO patient,
           V_HIS_DEPARTMENT_TRAN departmentTran,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           V_HIS_SERE_SERV_5 sereServ,
           HIS_SERE_SERV_EXT sereServExt,
           V_HIS_SERE_SERV_PTTT sereServsPttt,
           V_HIS_TREATMENT treatment,
           List<V_HIS_EKIP_USER> ekipUsers,
           HisExecuteRoleCFGPrint executeRoleCFG,
           V_HIS_BED_LOG bedLog,
           V_HIS_BED_LOG lastBedLog,
            List<string> manner,
            HIS_SKIN_SURGERY_DESC skinSurgeryDesc,
            List<HIS_SERE_SERV_FILE> sereServFile
           )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.executeRoleCFG = executeRoleCFG;
                this.ekipUsers = ekipUsers;
                this.SereServExt = sereServExt;
                this.sereServ = sereServ;
                this.BedLog = bedLog;
                this.LastBedLog = lastBedLog;
                this.Manner = manner;
                this.SkinSurgeryDesc = skinSurgeryDesc;
                this.SereServFile = sereServFile;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000033PDO(
           PatientADO patient,
           V_HIS_DEPARTMENT_TRAN departmentTran,
           V_HIS_SERVICE_REQ ServiceReqPrint,
           V_HIS_SERE_SERV_5 sereServ,
           HIS_SERE_SERV_EXT sereServExt,
           V_HIS_SERE_SERV_PTTT sereServsPttt,
           V_HIS_TREATMENT treatment,
           List<V_HIS_EKIP_USER> ekipUsers,
           HisExecuteRoleCFGPrint executeRoleCFG,
           V_HIS_BED_LOG bedLog,
           V_HIS_BED_LOG lastBedLog,
            List<string> manner,
            HIS_SKIN_SURGERY_DESC skinSurgeryDesc,
            List<HIS_SERE_SERV_FILE> sereServFile,
            List<V_HIS_SESE_PTTT_METHOD> sesePtttMethod
           )
        {
            try
            {
                this.Patient = patient;
                this.departmentTran = departmentTran;
                this.ServiceReqPrint = ServiceReqPrint;
                this.sereServsPttt = sereServsPttt;
                this.treatment = treatment;
                this.departmentName = departmentName;
                this.executeRoleCFG = executeRoleCFG;
                this.ekipUsers = ekipUsers;
                this.SereServExt = sereServExt;
                this.sereServ = sereServ;
                this.BedLog = bedLog;
                this.LastBedLog = lastBedLog;
                this.Manner = manner;
                this.SkinSurgeryDesc = skinSurgeryDesc;
                this.SereServFile = sereServFile;
                this.sesePtttMethod = sesePtttMethod;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

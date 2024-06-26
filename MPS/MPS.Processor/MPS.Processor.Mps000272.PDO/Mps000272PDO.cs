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
using MPS.Processor.Mps000272.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000272.PDO
{
    public class Mps000272PDO : RDOBase
    {
        public Mps000272PDO(V_HIS_PTTT_CALENDAR _ptttCalendar, List<V_HIS_SERE_SERV_13> _sereServ13s, List<V_HIS_EKIP_PLAN_USER> _ekipPlanUsers, ExecuteRoleCFG _executeRoleCFG)
        {
            try
            {
                this.PtttCalendar = _ptttCalendar;
                this.SereServ13s = _sereServ13s;
                this.EkipPlanUsers = _ekipPlanUsers;
                this.ExecuteRole = _executeRoleCFG;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000272PDO(V_HIS_PTTT_CALENDAR _ptttCalendar, List<V_HIS_SERE_SERV_13> _sereServ13s, List<V_HIS_EKIP_PLAN_USER> _ekipPlanUsers, ExecuteRoleCFG _executeRoleCFG, HIS_PTTT_METHOD _method, HIS_EMOTIONLESS_METHOD _emotionless)
        {
            try
            {
                this.PtttCalendar = _ptttCalendar;
                this.SereServ13s = _sereServ13s;
                this.EkipPlanUsers = _ekipPlanUsers;
                this.ExecuteRole = _executeRoleCFG;
                this.Method = _method;
                this.Emotionless = _emotionless;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000272PDO(V_HIS_PTTT_CALENDAR _ptttCalendar, List<V_HIS_SERE_SERV_13> _sereServ13s, List<V_HIS_EKIP_PLAN_USER> _ekipPlanUsers, ExecuteRoleCFG _executeRoleCFG, List<HIS_EXECUTE_ROLE> _HisExecuteRoles)
        {
            try
            {
                this.PtttCalendar = _ptttCalendar;
                this.SereServ13s = _sereServ13s;
                this.EkipPlanUsers = _ekipPlanUsers;
                this.ExecuteRole = _executeRoleCFG;
                this.HisExecuteRoles = _HisExecuteRoles;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000272PDO(V_HIS_PTTT_CALENDAR _ptttCalendar, List<V_HIS_SERE_SERV_13> _sereServ13s, List<V_HIS_EKIP_PLAN_USER> _ekipPlanUsers, ExecuteRoleCFG _executeRoleCFG, HIS_PTTT_METHOD _method, HIS_EMOTIONLESS_METHOD _emotionless, List<HIS_EXECUTE_ROLE> _HisExecuteRoles)
        {
            try
            {
                this.PtttCalendar = _ptttCalendar;
                this.SereServ13s = _sereServ13s;
                this.EkipPlanUsers = _ekipPlanUsers;
                this.ExecuteRole = _executeRoleCFG;
                this.Method = _method;
                this.Emotionless = _emotionless;
                this.HisExecuteRoles = _HisExecuteRoles;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        public HIS_PTTT_METHOD Method { get; set; }
        public V_HIS_PTTT_CALENDAR PtttCalendar { get; set; }
        public List<V_HIS_SERE_SERV_13> SereServ13s { get; set; }
        public List<V_HIS_EKIP_PLAN_USER> EkipPlanUsers { get; set; }
        public ExecuteRoleCFG ExecuteRole { get; set; }
        public HIS_EMOTIONLESS_METHOD Emotionless { get; set; }

        public List<HIS_EXECUTE_ROLE> HisExecuteRoles { get; set; }
    }
}

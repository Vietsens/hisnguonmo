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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000272.ADO
{
    class SereServADO : V_HIS_SERE_SERV_13
    {
        public string AGE { get; set; }
        public string PTV_LOGINNAME { get; set; }
        public string PTV_USERNAME { get; set; }
        public long? PLAN_DATE_FROM { get; set; }
        public string HOUR_STR { get; set; }
        public string LOGINNAME { get; set; }
        public string USERNAME { get; set; }

        public DataTable ExecuteRole { get; set; }

        public Dictionary<string, string> DicExecuteRole { get; set; }


        public SereServADO(V_HIS_SERE_SERV_13 data, List<V_HIS_EKIP_PLAN_USER> ekipPlanUsers, ExecuteRoleCFG executeRole, List<HIS_EXECUTE_ROLE> HisExecuteRoles)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_13>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (this.EKIP_PLAN_ID.HasValue && ekipPlanUsers != null && ekipPlanUsers.Count > 0)
                {
                    V_HIS_EKIP_PLAN_USER v_HIS_EKIP_PLAN_USER = ekipPlanUsers.FirstOrDefault(o => o.EKIP_PLAN_ID == this.EKIP_PLAN_ID.Value && o.EXECUTE_ROLE_ID == executeRole.EXECUTE_ROLE__PTV);
                    if (v_HIS_EKIP_PLAN_USER != null)
                    {
                        this.PTV_LOGINNAME = v_HIS_EKIP_PLAN_USER.LOGINNAME;
                        this.PTV_USERNAME = v_HIS_EKIP_PLAN_USER.USERNAME;
                    }

                    if (HisExecuteRoles != null && HisExecuteRoles.Count > 0)
                    {
                        DataTable dt = new DataTable();

                        DataRow _ravi = dt.NewRow();//khởi tạo 1 hàng trong DataTable

                        DicExecuteRole = new Dictionary<string, string>();

                        foreach (var item in HisExecuteRoles)
                        {
                            List<V_HIS_EKIP_PLAN_USER> hisEkipPlanUser = ekipPlanUsers.Where(o => o.EKIP_PLAN_ID == this.EKIP_PLAN_ID.Value && o.EXECUTE_ROLE_ID == item.ID).ToList();

                            if (hisEkipPlanUser != null && HisExecuteRoles.Count > 0)
                            {
                                string ColumnName = String.Format("EXECUTE_ROLE_{0}_USERNAME", item.EXECUTE_ROLE_CODE);
                                dt.Columns.Add(ColumnName);

                                string RowData = "";
                                foreach (var itemU in hisEkipPlanUser)
                                {
                                    RowData += itemU.USERNAME + "\n";
                                }

                                DicExecuteRole.Add(ColumnName, RowData);
                                _ravi[ColumnName] = RowData;

                            }
                        }
                        dt.Rows.Add(_ravi);

                        ExecuteRole = dt;

                        Inventec.Common.Logging.LogSystem.Debug("DicExecuteRole: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => DicExecuteRole), DicExecuteRole));
                        Inventec.Common.Logging.LogSystem.Debug("ExecuteRole: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ExecuteRole), ExecuteRole));
                    }
                }

                var lstekipPlanUsers = ekipPlanUsers.Where(o => o.EKIP_PLAN_ID == this.EKIP_PLAN_ID.Value).ToList();

                if (lstekipPlanUsers != null && lstekipPlanUsers.Count > 0)
                {
                    this.LOGINNAME = "";
                    this.USERNAME = "";
                    foreach (var item in lstekipPlanUsers)
                    {
                        string loginName = item.LOGINNAME;
                        string userName = item.USERNAME;
                        this.LOGINNAME = this.LOGINNAME + loginName + "\n";
                        this.USERNAME = this.USERNAME + userName + "\n";
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.LOGINNAME), this.LOGINNAME));
                if (this.PLAN_TIME_FROM.HasValue)
                {
                    this.PLAN_DATE_FROM = long.Parse(this.PLAN_TIME_FROM.ToString().Substring(0, 8) + "000000");
                    this.HOUR_STR = string.Format("{0}:{1}", this.PLAN_TIME_FROM.ToString().Substring(8, 2), this.PLAN_TIME_FROM.ToString().Substring(10, 2));
                }

                this.AGE = AgeUtil.CalculateFullAge(this.TDL_PATIENT_DOB);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //public SereServADO(V_HIS_SERE_SERV_13 data, List<V_HIS_EKIP_PLAN_USER> ekipPlanUsers, ExecuteRoleCFG executeRole, List<HIS_EKIP_USER> ekipUsers)
        //{
        //    try
        //    {
        //        System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_13>();
        //        foreach (var item in pi)
        //        {
        //            item.SetValue(this, (item.GetValue(data)));
        //        }
        //        if (this.EKIP_PLAN_ID.HasValue && ekipPlanUsers != null && ekipPlanUsers.Count > 0)
        //        {
        //            V_HIS_EKIP_PLAN_USER v_HIS_EKIP_PLAN_USER = ekipPlanUsers.FirstOrDefault(o => o.EKIP_PLAN_ID == this.EKIP_PLAN_ID.Value && o.EXECUTE_ROLE_ID == executeRole.EXECUTE_ROLE__PTV);
        //            if (v_HIS_EKIP_PLAN_USER != null)
        //            {
        //                this.PTV_LOGINNAME = v_HIS_EKIP_PLAN_USER.LOGINNAME;
        //                this.PTV_USERNAME = v_HIS_EKIP_PLAN_USER.USERNAME;

        //            }
        //        }
        //        if ( ekipPlanUsers != null && ekipPlanUsers.Count > 0)
        //        {
        //            V_HIS_EKIP_PLAN_USER v_HIS_EKIP_PLAN_USER = ekipPlanUsers.FirstOrDefault(o => o.EKIP_PLAN_ID == this.EKIP_PLAN_ID.Value);
        //            if (v_HIS_EKIP_PLAN_USER != null)
        //            {
        //                this.LOGINNAME = v_HIS_EKIP_PLAN_USER.LOGINNAME;
        //                this.USERNAME = v_HIS_EKIP_PLAN_USER.USERNAME;

        //            }
        //        }

        //        if (this.PLAN_TIME_FROM.HasValue)
        //        {
        //            this.PLAN_DATE_FROM = long.Parse(this.PLAN_TIME_FROM.ToString().Substring(0, 8) + "000000");
        //            this.HOUR_STR = string.Format("{0}:{1}", this.PLAN_TIME_FROM.ToString().Substring(8, 2), this.PLAN_TIME_FROM.ToString().Substring(10, 2));
        //        }

        //        this.AGE = AgeUtil.CalculateFullAge(this.TDL_PATIENT_DOB);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}

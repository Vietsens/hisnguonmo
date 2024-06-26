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
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReportCountTreatment.Get
{
    class GetTreatmentInfo
    {
        private List<long> TreatmentIds;
        internal List<V_HIS_DEPARTMENT_TRAN> ListDepartmentTran;

        public GetTreatmentInfo(List<long> treatmentIds)
        {
            try
            {
                this.TreatmentIds = treatmentIds;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>> Get()
        {
            Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>> result = new Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>>();
            try
            {
                ThreadGetTreatmentInfo();

                if (this.ListDepartmentTran != null && this.ListDepartmentTran.Count > 0)
                {
                    var groupTreatment = this.ListDepartmentTran.GroupBy(o => o.TREATMENT_ID).ToList();
                    foreach (var groups in groupTreatment)
                    {
                        V_HIS_DEPARTMENT_TRAN t = groups
                            .OrderByDescending(o => !o.DEPARTMENT_IN_TIME.HasValue)
                            .ThenByDescending(o => o.DEPARTMENT_IN_TIME)
                            .ThenByDescending(o => o.ID).FirstOrDefault();

                        //ADO.TreatmentInfoADO ado = new ADO.TreatmentInfoADO();
                        //if (result.ContainsKey(t.TREATMENT_ID))
                        //{
                        //    ado = result[t.TREATMENT_ID];
                        //}

                        //ado.DepartmentTran = t;
                        result[groups.First().TREATMENT_ID] = groups.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Dictionary<long, List<V_HIS_DEPARTMENT_TRAN>>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ThreadGetTreatmentInfo()
        {
            Thread depa = new Thread(GetDepartmentTran);
            Thread depa1 = new Thread(GetDepartmentTran);
            Thread depa2 = new Thread(GetDepartmentTran);
            Thread depa3 = new Thread(GetDepartmentTran);
            try
            {
                List<long> listid = new List<long>();
                List<long> listid1 = new List<long>();
                List<long> listid2 = new List<long>();
                List<long> listid3 = new List<long>();

                if (this.TreatmentIds != null && this.TreatmentIds.Count > 0)
                {
                    this.ListDepartmentTran = new List<V_HIS_DEPARTMENT_TRAN>();

                    int countid = this.TreatmentIds.Count / 4 + 1;
                    var skip = 0;
                    listid = this.TreatmentIds.Skip(skip).Take(countid).ToList();
                    skip += countid;
                    listid1 = this.TreatmentIds.Skip(skip).Take(countid).ToList();
                    skip += countid;
                    listid2 = this.TreatmentIds.Skip(skip).Take(countid).ToList();
                    skip += countid;
                    listid3 = this.TreatmentIds.Skip(skip).Take(countid).ToList();
                }

                depa.Start(listid);
                depa1.Start(listid1);
                depa2.Start(listid2);
                depa3.Start(listid3);

                depa.Join();
                depa1.Join();
                depa2.Join();
                depa3.Join();
            }
            catch (Exception ex)
            {
                depa.Abort();
                depa1.Abort();
                depa2.Abort();
                depa3.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDepartmentTran(object listid)
        {
            try
            {
                if (listid != null && listid.GetType() == typeof(List<long>))
                {
                    List<long> ids = (List<long>)listid;
                    if (ids != null && ids.Count > 0)
                    {
                        CommonParam param = new CommonParam();
                        var skip = 0;
                        while (ids.Count - skip > 0)
                        {
                            var listIds = ids.Skip(skip).Take(Base.Config.MAX_REQUEST_LENGTH_PARAM).ToList();
                            skip += Base.Config.MAX_REQUEST_LENGTH_PARAM;
                            HisDepartmentTranViewFilter filter = new HisDepartmentTranViewFilter();
                            filter.TREATMENT_IDs = listIds;
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                            if (apiResult != null && apiResult.Count > 0)
                            {
                                this.ListDepartmentTran.AddRange(apiResult);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
    class GetTreatment
    {
        private List<V_HIS_TREATMENT_4> treatmentIntime;
        private List<V_HIS_TREATMENT_4> treatmentBeforWithOutTime;
        private List<V_HIS_TREATMENT_4> treatmentBeforNotOutTime;
        private List<V_HIS_TREATMENT_4> treatmentOutInTime;
        private long timeFrom;
        private long timeTo;
        private List<long> treatmentTypeId;

        public GetTreatment(long _timeFrom, long _timeTo, List<long> _treatmentTypeId)
        {
            try
            {
                this.timeFrom = _timeFrom;
                this.timeTo = _timeTo;
                this.treatmentTypeId = _treatmentTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal List<V_HIS_TREATMENT_4> GetTotalTreatment()
        {
            List<V_HIS_TREATMENT_4> result = null;
            try
            {
                ThreadGetTreatment();

                List<V_HIS_TREATMENT_4> listTreatment = new List<V_HIS_TREATMENT_4>();

                if (treatmentIntime != null && treatmentIntime.Count > 0)
                {
                    listTreatment.AddRange(treatmentIntime);
                }

                if (treatmentBeforWithOutTime != null && treatmentBeforWithOutTime.Count > 0)
                {
                    listTreatment.AddRange(treatmentBeforWithOutTime);
                }

                if (treatmentBeforNotOutTime != null && treatmentBeforNotOutTime.Count > 0)
                {
                    listTreatment.AddRange(treatmentBeforNotOutTime);
                }

                if (treatmentOutInTime != null && treatmentOutInTime.Count > 0)
                {
                    listTreatment.AddRange(treatmentOutInTime);
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    Dictionary<long, V_HIS_TREATMENT_4> dicTreatment = new Dictionary<long, V_HIS_TREATMENT_4>();
                    foreach (var item in listTreatment)
                    {
                        if (!dicTreatment.ContainsKey(item.ID)) dicTreatment[item.ID] = item;
                    }
                    result = dicTreatment.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ThreadGetTreatment()
        {
            Thread inTime = new Thread(GetTreatmentInTime);
            Thread withOutTime = new Thread(GetTreatmentWithOutTime);
            Thread notOutTime = new Thread(GetTreatmentNotOutTime);
            Thread outInTime = new Thread(GetTreatmentOutInTime);
            try
            {
                inTime.Start();
                withOutTime.Start();
                notOutTime.Start();
                outInTime.Start();

                inTime.Join();
                withOutTime.Join();
                notOutTime.Join();
                outInTime.Join();
            }
            catch (Exception ex)
            {
                inTime.Abort();
                withOutTime.Abort();
                notOutTime.Abort();
                outInTime.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentOutInTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.OUT_TIME_FROM = this.timeFrom;
                filter.OUT_TIME_TO = this.timeTo;
                filter.TDL_TREATMENT_TYPE_IDs = this.treatmentTypeId;
                this.treatmentOutInTime = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentNotOutTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.IN_TIME_TO = this.timeFrom;
                filter.IS_PAUSE = false;
                filter.TDL_TREATMENT_TYPE_IDs = this.treatmentTypeId;
                this.treatmentBeforNotOutTime = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentWithOutTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.IN_TIME_TO = this.timeFrom;
                filter.OUT_TIME_FROM = this.timeFrom;
                filter.TDL_TREATMENT_TYPE_IDs = this.treatmentTypeId;
                this.treatmentBeforWithOutTime = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatmentInTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                filter.IN_TIME_FROM = this.timeFrom;
                filter.IN_TIME_TO = this.timeTo;
                filter.TDL_TREATMENT_TYPE_IDs = this.treatmentTypeId;
                this.treatmentIntime = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// type: 1-IN_TIME trong thời gian; 2: CLINICAL_IN_TIME trong thời gian
        /// </summary>
        /// <param name="type"></param>
        /// <param name="treatmentIds"></param>
        /// <returns></returns>
        internal List<V_HIS_TREATMENT_4> GetTreatmentIn(int type, List<long> treatmentIds)
        {
            List<V_HIS_TREATMENT_4> result = null;
            try
            {
                List<V_HIS_TREATMENT_4> listTreatment = new List<V_HIS_TREATMENT_4>();

                if (treatmentIds != null && treatmentIds.Count > 0)
                {
                    treatmentIds = treatmentIds.Distinct().ToList();
                    CommonParam param = new CommonParam();
                    var skip = 0;
                    while (treatmentIds.Count - skip > 0)
                    {
                        var listIds = treatmentIds.Skip(skip).Take(Base.Config.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip += Base.Config.MAX_REQUEST_LENGTH_PARAM;
                        HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                        filter.IDs = listIds;
                        if (type == 1)
                        {
                            filter.IN_TIME_FROM = timeFrom;
                            filter.IN_TIME_TO = timeTo;
                        }
                        else if (type == 2)
                        {
                            filter.CLINICAL_IN_TIME_FROM = timeFrom;
                            filter.CLINICAL_IN_TIME_TO = timeTo;
                        }

                        var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            listTreatment.AddRange(apiResult);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("apiResult null");
                        }
                    }
                }

                if (listTreatment != null && listTreatment.Count > 0)
                {
                    result = listTreatment.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

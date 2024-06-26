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
using HIS.Desktop.Plugins.ReportCountTreatment.Base;
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
    class GetServiceReq
    {
        private long TimeFrom;
        private long TimeTo;
        private List<long> ServiceTypeIds;
        private List<long> ExecuteRoomIds;
        private List<long> ListTreatmentIds;

        List<HIS_SERVICE_REQ> ServiceReqInTime;
        List<HIS_SERVICE_REQ> ServiceReqBefore;

        public GetServiceReq(long _timeFrom, long _timeTo, List<long> _ServiceTypeIds, List<long> executeRoomIds)
        {
            this.ServiceTypeIds = _ServiceTypeIds;
            this.TimeFrom = _timeFrom;
            this.TimeTo = _timeTo;
            this.ExecuteRoomIds = executeRoomIds;
        }

        internal List<HIS_SERVICE_REQ> Get()
        {
            List<HIS_SERVICE_REQ> result = null;
            try
            {
                ThreadGetServiceReq();

                List<HIS_SERVICE_REQ> listServiceReq = new List<HIS_SERVICE_REQ>();

                if (ServiceReqInTime != null && ServiceReqInTime.Count > 0)
                {
                    listServiceReq.AddRange(ServiceReqInTime);
                }

                if (ServiceReqBefore != null && ServiceReqBefore.Count > 0)
                {
                    listServiceReq.AddRange(ServiceReqBefore);
                }

                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    result = listServiceReq.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ThreadGetServiceReq()
        {
            Thread inTime = new Thread(GetServiceReqInTime);
            Thread beforeTime = new Thread(GetServiceReqBefor);
            try
            {
                inTime.Start();
                beforeTime.Start();

                inTime.Join();
                beforeTime.Join();
            }
            catch (Exception ex)
            {
                inTime.Abort();
                beforeTime.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReqBefor()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.INTRUCTION_TIME_TO = this.TimeFrom;
                filter.SERVICE_REQ_STT_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL };
                filter.SERVICE_REQ_TYPE_IDs = this.ServiceTypeIds;
                filter.EXECUTE_ROOM_IDs = this.ExecuteRoomIds;

                if (this.ListTreatmentIds != null && this.ListTreatmentIds.Count > 0)
                {
                    this.ServiceReqBefore = new List<HIS_SERVICE_REQ>();
                    int skip = 0;
                    while (ListTreatmentIds.Count - skip > 0)
                    {
                        var lstIds = ListTreatmentIds.Skip(skip).Take(Config.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip += Config.MAX_REQUEST_LENGTH_PARAM;
                        filter.TREATMENT_IDs = lstIds;
                        var req = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (req != null && req.Count > 0)
                        {
                            this.ServiceReqBefore.AddRange(req);
                        }
                    }
                }
                else
                {
                    this.ServiceReqBefore = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceReqInTime()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.INTRUCTION_TIME_FROM = this.TimeFrom;
                filter.INTRUCTION_TIME_TO = this.TimeTo;
                filter.SERVICE_REQ_TYPE_IDs = this.ServiceTypeIds;
                filter.EXECUTE_ROOM_IDs = this.ExecuteRoomIds;

                if (this.ListTreatmentIds != null && this.ListTreatmentIds.Count > 0)
                {
                    this.ServiceReqInTime = new List<HIS_SERVICE_REQ>();
                    int skip = 0;
                    while (ListTreatmentIds.Count - skip > 0)
                    {
                        var lstIds = ListTreatmentIds.Skip(skip).Take(Config.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip += Config.MAX_REQUEST_LENGTH_PARAM;
                        filter.TREATMENT_IDs = lstIds;
                        var req = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                        if (req != null && req.Count > 0)
                        {
                            this.ServiceReqInTime.AddRange(req);
                        }
                    }
                }
                else
                {
                    this.ServiceReqInTime = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal List<HIS_SERVICE_REQ> GetByTreatmentId(List<long> lstTreatment_id)
        {
            List<HIS_SERVICE_REQ> result = null;
            try
            {
                this.ListTreatmentIds = lstTreatment_id;

                ThreadGetServiceReq();

                List<HIS_SERVICE_REQ> listServiceReq = new List<HIS_SERVICE_REQ>();

                if (ServiceReqInTime != null && ServiceReqInTime.Count > 0)
                {
                    listServiceReq.AddRange(ServiceReqInTime);
                }

                if (ServiceReqBefore != null && ServiceReqBefore.Count > 0)
                {
                    listServiceReq.AddRange(ServiceReqBefore);
                }

                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    result = listServiceReq.Distinct().ToList();
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

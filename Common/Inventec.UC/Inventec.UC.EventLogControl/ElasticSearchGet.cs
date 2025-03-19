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
using Inventec.UC.EventLogControl.Base;
using Nest;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl
{
    class ElasticSearchGet
    {
        internal ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>> GetEventLog(SDA.Filter.SdaEventLogFilter filterQuery, int start, int limit)
        {
            ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>> apiResult = null;

            try
            {
                ISearchResponse<SDA_EVENT_LOG> response = null;

                var mustClauses = new List<QueryContainer>();
                var filterClauses = new List<QueryContainer>();

                if (!String.IsNullOrEmpty(filterQuery.KEY_WORD))
                {
                    IQueryContainer rangeQueryContainer = new QueryContainer();
                    string[] fields = new string[] { "ip", "creator", "modifier", "description", "login_name" };
                    rangeQueryContainer.MultiMatch = new MultiMatchQuery
                    {
                        Fields = fields,
                        Operator = Operator.Or,
                        Query = filterQuery.KEY_WORD
                    };

                    mustClauses.Add((QueryContainer)rangeQueryContainer);
                }

                if (filterQuery.CREATE_DATE_FROM.HasValue || filterQuery.CREATE_DATE_TO.HasValue)
                {
                    filterClauses.Add(new NumericRangeQuery
                    {
                        Field = new Field("create_date"),
                        LessThanOrEqualTo = filterQuery.CREATE_DATE_TO,
                        GreaterThanOrEqualTo = filterQuery.CREATE_DATE_FROM,
                        Name = "create_date_range"
                    });
                }

                var searchRequest = new SearchRequest<SDA_EVENT_LOG>("rawsearchdata_sda_event_log", "sda_event_log")
                {
                    Size = limit,
                    From = start,
                    Sort = new List<ISort>
                    {                       
                        new SortField { Field = "create_time", Order = SortOrder.Descending },     
                    },
                    Query = new BoolQuery
                    {
                        Must = mustClauses,
                        Filter = filterClauses
                    }
                };
                response = ApiConsumerStore.ElasticClientConsumer.Search<SDA_EVENT_LOG>(searchRequest);

                apiResult = new ApiResultObject<List<SDA_EVENT_LOG>>(response.Documents.ToList(), true);
                apiResult.Param = new CommonParam();
                apiResult.Param.Count = (int)response.Total;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return apiResult;
        }
    }
}

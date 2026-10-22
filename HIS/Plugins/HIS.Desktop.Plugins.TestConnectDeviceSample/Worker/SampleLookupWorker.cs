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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Worker
{
    internal class SampleLookupResult
    {
        internal List<V_LIS_SAMPLE> DueSamples = new List<V_LIS_SAMPLE>();
        internal List<V_LIS_SAMPLE> FutureSamples = new List<V_LIS_SAMPLE>();
        internal List<string> PendingTreatmentCodes = new List<string>();
        internal V_LIS_SAMPLE RecentSample;
        internal DateTime WindowFrom;
        internal bool IsApiError;
    }

    /// <summary>
    /// Finds the samples a scanned code refers to, with the same room filter as the Barcode (F4) box of
    /// HIS.Desktop.Plugins.SampleCollectionRoom.
    /// </summary>
    internal static class SampleLookupWorker
    {
        internal const string URI_LIS_SAMPLE_GET_VIEW = "api/LisSample/GetView";
        internal const string URI_HIS_TREATMENT_GET = "api/HisTreatment/Get";
        // Sample barcodes cycle (the weekday-prefixed format repeats after 7 days): only recent samples count
        internal const int BARCODE_WINDOW_DAYS = 6;

        internal static SampleLookupResult Find(string code, V_HIS_ROOM room, DateTime scanTime)
        {
            SampleLookupResult result = new SampleLookupResult();
            try
            {
                result.WindowFrom = scanTime.Date.AddDays(-BARCODE_WINDOW_DAYS);
                long windowFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(result.WindowFrom) ?? 0;
                List<long> pendingStatuses = new List<long>
                {
                    IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                    IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                };

                LisSampleViewFilter byBarcode = CreateRoomFilter(room);
                byBarcode.BARCODE__EXACT = code;
                byBarcode.SAMPLE_STT_IDs = pendingStatuses;
                byBarcode.BARCODE_TIME_FROM = windowFrom;
                List<V_LIS_SAMPLE> pending = GetSamples(byBarcode, result);

                if (pending != null && !pending.Any() && ScanTextUtil.IsServiceReqCode(code))
                {
                    // Requisition barcode: the service request code (unique, no date window)
                    LisSampleViewFilter byServiceReq = CreateRoomFilter(room);
                    byServiceReq.SERVICE_REQ_CODE__EXACT = code;
                    byServiceReq.SAMPLE_STT_IDs = pendingStatuses;
                    pending = GetSamples(byServiceReq, result);
                }
                if (pending == null)
                    return result;

                if (!pending.Any())
                {
                    // Only to explain why nothing opens: the latest recent sample with this barcode, whatever its status
                    LisSampleViewFilter recent = CreateRoomFilter(room);
                    recent.BARCODE__EXACT = code;
                    recent.BARCODE_TIME_FROM = windowFrom;
                    List<V_LIS_SAMPLE> recentSamples = GetSamples(recent, result) ?? new List<V_LIS_SAMPLE>();
                    if (!recentSamples.Any() && ScanTextUtil.IsServiceReqCode(code))
                    {
                        // Requisition scanned again after sampling: its tube barcode differs from the code
                        LisSampleViewFilter recentByServiceReq = CreateRoomFilter(room);
                        recentByServiceReq.SERVICE_REQ_CODE__EXACT = code;
                        recentSamples = GetSamples(recentByServiceReq, result) ?? new List<V_LIS_SAMPLE>();
                    }
                    result.RecentSample = recentSamples.FirstOrDefault(o => o.SAMPLE_TIME.HasValue) ?? recentSamples.FirstOrDefault();
                    return result;
                }

                long scanTimeNumber = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(scanTime) ?? 0;
                result.PendingTreatmentCodes = pending.Select(o => o.TREATMENT_CODE ?? "").Distinct().ToList();
                result.DueSamples = pending.Where(o => (o.INTRUCTION_TIME ?? 0) <= scanTimeNumber).OrderBy(o => o.ID).ToList();
                result.FutureSamples = pending.Where(o => (o.INTRUCTION_TIME ?? 0) > scanTimeNumber).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.IsApiError = true;
            }
            return result;
        }

        internal static V_LIS_SAMPLE GetSampleById(long id)
        {
            try
            {
                LisSampleViewFilter filter = new LisSampleViewFilter();
                filter.ID = id;
                List<V_LIS_SAMPLE> samples = GetSamples(filter, new SampleLookupResult());
                return samples != null ? samples.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Same rule as the take-sample button of SampleCollectionRoom: no sampling once treatment has ended.
        /// Returns null when it cannot be checked (the button stops in that case too).
        /// </summary>
        internal static bool? IsTreatmentPaused(string treatmentCode)
        {
            try
            {
                // A sample created on LIS may have no HIS treatment; an empty code would make MOS return every treatment
                if (String.IsNullOrWhiteSpace(treatmentCode))
                    return false;

                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;
                List<HIS_TREATMENT> treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    URI_HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);
                if (treatments == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("HisTreatment/Get failed" + Inventec.Common.Logging.LogUtil.TraceData("treatmentCode", treatmentCode)
                        + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                    return null;
                }
                HIS_TREATMENT treatment = treatments.FirstOrDefault();
                return treatment != null && treatment.IS_PAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// Room filter copied from SampleCollectionRoomUC.FillDataToGridSample so both screens see the same samples.
        /// </summary>
        private static LisSampleViewFilter CreateRoomFilter(V_HIS_ROOM room)
        {
            try
            {
                LisSampleViewFilter filter = new LisSampleViewFilter();
                filter.ORDER_FIELD = "BARCODE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (room == null)
                    return filter;

                if (room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BP)
                {
                    filter.SAMPLE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }
                else if (room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    filter.REQUEST_DEPARTMENT_CODE__EXACT = room.DEPARTMENT_CODE;
                }
                else if (room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL)
                {
                    V_HIS_EXECUTE_ROOM executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == room.ID);
                    if (executeRoom != null && executeRoom.IS_EXAM == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        filter.REQUEST_DEPARTMENT_CODE__EXACT = room.DEPARTMENT_CODE;
                    else
                        filter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }
                else
                {
                    filter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }
                return filter;
            }
            catch (Exception ex)
            {
                // Never return a filter without its room condition: the caller treats null as a lookup error
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private static List<V_LIS_SAMPLE> GetSamples(LisSampleViewFilter filter, SampleLookupResult result)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<V_LIS_SAMPLE> samples = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>(
                    URI_LIS_SAMPLE_GET_VIEW, ApiConsumers.LisConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);
                if (samples == null)
                {
                    result.IsApiError = true;
                    Inventec.Common.Logging.LogSystem.Error("LisSample/GetView failed"
                        + Inventec.Common.Logging.LogUtil.TraceData("filter", filter)
                        + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                }
                return samples;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.IsApiError = true;
                return null;
            }
        }
    }
}

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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PrepareAndExport
{
    /// <summary>
    /// Tra cuu HIS_TREATMENT theo danh sach ID cho man Soan phat thuoc.
    ///
    /// VI SAO PHAI CHIA LO: BackendAdapter.Get serialize filter ra JSON roi base64 vao QUERY STRING
    /// cua request GET (~22 byte cho moi ID). Tran do dai URL thuc te 16 KB, nen day ca nghin ID vao
    /// mot loi goi la vuot tran -> HTTP 414 / loi phia server, ma BackendAdapter chi tra null nen man
    /// hinh chi thay "trang du lieu" chu khong thay loi. 100 ID/lo (~2,2 KB) la muc an toan da dung o
    /// cho khac (UCTreatmentList__Print___KSK).
    ///
    /// CAC LO CHAY GOI DAU NHAU: lo sau khoi hanh ngay khi co luong ranh, khong cho lo truoc ve.
    /// Cap FETCH_THREADS luong de khong thay mot cu qua tai bang chuc truy van IN (100) dong thoi.
    ///
    /// Moi luot tra cuu deu ghi log voi tien to HIS_TREATMENT_GET de tra khi man hinh thieu du lieu:
    /// lo nao loi, lo nao tra null, lay ve bao nhieu ban ghi, het bao nhieu ms.
    /// </summary>
    internal static class TreatmentBatchLoader
    {
        /// <summary>So ID toi da cho mot loi goi api/HisTreatment/Get (xem giai thich tran URL o dau lop).</summary> 
        internal const int ID_BATCH_SIZE = 100;

        /// <summary>So lo chay dong thoi. Tang thi nhanh hon nhung don tai xuong server/DB cung luc.</summary>
        internal const int FETCH_THREADS = 4;

        /// <summary>.NET Framework mac dinh chi cho 2 ket noi HTTP/host -> khong nang thi cac lo van xep hang tung doi.</summary>
        private const int MIN_CONNECTION_LIMIT = 20;

        /// <summary>Tien to log de grep nhanh trong LogSystem.txt.</summary>
        private const string LOG_PREFIX = "HIS_TREATMENT_GET";

        /// <summary>
        /// Lay HIS_TREATMENT theo danh sach ID: chia lo ID_BATCH_SIZE, ban cac lo goi dau nhau, gom ket qua.
        /// Lo nao loi thi bo qua lo do va ghi log (KHONG vut toan bo du lieu da lay duoc) -> ket qua co the
        /// THIEU; xem dong "KET QUA" trong log de biet co lo nao loi khong.
        /// </summary>
        /// <param name="caller">Ten cho goi, chi de doc log.</param>
        /// <param name="ids">Danh sach ID dot dieu tri (tu dong loai trung va loai ID &lt;= 0).</param>
        /// <param name="applyMoreCondition">Gan them dieu kien cho filter cua TUNG lo (co the null).</param>
        internal static List<HIS_TREATMENT> GetByIds(string caller, List<long> ids, Action<HisTreatmentFilter> applyMoreCondition)
        {
            List<HIS_TREATMENT> result = new List<HIS_TREATMENT>();
            try
            {
                List<List<long>> batches = SplitIdBatches(ids);
                if (batches.Count == 0)
                {
                    LogSystem.Warn(LOG_PREFIX + " __ " + caller + " __ KHONG CO ID hop le de tra cuu (ids="
                        + ((ids == null) ? "null" : ids.Count.ToString()) + ")");
                    return result;
                }

                if (System.Net.ServicePointManager.DefaultConnectionLimit < MIN_CONNECTION_LIMIT)
                    System.Net.ServicePointManager.DefaultConnectionLimit = MIN_CONNECTION_LIMIT;

                int totalId = batches.Sum(o => o.Count);
                LogSystem.Debug(LOG_PREFIX + " __ " + caller + " __ BAT DAU: id=" + totalId
                    + ", so lo=" + batches.Count + " (moi lo toi da " + ID_BATCH_SIZE + " id)"
                    + ", so lo chay dong thoi=" + FETCH_THREADS
                    + ", DefaultConnectionLimit=" + System.Net.ServicePointManager.DefaultConnectionLimit);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var bag = new ConcurrentBag<HIS_TREATMENT>();
                int okBatch = 0;
                int failBatch = 0;

                Parallel.For(0, batches.Count,
                    new ParallelOptions { MaxDegreeOfParallelism = FETCH_THREADS },
                    idx =>
                    {
                        // Mot lo loi KHONG duoc giet ca luot: de loi thoat ra ngoai thi Parallel goi loi lai
                        // va huy cac lo con lai -> mat sach du lieu chi vi mot loi goi hong.
                        if (FetchOneBatch(caller, idx + 1, batches.Count, batches[idx], applyMoreCondition, bag))
                            Interlocked.Increment(ref okBatch);
                        else
                            Interlocked.Increment(ref failBatch);
                    });

                sw.Stop();
                result = bag.ToList();
                string summary = LOG_PREFIX + " __ " + caller + " __ KET QUA: id=" + totalId
                    + ", lo thanh cong=" + okBatch + "/" + batches.Count
                    + ", lo loi=" + failBatch
                    + ", ban ghi lay ve=" + result.Count
                    + ", het=" + sw.ElapsedMilliseconds + " ms";
                if (failBatch > 0)
                    LogSystem.Warn(summary + " __ CO LO LOI: du lieu tra ve KHONG DAY DU");
                else
                    LogSystem.Debug(summary);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Goi mot lo. Tra ve false khi lo do khong lay duoc du lieu (nem loi, hoac API tra null) —
        /// cho goi dem lai de biet ket qua co bi thieu hay khong.
        /// </summary>
        private static bool FetchOneBatch(string caller, int no, int total, List<long> batchIds,
            Action<HisTreatmentFilter> applyMoreCondition, ConcurrentBag<HIS_TREATMENT> bag)
        {
            // Moi lo mot CommonParam + mot BackendAdapter rieng: dung chung giua cac luong la khong an toan.
            CommonParam param = new CommonParam();
            try
            {
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.IDs = batchIds;
                if (applyMoreCondition != null) applyMoreCondition(filter);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                List<HIS_TREATMENT> part = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                sw.Stop();

                if (part == null)
                {
                    LogSystem.Warn(LOG_PREFIX + " __ " + caller + " __ lo " + no + "/" + total
                        + " TRA VE NULL (api loi, het phien, hoac URL qua dai): id=" + batchIds.Count
                        + ", id dau=" + batchIds[0] + ", id cuoi=" + batchIds[batchIds.Count - 1]
                        + ", hasException=" + param.HasException
                        + ", het=" + sw.ElapsedMilliseconds + " ms");
                    return false;
                }

                foreach (var treatment in part)
                {
                    if (treatment != null) bag.Add(treatment);
                }
                LogSystem.Debug(LOG_PREFIX + " __ " + caller + " __ lo " + no + "/" + total
                    + ": id=" + batchIds.Count + ", ban ghi=" + part.Count
                    + ", het=" + sw.ElapsedMilliseconds + " ms");
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(LOG_PREFIX + " __ " + caller + " __ lo " + no + "/" + total
                    + " NEM LOI: id=" + batchIds.Count
                    + ", id dau=" + batchIds[0] + ", id cuoi=" + batchIds[batchIds.Count - 1]
                    + ", hasException=" + param.HasException
                    + ", loi=" + ex.Message);
                LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>Loai ID trung / khong hop le roi chia thanh cac lo ID_BATCH_SIZE phan tu.</summary>
        private static List<List<long>> SplitIdBatches(List<long> ids)
        {
            List<List<long>> batches = new List<List<long>>();
            if (ids == null || ids.Count == 0) return batches;
            List<long> distinctIds = ids.Where(o => o > 0).Distinct().ToList();
            for (int skip = 0; skip < distinctIds.Count; skip += ID_BATCH_SIZE)
            {
                batches.Add(distinctIds.Skip(skip).Take(ID_BATCH_SIZE).ToList());
            }
            return batches;
        }
    }
}

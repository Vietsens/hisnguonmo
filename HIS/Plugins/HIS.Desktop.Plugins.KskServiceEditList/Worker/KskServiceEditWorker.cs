using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using HIS.Desktop.Plugins.KskServiceEditList.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.KskServiceEditList.Worker
{
    /// <summary>
    /// Gọi API api/HisKskContract/ServiceEdit theo lô 200 hồ sơ, tuần tự, gộp kết quả.
    /// </summary>
    internal class KskServiceEditWorker
    {
        /// <summary>
        /// baseSdo: thông tin chung (thao tác, người chỉ định...). TreatmentIds được gán lại theo từng lô.
        /// </summary>
        internal KskServiceEditBatchResultADO Run(HisKskServiceEditSDO baseSdo, List<V_HIS_TREATMENT_4> treatments)
        {
            KskServiceEditBatchResultADO total = new KskServiceEditBatchResultADO();
            try
            {
                int size = KskServiceEditConstant.BATCH_SIZE;
                int batchCount = (int)Math.Ceiling((double)treatments.Count / size);
                total.Summary.TotalTreatment = treatments.Count;
                for (int i = 0; i < batchCount; i++)
                {
                    List<V_HIS_TREATMENT_4> batch = treatments.Skip(i * size).Take(size).ToList();
                    HisKskServiceEditSDO sdo = this.CloneWithTreatmentIds(baseSdo, batch.Select(o => o.ID).ToList());

                    Inventec.Common.Logging.LogSystem.Debug("ServiceEdit lo " + (i + 1) + "/" + batchCount
                        + Inventec.Common.Logging.LogUtil.TraceData("TreatmentCount", batch.Count));

                    CommonParam param = new CommonParam();
                    var rs = new BackendAdapter(param).PostRO<HisKskServiceEditSDO>(
                        HisRequestUriStore.HIS_KSK_CONTRACT__SERVICE_EDIT, ApiConsumers.MosConsumer, sdo, param);
                    SessionManager.ProcessTokenLost(param);
                    // Mất phiên (ProcessTokenLostBase đặt cả 2 cờ) -> dừng các lô còn lại
                    if (GlobalVariables.IsLostToken && GlobalVariables.isLogouter)
                    {
                        total.NotProcessedCount += treatments.Count - i * size;
                        break;
                    }

                    if (rs != null && rs.Data != null && rs.Data.Results != null && (rs.Data.Results.Any() || rs.Success))
                    {
                        this.Merge(total, rs.Data);
                    }
                    else
                    {
                        this.MarkBatchError(total, batch, param);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return total;
        }

        /// <summary>
        /// Lấy dịch vụ (sere_serv) còn hiệu lực của các hồ sơ, chia lô theo TREATMENT_IDs.
        /// Bỏ thuốc/vật tư, dịch vụ đã xóa, dịch vụ "không thực hiện".
        /// </summary>
        internal List<HIS_SERE_SERV> GetSereServs(List<long> treatmentIds)
        {
            List<HIS_SERE_SERV> result = new List<HIS_SERE_SERV>();
            try
            {
                int size = KskServiceEditConstant.QUERY_CHUNK_SIZE;
                for (int i = 0; i < treatmentIds.Count; i += size)
                {
                    MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                    filter.TREATMENT_IDs = treatmentIds.Skip(i).Take(size).ToList();
                    CommonParam param = new CommonParam();
                    List<HIS_SERE_SERV> data = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(
                        HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, param);
                    SessionManager.ProcessTokenLost(param);
                    if (data != null) result.AddRange(data);
                }
                result = result.Where(o => o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE
                    && o.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.SERVICE_REQ_ID.HasValue
                    && !o.MEDICINE_ID.HasValue
                    && !o.MATERIAL_ID.HasValue).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HisKskServiceEditSDO CloneWithTreatmentIds(HisKskServiceEditSDO baseSdo, List<long> treatmentIds)
        {
            HisKskServiceEditSDO sdo = new HisKskServiceEditSDO();
            sdo.KskContractId = baseSdo.KskContractId;
            sdo.RequestRoomId = baseSdo.RequestRoomId;
            sdo.Loginname = baseSdo.Loginname;
            sdo.Username = baseSdo.Username;
            sdo.IntructionTime = baseSdo.IntructionTime;
            sdo.AddServices = baseSdo.AddServices;
            sdo.DeleteServiceIds = baseSdo.DeleteServiceIds;
            sdo.ChangeRooms = baseSdo.ChangeRooms;
            sdo.TreatmentIds = treatmentIds;
            return sdo;
        }

        private void Merge(KskServiceEditBatchResultADO total, HisKskServiceEditSDO data)
        {
            total.Results.AddRange(data.Results);
            KskServiceEditSummarySDO s = data.Summary;
            if (s == null) return;
            total.Summary.SuccessTreatment += s.SuccessTreatment;
            total.Summary.ErrorTreatment += s.ErrorTreatment;
            total.Summary.AddedCount += s.AddedCount;
            total.Summary.SkippedDuplicateCount += s.SkippedDuplicateCount;
            total.Summary.DeletedCount += s.DeletedCount;
            total.Summary.NotDeletedCount += s.NotDeletedCount;
            total.Summary.ChangedRoomCount += s.ChangedRoomCount;
            total.Summary.NotChangedRoomCount += s.NotChangedRoomCount;
        }

        /// <summary>
        /// Lô lỗi toàn bộ (lỗi kết nối, backend từ chối cả lô): đánh dấu mọi hồ sơ trong lô là lỗi, xử lý tiếp lô sau.
        /// </summary>
        private void MarkBatchError(KskServiceEditBatchResultADO total, List<V_HIS_TREATMENT_4> batch, CommonParam param)
        {
            string message = HIS.Desktop.LibraryMessage.MessageUtil.GetMessageAlert(param);
            if (String.IsNullOrWhiteSpace(message)) message = Resources.ResourceMessage.LoiKetNoiChuaXuLy;
            total.Messages.Add(message);
            total.Summary.ErrorTreatment += batch.Count;
            foreach (V_HIS_TREATMENT_4 t in batch)
            {
                KskServiceEditResultSDO r = new KskServiceEditResultSDO();
                r.TreatmentId = t.ID;
                r.TreatmentCode = t.TREATMENT_CODE;
                r.PatientName = t.TDL_PATIENT_NAME;
                r.ResultType = (short)EnumKskServiceEditResultType.Error;
                r.Descriptions = new List<string> { message };
                total.Results.Add(r);
            }
            Inventec.Common.Logging.LogSystem.Warn("ServiceEdit lo loi toan bo." + Inventec.Common.Logging.LogUtil.TraceData("Message", message));
        }
    }
}

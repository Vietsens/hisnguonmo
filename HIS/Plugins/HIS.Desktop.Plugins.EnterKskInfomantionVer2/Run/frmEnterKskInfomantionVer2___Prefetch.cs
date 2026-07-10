/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>
    /// Tối ưu tốc độ mở form: nạp SONG SONG (Task.WaitAll, 3 đợt) toàn bộ dữ liệu mà các
    /// FillDataPage* cần, TRƯỚC khi bind UI — thay cho ~25 lời gọi API tuần tự trên UI thread.
    /// Các FillData* đọc cache pre*; cache null (prefetch lỗi/chưa chạy) thì fallback gọi API
    /// như code cũ — hành vi không đổi. Cache sống theo vòng đời form (FillDataToPages chỉ chạy 1 lần ở Load).
    /// </summary>
    public partial class frmEnterKskInfomantionVer2
    {
        // ===== Đợt 1: bản ghi KSK theo SERVICE_REQ_ID + hồ sơ điều trị/bé/hợp đồng =====
        private List<HIS_KSK_GENERAL> preKskGenerals;
        private List<HIS_KSK_OCCUPATIONAL> preKskOccupationals;
        private List<HIS_KSK_OVER_EIGHTEEN> preKskOverEighteens;
        private List<HIS_KSK_UNDER_EIGHTEEN> preKskUnderEighteens;
        private List<HIS_KSK_DRIVER_CAR> preKskDriverCars;
        private List<HIS_KSK_PERIOD_DRIVER> preKskPeriodDrivers;
        private List<HIS_KSK_OTHER> preKskOthers;
        private List<HIS_KSK_UNDER_SIX> preKskUnderSixes;
        private List<HIS_TREATMENT> preTreatments;
        private List<V_HIS_BABY> preBabies;
        private List<HIS_KSK_CONTRACT> preKskContracts;
        // ===== Đợt 2: DHST (gom mọi DHST_ID) + tiêm chủng 6-18 + tiền sử (≥18, lái xe) =====
        private Dictionary<long, HIS_DHST> preDhstById;
        private List<HIS_KSK_UNEI_VATY> preUneiVatys;
        private List<HIS_PERIOD_DRIVER_DITY> preDitysOverE;
        private List<HIS_PERIOD_DRIVER_DITY> preDitysPeriodDriver;
        // ===== Đợt 3: danh mục theo ID xuất hiện ở đợt 2 =====
        private List<HIS_VACCINE_TYPE> preVaccineTypes;
        private List<HIS_DISEASE_TYPE> preDiseaseTypesOverE;
        private List<HIS_DISEASE_TYPE> preDiseaseTypesPeriodDriver;

        /// <summary>
        /// Nạp trước toàn bộ dữ liệu form (gọi 1 lần đầu frm_Load, trước FillDataToPages).
        /// Thay ~25 call tuần tự bằng 1 call gộp api/HisKskSync/GetKskData (SDO chứa cả 3 đợt).
        /// Chỉ V_HIS_BABY không có trong SDO nên gọi riêng (song song). Lỗi call gộp -> pre* null,
        /// FillData* tự fallback gọi API lẻ như code cũ -> hành vi không đổi.
        /// </summary>
        private void PrefetchFormData()
        {
            try
            {
                if (currentServiceReq == null) return;
                long serviceReqId = currentServiceReq.ID;
                long treatmentId = currentServiceReq.TREATMENT_ID;

                // .NET Framework mặc định CHỈ cho 2 kết nối HTTP đồng thời/host -> các task song song
                // bị xếp hàng từng đôi (chậm hơn cả tuần tự vì thêm chi phí chờ). Nâng giới hạn client.
                if (System.Net.ServicePointManager.DefaultConnectionLimit < 20)
                    System.Net.ServicePointManager.DefaultConnectionLimit = 20;

                var sw = System.Diagnostics.Stopwatch.StartNew();

                // ===== 1 call gộp lấy toàn bộ + call V_HIS_BABY riêng (chạy song song) =====
                MOS.SDO.HisKskDataSDO sdo = null;
                var tasks = new List<System.Threading.Tasks.Task>();
                tasks.Add(StartLong(() =>
                    sdo = GetKskDataSdo(new MOS.Filter.HisKskDataFilter
                    {
                        SERVICE_REQ_ID = serviceReqId,
                        TREATMENT_ID = treatmentId
                    })));
                if (treatmentId > 0)
                    tasks.Add(StartLong(() =>
                        preBabies = GetListPre<V_HIS_BABY>("api/HisBaby/GetView", new HisBabyFilter { TREATMENT_ID = treatmentId })));
                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

                // ===== Đổ pre* trực tiếp từ SDO (không tách đợt vì đã lấy 1 lần) =====
                if (sdo != null)
                {
                    preKskGenerals = sdo.HisKskGenerals;
                    preKskOccupationals = sdo.HisKskOccupationals;
                    preKskOverEighteens = sdo.HisKskOverEighteens;
                    preKskUnderEighteens = sdo.HisKskUnderEighteens;
                    preKskDriverCars = sdo.HisKskDriverCars;
                    preKskPeriodDrivers = sdo.HisKskPeriodDrivers;
                    preKskOthers = sdo.HisKskOthers;
                    preKskUnderSixes = sdo.HisKskUnderSixs;
                    preTreatments = sdo.HisTreatments;
                    preKskContracts = sdo.HisKskContracts;
                    preUneiVatys = sdo.HisKskUneiVatys;
                    preVaccineTypes = sdo.HisVaccineTypes;

                    // DHST: gom về Dictionary theo ID (call site đọc PreGetDhst).
                    if (sdo.HisDhsts != null)
                    {
                        preDhstById = new Dictionary<long, HIS_DHST>();
                        foreach (var d in sdo.HisDhsts)
                            if (d != null && !preDhstById.ContainsKey(d.ID)) preDhstById[d.ID] = d;
                    }

                    // Tiền sử bệnh: SDO gộp chung 1 list -> tách theo khóa (≥18 / lái xe định kỳ).
                    var o18 = (preKskOverEighteens != null) ? preKskOverEighteens.FirstOrDefault() : null;
                    var driver = (preKskPeriodDrivers != null) ? preKskPeriodDrivers.FirstOrDefault() : null;
                    if (sdo.HisPeriodDriverDitys != null)
                    {
                        if (o18 != null)
                            preDitysOverE = sdo.HisPeriodDriverDitys
                                .Where(x => x != null && x.KSK_OVER_EIGHTEEN_ID == o18.ID).ToList();
                        if (driver != null)
                            preDitysPeriodDriver = sdo.HisPeriodDriverDitys
                                .Where(x => x != null && x.KSK_PERIOD_DRIVER_ID == driver.ID).ToList();
                    }

                    // Danh mục bệnh: tách theo DISEASE_TYPE_ID mà từng nhóm dity tham chiếu.
                    if (sdo.HisDiseaseTypes != null)
                    {
                        if (preDitysOverE != null && preDitysOverE.Count > 0)
                        {
                            var ids = preDitysOverE.Select(o => o.DISEASE_TYPE_ID).ToList();
                            preDiseaseTypesOverE = sdo.HisDiseaseTypes.Where(d => d != null && ids.Contains(d.ID)).ToList();
                        }
                        if (preDitysPeriodDriver != null && preDitysPeriodDriver.Count > 0)
                        {
                            var ids = preDitysPeriodDriver.Select(o => o.DISEASE_TYPE_ID).ToList();
                            preDiseaseTypesPeriodDriver = sdo.HisDiseaseTypes.Where(d => d != null && ids.Contains(d.ID)).ToList();
                        }
                    }
                }

                sw.Stop();
                Inventec.Common.Logging.LogSystem.Debug("EnterKskInfomantionVer2.PrefetchFormData: "
                    + sw.ElapsedMilliseconds + " ms (1 call GetKskData, sdo="
                    + (sdo != null) + ", DefaultConnectionLimit="
                    + System.Net.ServicePointManager.DefaultConnectionLimit + ")");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi API gộp api/HisKskSync/GetKskData -> HisKskDataSDO (chứa toàn bộ dữ liệu 1 lượt KSK).
        /// Lỗi trả null -> PrefetchFormData không set pre* -> các FillData* tự fallback gọi API lẻ.
        /// </summary>
        private static MOS.SDO.HisKskDataSDO GetKskDataSdo(MOS.Filter.HisKskDataFilter filter)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<MOS.SDO.HisKskDataSDO>(
                    "api/HisKskSync/GetKskData", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Chạy task IO-blocking trên THREAD RIÊNG (LongRunning) — tránh nghẽn ThreadPool:
        /// StartNew mặc định dùng ThreadPool (min thread = số core), 11 task blocking cùng lúc
        /// làm ThreadPool phải bơm thêm thread ~500ms/cái → prefetch chậm hơn cả gọi tuần tự.
        /// </summary>
        private static System.Threading.Tasks.Task StartLong(Action action)
        {
            return System.Threading.Tasks.Task.Factory.StartNew(action,
                System.Threading.CancellationToken.None,
                System.Threading.Tasks.TaskCreationOptions.LongRunning,
                System.Threading.Tasks.TaskScheduler.Default);
        }

        /// <summary>Gọi API danh sách cho prefetch — lỗi trả null (call site fallback API như cũ).</summary>
        private static List<T> GetListPre<T>(string uri, object filter)
        {
            try
            {
                var param = new CommonParam();
                return new BackendAdapter(param).Get<List<T>>(uri, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Lấy DHST theo ID từ cache SDO (api/HisKskSync/GetKskData đã trả toàn bộ DHST liên quan).
        /// Trả về danh sách để khớp chỗ dùng cũ; không có trong cache → null (không gọi API lẻ).
        /// </summary>
        private List<HIS_DHST> PreGetDhst(long? dhstId)
        {
            if (dhstId == null || dhstId <= 0) return null;
            HIS_DHST hit;
            if (preDhstById != null && preDhstById.TryGetValue(dhstId.Value, out hit))
                return new List<HIS_DHST> { hit };
            return null;
        }
    }
}

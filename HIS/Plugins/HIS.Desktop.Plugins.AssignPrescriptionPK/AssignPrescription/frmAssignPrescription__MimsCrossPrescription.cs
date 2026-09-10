/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 * All rights reserved.
 * Việc 52540 — Kiểm tra tương tác thuốc giữa các đơn khác nhau của hồ sơ bằng MIMS.
 * Lấy thuốc còn hiệu lực của các đơn KHÁC trong hồ sơ (hoặc toàn lịch sử bệnh nhân)
 * để đưa vào phép kiểm tra tương tác cùng thuốc đơn đang kê.
 */
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        #region Fields

        /// <summary>
        /// Thuốc thô của các đơn KHÁC trong phạm vi cấu hình (prefetch async khi mở form/đổi giờ chỉ định).
        /// </summary>
        List<V_HIS_EXP_MEST_MEDICINE> crossPrescriptionMedicines;

        /// <summary>true = đã nạp xong (kể cả kết quả rỗng).</summary>
        bool isCrossPrescriptionLoaded = false;

        /// <summary>
        /// Khoá cache = giờ chỉ định đã nạp. Đổi ngày/giờ chỉ định thì phải nạp lại
        /// vì điều kiện "thuốc còn hiệu lực" tính theo mốc thời gian này.
        /// </summary>
        long crossPrescriptionLoadedKey = 0;

        #endregion

        /// <summary>
        /// Prefetch bất đồng bộ thuốc các đơn khác — chỉ chạy khi dùng MIMS và bật phạm vi chéo đơn.
        /// Không chặn UI, không thao tác control trong thread.
        /// </summary>
        private void PrefetchMimsCrossPrescription()
        {
            try
            {
                if (HisConfigCFG.ConnectDrugInterventionInfo != "2"
                    || !HisConfigCFG.IsCheckMimsCrossPrescription)
                    return;
                if (this.currentTreatmentWithPatientType == null)
                    return;

                long instructionTime = this.InstructionTime;
                if (instructionTime <= 0)
                    return;
                if (this.isCrossPrescriptionLoaded && this.crossPrescriptionLoadedKey == instructionTime)
                    return;

                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        var data = GetCrossPrescriptionMedicines(instructionTime);
                        this.crossPrescriptionMedicines = data;
                        this.crossPrescriptionLoadedKey = instructionTime;
                        this.isCrossPrescriptionLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi API lấy thuốc các đơn khác theo phạm vi cấu hình (QT-02, QT-03).
        /// Lỗi API → trả về danh sách rỗng, KHÔNG chặn luồng lưu đơn (QT-21).
        /// </summary>
        private List<V_HIS_EXP_MEST_MEDICINE> GetCrossPrescriptionMedicines(long instructionTime)
        {
            var result = new List<V_HIS_EXP_MEST_MEDICINE>();
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();

                if (HisConfigCFG.MimsInteractionScopeOption == HisConfigCFG.MIMS_INTERACTION_SCOPE__PATIENT)
                {
                    filter.TDL_PATIENT_ID = this.currentTreatmentWithPatientType.PATIENT_ID;
                }
                else
                {
                    filter.TDL_TREATMENT_ID = this.currentTreatmentWithPatientType.ID;
                }

                filter.IS_INCLUDE_DELETED = false;
                filter.DATA_DOMAIN_FILTER = false;
                // Giới hạn khoảng ngày ngay tại API để giảm khối lượng dữ liệu trả về
                filter.TDL_INTRUCTION_TIME_FROM = GetCrossPrescriptionMinIntructionTime(instructionTime);

                Inventec.Common.Logging.LogSystem.Debug(
                    "GetCrossPrescriptionMedicines - INPUT"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

                var data = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(
                    HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer,
                    filter, ProcessLostToken, param);

                if (data != null)
                    result = data;

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "GetCrossPrescriptionMedicines - so ban ghi tra ve = {0}", result.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Mốc thời gian chỉ định nhỏ nhất được lấy = giờ chỉ định lùi lại số ngày cấu hình,
        /// về 00:00:00 của ngày đó.
        /// </summary>
        private long GetCrossPrescriptionMinIntructionTime(long instructionTime)
        {
            try
            {
                var instructionDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(instructionTime);
                if (instructionDateTime == null)
                    return 0;

                var minDate = instructionDateTime.Value.Date.AddDays(-HisConfigCFG.MimsPreviousPrescriptionDayRange);
                return Inventec.Common.TypeConvert.Parse.ToInt64(minDate.ToString("yyyyMMdd") + "000000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        /// <summary>
        /// Dựng danh sách thuốc đơn khác gửi MIMS (QT-04 → QT-07) và thông tin nguồn đơn để hiển thị.
        /// currentDrugs = thuốc đang xét của đơn hiện tại (toàn grid khi lưu, dòng đang chọn khi chuột phải).
        /// Trả về danh sách rỗng khi tắt cấu hình → thư viện chạy đúng nhánh như trước việc 52540.
        /// </summary>
        private List<DrugItem> BuildCrossPrescriptionDrugItems(List<MediMatyTypeADO> currentDrugs,
            out List<MimsPreviousDrugInfo> previousDrugInfos)
        {
            previousDrugInfos = new List<MimsPreviousDrugInfo>();
            var drugItems = new List<DrugItem>();
            try
            {
                if (HisConfigCFG.ConnectDrugInterventionInfo != "2"
                    || !HisConfigCFG.IsCheckMimsCrossPrescription
                    || this.currentTreatmentWithPatientType == null)
                    return drugItems;

                long instructionTime = this.InstructionTime;
                if (instructionTime <= 0)
                    return drugItems;

                // Prefetch chưa xong hoặc đã cũ so với giờ chỉ định hiện tại → lấy đồng bộ 1 lần
                if (!this.isCrossPrescriptionLoaded || this.crossPrescriptionLoadedKey != instructionTime)
                {
                    this.crossPrescriptionMedicines = GetCrossPrescriptionMedicines(instructionTime);
                    this.crossPrescriptionLoadedKey = instructionTime;
                    this.isCrossPrescriptionLoaded = true;
                }

                var source = this.crossPrescriptionMedicines;
                if (source == null || source.Count == 0)
                    return drugItems;

                var currentCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (currentDrugs != null)
                {
                    foreach (var item in currentDrugs)
                    {
                        if (item != null && !string.IsNullOrWhiteSpace(item.MEDICINE_TYPE_CODE))
                            currentCodes.Add(item.MEDICINE_TYPE_CODE);
                    }
                }

                long oldServiceReqId = this.oldServiceReq != null ? this.oldServiceReq.ID : 0;
                long minIntructionTime = GetCrossPrescriptionMinIntructionTime(instructionTime);

                var valid = source
                    .Where(o => o.IS_DELETE != 1
                        && !string.IsNullOrWhiteSpace(o.MEDICINE_TYPE_CODE)
                        && !currentCodes.Contains(o.MEDICINE_TYPE_CODE)
                        && (oldServiceReqId <= 0 || o.TDL_SERVICE_REQ_ID != oldServiceReqId)
                        && (o.USE_TIME_TO != null && o.USE_TIME_TO > 0
                                ? o.USE_TIME_TO.Value >= instructionTime
                                : (o.TDL_INTRUCTION_TIME ?? 0) >= minIntructionTime))
                    .GroupBy(o => o.MEDICINE_TYPE_CODE, StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.OrderByDescending(o => o.USE_TIME_TO ?? 0).First())
                    .OrderByDescending(o => o.USE_TIME_TO ?? 0)
                    .ToList();

                int totalValid = valid.Count;
                if (totalValid > HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG)
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "BuildCrossPrescriptionDrugItems - cat {0}/{1} thuoc don khac do vuot gioi han {2}",
                        totalValid - HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG, totalValid,
                        HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG));
                    valid = valid.Take(HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG).ToList();
                }

                var departmentDic = GetDepartmentNameDictionary();

                foreach (var item in valid)
                {
                    drugItems.Add(new DrugItem(item.MEDICINE_TYPE_CODE, item.MEDICINE_TYPE_NAME, null, MimsType.GenericItem));

                    string departmentName = null;
                    if (departmentDic != null)
                        departmentDic.TryGetValue(item.REQ_DEPARTMENT_ID, out departmentName);

                    previousDrugInfos.Add(new MimsPreviousDrugInfo
                    {
                        HisDrugCode = item.MEDICINE_TYPE_CODE,
                        DrugName = item.MEDICINE_TYPE_NAME,
                        ExpMestCode = item.EXP_MEST_CODE,
                        RequestUserName = item.REQ_USERNAME,
                        DepartmentName = departmentName,
                        IntructionTime = item.TDL_INTRUCTION_TIME,
                        UseTimeTo = item.USE_TIME_TO
                    });
                }

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "BuildCrossPrescriptionDrugItems - scope={0}, tho={1}, hop le={2}, gui MIMS={3}",
                    HisConfigCFG.MimsInteractionScopeOption, source.Count, totalValid, drugItems.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return drugItems;
        }

        /// <summary>
        /// Từ điển Id khoa → tên khoa, lấy 1 lần từ cache để không tra trong vòng lặp.
        /// </summary>
        private Dictionary<long, string> GetDepartmentNameDictionary()
        {
            try
            {
                var departments = BackendDataWorker.Get<HIS_DEPARTMENT>();
                if (departments == null || departments.Count == 0)
                    return null;

                var result = new Dictionary<long, string>();
                foreach (var department in departments)
                {
                    if (department == null || result.ContainsKey(department.ID))
                        continue;
                    result.Add(department.ID, department.DEPARTMENT_NAME);
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Dựng cấu hình truyền xuống thư viện MIMS. Tiêu đề/mẫu chữ lấy từ resx của plugin;
        /// nếu resx trả rỗng (thiếu satellite khi deploy DLL lẻ) thì thư viện tự dùng mặc định.
        /// </summary>
        private MimsCrossPrescriptionOption BuildMimsCrossPrescriptionOption()
        {
            try
            {
                return new MimsCrossPrescriptionOption
                {
                    RequestMode = HisConfigCFG.MimsCrossPrescriptionRequestMode,
                    UseAlertFilterByDrug = true,
                    BannerTitle = Resources.ResourceMessage.ThuocDangDungTuDonKhacTrongHoSo,
                    SourceFormat = Resources.ResourceMessage.DonNgayDungDenNgay
                };
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new MimsCrossPrescriptionOption();
            }
        }
    }
}

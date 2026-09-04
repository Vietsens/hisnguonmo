/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000510.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000510
{
    /// <summary>
    /// LOG CHẨN ĐOÁN (tạm thời) cho ca "bảng kê 510/511 in ra rỗng".
    /// ProcessData return false khi sereServADOs rỗng -> KHÔNG in gì và KHÔNG có thông báo lỗi,
    /// nên phải đo đếm từng chặng: dữ liệu vào (rdo.SereServs) -> map ADO -> lọc khoa -> 4 đk cơ bản
    /// (AMOUNT/PRICE/IS_EXPEND/IS_NO_EXECUTE) -> đk đối tượng (PATIENT_TYPE_ID != BHYT) -> 3 bộ gom.
    ///
    /// Ghi ở mức ERROR có chủ đích (tạm thời): chắc chắn xuống file với mọi cấu hình log4net
    /// (kể cả môi trường khách hàng bị nâng level/threshold appender lên ERROR) và dễ lọc.
    /// Xong việc thì đổi 1 dòng trong hàm Diag sang LogSystem.Debug hoặc xoá cả file này.
    ///
    /// Xem log:  findstr /C:"MPS510_DIAG" Logs\LogSystem.txt
    /// </summary>
    public partial class Mps000510Processor : AbstractProcessor
    {
        private const string DIAG = "MPS510_DIAG";

        /// <summary>Số dòng mẫu in chi tiết ra log (tránh phình file log).</summary>
        private const int DIAG_SAMPLE_ROW = 10;

        private static void Diag(string message)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error(DIAG + " | " + message);
            }
            catch { }
        }

        private static string Num(object v)
        {
            return v == null ? "null" : v + "";
        }

        /// <summary>Dữ liệu đầu vào của PDO: thiếu master data hay thiếu hẳn sere_serv.</summary>
        private void DiagInputData()
        {
            try
            {
                if (rdo == null)
                {
                    Diag("[1-INPUT] rdo = NULL -> không có dữ liệu đầu vào.");
                    return;
                }

                Diag("[1-INPUT] TREATMENT_CODE=" + (rdo.Treatment != null ? rdo.Treatment.TREATMENT_CODE : "null")
                    + " ; TREATMENT_ID=" + (rdo.Treatment != null ? Num(rdo.Treatment.ID) : "null")
                    + " ; GroupType=" + rdo.GroupType
                    + " ; FilterDepartmentId=" + Num(rdo.FilterDepartmentId));

                Diag("[1-INPUT] rdo.SereServs=" + (rdo.SereServs == null ? "NULL" : rdo.SereServs.Count + "")
                    + " ; SereServExts=" + (rdo.SereServExts == null ? "NULL" : rdo.SereServExts.Count + "")
                    + " ; Services=" + (rdo.Services == null ? "NULL" : rdo.Services.Count + "")
                    + " ; Rooms=" + (rdo.Rooms == null ? "NULL" : rdo.Rooms.Count + "")
                    + " ; Departments=" + (rdo.Departments == null ? "NULL" : rdo.Departments.Count + "")
                    + " ; HeinServiceTypes=" + (rdo.HeinServiceTypes == null ? "NULL" : rdo.HeinServiceTypes.Count + "")
                    + " ; MedicineLines=" + (rdo.MedicineLines == null ? "NULL" : rdo.MedicineLines.Count + "")
                    + " ; HisServiceUnit=" + (rdo.HisServiceUnit == null ? "NULL" : rdo.HisServiceUnit.Count + "")
                    + " ; medicineTypes=" + (rdo.medicineTypes == null ? "NULL" : rdo.medicineTypes.Count + "")
                    + " ; ServiceReqs=" + (rdo.ServiceReqs == null ? "NULL" : rdo.ServiceReqs.Count + "")
                    + " ; ListOtherPaySource=" + (rdo.ListOtherPaySource == null ? "NULL" : rdo.ListOtherPaySource.Count + ""));

                if (rdo.PatientTypeCFG == null)
                {
                    // Cảnh báo nặng: mọi lượt lọc/gom đều so PATIENT_TYPE_ID với PatientTypeCFG
                    // -> null sẽ ném NullReference và bị catch, kết quả là danh sách rỗng.
                    Diag("[1-INPUT] *** rdo.PatientTypeCFG = NULL -> các bước lọc theo đối tượng sẽ lỗi/rỗng.");
                }
                else
                {
                    Diag("[1-INPUT] PatientTypeCFG.PATIENT_TYPE__BHYT=" + Num(rdo.PatientTypeCFG.PATIENT_TYPE__BHYT)
                        + " ; PATIENT_TYPE__FEE=" + Num(rdo.PatientTypeCFG.PATIENT_TYPE__FEE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Đếm trực tiếp trên dữ liệu THÔ của view (trước khi map/ComputePrices ghi đè PRICE, AMOUNT).
        /// Dùng để phân biệt "cổng lấy dữ liệu trả về 0 dòng" với "map/ComputePrices làm dòng bị loại".
        /// </summary>
        private void DiagRawSereServ()
        {
            try
            {
                if (rdo == null || rdo.SereServs == null)
                {
                    Diag("[2-RAW] rdo.SereServs = NULL -> API lấy V_HIS_SERE_SERV_2 không trả dữ liệu.");
                    return;
                }

                List<V_HIS_SERE_SERV_2> rows = rdo.SereServs;
                if (rows.Count == 0)
                {
                    Diag("[2-RAW] rdo.SereServs.Count = 0 -> kiểm tra api/HisSereServ/GetView2 + bước lọc theo danh sách chọn ở Behavior.");
                    return;
                }

                long? bhytId = rdo.PatientTypeCFG != null ? rdo.PatientTypeCFG.PATIENT_TYPE__BHYT : null;

                Diag("[2-RAW] Count=" + rows.Count
                    + " ; AMOUNT<=0: " + rows.Count(o => !(o.AMOUNT > 0))
                    + " ; PRICE<=0: " + rows.Count(o => !(o.PRICE > 0))
                    + " ; IS_EXPEND=1: " + rows.Count(o => o.IS_EXPEND == 1)
                    + " ; IS_NO_EXECUTE=1: " + rows.Count(o => o.IS_NO_EXECUTE == 1)
                    + " ; PATIENT_TYPE_ID=BHYT(" + Num(bhytId) + "): " + rows.Count(o => o.PATIENT_TYPE_ID == bhytId)
                    + " ; PATIENT_TYPE_ID!=BHYT: " + rows.Count(o => o.PATIENT_TYPE_ID != bhytId));

                // Bẫy đã biết: ComputePrices gán PRICE = (LIMIT_PRICE ?? 0) khi có PRIMARY_PATIENT_TYPE_ID
                // -> LIMIT_PRICE null/0 làm PRICE = 0 và dòng bị đk PRICE>0 loại sạch.
                Diag("[2-RAW] PRIMARY_PATIENT_TYPE_ID có giá trị: " + rows.Count(o => o.PRIMARY_PATIENT_TYPE_ID.HasValue)
                    + " ; trong đó LIMIT_PRICE null: " + rows.Count(o => o.PRIMARY_PATIENT_TYPE_ID.HasValue && !o.LIMIT_PRICE.HasValue)
                    + " ; LIMIT_PRICE<=0: " + rows.Count(o => o.PRIMARY_PATIENT_TYPE_ID.HasValue && o.LIMIT_PRICE.HasValue && o.LIMIT_PRICE.Value <= 0));

                foreach (var g in rows.GroupBy(o => o.PATIENT_TYPE_ID + ""))
                {
                    Diag("[2-RAW] PATIENT_TYPE_ID=" + g.Key + " -> " + g.Count() + " dòng");
                }

                int i = 0;
                foreach (V_HIS_SERE_SERV_2 o in rows)
                {
                    if (i++ >= DIAG_SAMPLE_ROW)
                        break;
                    Diag("[2-RAW-ROW] " + i
                        + " SS_ID=" + Num(o.ID)
                        + " SERVICE_ID=" + Num(o.SERVICE_ID)
                        + " NAME=" + o.TDL_SERVICE_NAME
                        + " AMOUNT=" + Num(o.AMOUNT)
                        + " PRICE=" + Num(o.PRICE)
                        + " VIR_PRICE=" + Num(o.VIR_PRICE)
                        + " ORIGINAL_PRICE=" + Num(o.ORIGINAL_PRICE)
                        + " LIMIT_PRICE=" + Num(o.LIMIT_PRICE)
                        + " HEIN_LIMIT_PRICE=" + Num(o.HEIN_LIMIT_PRICE)
                        + " PATIENT_TYPE_ID=" + Num(o.PATIENT_TYPE_ID)
                        + " PRIMARY_PATIENT_TYPE_ID=" + Num(o.PRIMARY_PATIENT_TYPE_ID)
                        + " IS_EXPEND=" + Num(o.IS_EXPEND)
                        + " IS_NO_EXECUTE=" + Num(o.IS_NO_EXECUTE)
                        + " VIR_TOTAL_HEIN_PRICE=" + Num(o.VIR_TOTAL_HEIN_PRICE)
                        + " VIR_TOTAL_PRICE_NO_EXPEND=" + Num(o.VIR_TOTAL_PRICE_NO_EXPEND)
                        + " REQ_ROOM=" + Num(o.TDL_REQUEST_ROOM_ID)
                        + " EXE_ROOM=" + Num(o.TDL_EXECUTE_ROOM_ID)
                        + " REQ_DEPA=" + Num(o.TDL_REQUEST_DEPARTMENT_ID)
                        + " EXE_DEPA=" + Num(o.TDL_EXECUTE_DEPARTMENT_ID));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bóc tách từng điều kiện lọc trên danh sách ADO: đếm số dòng TRƯỢT mỗi đk và số dòng
        /// còn lại sau khi ghép đủ đk (đúng như 3 hàm MergeDuplicate* đang dùng).
        /// </summary>
        private void DiagFilterBreakdown(string stage, List<SereServADO> rows)
        {
            try
            {
                if (rows == null)
                {
                    Diag("[" + stage + "] danh sách = NULL");
                    return;
                }
                if (rows.Count == 0)
                {
                    Diag("[" + stage + "] Count = 0");
                    return;
                }

                long? bhytId = rdo != null && rdo.PatientTypeCFG != null ? rdo.PatientTypeCFG.PATIENT_TYPE__BHYT : null;

                int passBase = rows.Count(o => o.AMOUNT > 0 && o.PRICE > 0 && o.IS_EXPEND != 1 && o.IS_NO_EXECUTE != 1);
                int passVienPhi = rows.Count(o => o.AMOUNT > 0 && o.PRICE > 0 && o.IS_EXPEND != 1 && o.IS_NO_EXECUTE != 1
                    && o.PATIENT_TYPE_ID != bhytId);
                int passBhytCoPay = rows.Count(o => o.AMOUNT > 0 && o.PRICE > 0 && o.IS_EXPEND != 1 && o.IS_NO_EXECUTE != 1
                    && o.PATIENT_TYPE_ID == bhytId && (o.VIR_TOTAL_HEIN_PRICE ?? 0) <= 0);

                Diag("[" + stage + "] Count=" + rows.Count
                    + " ; trượt AMOUNT<=0: " + rows.Count(o => !(o.AMOUNT > 0))
                    + " ; trượt PRICE<=0: " + rows.Count(o => !(o.PRICE > 0))
                    + " ; trượt IS_EXPEND=1: " + rows.Count(o => o.IS_EXPEND == 1)
                    + " ; trượt IS_NO_EXECUTE=1: " + rows.Count(o => o.IS_NO_EXECUTE == 1)
                    + " ; ĐẠT 4 đk cơ bản: " + passBase
                    + " ; ĐẠT + không BHYT (đầu vào 3 bộ gom): " + passVienPhi
                    + " ; ĐẠT + BHYT quỹ không chi trả (lượt đang BỊ BỎ): " + passBhytCoPay);

                if (passVienPhi == 0)
                {
                    Diag("[" + stage + "] *** 0 dòng vào bộ gom. Nguyên nhân theo số đếm ở trên"
                        + " (4 đk cơ bản = " + passBase + ", đk PATIENT_TYPE_ID != BHYT(" + Num(bhytId) + ") loại "
                        + (passBase - passVienPhi) + " dòng).");
                }

                foreach (var g in rows.GroupBy(o => o.GROUP_DEPARTMENT_ID))
                {
                    Diag("[" + stage + "] GROUP_DEPARTMENT_ID=" + g.Key
                        + " (" + (g.First().GROUP_DEPARTMENT_NAME ?? "") + ") -> " + g.Count() + " dòng");
                }

                // Mẫu các dòng bị loại + lý do cụ thể
                List<SereServADO> dropped = rows.Where(o => !(o.AMOUNT > 0 && o.PRICE > 0 && o.IS_EXPEND != 1
                    && o.IS_NO_EXECUTE != 1 && o.PATIENT_TYPE_ID != bhytId)).ToList();
                int i = 0;
                foreach (SereServADO o in dropped)
                {
                    if (i++ >= DIAG_SAMPLE_ROW)
                        break;
                    Diag("[" + stage + "-DROP] " + i + " " + DiagRowInfo(o) + " ; LÝ DO: " + DiagDropReason(o, bhytId));
                }
                if (dropped.Count > DIAG_SAMPLE_ROW)
                    Diag("[" + stage + "-DROP] ... còn " + (dropped.Count - DIAG_SAMPLE_ROW) + " dòng bị loại nữa");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string DiagRowInfo(SereServADO o)
        {
            return "SS_ID=" + Num(o.ID)
                + " SERVICE_ID=" + Num(o.SERVICE_ID)
                + " NAME=" + o.SERVICE_NAME
                + " AMOUNT=" + Num(o.AMOUNT)
                + " PRICE=" + Num(o.PRICE)
                + " PRICE_VP=" + Num(o.PRICE_VP)
                + " PATIENT_TYPE_ID=" + Num(o.PATIENT_TYPE_ID)
                + " PRIMARY_PATIENT_TYPE_ID=" + Num(o.PRIMARY_PATIENT_TYPE_ID)
                + " LIMIT_PRICE=" + Num(o.LIMIT_PRICE)
                + " IS_EXPEND=" + Num(o.IS_EXPEND)
                + " IS_NO_EXECUTE=" + Num(o.IS_NO_EXECUTE)
                + " HEIN_SERVICE_TYPE_ID=" + Num(o.HEIN_SERVICE_TYPE_ID)
                + " DEPA=" + o.GROUP_DEPARTMENT_ID
                + " ROOM=" + o.GROUP_ROOM_ID;
        }

        private static string DiagDropReason(SereServADO o, long? bhytId)
        {
            StringBuilder sb = new StringBuilder();
            if (!(o.AMOUNT > 0))
                sb.Append("AMOUNT<=0;");
            if (!(o.PRICE > 0))
            {
                sb.Append("PRICE<=0");
                if (o.PRIMARY_PATIENT_TYPE_ID.HasValue)
                    sb.Append("(do PRIMARY_PATIENT_TYPE_ID có giá trị nên PRICE=LIMIT_PRICE=" + Num(o.LIMIT_PRICE) + ")");
                sb.Append(";");
            }
            if (o.IS_EXPEND == 1)
                sb.Append("IS_EXPEND=1;");
            if (o.IS_NO_EXECUTE == 1)
                sb.Append("IS_NO_EXECUTE=1;");
            if (o.PATIENT_TYPE_ID == bhytId)
                sb.Append("PATIENT_TYPE_ID=BHYT(bảng kê viện phí bỏ dòng BHYT);");
            return sb.Length == 0 ? "<không rõ>" : sb.ToString();
        }

        /// <summary>Số dòng của mọi bộ dữ liệu đẩy sang template - bộ nào rỗng thì band đó không in.</summary>
        private void DiagOutputData(string stage)
        {
            try
            {
                Diag("[" + stage + "] Service(bộ1 khoa+phòng)=" + (sereServADOs == null ? "NULL" : sereServADOs.Count + "")
                    + " ; ServiceByDepa(bộ2 khoa)=" + (sereServADOsByDepa == null ? "NULL" : sereServADOsByDepa.Count + "")
                    + " ; ServiceLoaiDV(bộ3 loại DV)=" + (sereServADOsLoaiDV == null ? "NULL" : sereServADOsLoaiDV.Count + "")
                    + " ; HeinServiceType=" + (heinServiceTypeADOs == null ? "NULL" : heinServiceTypeADOs.Count + "")
                    + " ; HeinServiceTypeByDepa=" + (heinServiceTypeADOs_ByDepa == null ? "NULL" : heinServiceTypeADOs_ByDepa.Count + "")
                    + " ; HeinServiceTypeLoaiDV=" + (heinServiceTypeADOs_LoaiDV == null ? "NULL" : heinServiceTypeADOs_LoaiDV.Count + "")
                    + " ; ServiceGroupByDepa=" + (ServiceGroupByDepa == null ? "NULL" : ServiceGroupByDepa.Count + "")
                    + " ; ServiceGroupByRoom=" + (ServiceGroupByRoom == null ? "NULL" : ServiceGroupByRoom.Count + "")
                    + " ; MedicineLine=" + (medicineLineADOs == null ? "NULL" : medicineLineADOs.Count + "")
                    + " ; HeinServiceTypeBed=" + (HeinServiceTypeBeds == null ? "NULL" : HeinServiceTypeBeds.Count + "")
                    + " ; PatyAlterBHYT=" + (patyAlterBHYTADOs == null ? "NULL" : patyAlterBHYTADOs.Count + ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

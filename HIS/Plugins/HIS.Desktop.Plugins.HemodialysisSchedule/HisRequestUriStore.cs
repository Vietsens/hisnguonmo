/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    internal class HisRequestUriStore
    {
        // CRUD slot lịch chạy thận (HIS_HEMODIALYSIS_SCHEDULE)
        internal const string HEMODIALYSIS_SCHEDULE_GET = "api/HisHemodialysisSchedule/Get";
        internal const string HEMODIALYSIS_SCHEDULE_CREATE_LIST = "api/HisHemodialysisSchedule/CreateList";
        internal const string HEMODIALYSIS_SCHEDULE_UPDATE = "api/HisHemodialysisSchedule/Update";
        internal const string HEMODIALYSIS_SCHEDULE_DELETE = "api/HisHemodialysisSchedule/Delete";
        internal const string HEMODIALYSIS_SCHEDULE_COPY = "api/HisHemodialysisSchedule/CopySchedule";

        // Danh sách bệnh nhân đang điều trị (vùng dưới)
        internal const string V_HIS_TREATMENT_4_GET = "api/HisTreatment/GetView4";

        // Lấy thông tin điều trị (bảng HIS_TREATMENT) để enrich cột hiển thị lưới lịch
        internal const string HIS_TREATMENT_GET = "api/HisTreatment/Get";

        // Gói vật tư (HIS_EXP_MEST_TEMPLATE)
        internal const string HIS_EXP_MEST_TEMPLATE_GET = "api/HisExpMestTemplate/Get";
    }
}

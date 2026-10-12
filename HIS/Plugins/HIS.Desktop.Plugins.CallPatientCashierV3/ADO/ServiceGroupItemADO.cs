namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Một nhóm dịch vụ hiển thị trên tivi / trong bộ lọc.
    /// Key: "T{HIS_SERVICE_TYPE.ID}" (loại dịch vụ) hoặc "E{HIS_EXECUTE_GROUP.ID}" (nhóm thực hiện của dịch vụ loại CĐHA).
    /// </summary>
    public class ServiceGroupItemADO
    {
        public string Key { get; set; }
        public string Name { get; set; }
        /// <summary>Thứ tự loại dịch vụ: Khám trước, còn lại theo NUM_ORDER / ID danh mục (R5)</summary>
        public long SortOrder { get; set; }
        /// <summary>Thứ tự phụ trong cùng loại (tên nhóm thực hiện)</summary>
        public string SubOrder { get; set; }
    }
}

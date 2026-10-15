namespace HIS.Desktop.Plugins.AnticipateCreateV2
{
    /// <summary>
    /// Giá trị ô lọc "Thông tin" trên màn Tạo dự trù 2: lọc loại thuốc, vật tư theo thông tin BHYT
    /// khai trong danh mục loại. Mapping với filter IS_MISS_BHYT_INFO của HisMedicineTypeViewFilter
    /// và HisMaterialTypeViewFilter.
    /// </summary>
    public enum EnumBhytInfoFilter
    {
        /// <summary>Tất cả — không lọc (IS_MISS_BHYT_INFO không truyền)</summary>
        All = 0,

        /// <summary>Đầy đủ thông tin BHYT — IS_MISS_BHYT_INFO = false</summary>
        Full = 1,

        /// <summary>Thiếu thông tin BHYT — IS_MISS_BHYT_INFO = true</summary>
        Missing = 2
    }
}

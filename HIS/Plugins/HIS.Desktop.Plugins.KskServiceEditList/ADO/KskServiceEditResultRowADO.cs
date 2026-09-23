namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// 1 dòng grid kết quả xử lý (đã tính sẵn chuỗi hiển thị trước khi bind)
    /// </summary>
    public class KskServiceEditResultRowADO
    {
        public string TreatmentCode { get; set; }
        public string PatientName { get; set; }
        public string ActionDisplay { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public short ResultType { get; set; }
        public string ResultDisplay { get; set; }
        public string Description { get; set; }
    }
}

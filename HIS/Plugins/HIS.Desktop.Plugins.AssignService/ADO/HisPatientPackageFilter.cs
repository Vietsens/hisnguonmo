namespace HIS.Desktop.Plugins.AssignService.ADO
{
    public class HisPatientPackageFilter
    {
        public long? ID { get; set; }
        public long? PATIENT_ID { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public short? IS_ACTIVE { get; set; }
        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }

    public class HisPatientPackageDtViewFilter
    {
        public long? PATIENT_PACKAGE_ID { get; set; }
        public short? IS_ACTIVE { get; set; }
        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}

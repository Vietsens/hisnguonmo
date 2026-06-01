namespace HIS.Desktop.Plugins.AssignService.ADO
{
    public class PatientPackageDtADO : MOS.EFMODEL.DataModels.V_HIS_PATIENT_PACKAGE_DT
    {
        public bool IsChecked { get; set; }

        public decimal AmountThisTime { get; set; } = 1;

        public string PATIENT_PACKAGE_NAME { get; set; }

        public long? PATIENT_PACKAGE_PATIENT_TYPE_ID { get; set; }
    }
}

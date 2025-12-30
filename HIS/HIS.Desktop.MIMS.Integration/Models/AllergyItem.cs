namespace HIS.Desktop.MIMS.Integration.Models
{
    public class AllergyItem
    {
        public string MimsGuid { get; set; }
        public AllergyType Type { get; set; }
    }

    public enum AllergyType
    {
        Molecule,
        SubstanceClass
    }
}

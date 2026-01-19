namespace HIS.Desktop.MIMS.Integration.Models
{
    public class AllergyItem
    {
        public string HisCode { get; set; }
        public short? HisType { get; set; }
        public string Name { get; set; }
        public string MimsGuid { get; set; }
        public MimsType Type { get; set; }

        public AllergyItem() { }

        public AllergyItem(string hisCode, short? hisType)
        {
            HisCode = hisCode;
            HisType = hisType;
        }

        public AllergyItem(string hisCode, short? hisType, string name, string mimsGuid, MimsType type)
        {
            HisCode = hisCode;
            HisType = hisType;
            Name = name;
            MimsGuid = mimsGuid;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, HisCode);
        }
    }
}

using System;

namespace HIS.Desktop.MIMS.Integration.Models
{
    public class DrugItem
    {
        public string HisDrugCode { get; set; }
        public string Name { get; set; }
        public string MimsGuid { get; set; }     // Product / GGPI / GenericItem GUID
        public MimsDrugType DrugType { get; set; }

        public DrugItem() { }

        public DrugItem(string hisDrugCode, string name, string mimsGuid, MimsDrugType drugType)
        {
            HisDrugCode = hisDrugCode;
            Name = name;
            MimsGuid = mimsGuid;
            DrugType = drugType;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, HisDrugCode);
        }
    }

    public enum MimsDrugType
    {
        Product,
        GGPI,
        GenericItem
    }
}

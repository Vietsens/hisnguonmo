using System;

namespace HIS.Desktop.MIMS.Integration.Models
{
    public class ActiveMedication
    {
        public DrugItem Drug { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Route { get; set; }

        public ActiveMedication() { }

        public ActiveMedication(DrugItem drug, DateTime? startDate, DateTime? endDate, string dosage, string frequency, string route)
        {
            Drug = drug;
            StartDate = startDate;
            EndDate = endDate;
            Dosage = dosage;
            Frequency = frequency;
            Route = route;
        }

        public bool IsActive()
        {
            return !EndDate.HasValue || EndDate.Value >= DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) {2} {3}", Drug != null ? Drug.Name : "", Dosage, Frequency, Route);
        }
    }
}

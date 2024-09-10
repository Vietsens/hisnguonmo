using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.QrCodeBHYT
{
    public class HeinCardData
    {
        public HeinCardData() { }

        public string HeinCardNumber { get; set; }
        public string PatientName { get; set; }
        public string Dob { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string MediOrgCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string IssueDate { get; set; }
        public string ManagerCodeBHXH { get; set; }
        public string ParentName { get; set; }
        public string LiveAreaCode { get; set; }
        public string FineYearMonthDate { get; set; }
    }
}

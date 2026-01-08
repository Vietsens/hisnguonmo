using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee
{
    public class DataInput
    {
        public string hasUri { get; set; }
        public string acsUri { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string applicationCode { get; set; }
        public string limet { get; set; }
        public string cskcbbd { get; set; }
        public UseRequest useRequest { get; set; }
        public RegisterUseRequest registerUseRequest { get; set; }
        public CancelRegisterUseRequest cancelRegisterUseRequest { get; set; }
        public ModifyRequest modifyRequest { get; set; }
        public VerifyRequest verifyRequest { get; set; }
        public InquiryRequest inquiryRequest { get; set; }
        public AvailableBalanceInfoRequest availableBalanceInfoRequest { get; set; }
    }
}

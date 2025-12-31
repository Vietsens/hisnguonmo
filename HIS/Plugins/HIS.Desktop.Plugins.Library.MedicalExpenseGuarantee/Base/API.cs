using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.Base
{
    public class API
    {
        public const string API_GUARANTEE_REGISTER_USE = "/api/Guarantee/RegisterUse";
        public const string API_GUARANTEE_USE = "/api/Guarantee/Use";
        public const string API_GUARANTEE_VERIFY = "/api/Guarantee/Verify";
        public const string API_GUARANTEE_CANCEL_REGISTER_USE = "/api/Guarantee/CancelRegisterUse";
        public const string API_GUARANTEE_MODIFY = "/api/Guarantee/Modify";
        public const string API_GUARANTEE_INQUIRY = "/api/Guarantee/Inquiry";
        public const string API_GUARANTEE_AVAILABLE_BALANCE_INFO = "/api/Guarantee/AvailableBalanceInfo";
    }
}

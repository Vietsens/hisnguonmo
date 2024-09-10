using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill
{
    public class CmdType
    {
        public const int Undefined = 0;

        public const int ImportAndPublishInv = 100;
        public const int downloadInvPDFFkeyNoPay = 101;
        public const int DeleteInvFkey = 102;
        public const int ConvertForStoreFkey = 103;
        public const int DeleteInvoiceByFkey = 104;
        public const int CancelInv = 105;
        public const int ConvertForVerifyFkey = 106;
        public const int CancelInvNoPay = 107;
        public const int listInvByCusFkey = 108;
        public const int GetInvErrorViewFkey = 109;
    }
}

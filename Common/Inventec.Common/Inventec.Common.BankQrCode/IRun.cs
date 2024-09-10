using Inventec.Common.BankQrCode.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode
{
    public interface IRun
    {
        ResultQrCode Run();
        ResultQrCode RunConsumer();
    }
}

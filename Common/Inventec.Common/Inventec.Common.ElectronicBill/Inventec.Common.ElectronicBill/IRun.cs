using Inventec.Common.ElectronicBill.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill
{
    public interface IRun
    {
        ElectronicBillResult Run(int cmdType);
    }
}

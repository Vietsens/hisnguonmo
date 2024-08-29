using Inventec.Common.ElectronicBillViettel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel
{
    public interface IRun
    {
        Response Run(object data);
    }
}

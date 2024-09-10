using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EHoaDon
{
    public interface IRun
    {
        List<InvoiceResult> Run(int cmdType);
    }
}

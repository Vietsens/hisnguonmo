using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport
{
    public delegate void ProcessHasException(Inventec.Core.CommonParam param);
    public delegate void CloseContainerForm();
    public delegate object GetObjectFilter(Data.SaveClickSDO saveData);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Core
{
    public interface IDesktopInit
    {
        void RunInit(object desktopDesign, object[] args);
    }
}

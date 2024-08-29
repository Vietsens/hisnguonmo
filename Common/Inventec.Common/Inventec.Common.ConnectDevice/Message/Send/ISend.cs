using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message.Send
{
    interface ISend
    {
        void Run(ConnectToDevice connectDevice);
    }
}

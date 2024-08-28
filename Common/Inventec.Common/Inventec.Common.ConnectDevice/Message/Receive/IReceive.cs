using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message.Receive
{
    interface IReceive
    {
        Task Run(ConnectToDevice connectDevice);
    }
}

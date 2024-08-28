using Inventec.Common.Repository;
using Inventec.UC.ChangePassword.Sda.SdaEventLogCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Process
{
    class EventLogWorker
    {
        internal static SdaEventLogCreate Creator { get { return (SdaEventLogCreate)Worker.Get<SdaEventLogCreate>(); } }
    }
}

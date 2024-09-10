using Inventec.Common.Repository;
using Inventec.UC.Login.Sda.SdaEventLogCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Base
{
    internal partial class EventLogWorker
    {
        internal static SdaEventLogCreate Creator { get { return (SdaEventLogCreate)Worker.Get<SdaEventLogCreate>(); } }
    }
}

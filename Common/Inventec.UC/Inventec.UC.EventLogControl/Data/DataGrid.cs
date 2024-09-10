using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Data
{
    class DataGrid : SDA.EFMODEL.DataModels.SDA_EVENT_LOG
    {
        public string EVENT_TIME_DISPLAY { get; set; }
        public string LOG_CREATE_TIME { get; set; }
        public string LOG_MODIFY_TIME { get; set; }
    }
}

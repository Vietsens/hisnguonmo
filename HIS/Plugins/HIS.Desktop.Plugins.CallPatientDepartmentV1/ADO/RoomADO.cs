using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientDepartmentV1.ADO
{
    public class RoomADO
    {
        public long ROOM_ID { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public bool IsCheck { get; set; }
        public int OrderIndex { get; set; }
    }
}

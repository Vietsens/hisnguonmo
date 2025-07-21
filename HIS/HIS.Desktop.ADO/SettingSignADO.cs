using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class SettingSignADO
    {
        public bool IsHsm { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string SerialNumber { get; set; }
        public string SercetKey { get; set; }
        public string CccdNumber { get; set; }
        public string SerialNumberInfo { get; set; }
    }
}

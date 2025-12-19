using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.ADO
{
    /// <summary>
    /// ADO dùng chung cho các combo có d?ng Code - Name
    /// Có th? dùng cho: Trình ??, Lo?i hình, Tr?ng thái, v.v.
    /// </summary>
    public class KeyValueADO
    {
        public string CODE { get; set; }
        public string NAME { get; set; }

        public KeyValueADO()
        {
        }

        public KeyValueADO(string code, string name)
        {
            this.CODE = code;
            this.NAME = name;
        }
    }
}

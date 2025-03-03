using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.RoleUserGrid_F36__.ADO
{
    class HisMestInveUserAdo
    {
        public int Action { get; set; }
        public string LOGINNAME { get; set; }
        public string EXECUTE_ROLE_CODE { get; set; }

        public HisMestInveUserAdo()
        {
            Action = GlobalVariables.ActionEdit;
        }
    }
}

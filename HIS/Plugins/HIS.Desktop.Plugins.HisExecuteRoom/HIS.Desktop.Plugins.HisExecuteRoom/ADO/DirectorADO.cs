using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExecuteRoom.ADO
{
    public class DirectorADO
    {
        public string LOGINNAME { get; set; } 
        public string USERNAME { get; set; }    

        public DirectorADO(ACS.EFMODEL.DataModels.ACS_USER user)
        {
            if (user != null)
            {
                LOGINNAME = user.LOGINNAME;
                USERNAME = user.USERNAME;
            }
        }
    }
}

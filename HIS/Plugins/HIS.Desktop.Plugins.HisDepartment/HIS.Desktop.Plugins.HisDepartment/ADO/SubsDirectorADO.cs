using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisDepartment.ADO
{
    class SubsDirectorADO
    {
        public string LOGINNAME { get; set; }
        public string USERNAME { set; get; }
        public SubsDirectorADO() { }
        public SubsDirectorADO(ACS.EFMODEL.DataModels.ACS_USER user)
        {
            this.LOGINNAME = user.LOGINNAME;
            this.USERNAME = user.USERNAME; 
        }
    }
}

using IMSys.DbConfig.HIS_RS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL;

namespace HIS.Desktop.Plugins.CallPatientExpMest.ADO
{
    public  class ExpMestSttADO :MOS.EFMODEL.DataModels.HIS_EXP_MEST_STT
    {
        public bool checkStt { get; set; }
    }
}

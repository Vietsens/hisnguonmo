using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class TheFormTypeConfig
    {
        private static List<THE.EFMODEL.DataModels.THE_BRANCH> theBranchs;
        public static List<THE.EFMODEL.DataModels.THE_BRANCH> TheBranchs
        {
            get
            {
                if (theBranchs == null || theBranchs.Count == 0)
                {
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    THE.Filter.TheBranchFilter filter = new THE.Filter.TheBranchFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var branch = BackendDataWorker.Get<THE.EFMODEL.DataModels.THE_BRANCH>(RequestUriStore.THE_BRANCH_GET, ApiConsumerStore.TheConsumer, filter);
                    if (branch != null && branch.Count > 0)
                    {
                        var branchAdmin = branch.FirstOrDefault(o => o.ID == FormTypeConfig.BranchId);
                        if (branchAdmin != null && branchAdmin.IS_ADMIN != 1)
                        {
                            theBranchs = new System.Collections.Generic.List<THE.EFMODEL.DataModels.THE_BRANCH>();
                            theBranchs.Add(branchAdmin);
                        }
                        else
                        {
                            theBranchs = branch;
                        }
                    }
                }
                return theBranchs;
            }
            set
            {
                theBranchs = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Login.Set.SetBranch
{
    class BranchFactory
    {
        internal static void MakeISetBranch(UserControl uc, object branchs, long? firstBranchId)
        {
            ISetBranch result = null;
            try
            {
                result = new SetBranch(branchs, firstBranchId);
                if (result!=null) result.Run(uc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

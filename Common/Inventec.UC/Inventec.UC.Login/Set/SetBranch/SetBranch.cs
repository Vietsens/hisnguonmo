using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Set.SetBranch
{
    class SetBranch : ISetBranch
    {
        Object obj;
        long? id;

        internal SetBranch(Object obj, long? id)
            : base()
        {
            this.obj = obj;
            this.id = id;
        }

        void ISetBranch.Run(System.Windows.Forms.UserControl UC)
        {
            try
            {
                if (UC.GetType() == typeof(Design.Template3.Template3))
                {
                    Design.Template3.Template3 UCLogin = (Design.Template3.Template3)UC;
                    UCLogin.SetBranch(obj, id);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

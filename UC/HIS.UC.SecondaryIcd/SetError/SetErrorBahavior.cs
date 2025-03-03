using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.SecondaryIcd.SetError
{
    class SetErrorBahavior: ISetError
    {
        UserControl control;
        string error;
        public SetErrorBahavior()
            : base()
        { }

        public SetErrorBahavior(CommonParam param, UserControl uc,string error)
            : base()
        {
            this.control = uc;
            this.error = error;
        }

        void ISetError.Run()
        {
            try
            {
                ((UCSecondaryIcd)this.control).SetError(error);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

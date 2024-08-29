using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Loading.Set.Delegate.SetDelegateBWDoWorker
{
    interface ISetDelegateBWDoWorker
    {
        bool SetDelegateDoWorker(UserControl UC, BWDoWorker DoWorker);
    }
}

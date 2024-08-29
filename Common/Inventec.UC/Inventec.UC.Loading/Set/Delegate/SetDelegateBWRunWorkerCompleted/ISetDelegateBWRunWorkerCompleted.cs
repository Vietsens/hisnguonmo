using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Loading.Set.Delegate.SetDelegateBWRunWorkerCompleted
{
    interface ISetDelegateBWRunWorkerCompleted
    {
        bool SetDelegateRunCompleted(UserControl UC, BWRunWorkerCompleted RunCompleted);
    }
}

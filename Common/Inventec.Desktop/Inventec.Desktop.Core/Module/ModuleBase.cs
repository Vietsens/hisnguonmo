#region License

// Creater by phuongdt

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Core
{
    public abstract class ModuleBase
    {
        // Fields
        public const string MethodNameIsEnable = "IsEnable";

        // Methods
        protected ModuleBase()
        {
        }

        public virtual bool IsEnable()
        {
            return false;
        }
    }


}

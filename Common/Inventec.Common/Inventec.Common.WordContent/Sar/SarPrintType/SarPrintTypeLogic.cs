using Inventec.Common.WordContent.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Sar.SarPrintType
{
    internal class SarPrintTypeLogic : BusinessLogicBase
    {
        internal SarPrintTypeLogic(CommonParam param)
            : base(param)
        {
        }

        internal SarPrintTypeLogic()
            : base()
        { }

        internal T Get<T>(object data)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new SarPrintTypeGet(param, data);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}

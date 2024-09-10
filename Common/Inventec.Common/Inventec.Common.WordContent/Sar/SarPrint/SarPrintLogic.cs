using Inventec.Common.WordContent.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.SarPrint
{
    internal class SarPrintLogic : BusinessLogicBase
    {
        internal SarPrintLogic(CommonParam param)
            : base(param)
        {
        }

        internal SarPrintLogic()
            : base()
        { }

        internal T Get<T>(object data)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new SarPrintGet(param, data);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }

        internal object Create(object data)
        {
            object result = null;
            try
            {
                IDelegacy delegacy = new SarPrintCreate(param, data);
                result = delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        internal object Update(object data)
        {
            object result = null;
            try
            {
                IDelegacy delegacy = new SarPrintUpdate(param, data);
                result = delegacy.Execute();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboProvince.Set.SetDataInit
{
    class SetDataInitFactory
    {
        internal static ISetDataInit MakeISetDataInit()
        {
            ISetDataInit result = null;
            try
            {
                result = new SetDataInit();
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

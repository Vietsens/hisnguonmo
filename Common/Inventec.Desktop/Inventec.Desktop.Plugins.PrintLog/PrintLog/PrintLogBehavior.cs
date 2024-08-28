using Inventec.Core;
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Plugins.PrintLog.PrintLog
{
    class PrintLogBehavior : BusinessBase, IPrintLog
    {
        object[] entity;
        internal PrintLogBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPrintLog.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                string printTypeCode = "", uniqueCode = "";

                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is HIS.Desktop.ADO.PrintLogADO)
                            {
                                printTypeCode = ((HIS.Desktop.ADO.PrintLogADO)entity[i]).PRINT_TYPE_CODE;
                                uniqueCode = ((HIS.Desktop.ADO.PrintLogADO)entity[i]).UNIQUE_CODE;
                            }
                        }
                    }
                }

                return new frmPrintLog(moduleData, printTypeCode, uniqueCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}

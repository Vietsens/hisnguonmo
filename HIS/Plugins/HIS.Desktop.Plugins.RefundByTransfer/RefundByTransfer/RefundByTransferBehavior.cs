<<<<<<< HEAD
﻿using Inventec.Core;
=======
﻿using HIS.Desktop.Common;
using Inventec.Core;
>>>>>>> Nampp
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RefundByTransfer.RefundByTransfer
{
    class RefundByTransferBehavior : Tool<IDesktopToolContext>, IRefundByTransfer
    {
        object[] entity;

        public RefundByTransferBehavior()
            : base()
        {

        }

        public RefundByTransferBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IRefundByTransfer.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_TREATMENT treatment = null;
                HIS_TRANSACTION transaction = null;
                string bankCode = "";
<<<<<<< HEAD
=======
                RefeshReference refresh = null;
>>>>>>> Nampp
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
                            else if (entity[i] is HIS_TREATMENT)
                            {
                                treatment = (HIS_TREATMENT)entity[i];
                            }
                            else if (entity[i] is HIS_TRANSACTION)
                            {
                                transaction = (HIS_TRANSACTION)entity[i];
                            }
                            else if (entity[i] is string)
                            {
                                bankCode = (string)entity[i];
                            }
<<<<<<< HEAD
=======
                            else if (entity[i] is HIS.Desktop.Common.RefeshReference)
                            {
                                refresh = (HIS.Desktop.Common.RefeshReference)entity[i];
                            }
>>>>>>> Nampp
                        }
                    }
                }

<<<<<<< HEAD
                return new FormRefundByTransfer(moduleData, bankCode, treatment, transaction);
=======
                return new FormRefundByTransfer(moduleData, bankCode, treatment, transaction, refresh);
>>>>>>> Nampp
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}

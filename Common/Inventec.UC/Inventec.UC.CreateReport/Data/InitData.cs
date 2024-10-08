﻿using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Data
{
    public class InitData
    {
        internal Inventec.Token.ClientSystem.ClientTokenManager clientToken;
        internal Inventec.Common.WebApiClient.ApiConsumer sarconsumer;
        internal Inventec.Common.WebApiClient.ApiConsumer mrsconsumer;

        public InitData(Inventec.Common.WebApiClient.ApiConsumer sarConsumer,SAR.EFMODEL.DataModels.SAR_REPORT_TYPE sarreporttype,Inventec.Common.WebApiClient.ApiConsumer mrsConsumer, Inventec.Token.ClientSystem.ClientTokenManager clientTokenManager)
        {
            try
            {
                this.sarconsumer = sarConsumer;
                this.mrsconsumer = mrsConsumer;
                this.clientToken = clientTokenManager;
                GlobalStore.reportType = sarreporttype;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

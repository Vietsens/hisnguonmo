﻿using Inventec.Core;
using Inventec.UC.Login.Base;
using SDA.Filter;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Sda
{
    internal partial class SdaLanguageGet : BusinessBase
    {
        public SdaLanguageGet()
            : base()
        {

        }

        public SdaLanguageGet(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        public List<SDA.EFMODEL.DataModels.SDA_LANGUAGE> Get()
        {
            List<SDA.EFMODEL.DataModels.SDA_LANGUAGE> result = null;
            try
            {
                SdaLanguageFilter languageFilter = new SdaLanguageFilter();
                languageFilter.IS_ACTIVE = 1;
                var rs = ApiConsumerStore.SdaConsumer.Get<Inventec.Core.ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_LANGUAGE>>>("/api/SdaLanguage/Get", param, languageFilter);
                if (rs != null)
                {
                    result = rs.Data;
                }
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

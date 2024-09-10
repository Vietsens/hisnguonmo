using Inventec.Core;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Config
{
    class Loader : Inventec.UC.CreateReport.Base.BusinessBase
    {
        public static Dictionary<string, SDA_CONFIG> dictionaryConfig = new Dictionary<string, SDA_CONFIG>();

        public static bool RefreshConfig()
        {
            bool result = false;
            try
            {
                CommonParam paramGet = new CommonParam();
                var ro = Base.ApiConsumerStore.SdaConsumer.Get<Inventec.Core.ApiResultObject<List<SDA_CONFIG>>>("/api/SdaConfig/Get", paramGet, new SDA.Filter.SdaConfigFilter());
                if (ro.Data != null && ro.Data.Count > 0)
                {
                    foreach (var config in ro.Data)
                    {
                        if (!String.IsNullOrWhiteSpace(config.KEY))
                        {
                            dictionaryConfig[config.KEY] = config; //Ghi de du lieu cu ==> luu y tinh huong neu 2 config trung key thi config sau se de len config truoc. Loi nay thuoc ve constraint du lieu ko thuoc trach nhiem cua Loader.
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Key null." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                        }
                    }
                    result = true;
                }
                else if (paramGet.HasException)
                {
                    Inventec.Common.Logging.LogSystem.Error("Query SdaConfig co exception.");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong co du lieu SdaConfig & khong co exception.");
                    result = true;
                }
                if (result)
                {
                    Inventec.Common.Logging.LogSystem.Info("Load du lieu cau hinh SdaConfig thanh cong.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}

using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class SarFormTypeConfig
    {
        private static List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE> sarFormType;
        public static List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE> SarFormType
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(SAR.EFMODEL.DataModels.SAR_FORM_TYPE));
                if (sarFormType == null || sarFormType.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarFormTypeFilter filter = new SAR.Filter.SarFormTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sarFormType = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>(RequestUriStore.SAR_FORM_TYPE_GET, ApiConsumerStore.SarConsumer, filter);
                }
                return sarFormType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                sarFormType = value;
            }
        }

    }
}

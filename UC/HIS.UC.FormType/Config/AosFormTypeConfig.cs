using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class AosFormTypeConfig
    {
        private static List<AOS.EFMODEL.DataModels.AOS_ACCOUNT_TYPE> aosAccountTpye;
        public static List<AOS.EFMODEL.DataModels.AOS_ACCOUNT_TYPE> AosAccountTpye
        {
            get
            {
                if (aosAccountTpye == null || aosAccountTpye.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    THE.EFMODEL.DataModels.THE_BRANCH the = new THE.EFMODEL.DataModels.THE_BRANCH();
                    if (FormTypeConfig.BranchId > 0)
                    {
                        the = TheFormTypeConfig.TheBranchs.FirstOrDefault(o => o.ID == FormTypeConfig.BranchId);
                    }

                    AOS.Filter.AosAccountTypeFilter filter = new AOS.Filter.AosAccountTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var lstAccountType = BackendDataWorker.Get<AOS.EFMODEL.DataModels.AOS_ACCOUNT_TYPE>("/api/AosAccountType/Get", ApiConsumerStore.AosConsumer, filter);
                    if (lstAccountType != null && lstAccountType.Count > 0)
                    {
                        if (the != null && the.IS_ADMIN != 1)
                        {
                            aosAccountTpye = lstAccountType.Where(o => (!String.IsNullOrWhiteSpace(o.BRANCH_CODES) && o.BRANCH_CODES.Contains(the.BRANCH_CODE))
                                || (!String.IsNullOrWhiteSpace(o.OWNER_BRANCH_CODE) && o.OWNER_BRANCH_CODE.Contains(the.BRANCH_CODE))).ToList();
                        }
                        else
                        {
                            aosAccountTpye = lstAccountType;
                        }
                    }
                }
                return aosAccountTpye;
            }
            set
            {
                aosAccountTpye = value;
            }
        }
    }
}

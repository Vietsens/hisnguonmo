using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class AcsFormTypeConfig
    {
        private static List<ACS.EFMODEL.DataModels.ACS_USER> AcsUser;
        public static List<ACS.EFMODEL.DataModels.ACS_USER> HisAcsUser
        {
            get
            {
                return AcsUser;
            }
            set
            {
                AcsUser = value;
            }
          
              
         
        }

        private static List<ACS.EFMODEL.DataModels.ACS_USER> AcsUserCashier;
        public static List<ACS.EFMODEL.DataModels.ACS_USER> HisAcsUserCashier
        {
            get
            {
                if (AcsUserCashier == null || AcsUserCashier.Count == 0)
                {
                    if (FormTypeDelegate.GetCashierUser != null) FormTypeDelegate.GetCashierUser();
                }
                return AcsUserCashier.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                AcsUserCashier = value;
            }
        }


        private static List<ACS.EFMODEL.DataModels.ACS_USER> AcsUserSale;
        public static List<ACS.EFMODEL.DataModels.ACS_USER> HisAcsUserSale
        {
            get
            {
                if (AcsUserSale == null || AcsUserSale.Count == 0)
                {
                    if (FormTypeDelegate.GetSaleUser != null) FormTypeDelegate.GetSaleUser();
                }
                return AcsUserSale.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                AcsUserSale = value;
            }
        }
    }
}

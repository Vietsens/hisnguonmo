using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Base
{
    public abstract class BusinessBase : EntityBase
    {
        protected CommonParam param { get; set; }

        public BusinessBase()
            : base()
        {
            param = new CommonParam();
            try
            {
                UserName = "";
            }
            catch (Exception)
            {
            }
        }

        public BusinessBase(CommonParam paramBusiness)
            : base()
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
            try
            {
                UserName = "";
            }
            catch (Exception)
            {
            }
        }
    }
}

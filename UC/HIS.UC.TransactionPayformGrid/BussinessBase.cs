/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.UC.TransactionPayformGrid
{
    public abstract class BussinessBase : EntityBase
    {
        protected CommonParam param { get; set; }
         
        public BussinessBase()
            : base()
        {
            param = new CommonParam();
        }

        public BussinessBase(CommonParam paramBusiness)
            : base()
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }
    }
}

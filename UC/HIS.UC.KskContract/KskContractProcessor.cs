/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.UC.KskContract.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.KskContract.GetValue;
using MOS.EFMODEL.DataModels;
using HIS.UC.KskContract.SetValue;
using HIS.UC.KskContract.InFocus;
using HIS.UC.KskContract.ADO;
using HIS.UC.KskContract.ResetValue;
using HIS.UC.KskContract.GetValidate;

namespace HIS.UC.KskContract
{
    public class KskContractProcessor : BussinessBase
    {
        object uc;
        TemplateType.ENUM Template;
        public KskContractProcessor(TemplateType.ENUM temp)
            : base()
        {
            this.Template = temp;
        }

        public KskContractProcessor(CommonParam paramBusiness, TemplateType.ENUM temp)
            : base(paramBusiness)
        {
            this.Template = temp;
        }

        public object Run(KskContractInput arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIKskContract(param,arg);
                uc = behavior != null ? (behavior.Run( this.Template)) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public object GetValue(UserControl control)
        {
            object result = null;
            try
            {
                IGetValue behavior = GetValueFactory.MakeIGetValue(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run(this.Template) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public bool GetValidate(UserControl control)
        {
            bool result = true;
            try
            {
                IGetValidate behavior = GetValidateFactory.MakeIGetValidate(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run(this.Template) : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object SetValue(UserControl control,HIS_KSK_CONTRACT dhst)
        {
            object result = null;
            try
            {
                ISetValue behavior = SetValueFactory.MakeISetValue(param, (control == null ? (UserControl)uc : control), dhst);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void InFocus(UserControl control)
        {
            try
            {
                IInFocus behavior = InFocusFactory.MakeIInFocus(param, (control == null ? (UserControl)uc : control));
                behavior.Run(this.Template);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ResetValue(UserControl control)
        {
            try
            {
                IResetValue behavior = ResetValueFactory.MakeIResetValue(param, (control == null ? (UserControl)uc : control));
                behavior.Run(this.Template);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

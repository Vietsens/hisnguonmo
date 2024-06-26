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

using HIS.UC.DHST.Run;
using HIS.UC.DHST.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.DHST.GetValue;
using MOS.EFMODEL.DataModels;
using HIS.UC.DHST.SetValue;
using HIS.UC.DHST.IsReadOnly;
using HIS.UC.DHST.InFocus;
using HIS.UC.DHST.SetValidate;
using HIS.UC.DHST.SetExecuteTime;

namespace HIS.UC.DHST
{
    public class DHSTProcessor : BussinessBase
    {
        object uc;
        public DHSTProcessor()
            : base()
        {
        }

        public DHSTProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(DHSTInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIDHST(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
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
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object SetValue(UserControl control,HIS_DHST dhst)
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

        public object SetValidate(UserControl control, bool isValidate)
        {
            object result = null;
            try
            {
                ISetValidate behavior = SetValidateFactory.MakeISetValidate(param, (control == null ? (UserControl)uc : control), isValidate);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object InFocus(UserControl control)
        {
            object result = null;
            try
            {
                IInFocus behavior = InFocusFactory.MakeIInFocus(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object IsReadOnly(UserControl control, bool isReadOnly)
        {
            object result = null;
            try
            {
                IIsReadOnly behavior = IsReadOnlyFactory.MakeIIsReadOnly(param, (control == null ? (UserControl)uc : control), isReadOnly);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public object SetExecuteTime(UserControl control, long? executeTime)
        {
            object result = null;
            try
            {
                ISetExecuteTime behavior = SetExecuteTimeFactory.MakeISetExecuteTime(param, (control == null ? (UserControl)uc : control), executeTime);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

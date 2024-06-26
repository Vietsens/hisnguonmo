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
using HIS.UC.ServiceUnit.ADO;
using HIS.UC.ServiceUnit.FocusControl;
using HIS.UC.ServiceUnit.GetValue;
//using HIS.UC.ServiceUnit.NextFocus;
using HIS.UC.ServiceUnit.ReadOnly;
using HIS.UC.ServiceUnit.Reload;
using HIS.UC.ServiceUnit.Run;
using HIS.UC.ServiceUnit.SetRequired;
using HIS.UC.ServiceUnit.SetValue;
using HIS.UC.ServiceUnit.ValidationServiceUnit;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.ServiceUnit
{
    public class ServiceUnitProcessor : BussinessBase
    {
        object uc;
        public ServiceUnitProcessor()
            : base()
        {
        }

        public ServiceUnitProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(ServiceUnitInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIServiceUnit(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, ServiceUnitInputADO data)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValue(UserControl control, Object data)
        {
            try
            {
                ISetValue behavior = SetValueFactory.MakeISetValue(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        public void FocusControl(UserControl control)
        {
            try
            {
                IFocusControl behavior = FocusControlFactory.MakeIFocusControl(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetRequired(UserControl control, bool isRequired)
        {
            try
            {
                ISetRequired behavior = SetRequiredFactory.MakeISetRequired(param, (control == null ? (UserControl)uc : control), isRequired);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReadOnly(UserControl control, bool isReadOnly)
        {
            try
            {
                IReadOnly behavior = ReadOnlyFactory.MakeIReadOnly(param, (control == null ? (UserControl)uc : control), isReadOnly);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public void NextFocus(UserControl control, Object data)
        //{
        //    try
        //    {
        //        INextFocus behavior = NextFocusFactory.MakeINextFocus(param, (control == null ? (UserControl)uc : control), data);
        //        if (behavior != null) behavior.Run();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        public object ValidationServiceUnit(UserControl control)
        {
            object result = false;
            try
            {
                IValidationServiceUnit behavior = ValidationServiceUnitFactory.MakeIValidationServiceUnit(param, (control == null ? (UserControl)uc : control));
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

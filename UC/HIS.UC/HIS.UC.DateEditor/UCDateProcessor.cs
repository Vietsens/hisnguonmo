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
using HIS.UC.DateEditor.ADO;
using HIS.UC.DateEditor.Enable;
using HIS.UC.DateEditor.FocusControl;
using HIS.UC.DateEditor.GetChkMultiDateState;
using HIS.UC.DateEditor.GetValue;
using HIS.UC.DateEditor.NextFocus;
using HIS.UC.DateEditor.ReadOnly;
using HIS.UC.DateEditor.Reload;
using HIS.UC.DateEditor.Run;
using HIS.UC.DateEditor.SetValue;
using HIS.UC.DateEditor.ValidationFormWithMessage;
using HIS.UC.DateEditor.ValidationIcd;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.DateEditor
{
    public class UCDateProcessor : BussinessBase
    {
        object uc;
        public UCDateProcessor()
            : base()
        {
        }

        public UCDateProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(DateInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIIcd(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, DateInputADO data)
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

        public List<long> GetValue(UserControl control)
        {
            List<long> result = null;
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

        public bool GetChkMultiDateState(UserControl control)
        {
            bool result = false;
            try
            {
                IGetChkMultiDateState behavior = GetChkMultiDateStateFactory.MakeIGetChkMultiDateState(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : false;
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
        public void EnableCheckBoxMultiIntructionTime(UserControl control, bool isEnable)
        {
            try
            {
                IEnable behavior = EnableFactory.MakeIEnable(param, (control == null ? (UserControl)uc : control), isEnable);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void NextFocus(UserControl control, Object data)
        {
            try
            {
                INextFocus behavior = NextFocusFactory.MakeINextFocus(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object ValidationForm(UserControl control)
        {
            object result = false;
            try
            {
                IValidationIcd behavior = ValidationIcdFactory.MakeIValidationIcd(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object ValidationFormWithMessage(UserControl control, List<string> errorEmpty, List<string> errorOther)
        {
            object result = false;
            try
            {
                IValidationFormWithMessage behavior = ValidationFormWithMessageFactory.MakeIValidationFormWithMessage(param, (control == null ? (UserControl)uc : control), errorEmpty, errorOther);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object ResetValidation(UserControl control)
        {
            object result = false;
            try
            {
                IValidationIcd behavior = ValidationIcdFactory.MakeIValidationIcd(param, (control == null ? (UserControl)uc : control));
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

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
using HIS.UC.NextTreatmentInstruction.ADO;
using HIS.UC.NextTreatmentInstruction.FocusControl;
using HIS.UC.NextTreatmentInstruction.GetValue;
//using HIS.UC.NextTreatmentInstruction.NextFocus;
using HIS.UC.NextTreatmentInstruction.ReadOnly;
using HIS.UC.NextTreatmentInstruction.Reload;
using HIS.UC.NextTreatmentInstruction.Run;
using HIS.UC.NextTreatmentInstruction.SetEnabled;
using HIS.UC.NextTreatmentInstruction.SetValue;
using HIS.UC.NextTreatmentInstruction.ValidationNextTreatmentInstruction;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.NextTreatmentInstruction
{
    public class NextTreatmentInstructionProcessor : BussinessBase
    {
        object uc;
        public NextTreatmentInstructionProcessor()
            : base()
        {
        }

        public NextTreatmentInstructionProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(NextTreatmentInstructionInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeINextTreatmentInstruction(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, NextTreatmentInstructionInputADO data)
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

        public void SetEnabled(UserControl control, bool isEnabled)
        {
            try
            {
                ISetEnabled behavior = SetEnabledFactory.MakeISetEnabled(param, (control == null ? (UserControl)uc : control), isEnabled);
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

        public object ValidationNextTreatmentInstruction(UserControl control)
        {
            object result = false;
            try
            {
                IValidationNextTreatmentInstruction behavior = ValidationNextTreatmentInstructionFactory.MakeIValidationNextTreatmentInstruction(param, (control == null ? (UserControl)uc : control));
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

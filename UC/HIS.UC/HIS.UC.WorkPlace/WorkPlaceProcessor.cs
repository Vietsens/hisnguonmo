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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using HIS.UC.WorkPlace.Async;
using HIS.UC.WorkPlace;
using System.Windows.Forms;
using HIS.UC.WorkPlace.ValidationCombo;
using HIS.UC.WorkPlace.ResetValidate;
namespace HIS.UC.WorkPlace
{
    public class WorkPlaceProcessor : BussinessBase
    {
        object uc;
        public WorkPlaceProcessor(CommonParam param)
            : base(param)
        {
        }

        public enum Template
        {
            Combo,
            Textbox,
            Combo1,
            Textbox1
        }

        public object ucWorkPlace;

        public async Task<object> Generate(WorkPlaceInitADO data)
        {
            try
            {
                CommonParam param = new CommonParam();
                IAppDelegacyAsync delegacy = new WorkPlaceGenerate(param, data);
                ucWorkPlace = await delegacy.Execute().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                ucWorkPlace = null;
            }
            return ucWorkPlace;
        }

        public object GetValue(object uc, Template data)
        {
            object result = null;
            try
            {
                if (uc is UCWorkPlaceCombo)
                {
                    result = (uc as UCWorkPlaceCombo).GetValue();
                }
                else if (uc is UCWorkPlaceTextbox)
                {
                    result = (uc as UCWorkPlaceTextbox).GetValue();
                }
                else if (uc is UCWorkPlaceCombo1)
                {
                    result = (uc as UCWorkPlaceCombo1).GetValue();
                }
                else if (uc is UCWorkPlaceTextbox1)
                {
                    result = (uc as UCWorkPlaceTextbox1).GetValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public void FocusControl(Template data)
        {
            try
            {
                switch (data)
                {
                    case Template.Combo:
                        ((UCWorkPlaceCombo)ucWorkPlace).FocusControl();
                        break;
                    case Template.Textbox:
                        ((UCWorkPlaceTextbox)ucWorkPlace).FocusControl();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            try
            {
                switch (data)
                {
                    case Template.Combo1:
                        ((UCWorkPlaceCombo1)ucWorkPlace).FocusControl();
                        break;
                    case Template.Textbox1:
                        ((UCWorkPlaceTextbox1)ucWorkPlace).FocusControl();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusNextUserControl(Template data, DelegateFocusMoveout _dlg)
        {
            try
            {
                if(data == Template.Combo1)
                    ((UCWorkPlaceCombo1)ucWorkPlace).SetDelegateFocusMoveOutControl(_dlg);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(object uc, object value)
        {
            try
            {
                if (uc is UCWorkPlaceCombo)
                {
                    (uc as UCWorkPlaceCombo).SetValue(value);
                }
                else if (uc is UCWorkPlaceTextbox)
                {
                    (uc as UCWorkPlaceTextbox).SetValue(value);
                }
                else if (uc is UCWorkPlaceCombo1)
                {
                    (uc as UCWorkPlaceCombo1).SetValue(value);
                }
                else if (uc is UCWorkPlaceTextbox1)
                {
                    (uc as UCWorkPlaceTextbox1).SetValue(value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(Template template, object data)
        {
            try
            {
                switch (template)
                {
                    case Template.Combo:
                        ((UCWorkPlaceCombo)ucWorkPlace).ReloadData(data);
                        break;
                    case Template.Combo1:
                        ((UCWorkPlaceCombo1)ucWorkPlace).ReloadData(data);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object ValidationCombo(UserControl control,Template template)
        {
            object result = false;
            try
            {
                IValidationCombo behavior = ValidationComboFactory.MakeIValidationCombo(param, (control == null ? (UserControl)uc : control),template);
                result = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void ResetValidation(UserControl control)
        {
            try
            {
                IResetValidate behavior = ResetValidateFactory.MakeIResetValidation(param, (control == null ? (UserControl)uc : control));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

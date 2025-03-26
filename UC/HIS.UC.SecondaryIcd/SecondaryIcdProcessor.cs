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
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.SecondaryIcd.FocusControl;
using HIS.UC.SecondaryIcd.GetValidate;
using HIS.UC.SecondaryIcd.GetValidateWithMessage;
using HIS.UC.SecondaryIcd.GetValue;
using HIS.UC.SecondaryIcd.ReadOnly;
using HIS.UC.SecondaryIcd.Reload;
using HIS.UC.SecondaryIcd.ResetValidate;
using HIS.UC.SecondaryIcd.Run;
using HIS.UC.SecondaryIcd.SetAttachIcd;
using HIS.UC.SecondaryIcd.SetError;
using HIS.UC.SecondaryIcd.SetValue;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.UTILITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.SecondaryIcd
{
    public class SecondaryIcdProcessor : BussinessBase
    {
        private List<HIS_ICD> HisIcds { get; set; }
        private List<V_HIS_ICD> ViewHisIcds { get; set; }

        object uc;
        public SecondaryIcdProcessor()
            : base()
        { }

        public SecondaryIcdProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        { }

        public SecondaryIcdProcessor(CommonParam paramBusiness, List<HIS_ICD> _HisIcds)
            : base(paramBusiness)
        {
            if (_HisIcds != null && _HisIcds.Count > 0)
            {
                HisIcds = _HisIcds.Where(p => p.IS_ACTIVE == 1).ToList();
                List<HIS_ICD> icdIsTraditionals = (HisIcds != null && HisIcds.Count > 0) ? HisIcds.Where(o => o.IS_TRADITIONAL == Constant.IS_TRUE).ToList() : null;
                List<HIS_ICD> icdNotIsTraditionals = (HisIcds != null && HisIcds.Count > 0) ? HisIcds.Where(o => o.IS_TRADITIONAL == null || o.IS_TRADITIONAL == Constant.IS_FALSE).ToList() : null;
                if (icdIsTraditionals != null && icdIsTraditionals.Count > 0 && icdNotIsTraditionals != null && icdNotIsTraditionals.Count > 0)
                {
                    HisIcds = icdNotIsTraditionals;
                }
            }
        }
        public SecondaryIcdProcessor(CommonParam paramBusiness, List<V_HIS_ICD> ViewHisIcds)
    : base(paramBusiness)
        {
            if (ViewHisIcds != null && ViewHisIcds.Count > 0)
            {
                this.ViewHisIcds = ViewHisIcds.Where(p => p.IS_ACTIVE == 1).ToList();
                List<V_HIS_ICD> icdIsTraditionals = (this.ViewHisIcds != null && this.ViewHisIcds.Count > 0) ? this.ViewHisIcds.Where(o => o.IS_TRADITIONAL == Constant.IS_TRUE).ToList() : null;
                List<V_HIS_ICD> icdNotIsTraditionals = (this.ViewHisIcds != null && this.ViewHisIcds.Count > 0) ? this.ViewHisIcds.Where(o => o.IS_TRADITIONAL == null || o.IS_TRADITIONAL == Constant.IS_FALSE).ToList() : null;
                if (icdIsTraditionals != null && icdIsTraditionals.Count > 0 && icdNotIsTraditionals != null && icdNotIsTraditionals.Count > 0)
                {
                    this.ViewHisIcds = icdNotIsTraditionals;
                }
            }
        }

        public object Run(SecondaryIcdInitADO arg)
        {
            uc = null;
            try
            {
                if (arg.HisIcds == null)
                {
                    arg.HisIcds = HisIcds;
                }
                if (arg.ViewHisIcds == null)
                {
                    arg.ViewHisIcds = ViewHisIcds;
                }
                IRun behavior = RunFactory.MakeISecondaryIcd(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
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

        public bool GetValidate(UserControl control)
        {
            bool result = true;
            try
            {
                IGetValidate behavior = GetValidateFactory.MakeIGetValidate(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public bool ResetValidate(UserControl control)
        {
            bool result = true;
            try
            {
                IResetValidate behavior = ResetValidateFactory.MakeIResetValidate(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public bool GetValidateWithMessage(UserControl control, List<string> errorEmpty, List<string> errorOther)
        {
            bool result = true;
            try
            {
                IGetValidateWithMessage behavior = GetValidateWithMessageFactory.MakeIGetValidateWithMessage(param, (control == null ? (UserControl)uc : control), errorEmpty, errorOther);
                result = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        public void Reload(UserControl control, SecondaryIcdDataADO data)
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
        public void SetAttachIcd(UserControl control, SecondaryIcdDataADO data)
        {
            try
            {
                ISetAttachIcd behavior = SetAttachIcdFactory.MakeISetAttachIcd(param, (control == null ? (UserControl)uc : control), data);
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
        public void SetError(UserControl control, string data)
        {
            try
            {
                ISetError behavior = SetErrorFactory.MakeISetError(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object ShowPopupIcdChoose(UserControl control)
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
    }
}

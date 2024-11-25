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

using HIS.UC.ExamTreatmentFinish.Run;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.UC.ExamTreatmentFinish.GetValue;
using MOS.EFMODEL.DataModels;
using HIS.UC.ExamTreatmentFinish.ADO;
using HIS.UC.ExamTreatmentFinish.SetValue;
using HIS.UC.ExamTreatmentFinish.SetValueV2;
using HIS.UC.ExamTreatmentFinish.Reload;
using HIS.UC.ExamTreatmentFinish.Validate;
using HIS.UC.ExamTreatmentFinish.FocusControl;
using HIS.UC.ExamTreatmentFinish.GetIcd;

namespace HIS.UC.ExamTreatmentFinish
{
    public class ExamTreatmentFinishProcessor : BussinessBase
    {
        object uc;
        public ExamTreatmentFinishProcessor()
            : base()
        {
        }

        public ExamTreatmentFinishProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(TreatmentFinishInitADO arg,HIS_TREATMENT_EXT _treatmentExt)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIExamTreatmentFinish(param, arg, _treatmentExt);
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
        public object GetIcd(UserControl control)
        {
            object result = null;
            try
            {
                IGetIcd behavior = GetIcdFactory.MakeIGetIcd(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void SetValue(UserControl control, Inventec.Desktop.Common.Modules.Module currentModule, HIS_TREATMENT treatment)
        {
            try
            {
                ISetValue behavior = SetValueFactory.MakeISetValue(param, (control == null ? (UserControl)uc : control), currentModule, treatment);
                if (behavior != null)
                    behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValueV2(UserControl control, Inventec.Desktop.Common.Modules.Module currentModule, HisServiceReqExamUpdateResultSDO HisServiceReqResult)
        {
            try
            {
                ISetValueV2 behavior = SetValueV2Factory.MakeISetValue(param, (control == null ? (UserControl)uc : control), currentModule, HisServiceReqResult);
                if (behavior != null)
                    behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void UpdateProgramData(UserControl control, List<HIS_PATIENT_PROGRAM> patientPrograms, List<V_HIS_DATA_STORE> dataStores)
        {
            try
            {
                if (control is UCExamTreatmentFinish)
                {
                    ((UCExamTreatmentFinish)control).UpdateProgramData(patientPrograms, dataStores);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReLoad(UserControl control, TreatmentFinishInitADO data)
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

        public bool Validate(UserControl control, bool IsNotCheckValidateIcdUC)
        {
            bool result = false;
            try
            {
                IValidate behavior = ValidateFactory.MakeIValidate(param, (control == null ? (UserControl)uc : control), IsNotCheckValidateIcdUC);
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
    }
}

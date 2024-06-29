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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.UC.TreatmentFinish.ADO;
using HIS.UC.TreatmentFinish.Run;
using System.Windows.Forms;
using HIS.UC.TreatmentFinish.Reload;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using HIS.UC.TreatmentFinish.GetData;
using HIS.UC.TreatmentFinish.GetDataOutput;
using HIS.UC.TreatmentFinish.FocusControl;
using HIS.UC.TreatmentFinish.GetValidate;
using HIS.UC.TreatmentFinish.GetUseDay;
using HIS.UC.TreatmentFinish.ShowPopupAppointmentControl;
using HIS.UC.TreatmentFinish.ShowPopupWhenNotFinishingIncaseOfOutPatient;
//using HIS.UC.TreatmentFinish.ChangeStateCreateEMRVBA;
using HIS.UC.TreatmentFinish.SetDelegateCreateEMRVBA;

namespace HIS.UC.TreatmentFinish
{
    public class TreatmentFinishProcessor : BussinessBase
    {
        object uc;
        public TreatmentFinishProcessor()
            : base()
        {
        }

        public TreatmentFinishProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(TreatmentFinishInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIDepositRequestList(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, DataInputADO dataInputADO)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), dataInputADO);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public HisTreatmentFinishSDO GetData(UserControl control)
        {
            HisTreatmentFinishSDO result = null;
            try
            {
                IGetData behavior = GetDataFactory.MakeIGetData(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public DataOutputADO GetDataOutput(UserControl control)
        {
            DataOutputADO result = null;
            try
            {
                IGetDataOutput behavior = GetDataOutputFactory.MakeIGetDataOutput(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool GetValidate(UserControl control)
        {
            bool result = true;
            try
            {
                IGetValidate behavior = GetValidateFactory.MakeIGetValidate(control, false, null, null);
                result = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool GetValidateWithMessage(UserControl control, List<string> errorEmpty, List<string> errorOther)
        {
            bool result = true;
            try
            {
                IGetValidate behavior = GetValidateFactory.MakeIGetValidate(control, true, errorEmpty, errorOther);
                result = (behavior != null) ? behavior.Run() : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void FocusControl(UserControl control)
        {
            try
            {
                IFocusControl behavior = FocusControlFactory.MakeIFocusControl(param, control);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetUseDay(UserControl control)
        {
            decimal result = 0;
            try
            {
                IGetUseDay behavior = GetUseDayFactory.MakeIGetUseDay(control);
                result = (behavior != null) ? behavior.Run() : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }

        public void CheckChangeAutoTreatmentFinish(UserControl control, bool check)
        {
            try
            {
                if (control is UCTreatmentFinish)
                {
                    ((UCTreatmentFinish)control).CheckChangeAutoTreatmentFinish(check);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void EnableChangeAutoTreatmentFinish(UserControl control, bool enable)
        {
            try
            {
                if (control is UCTreatmentFinish)
                {
                    ((UCTreatmentFinish)control).EnableChangeAutoTreatmentFinish(enable);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitNeedSickLeaveCert(UserControl control, bool isNeedSickLeaveCert)
        {
            try
            {
                if (control is UCTreatmentFinish)
                {
                    ((UCTreatmentFinish)control).InitNeedSickLeaveCert(isNeedSickLeaveCert);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void UpdateTreatmentData(UserControl control, HisTreatmentWithPatientTypeInfoSDO treatment)
        {
            try
            {
                if (control is UCTreatmentFinish)
                {
                    ((UCTreatmentFinish)control).UpdateTreatmentData(treatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void UpdateStoreCode(UserControl control, string storeCode)
        {
            try
            {
                if (control is UCTreatmentFinish)
                {
                    ((UCTreatmentFinish)control).UpdateStoreCode(storeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ShowPopupAppointmentControl(UserControl control)
        {
            try
            {
                IShowPopupAppointmentControl behavior = ShowPopupAppointmentControlFactory.MakeIShowPopupAppointmentControl(param, control);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ShowPopupWhenNotFinishingIncaseOfOutPatient(UserControl control)
        {
            try
            {
                IShowPopupWhenNotFinishingIncaseOfOutPatient behavior = ShowPopupWhenNotFinishingIncaseOfOutPatientFactory.MakeIShowPopupWhenNotFinishingIncaseOfOutPatient(param, control);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetDelegateCreateEMRVBA(UserControl control, CreateEMRVBAOnClick dlgCreateEMRVBAOnClick)
        {
            try
            {
                ISetDelegateCreateEMRVBA behavior = SetDelegateCreateEMRVBAFactory.MakeISetDelegateCreateEMRVBA(param, control, dlgCreateEMRVBAOnClick);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public void ChangeStateCreateEMRVBA(UserControl control, bool show)
        //{
        //    try
        //    {
        //        IChangeStateCreateEMRVBA behavior = ChangeStateCreateEMRVBAFactory.MakeIChangeStateCreateEMRVBA(param, control, show);
        //        if (behavior != null) behavior.Run();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}

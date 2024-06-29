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
using HIS.UC.PatientSelect.ADO;
using HIS.UC.PatientSelect.Run;
using System.Windows.Forms;
using HIS.UC.PatientSelect.Reload;
using HIS.UC.PatientSelect.GetFocusRow;
using MOS.EFMODEL.DataModels;
using HIS.UC.PatientSelect.FLoad;
using HIS.UC.PatientSelect.ReloadStatePrescriptionPerious;
using HIS.UC.PatientSelect.FocusSearchTextbox;
using HIS.UC.PatientSelect.FLoadWithFilter;
using HIS.UC.PatientSelect.GetSelectedRows;
using HIS.UC.PatientSelect.SetOnlyOneRow;

namespace HIS.UC.PatientSelect
{
    public class PatientSelectProcessor : BussinessBase
    {
        object uc;
        public PatientSelectProcessor()
            : base()
        {
        }

        public PatientSelectProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(PatientSelectInitADO arg)
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

        public void Load(UserControl control)
        {
            try
            {
                IFLoad behavior = FLoadFactory.MakeIFLoad(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadWithFilter(UserControl control, MOS.Filter.HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilter)
        {
            try
            {
                IFLoadWithFilter behavior = FLoadWithFilterFactory.MakeIFLoadWithFilter(param, (control == null ? (UserControl)uc : control), treatmentBedRoomLViewFilter);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM> datas)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), datas);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReloadStatePrescriptionPerious(UserControl control)
        {
            try
            {
                IReloadStatePrescriptionPerious behavior = ReloadStatePrescriptionPeriousFactory.MakeIReloadStatePrescriptionPerious(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusSearchTextbox(UserControl control)
        {
            try
            {
                IFocusSearchTextbox behavior = FocusSearchTextboxFactory.MakeIFocusSearchTextbox(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetOnlyOneRow(UserControl control, bool IsEnableOnly)
        {
            try
            {
                ISetOnlyOneRow behavior = SetOnlyOneRowFactory.MakeISetOnlyOneRow(IsEnableOnly, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public V_HIS_TREATMENT_BED_ROOM GetFocusRow(UserControl control)
        {
            V_HIS_TREATMENT_BED_ROOM result = null;
            try
            {
                IGetFocusRow behavior = GetFocusRowFactory.MakeIGetFocusRow(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public List<V_HIS_TREATMENT_BED_ROOM> GetSelectedRows(UserControl control)
        {
            List<V_HIS_TREATMENT_BED_ROOM> result = null;
            try
            {
                IGetSelectedRows behavior = GetSelectedRowsFactory.MakeIGetSelectedRows(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}

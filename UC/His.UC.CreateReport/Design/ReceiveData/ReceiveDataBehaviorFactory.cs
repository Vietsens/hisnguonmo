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
//using MOS.Filter;
using System;
using DCV.APP.Report.JsonOutput;
using His.UC.CreateReport.Design.CreateReport.JsonOutput;


namespace DCV.APP.Report.ReceiveData
{
    class ReceiveDataBehaviorFactory
    {
        internal static bool MakeIGetListV(CommonParam param, object dataItem, object value)
        {
            bool result = false;
            try
            {
                if (dataItem is HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox)
                {
                    result = ((HIS.UC.FormType.TreatmentTypeGridCheckBox.UCTreatmentTypeGridCheckBox)dataItem).GetValueReceive(value);
                }
                if (dataItem is HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo)
                {
                    result = ((HIS.UC.FormType.DepartmentCombo.UCDepartmentCombo)dataItem).GetValueReceive(value);
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + dataItem.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataItem), dataItem), ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

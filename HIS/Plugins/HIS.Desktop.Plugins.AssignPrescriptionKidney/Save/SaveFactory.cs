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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Save.Create;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Save.Update;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Save
{
    class SaveFactory
    {
        internal static ISave MakeISave(CommonParam param,
            List<MediMatyTypeADO> mediMatyTypeADOs,
            frmAssignPrescription frmAssignPrescription,
            int actionType,
            bool isSaveAndPrint,
            long parentServiceReqId,
            long sereServParentId)
        {
            ISave result = null;
            try
            {
                if (actionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    result = new SaveCreateBehavior(param, mediMatyTypeADOs, frmAssignPrescription, actionType, isSaveAndPrint, parentServiceReqId, sereServParentId);
                else
                    result = new SaveUpdateBehavior(param, mediMatyTypeADOs, frmAssignPrescription, actionType, isSaveAndPrint, parentServiceReqId, sereServParentId);

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + mediMatyTypeADOs.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mediMatyTypeADOs), mediMatyTypeADOs)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => actionType), actionType)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isSaveAndPrint), isSaveAndPrint)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServParentId), sereServParentId)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parentServiceReqId), parentServiceReqId), ex);
                result = null;
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

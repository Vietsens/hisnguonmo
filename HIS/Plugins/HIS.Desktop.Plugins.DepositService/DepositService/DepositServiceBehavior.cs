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
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.DepositService.DepositService;
using HIS.Desktop.Plugins.DepositService.ADO;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    class DepositServiceBehavior : BusinessBase, IDepositService
    {

        Inventec.Desktop.Common.Modules.Module moduleData = null;
        long hisTreatmentId = 0;
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE treatment = null;
        MOS.SDO.HisTransactionDepositSDO depositSdo = null;
        long? branchId = null;
        long CashierRoomId = 0;
        SendResultToOtherForm sendResultToOtherForm = null;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs;
        HIS.Desktop.Common.DelegateReturnSuccess returnData = null;
        bool? IsDepositAll = null;

        internal DepositServiceBehavior(Inventec.Desktop.Common.Modules.Module moduleData, long hisTreatmentId, MOS.SDO.HisTransactionDepositSDO depositSdo, long? branchId, long CashierRoomId, SendResultToOtherForm sendResultToOtherForm, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs,HIS.Desktop.Common.DelegateReturnSuccess returnSuccess, bool? isDepositAll)
            : base()
        {
            try
            {
                this.moduleData = moduleData;
                this.hisTreatmentId = hisTreatmentId;
                this.depositSdo = depositSdo;
                this.branchId = branchId;
                this.CashierRoomId = CashierRoomId;
                this.sereServs = sereServs;
                this.sendResultToOtherForm = sendResultToOtherForm;
                this.returnData = returnSuccess;
                this.IsDepositAll = isDepositAll;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal DepositServiceBehavior(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_TREATMENT_FEE treatment, MOS.SDO.HisTransactionDepositSDO depositSdo, long? branchId, long CashierRoomId, SendResultToOtherForm sendResultToOtherForm, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs, HIS.Desktop.Common.DelegateReturnSuccess returnSuccess, bool? isDepositAll)
            : base()
        {
            try
            {
                this.moduleData = moduleData;
                this.treatment = treatment;
                this.depositSdo = depositSdo;
                this.branchId = branchId;
                this.CashierRoomId = CashierRoomId;
                this.sereServs = sereServs;
                this.sendResultToOtherForm = sendResultToOtherForm;
                this.returnData = returnSuccess;
                this.IsDepositAll = isDepositAll;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        object IDepositService.Run()
        {
            object result = null;
            try
            {
                if (this.moduleData != null && this.hisTreatmentId > 0 && this.CashierRoomId > 0)
                {
                    result = new frmDepositService(this.hisTreatmentId, this.depositSdo, this.sendResultToOtherForm, this.branchId, this.CashierRoomId, this.sereServs, this.moduleData, this.returnData, IsDepositAll);
                }
                else if (this.moduleData != null && this.treatment != null && this.CashierRoomId > 0)
                {
                    result = new frmDepositService(this.treatment, this.depositSdo, this.sendResultToOtherForm, this.branchId, this.CashierRoomId, this.sereServs, this.moduleData, this.returnData, IsDepositAll);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                result = null;
            }
            return result;
        }
    }
}

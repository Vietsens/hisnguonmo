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
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.DepositService.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    class DepositServiceFactory
    {
        internal static IDepositService MakeIDeposit(CommonParam param, object[] data)
        {
            IDepositService result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long hisTreatmentId = 0;
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE treatment = null;
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs = null;
            MOS.SDO.HisTransactionDepositSDO depositSdo = null;
            long? branchId = null;
            long CashierRoomId = 0;
            SendResultToOtherForm sendResultToOtherForm = null;
            HIS.Desktop.Common.DelegateReturnSuccess returnData = null;
            bool? IsDepositAll = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {

                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is DepositServiceADO)
                            {
                                treatment = ((DepositServiceADO)data[i]).hisTreatment;
                                hisTreatmentId = ((DepositServiceADO)data[i]).hisTreatmentId;
                                depositSdo = ((DepositServiceADO)data[i]).depositSdo;
                                branchId = ((DepositServiceADO)data[i]).BRANCH_ID;
                                CashierRoomId = ((DepositServiceADO)data[i]).CashierRoomId;
                                sereServs = ((DepositServiceADO)data[i]).SereServs;
                                returnData = ((DepositServiceADO)data[i]).returnSuccess;
                                IsDepositAll = ((DepositServiceADO)data[i]).IsDepositAll;
                            }
                        }

                        if (moduleData != null && hisTreatmentId > 0 && CashierRoomId > 0)
                        {
                            result = new DepositServiceBehavior(moduleData, hisTreatmentId, depositSdo, branchId, CashierRoomId, sendResultToOtherForm, sereServs, returnData, IsDepositAll);
                        }
                        else if (moduleData != null && treatment != null && CashierRoomId > 0)
                        {
                            result = new DepositServiceBehavior(moduleData, treatment, depositSdo, branchId, CashierRoomId, sendResultToOtherForm, sereServs, returnData, IsDepositAll);
                        }
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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

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
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ImpMestCreate.Save
{
    class SaveFactory
    {
        internal static ISaveInit MakeIServiceRequestRegister(
            CommonParam param,
            List<VHisServiceADO> serviceADOs,
            UCImpMestCreate ucImpMestCreate,
            Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicbidmedicine,
            Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicbidmaterial,
            long roomId,
            ResultImpMestADO resultADO)
        {
            ISaveInit result = null;
            try
            {
                long impMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((ucImpMestCreate.cboImpMestType.EditValue ?? "0").ToString());
                if (impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__DK)
                {
                    result = new SaveInitBehavior(param, serviceADOs, ucImpMestCreate, dicbidmedicine, dicbidmaterial, roomId, resultADO);
                }
                else if (impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KK)
                {
                    result = new SaveInveBehavior(param, serviceADOs, ucImpMestCreate, dicbidmedicine, dicbidmaterial, roomId, resultADO);
                }
                else if (impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__KHAC)
                {
                    result = new SaveOtherBehavior(param, serviceADOs, ucImpMestCreate, dicbidmedicine, dicbidmaterial, roomId, resultADO);
                }
                else if (impMestTypeId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__NCC)
                {
                    result = new SaveManuBehavior(param, serviceADOs, ucImpMestCreate, dicbidmedicine, dicbidmaterial, roomId, resultADO);
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + serviceADOs.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceADOs), serviceADOs), ex);
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

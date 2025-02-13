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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ImpMestCreate.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.Save
{
    class SaveInveBehavior : SaveAbstract, ISaveInit
    {
        HIS_IMP_MEST _ImpMestUp { get; set; }
        internal SaveInveBehavior(CommonParam param,
            List<VHisServiceADO> serviceADOs,
            UCImpMestCreate ucImpMestCreate,
            Dictionary<string, V_HIS_BID_MEDICINE_TYPE> dicbidmedicine,
            Dictionary<string, V_HIS_BID_MATERIAL_TYPE> dicbidmaterial,
            long roomId,
            ResultImpMestADO resultADO)
            : base( param,
                serviceADOs,
                ucImpMestCreate,
                dicbidmedicine,
                dicbidmaterial,
                roomId,
                resultADO)
        {
            this._ImpMestUp = ucImpMestCreate._currentImpMestUp;
        }

        object ISaveInit.Run()
        {
            ResultImpMestADO result = null;
            if (this.CheckValid())
            {
                this.InitBase();

                HisImpMestInveSDO inputImpMestSDO = new HisImpMestInveSDO();
                inputImpMestSDO.ImpMest = this._ImpMestUp != null ? this._ImpMestUp : this.ImpMest;
                inputImpMestSDO.InveMaterials = this.MaterialWithPatySDOs;
                inputImpMestSDO.InveMedicines = this.MedicineWithPatySDOs;
                if (this.ResultADO != null)
                {
                    inputImpMestSDO.ImpMest = this.ResultADO.HisInveSDO.ImpMest;
                }
                HisImpMestInveSDO rs = null;
                //if (this.ResultADO == null)

                if (this._ImpMestUp == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CALL API: api/HisImpMest/InveCreate" + Inventec.Common.Logging.LogUtil.TraceData("", inputImpMestSDO));
                    rs = new Inventec.Common.Adapter.BackendAdapter(Param).Post<HisImpMestInveSDO>("api/HisImpMest/InveCreate", ApiConsumers.MosConsumer, inputImpMestSDO, Param);
                }
                else
                {
                    inputImpMestSDO.ImpMest.IMP_MEST_STT_ID = IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST;
                    Inventec.Common.Logging.LogSystem.Debug("CALL API: api/HisImpMest/InveUpdate" + Inventec.Common.Logging.LogUtil.TraceData("", inputImpMestSDO));
                    rs = new Inventec.Common.Adapter.BackendAdapter(Param).Post<HisImpMestInveSDO>("api/HisImpMest/InveUpdate", ApiConsumers.MosConsumer, inputImpMestSDO, Param);
                }

                if (rs != null)
                    result = this.ResultADO = new ResultImpMestADO(rs);
                //else
                //{
                //    MessageManager.Show(Param, false);
                //}
            }

            return result;
        }
    }
}

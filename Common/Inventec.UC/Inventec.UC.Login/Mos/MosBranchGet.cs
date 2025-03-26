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
using Inventec.UC.Login.Base;
using THE.Filter;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Login.Mos
{
    internal partial class MosBranchGet : BusinessBase
    {
        public MosBranchGet()
            : base()
        {

        }

        public MosBranchGet(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        public List<THE.EFMODEL.DataModels.THE_BRANCH> Get(TheBranchFilter branchFilter)
        {
            List<THE.EFMODEL.DataModels.THE_BRANCH> result = null;
            try
            {
                if (ApiConsumerStore.MosConsumer != null)
                {
                    Inventec.Core.ApiResultObject<List<THE.EFMODEL.DataModels.THE_BRANCH>> aro = ApiConsumerStore.MosConsumer.Get<Inventec.Core.ApiResultObject<List<THE.EFMODEL.DataModels.THE_BRANCH>>>("/api/TheBranch/Get", param, branchFilter);
                    if (aro != null)
                    {
                        if (aro.Param != null)
                        {
                            param.Messages.AddRange(aro.Param.Messages);
                            param.BugCodes.AddRange(aro.Param.BugCodes);
                        }
                        result = aro.Data;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
                result = null;
            }
            return result;
        }
    }
}

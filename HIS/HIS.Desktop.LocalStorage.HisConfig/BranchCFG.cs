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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.HisConfig
{
    class BranchCFG
    {
        private static HIS_BRANCH branch;
        public static HIS_BRANCH Branch
        {
            get
            {
                if (branch == null || branch.ID == 0)
                {
                    branch = GetBranch();
                }
                return branch;
            }
            set
            {
                branch = value;
            }
        }

        private static MOS.EFMODEL.DataModels.HIS_BRANCH GetBranch()
        {
            MOS.EFMODEL.DataModels.HIS_BRANCH result = null;
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisBranchFilter configFilter = new MOS.Filter.HisBranchFilter();
                configFilter.IS_ACTIVE = 1;
                configFilter.ID = BranchWorker.GetCurrentBranchId();
                //configFilter.WORKING_BRANCH_ID = BranchDataWorker.GetCurrentBranchId();//TODO
                result = new BackendAdapter(paramGet).Get<List<MOS.EFMODEL.DataModels.HIS_BRANCH>>("/api/HisBranch/Get", ApiConsumers.MosConsumer, configFilter, paramGet).FirstOrDefault();
                if (result == null)
                    result = new MOS.EFMODEL.DataModels.HIS_BRANCH();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = new MOS.EFMODEL.DataModels.HIS_BRANCH();
            }
            return result;
        }
    }
}

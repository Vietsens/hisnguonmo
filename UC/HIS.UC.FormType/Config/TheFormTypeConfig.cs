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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class TheFormTypeConfig
    {
        private static List<THE.EFMODEL.DataModels.THE_BRANCH> theBranchs;
        public static List<THE.EFMODEL.DataModels.THE_BRANCH> TheBranchs
        {
            get
            {
                if (theBranchs == null || theBranchs.Count == 0)
                {
                    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                    THE.Filter.TheBranchFilter filter = new THE.Filter.TheBranchFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var branch = BackendDataWorker.Get<THE.EFMODEL.DataModels.THE_BRANCH>(RequestUriStore.THE_BRANCH_GET, ApiConsumerStore.TheConsumer, filter);
                    if (branch != null && branch.Count > 0)
                    {
                        var branchAdmin = branch.FirstOrDefault(o => o.ID == FormTypeConfig.BranchId);
                        if (branchAdmin != null && branchAdmin.IS_ADMIN != 1)
                        {
                            theBranchs = new System.Collections.Generic.List<THE.EFMODEL.DataModels.THE_BRANCH>();
                            theBranchs.Add(branchAdmin);
                        }
                        else
                        {
                            theBranchs = branch;
                        }
                    }
                }
                return theBranchs;
            }
            set
            {
                theBranchs = value;
            }
        }
    }
}

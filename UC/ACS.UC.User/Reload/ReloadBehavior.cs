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
using ACS.UC.User.Reload;
using Inventec.Core;
using ACS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACS.UC.User.Reload
{
    public sealed class ReloadBehavior : IReload
    {
        UserControl control;
        List<UserADO> entity;
        public ReloadBehavior()
            : base()
        {
        }

        public ReloadBehavior(CommonParam param, UserControl uc, List<ACS.UC.User.UserADO> data)
            : base()
        {
            this.control = uc;
            this.entity = data;
        }

        void IReload.Run()
        {
            try
            {
                ((UCUser)this.control).Reload(entity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using Inventec.Core;
using System.Windows.Forms;
namespace HIS.UC.ExecuteRoomView.ReloadColumn
{
    class ReloadColumnBehavior : IRealoadColumn
    {
        UserControl control;
        object entity;
        public ReloadColumnBehavior()
            : base()
        {
        }

        public ReloadColumnBehavior(CommonParam param, UserControl uc, object data)
            : base()
        {
            this.control = uc;
            this.entity = data;
        }

        void IRealoadColumn.Run()
        {
            try
            {
                if (this.entity.GetType() == typeof(List<ExecuteRoomViewColumn>))
                    ((UC_ExecuteRoom)this.control).ReloadColumn((List<ExecuteRoomViewColumn>)entity);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

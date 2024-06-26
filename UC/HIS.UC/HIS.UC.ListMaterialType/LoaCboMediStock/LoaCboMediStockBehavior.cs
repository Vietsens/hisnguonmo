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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.UC.ListMaterialType.LoaCboMediStock
{
    public sealed class LoaCboMediStockBehavior : ILoaCboMediStock
    {
        UserControl control;
        List<V_HIS_MEST_ROOM> entity;
        bool readonl;

        public LoaCboMediStockBehavior()
            : base()
        {
        }

        public LoaCboMediStockBehavior(CommonParam param, UserControl uc, List<V_HIS_MEST_ROOM> data, bool readon)
            : base()
        {
            this.control = uc;
            this.entity = data;
            this.readonl = readon;
        }

        void ILoaCboMediStock.Run()
        {
            try
            {
                ((UCListMaterialType)this.control).LoaCboMediStock(entity, readonl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}




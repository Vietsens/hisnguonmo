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
using System.Windows.Forms;

namespace Inventec.UC.TreeSereServHein.Init
{
    class InitUCBehavior : IInitUC
    {
        private Data.InitData Data { get; set; }

        internal InitUCBehavior(Data.InitData data)
        {
            Data = data;
        }

        System.Windows.Forms.UserControl IInitUC.Init(MainTreeSereServHein.EnumTemplate template)
        {
            UserControl result = null;
            try
            {
                bool valid = true;
                valid = valid & (Data != null);
                if (valid)
                {
                    if (template == MainTreeSereServHein.EnumTemplate.TEMPLATE1)
                    {
                        result = new UCTreeSereServHein(Data);
                    }

                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khoi tao UC Template khong duoc. EnumTemplate." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => template), template));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
                }


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

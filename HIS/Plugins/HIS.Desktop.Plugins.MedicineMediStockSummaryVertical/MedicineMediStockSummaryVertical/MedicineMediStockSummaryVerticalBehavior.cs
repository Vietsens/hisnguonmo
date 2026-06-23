/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.MedicineMediStockSummaryVertical;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicineMediStockSummaryVertical.MedicineMediStockSummaryVertical
{
    public sealed class MedicineMediStockSummaryVerticalBehavior : Tool<IDesktopToolContext>, IMedicineMediStockSummaryVertical
    {
        object[] entity;

        public MedicineMediStockSummaryVerticalBehavior()
            : base()
        {
        }

        public MedicineMediStockSummaryVerticalBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IMedicineMediStockSummaryVertical.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                    }
                }

                // Loại thuốc/vật tư cần tra cứu được đẩy qua MedicineMediStockVerticalRequestStore
                // (để tab UC đang mở cũng nhận được), không truyền qua constructor nữa.
                result = new ucMedicineMediStockSummaryVertical(moduleData);
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

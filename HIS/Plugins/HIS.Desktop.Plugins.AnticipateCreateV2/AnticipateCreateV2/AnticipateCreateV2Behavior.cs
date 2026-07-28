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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.AnticipateCreateV2;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.AnticipateCreateV2.AnticipateCreateV2
{
    public sealed class AnticipateCreateV2Behavior : Tool<IDesktopToolContext>, IAnticipateCreateV2
    {
        object[] entity;
        public AnticipateCreateV2Behavior()
            : base()
        {
        }

        public AnticipateCreateV2Behavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IAnticipateCreateV2.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                V_HIS_ANTICIPATE anticipate = null;
                HIS.Desktop.Common.DelegateRefreshData delegateRefresh = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is V_HIS_ANTICIPATE)
                        {
                            anticipate = (V_HIS_ANTICIPATE)entity[i];
                        }
                        if (entity[i] is HIS.Desktop.Common.DelegateRefreshData)
                        {
                            delegateRefresh = (HIS.Desktop.Common.DelegateRefreshData)entity[i];
                        }
                    }
                }
                if (moduleData == null)
                {
                    return null;
                }
                // Đủ arg phiếu + delegate refresh (gọi từ Danh sách dự trù, BV HAGL) → mở popup Form Sửa.
                // Chỉ Module (mở từ menu) → giữ nguyên UC "Tạo dự trù v2" như hiện tại.
                if (anticipate != null && delegateRefresh != null)
                {
                    return new frmAnticipateCreateV2Edit(moduleData, anticipate, delegateRefresh);
                }
                return new UCAnticipateCreateV2(moduleData, moduleData.RoomId, moduleData.RoomTypeId);
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

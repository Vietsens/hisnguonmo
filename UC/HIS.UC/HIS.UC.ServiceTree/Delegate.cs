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

namespace HIS.UC.ServiceTree
{
    public delegate void ServiceTree_NodeCellStyle(object data, DevExpress.Utils.AppearanceObject appearanceObject);
    public delegate void ServiceHandler(ServiceADO data);
    public delegate void ServiceTree_GetStateImage(ServiceADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e);
    public delegate void ServiceTree_GetSelectImage(ServiceADO data, DevExpress.XtraTreeList.GetSelectImageEventArgs e);
    public delegate void ServiceTree_CustomUnboundColumnData(ServiceADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e);
    public delegate List<DevExpress.Utils.Menu.DXMenuItem> MenuItems(ServiceADO data);
    public delegate void ServiceTree_CustomNodeCellEdit(ServiceADO data, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e);
}

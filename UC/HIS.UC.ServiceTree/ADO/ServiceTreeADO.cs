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
    public class ServiceTreeADO
    {
        public ServiceTreeADO()
        {

        }

        public List<ColumnButtonEditADO> ColumnButtonEdits { get; set; }
        public List<ServiceTreeColumn> ServiceTreeColumns { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_SERVICE> Services { get; set; }
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }

        public bool? IsShowSearchPanel { get; set; }
        public ServiceTree_NodeCellStyle ServiceNodeCellStyle { get; set; }
        public ServiceHandler ServiceTreeClick { get; set; }

        public ServiceTree_GetStateImage ServiceTree_GetStateImage { get; set; }
        public ServiceTree_GetSelectImage ServiceTree_GetSelectImage { get; set; }
        public ServiceHandler ServiceTree_StateImageClick { get; set; }
        public ServiceHandler ServiceTree_SelectImageClick { get; set; }
        public ServiceTree_CustomUnboundColumnData ServiceTree_CustomUnboundColumnData { get; set; }
        public DevExpress.Utils.ImageCollection StateImageCollection { get; set; }
        public DevExpress.Utils.ImageCollection SelectImageCollection { get; set; }

        public ServiceHandler UpdateSingleRow { get; set; }
        public MenuItems MenuItems { get; set; }
        public ServiceTree_CustomNodeCellEdit ServiceTree_CustomNodeCellEdit { get; set; }
    }
}

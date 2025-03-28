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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceReqDetail.ADO
{
    public class ServiceReqDetailInitADO
    {
        public List<ServiceReqDetailADO> ListServiceReqDetailADO { get; set; }
        public ServiceReqDetailCommonADO ServiceReqDetailCommonADO { get; set; }
        public List<ServiceReqDetailColumn> ListServiceReqDetailColumn { get; set; }
        public bool? IsShowSearchPanel { get; set; }
        public Grid_CustomUnboundColumnData Grid_CustomUnboundColumnData { get; set; }
        public btn_Radio_Enable_Click1 btn_Radio_Enable_Click1 { get; set; }
        public Check__Enable_CheckedChanged Check__Enable_CheckedChanged { get; set; }
        public gridView_MouseDownAccount gridView_MouseDown { get; set; }
        public GridView_RowCellStyle gridView_RowCellStyle { get; set; }
        public GridView_MouseRightClick GridView_MouseRightClick { get; set; }
        public bool? IsShowMenuPopup { get; set; }
    }
}

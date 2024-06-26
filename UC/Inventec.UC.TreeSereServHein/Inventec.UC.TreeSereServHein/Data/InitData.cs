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
using Inventec.UC.TreeSereServHein.Delegate;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.TreeSereServHein.Data
{
    public class InitData
    {
        public HisTreatmentHeinSDO hisTreatmentSDO { get; set; }
        public Inventec.UC.TreeSereServHein.Store.GlobalStore.ModuleType moduleType { get; set; }
        public PopupMenuShowingForTreeList PopupMenuShowing_Click { get; set; } // delegate click chuột phải 
        public GridViewOnClickGetRow GridViewOnClickGetRow_Click { get; set; } //delegate onClick row
        public TotalPrice TotalPrice { get; set; }
    }
}

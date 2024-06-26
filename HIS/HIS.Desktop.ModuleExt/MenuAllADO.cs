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
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ModuleExt
{
    public class MenuAllADO
    {
        public MenuAllADO()
        {

        }
        public MenuAllADO(List<Inventec.Desktop.Common.Modules.Module> moduleAllInPages, ImageCollection imageCollection)
        {
            this.ModuleAllInPages = moduleAllInPages;
            this.ImageCollection = imageCollection;
        }
        public ImageCollection ImageCollection { get; set; }
        public List<Inventec.Desktop.Common.Modules.Module> ModuleAllInPages { get; set; }
    }
}

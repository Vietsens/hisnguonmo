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

namespace HIS.Desktop.Plugins.PayClinicalResult.PACS.ADO
{
    class SeriesADO
    {
        public SeriesADO() { }

        public string ModalityName { get; set; }

        public string MachineName { get; set; }

        public string BodyPart { get; set; }

        public string ViewPosition { get; set; }

        public string SeriesDateTime { get; set; }

        public int SeriesInstances { get; set; }

        public int SeriesNumber { get; set; }

        public List<ImagesADO> Images { get; set; }
    }
}

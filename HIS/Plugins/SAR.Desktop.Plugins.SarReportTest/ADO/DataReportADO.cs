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

namespace SAR.Desktop.Plugins.SarReportTest.ADO
{
    class DataReportADO
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string ValueRowCell { get; set; }
        public string Font { get; set; }
        public string Color { get; set; }
        public string SheetName { get; set; }
        public string REPORT_CODE { get; set; }
        public int Status { get; set; }
        //1 Đang tao bao cao nguon
        //2 Đang tao bao cao dich
        //3 Hoan thanh
        public string REPORT_TYPE_CODE { get; set; }
        public bool Result { get; set; }

    }
}

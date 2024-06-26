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
using EMR.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTreatmentFile.Base
{
    class AttackADO : HIS_TREATMENT_FILE
    {
        public string FILE_NAME { get; set; }
        public bool IsChecked { get; set; }
        public string Base64Data { get; set; }
        public long DocumentId { get; set; }
        public string Extension { get; set; }
        public string FullName { get; set; }
        public string Url { get; set; }
        public int Dem { get; set; }
        public int SttPdf { get; set; }
        public bool IsFss { get; set; }
        public System.Drawing.Image image { get; set; }

        public AttackADO() { }
    }
}

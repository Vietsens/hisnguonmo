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

namespace HIS.Desktop.Plugins.HisImportCareer.ADO
{
    /// <summary>
    /// Dong doc tu file excel danh muc nghe nghiep (dang phang, 8 cot theo thu tu man danh muc):
    /// Ma nghe | Ten nghe | Ma cap 2 | Ten cap 2 | Ma cap 3 | Ten cap 3 | Ma cap 4 | Ten cap 4.
    /// LEVEL2/3/4_CODE + LEVEL2/3/4_NAME dung property cua base HIS_CAREER (EFMODEL moi)
    /// </summary>
    public class CareerADO : MOS.EFMODEL.DataModels.HIS_CAREER
    {
        public string ERROR { get; set; }
    }
}

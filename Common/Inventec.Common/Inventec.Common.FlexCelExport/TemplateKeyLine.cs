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

namespace Inventec.Common.FlexCellExport
{
    /// <summary>
    /// 1 dong cua bang {KEY}_LINES - dung de in key van ban dai tren nhieu dong Excel.
    /// </summary>
    public class TemplateKeyLine
    {
        public long NUM_ORDER { get; set; }
        public string LINE { get; set; }

        public TemplateKeyLine()
        {
        }

        public TemplateKeyLine(long numOrder, string line)
        {
            NUM_ORDER = numOrder;
            LINE = line;
        }
    }
}

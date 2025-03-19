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

namespace Inventec.Common.XmlConfig
{
    public class ElementNode
    {
        public ElementNode() { }
        public string KeyCode { get; set; }
        public string Title { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public string ValueType { get; set; }
        public string ValueTypeDescription { get; set; }
        public object ValueAllowMin { get; set; }
        public object ValueAllowMax { get; set; }
        public object ValueAllowIn { get; set; }
        public string Tutorial { get; set; }
        public long CreateTime { get; set; }
        public long ModifyTime { get; set; }
        public string Creator { get; set; }
        public string Modifier { get; set; }
    }
}

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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Utility
{
    public class FileAppInfo
    {
        public FileAppInfo()
        {
           
        }
        public FileAppInfo(string fullName, long length, DateTime lastWriteTime)
        {
            this.FullName = fullName;
            this.Length = length;
            this.LastWriteTime = lastWriteTime;
        }
        public string FullName { get; set; }
        public long Length { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string Content { get; set; }
        public List<System.IO.FileInfo> TmpNameList { get; set; } // danh sách tên các file template
    }
}

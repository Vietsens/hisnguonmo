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
using System.Configuration;
namespace Inventec.Desktop.Common.LocalStorage.Location
{
    public class PrintStoreLocation
    {
        static string ROOT_PATH = "PrintTemplate";
        static string _rootpath;
        public static string PrintTemplatePath
        {
            get
            {
                if (String.IsNullOrEmpty(_rootpath))
                {
                    _rootpath = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "").Replace("file:/", "/") + System.IO.Path.DirectorySeparatorChar, ROOT_PATH));
                }
                return _rootpath;
            }
            set
            {
                _rootpath = value;
            }
        }
    }
}

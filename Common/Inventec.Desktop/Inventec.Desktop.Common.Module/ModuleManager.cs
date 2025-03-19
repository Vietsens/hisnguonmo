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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Desktop.Common.Modules
{
    public class ModuleManager
    {
        private const string SessionKey = "INVENTEC.DESKTOP.SESSIONKEY";
        public static object moduleToken;

        /// <summary>
        /// Ham generate danh sach module luu tru duoi dang tree
        /// Root module
        /// -- Child level 1 module
        /// -- -- Child level 2 module
        /// -- Child level 1 module
        /// -- Child level 1 module
        /// -- -- Child level 2 module
        /// 
        /// Ham xu ly se bo qua cac module khong co cha (ngoai tru root module - parent_code == null)
        /// Can luu y dieu nay khi code nghiep vu phan quyen (gan quyen cho module con thi phai tu dong gan quyen cho module cha va nguoc lai, bo gan quyen module cha thi phai tu dong bo gan quyen tat ca module con)
        /// </summary>
        /// <param name="listFlat"></param>
        /// <returns></returns>
        public static List<Module> GenerateListTree(List<Module> listFlat)
        {
            try
            {
                List<Module> listResult = new List<Module>();
                if (listFlat != null && listFlat.Count > 0)
                {
                    foreach (var module in listFlat)
                    {
                        List<Module> listChild = listFlat.FindAll(o => o.ParentCode == module.ModuleCode);
                        module.Children = listChild;
                    }
                    listResult = listFlat.FindAll(o => String.IsNullOrEmpty(o.ParentCode));
                }
                return listResult;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        public static List<Module> GetListTreeFromSession()
        {
            try
            {
                return (List<Module>)moduleToken;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        public static bool SetListTreeToSession(List<Module> listTree)
        {
            try
            {
                if (listTree != null)
                {
                    moduleToken = listTree;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return false;
        }
    }
}

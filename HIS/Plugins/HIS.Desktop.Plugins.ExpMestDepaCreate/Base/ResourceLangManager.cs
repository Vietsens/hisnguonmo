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
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestDepaCreate.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCExpMestDepaCreate { get; set; }
        internal static ResourceManager LanguageFrmPrintByCondition { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCExpMestDepaCreate = new ResourceManager("HIS.Desktop.Plugins.ExpMestDepaCreate.Resources.Lang", typeof(UCExpMestDepaCreate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                LanguageFrmPrintByCondition = new ResourceManager("HIS.Desktop.Plugins.ExpMestDepaCreate.Resources.Lang", typeof(Print.frmPrintByCondition).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
